using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_JetpackReboot : Minigame
	{
		public const float JBR_ORTHOSIZE = 3.84f;

		private mg_jr_Resources m_resources = new mg_jr_Resources();

		private mg_jr_GameLogic m_gameLogic;

		private Camera mainCamera;

		private mg_jr_IStatPersistenceStrategy m_playerStatPersitenceStrat = new mg_jr_PersistStatsInPlayerPrefs();

		private float m_orthoSizeAtLastBoundsUpdate = 0f;

		private Bounds m_visibleWorldBounds;

		public mg_jr_Resources Resources
		{
			get
			{
				return m_resources;
			}
		}

		internal mg_jr_GameLogic GameLogic
		{
			get
			{
				return m_gameLogic;
			}
		}

		private Camera MainSystemCamera
		{
			get
			{
				if (mainCamera == null)
				{
					mainCamera = Camera.main;
				}
				return mainCamera;
			}
		}

		public mg_jr_InputManager InputManager
		{
			get;
			set;
		}

		public mg_jr_MusicManager MusicManager
		{
			get;
			private set;
		}

		public mg_jr_GoalManager GoalManager
		{
			get;
			private set;
		}

		public mg_jr_PlayerStats PlayerStats
		{
			get;
			private set;
		}

		public Bounds VisibleWorldBounds
		{
			get
			{
				if (!Mathf.Approximately(MainSystemCamera.orthographicSize, m_orthoSizeAtLastBoundsUpdate))
				{
					Camera mainSystemCamera = MainSystemCamera;
					m_visibleWorldBounds = new Bounds(Vector3.zero, new Vector3(mainSystemCamera.orthographicSize * 2f * mainSystemCamera.aspect, mainSystemCamera.orthographicSize * 2f, 40f));
				}
				return m_visibleWorldBounds;
			}
		}

		public override void Awake()
		{
			base.Awake();
			base.PasuseScreenName = "mg_jr_PauseScreen";
		}

		private IEnumerator Start()
		{
			mg_jr_PlayerPrefsKey.InitializeForCurrentPlayer();
			PlayerStats = m_playerStatPersitenceStrat.LoadOrCreate();
			GoalManager = base.gameObject.AddComponent<mg_jr_GoalManager>();
			SetMainCamera("mg_jr_MainCamera");
			InputManager.Prepare(base.MainCamera);
			base.MainCamera.orthographicSize = 3.84f;
			base.MainCamera.transform.position = new Vector3(0f, 0f, -20f);
			base.MainCamera.transform.rotation = Quaternion.identity;
			GameObject resourceContainer = new GameObject("mg_jr_ResourceContainer");
			resourceContainer.transform.parent = base.transform;
			m_resources.LoadTextResources();
			UIManager.Instance.OpenScreen("mg_jr_SplashScreen", false, null, null);
			yield return 0;
			m_resources.ResourceContainer = resourceContainer;
			yield return StartCoroutine(m_resources.LoadResourcesCoRoutine());
			GameObject musicManagerGo = new GameObject("mg_jr_MusicManager");
			musicManagerGo.transform.parent = base.transform;
			MusicManager = musicManagerGo.AddComponent<mg_jr_MusicManager>();
			MusicManager.CreateTracks(m_resources.MusicClips);
			GameObject gameWorld = m_resources.GetInstancedResource(mg_jr_ResourceList.GAME_WORLD_LOGIC);
			gameWorld.transform.parent = base.transform;
			m_gameLogic = gameWorld.GetComponent<mg_jr_GameLogic>();
			if (m_gameLogic == null)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogDebug(m_gameLogic, "JetpackReboot game world doesn't contain any game logic.");
			}
			yield return 0;
			MusicManager.SetVolume(MusicVolume);
			GameLogic.LoadInitialState();
		}

		public override void MinigameUpdate(float _deltaTime)
		{
			if (GameLogic != null && GameLogic.enabled)
			{
				GameLogic.MinigameUpdate(_deltaTime);
			}
		}

		public void OnDestroy()
		{
			InputManager = null;
			Resources.UnloadAllResources();
		}

		protected override void OnPause()
		{
			if (InputManager != null)
			{
				InputManager.enabled = false;
			}
			base.OnPause();
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (InputManager != null)
			{
				InputManager.enabled = true;
			}
		}

		public override string GetMinigameName()
		{
			if (Service.IsSet<Localizer>())
			{
				return Service.Get<Localizer>().GetTokenTranslation("Activity.JetpackBoost.Title");
			}
			return "Jetpack Boost";
		}

		public void SaveState()
		{
			GoalManager.SaveGoalProgress();
			PlayerStats.SaveStats();
		}

		public override void OnQuit()
		{
			SaveState();
			base.OnQuit();
		}
	}

	internal static class mg_jr_PlayerPrefsKey
	{
		private static string playerScope;

		public static void InitializeForCurrentPlayer()
		{
			playerScope = null;
			Type rememberMeType = findType("ClubPenguin.RememberMeService");
			if (rememberMeType == null)
			{
				return;
			}
			MethodInfo isSetMethod = typeof(Service).GetMethod("IsSet").MakeGenericMethod(rememberMeType);
			if (!(bool)isSetMethod.Invoke(null, null))
			{
				return;
			}
			MethodInfo getMethod = typeof(Service).GetMethod("Get").MakeGenericMethod(rememberMeType);
			object rememberMeService = getMethod.Invoke(null, null);
			PropertyInfo usernameProperty = rememberMeType.GetProperty("CurrentUsername");
			string username = (usernameProperty != null) ? usernameProperty.GetValue(rememberMeService, null) as string : null;
			if (!string.IsNullOrEmpty(username))
			{
				playerScope = HashUsername(username);
				MigrateLegacyData();
			}
		}

		public static string Get(string key)
		{
			return string.IsNullOrEmpty(playerScope) ? key : "mg_jr_" + playerScope + "_" + key;
		}

		private static string GetPlayerPrefsKey(string key)
		{
			string scopedKey = Get(key);
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (Application.isEditor)
			{
				return "Editor_" + scopedKey;
			}
#endif
			return scopedKey;
		}

		private static Type findType(string fullName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type type = assemblies[i].GetType(fullName, false);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		private static string HashUsername(string username)
		{
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(username.ToLowerInvariant()));
				return BitConverter.ToString(hash).Replace("-", string.Empty);
			}
		}

		private static void MigrateLegacyData()
		{
			const string migrationKey = "mg_jr_PlayerScopedMigration_v2";
			string scopedMigrationKey = Get(migrationKey);
			if (PlayerPrefs.HasKey(scopedMigrationKey))
			{
				return;
			}

			CopyMaxInt("mg_jr_BestDistance");
			CopyMaxInt("mg_jr_MostCoins");
			for (int goalType = 0; goalType < (int)mg_jr_Goal.GoalType.MAX; goalType++)
			{
				for (int duration = 0; duration < (int)mg_jr_Goal.GoalDuration.MAX; duration++)
				{
					string goalName = ((mg_jr_Goal.GoalType)goalType).ToString() + "_" + ((mg_jr_Goal.GoalDuration)duration).ToString();
					MigrateGoalProgress(goalName, "mg_jr_");
					MigrateGoalProgress(goalName, string.Empty);
				}
			}
			for (int activeGoal = 0; activeGoal < mg_jr_GoalManager.MAX_ACTIVE_GOALS; activeGoal++)
			{
				string key = "activeGoal_number_" + activeGoal + "_";
				CopyInt("mg_jr_" + key + "duration", key + "duration");
				CopyInt("mg_jr_" + key + "progress", key + "progress");
				CopyInt(key + "duration", key + "duration");
				CopyInt(key + "progress", key + "progress");
			}

			PlayerPrefs.SetInt(scopedMigrationKey, 1);
		}

		private static void CopyInt(string legacyKeyName, string scopedKeyName)
		{
			string legacyKey = GetLegacy(legacyKeyName);
			string scopedKey = GetPlayerPrefsKey(scopedKeyName);
			if (PlayerPrefs.HasKey(legacyKey) && !PlayerPrefs.HasKey(scopedKey))
			{
				PlayerPrefs.SetInt(scopedKey, PlayerPrefs.GetInt(legacyKey));
			}
		}

		private static void CopyMaxInt(string key)
		{
			string legacyKey = GetLegacy(key);
			string scopedKey = GetPlayerPrefsKey(key);
			if (PlayerPrefs.HasKey(legacyKey) && (!PlayerPrefs.HasKey(scopedKey) || PlayerPrefs.GetInt(legacyKey) > PlayerPrefs.GetInt(scopedKey)))
			{
				PlayerPrefs.SetInt(scopedKey, PlayerPrefs.GetInt(legacyKey));
			}
		}

		private static void MigrateGoalProgress(string goalName, string legacyPrefix)
		{
			string legacyLevelKey = GetLegacy(legacyPrefix + goalName + "_level");
			string scopedLevelKey = GetPlayerPrefsKey(goalName + "_level");
			string legacyProgressKey = GetLegacy(legacyPrefix + goalName + "_progress");
			string scopedProgressKey = GetPlayerPrefsKey(goalName + "_progress");
			bool hasLegacyLevel = PlayerPrefs.HasKey(legacyLevelKey);
			bool hasLegacyProgress = PlayerPrefs.HasKey(legacyProgressKey);
			if (!hasLegacyLevel)
			{
				if (hasLegacyProgress && (!PlayerPrefs.HasKey(scopedProgressKey) ||
					PlayerPrefs.GetFloat(legacyProgressKey) > PlayerPrefs.GetFloat(scopedProgressKey)))
				{
					PlayerPrefs.SetFloat(scopedProgressKey, PlayerPrefs.GetFloat(legacyProgressKey));
				}
				return;
			}

			int legacyLevel = PlayerPrefs.GetInt(legacyLevelKey);
			if (!PlayerPrefs.HasKey(scopedLevelKey) || legacyLevel > PlayerPrefs.GetInt(scopedLevelKey))
			{
				PlayerPrefs.SetInt(scopedLevelKey, legacyLevel);
				if (hasLegacyProgress)
				{
					PlayerPrefs.SetFloat(scopedProgressKey, PlayerPrefs.GetFloat(legacyProgressKey));
				}
				else
				{
					PlayerPrefs.SetFloat(scopedProgressKey, 0f);
				}
			}
			else if (legacyLevel == PlayerPrefs.GetInt(scopedLevelKey) && hasLegacyProgress &&
				(!PlayerPrefs.HasKey(scopedProgressKey) || PlayerPrefs.GetFloat(legacyProgressKey) > PlayerPrefs.GetFloat(scopedProgressKey)))
			{
				PlayerPrefs.SetFloat(scopedProgressKey, PlayerPrefs.GetFloat(legacyProgressKey));
			}
		}

		private static string GetLegacy(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}
	}
}
