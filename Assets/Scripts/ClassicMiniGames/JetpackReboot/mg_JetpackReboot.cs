using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections;
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
}
