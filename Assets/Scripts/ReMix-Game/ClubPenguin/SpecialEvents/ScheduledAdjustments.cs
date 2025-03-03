using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Foundation.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.SpecialEvents
{
	public class ScheduledAdjustments : MonoBehaviour
	{
		private const string FTUE_QUEST_NAME = "AAC001Q001LeakyShip";

		public string EventName;

		public bool HideWhenQuestActive = true;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		[Header("## Always Happens ##########")]
		[Header("-- Fog settings -----")]
		public ScheduledFogData FogData;

		[Header("-- Skybox setting -----")]
		public MaterialContentKey SkyboxMaterialKey;

		[Header("-- Swap Material settings -----")]
		public ScheduledSwapMaterialData[] SwapMaterialData;

		[Header("-- Destroy Object settings -----")]
		public GameObject[] DestroyData;

		[Header("-- Spawn Prefab settings -----")]
		[Header("## Happens when 'HideWhenQuestActive' is false #####")]
		public ScheduledSpawnData[] SpawnData;

		[Header("-- Additive Scene settings -----")]
		[Scene]
		public string AdditiveScene;

		[Header("-- Disable Object settings -----")]
		public GameObject[] DisableData;

		[Header("-- Enable Object settings -----")]
		public GameObject[] EnableData;

		private ScheduledEventDateDefinition dateDefinition;

		private EventChannel eventChannel;

		private List<GameObject> spawnedObjects;

		private Stopwatch swapMaterialsLoadTimer;

		private List<ScheduledSwapMaterialData> materialsToSwap;

		private Stopwatch spawnPrefabsLoadTimer;

		private List<ScheduledSpawnData> prefabsToSpawn;

		private Stopwatch additiveSceneTimer;

		private bool isSceneLoaded;

		private bool areAdjustmentsVisible;

		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			dateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[DateDefinitionKey.Id];
			eventChannel.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (spawnedObjects == null)
			{
				return;
			}
			for (int i = 0; i < spawnedObjects.Count; i++)
			{
				if (spawnedObjects[i] != null)
				{
					Object.Destroy(spawnedObjects[i]);
				}
			}
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			eventChannel.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			Initialize();
			return false;
		}

		private void Initialize()
		{
			isSceneLoaded = false;
			areAdjustmentsVisible = false;
			if (Service.Get<GameStateController>().IsFTUEComplete)
			{
				CheckWithinDateRange();
			}
			else
			{
				eventChannel.AddListener<QuestEvents.QuestUpdated>(onFTUEQuestUpdated);
			}
		}

		private bool onFTUEQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.Id == "AAC001Q001LeakyShip" && evt.Quest.State == Quest.QuestState.Completed)
			{
				eventChannel.RemoveListener<QuestEvents.QuestUpdated>(onFTUEQuestUpdated);
				CheckWithinDateRange();
			}
			return false;
		}

		private void CheckWithinDateRange()
		{
			if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(dateDefinition))
			{
				displayAdjustments();
			}
		}

		private void displayAdjustments()
		{
			adjustFog();
			adjustSkybox();
			swapMaterials();
			destroyObjects();
			if (!shouldAdjustmentsBeHidden())
			{
				areAdjustmentsVisible = true;
				spawnPrefabs();
				CoroutineRunner.Start(loadAdditiveScene(), this, "loadAdditiveScene");
				disableObjects();
				enableObjects();
			}
			if (HideWhenQuestActive)
			{
				addReactiveQuestListeners();
			}
		}

		private void addReactiveQuestListeners()
		{
			eventChannel.AddListener<QuestEvents.SuspendQuest>(onSuspendQuest);
			eventChannel.AddListener<QuestEvents.StartQuest>(onStartQuest);
			eventChannel.AddListener<QuestEvents.ReplayQuest>(onReplayQuest);
			eventChannel.AddListener<QuestEvents.ResumeQuest>(onResumeQuest);
			eventChannel.AddListener<QuestEvents.RestartQuest>(onRestartQuest);
			eventChannel.AddListener<QuestEvents.QuestCompleted>(onCompleteQuest);
			eventChannel.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
		}

		private bool onSuspendQuest(QuestEvents.SuspendQuest evt)
		{
			adjustSceneForQuest(false);
			return false;
		}

		private bool onStartQuest(QuestEvents.StartQuest evt)
		{
			adjustSceneForQuest(true);
			return false;
		}

		private bool onReplayQuest(QuestEvents.ReplayQuest evt)
		{
			adjustSceneForQuest(true);
			return false;
		}

		private bool onResumeQuest(QuestEvents.ResumeQuest evt)
		{
			adjustSceneForQuest(true);
			return false;
		}

		private bool onRestartQuest(QuestEvents.RestartQuest evt)
		{
			adjustSceneForQuest(true);
			return false;
		}

		private bool onCompleteQuest(QuestEvents.QuestCompleted evt)
		{
			adjustSceneForQuest(false);
			return false;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State == Quest.QuestState.Completed || evt.Quest.State == Quest.QuestState.Suspended)
			{
				adjustSceneForQuest(false);
			}
			else if (evt.Quest.State == Quest.QuestState.Active)
			{
				adjustSceneForQuest(true);
			}
			return false;
		}

		private void adjustFog()
		{
			if (FogData.FogEnabled)
			{
				RenderSettings.fog = FogData.FogEnabled;
				RenderSettings.fogColor = FogData.Color;
				RenderSettings.fogDensity = FogData.Density;
				RenderSettings.fogMode = FogData.FogMode;
				RenderSettings.fogStartDistance = FogData.StartDistance;
				RenderSettings.fogEndDistance = FogData.EndDistance;
			}
		}

		private void adjustSkybox()
		{
			if (SkyboxMaterialKey != null && !string.IsNullOrEmpty(SkyboxMaterialKey.Key))
			{
				Texture mainTexture = RenderSettings.skybox.mainTexture;
				RenderSettings.skybox.mainTexture = null;
				ComponentExtensions.DestroyResource(mainTexture);
				Content.LoadAsync(delegate(string path, Material material)
				{
					onScheduledSkyboxLoaded(material);
				}, SkyboxMaterialKey);
			}
		}

		private void onScheduledSkyboxLoaded(Material material)
		{
			RenderSettings.skybox = material;
		}

		private void spawnPrefabs()
		{
			spawnedObjects = new List<GameObject>();
			prefabsToSpawn = new List<ScheduledSpawnData>();
			int num = SpawnData.Length;
			for (int i = 0; i < num; i++)
			{
				ScheduledSpawnData scheduledSpawnData = SpawnData[i];
				if (scheduledSpawnData.SpawnPrefabKey != null && !string.IsNullOrEmpty(scheduledSpawnData.SpawnPrefabKey.Key))
				{
					prefabsToSpawn.Add(scheduledSpawnData);
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has a Spawn data field with a null prefab entry", base.gameObject.GetPath()));
				}
			}
			if (prefabsToSpawn.Count > 0)
			{
				Service.Get<LoadingController>().AddLoadingSystem(this);
				spawnPrefabsLoadTimer = new Stopwatch();
				spawnPrefabsLoadTimer.Start();
				foreach (ScheduledSpawnData item in prefabsToSpawn)
				{
					GameObject result;
					if (Content.TryLoadImmediate(out result, item.SpawnPrefabKey))
					{
						onSpawnPrefabLoaded(result, item);
					}
				}
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				spawnPrefabsLoadTimer.Stop();
			}
		}

		private void onSpawnPrefabLoaded(GameObject prefab, ScheduledSpawnData data)
		{
			Transform parent = base.gameObject.transform;
			if (data.SpawnParentTransform != null)
			{
				parent = data.SpawnParentTransform;
			}
			GameObject gameObject = Object.Instantiate(prefab, parent);
			spawnedObjects.Add(gameObject);
			if (data.SpawnOffset != Vector3.zero)
			{
				gameObject.transform.localPosition = data.SpawnOffset;
			}
			if (data.SpawnRotation != Vector3.zero)
			{
				gameObject.transform.localEulerAngles = data.SpawnRotation;
			}
			if (data.SpawnScale != Vector3.zero)
			{
				gameObject.transform.localScale = data.SpawnScale;
			}
		}

		private IEnumerator loadAdditiveScene()
		{
			if (!string.IsNullOrEmpty(AdditiveScene))
			{
				Service.Get<LoadingController>().AddLoadingSystem(this);
				additiveSceneTimer = new Stopwatch();
				additiveSceneTimer.Start();
				yield return SceneManager.LoadSceneAsync(AdditiveScene, LoadSceneMode.Additive);
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				additiveSceneTimer.Stop();
				isSceneLoaded = true;
			}
		}

		private void swapMaterials()
		{
			materialsToSwap = new List<ScheduledSwapMaterialData>();
			ScheduledSwapMaterialData[] swapMaterialData = SwapMaterialData;
			foreach (ScheduledSwapMaterialData scheduledSwapMaterialData in swapMaterialData)
			{
				Renderer component = scheduledSwapMaterialData.SwapTarget.GetComponent<Renderer>();
				if (component != null)
				{
					if (scheduledSwapMaterialData.SwapMaterialKey != null && !string.IsNullOrEmpty(scheduledSwapMaterialData.SwapMaterialKey.Key))
					{
						materialsToSwap.Add(scheduledSwapMaterialData);
					}
					else
					{
						Log.LogError(this, string.Format("Error: {0} has a Swap Material data field with a null material entry", base.gameObject.GetPath()));
					}
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has a no renderer component attached", base.gameObject.GetPath()));
				}
			}
			if (materialsToSwap.Count > 0)
			{
				Service.Get<LoadingController>().AddLoadingSystem(this);
				swapMaterialsLoadTimer = new Stopwatch();
				swapMaterialsLoadTimer.Start();
				foreach (ScheduledSwapMaterialData item in materialsToSwap)
				{
					Renderer component = item.SwapTarget.GetComponent<Renderer>();
					Material material = component.material;
					Texture mainTexture = component.material.mainTexture;
					component.material.mainTexture = null;
					component.material = null;
					if (item.DestroyTexture)
					{
						ComponentExtensions.DestroyResource(mainTexture);
					}
					ComponentExtensions.DestroyResource(material);
					Material result;
					if (Content.TryLoadImmediate(out result, item.SwapMaterialKey))
					{
						component.material = result;
					}
				}
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				swapMaterialsLoadTimer.Stop();
			}
		}

		private void disableObjects()
		{
			GameObject[] disableData = DisableData;
			foreach (GameObject gameObject in disableData)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has a Disable Object field with a null Gameobject entry", base.gameObject.GetPath()));
				}
			}
		}

		private void enableObjects()
		{
			GameObject[] enableData = EnableData;
			foreach (GameObject gameObject in enableData)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has an Enable Object field with a null Gameobject entry", base.gameObject.GetPath()));
				}
			}
		}

		private void adjustSceneForQuest(bool isQuestActive)
		{
			bool flag = false;
			if (!isQuestActive && (spawnedObjects == null || spawnedObjects.Count == 0))
			{
				if (!areAdjustmentsVisible)
				{
					spawnPrefabs();
					CoroutineRunner.Start(loadAdditiveScene(), this, "loadAdditiveScene");
					areAdjustmentsVisible = true;
					flag = true;
				}
			}
			else if (areAdjustmentsVisible)
			{
				foreach (GameObject spawnedObject in spawnedObjects)
				{
					if (spawnedObject != null)
					{
						spawnedObject.SetActive(!isQuestActive);
					}
				}
				if (isSceneLoaded)
				{
					SceneManager.UnloadSceneAsync(AdditiveScene);
					isSceneLoaded = false;
				}
				areAdjustmentsVisible = false;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			GameObject[] disableData = DisableData;
			foreach (GameObject current in disableData)
			{
				if (current != null)
				{
					current.SetActive(isQuestActive);
				}
			}
			disableData = EnableData;
			foreach (GameObject current in disableData)
			{
				if (current != null)
				{
					current.SetActive(!isQuestActive);
				}
			}
		}

		private void destroyObjects()
		{
			GameObject[] destroyData = DestroyData;
			foreach (GameObject gameObject in destroyData)
			{
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has an Destroy Object field with a null Gameobject entry", base.gameObject.GetPath()));
				}
			}
		}

		private bool shouldAdjustmentsBeHidden()
		{
			if (HideWhenQuestActive && Service.IsSet<QuestService>())
			{
				return Service.Get<QuestService>().ActiveQuest != null;
			}
			return false;
		}
	}
}
