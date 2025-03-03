using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.SpecialEvents
{
	public abstract class AbstractEventCutsceneController : MonoBehaviour
	{
		[Serializable]
		public struct EventCutsceneEvent
		{
			public string Name;

			public ScheduledEventDateDefinitionKey DateDefinitionKey;

			public ScheduledDecorationData[] DecorationData;

			public ScheduledCutSceneData CutsceneData;
		}

		private struct LoadingData
		{
			public int Count;

			public bool IsDecoration;

			public LoadingData(int count, bool isDecoration)
			{
				Count = count;
				IsDecoration = isDecoration;
			}
		}

		private delegate void additiveLoadDelegate(string assetName, additiveCompleteDelegate delegateMethod);

		private delegate void additiveCompleteDelegate();

		private const string FTUE_QUEST_NAME = "AAC001Q001LeakyShip";

		private const bool IS_DECORATION = true;

		private const bool IS_CUTSCENE = false;

		public string EventName;

		public ZoneDefinition EventZone;

		public bool HideWhenQuestActive = true;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		[Header("-- Cutscene and Decoration settings -----")]
		public bool RemoveDecorationsAfterCutscene = false;

		public EventCutsceneEvent[] Events;

		protected EventChannel eventChannel;

		protected EventCutsceneEvent currentCutsceneEvent;

		protected ContentSchedulerService contentSchedulerService;

		private Stopwatch additiveSceneTimer;

		private Dictionary<string, LoadingData> loadingCounts;

		private Dictionary<int, ScheduledEventDateDefinition> dateDefinitions;

		public virtual void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			dateDefinitions = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>();
			contentSchedulerService = Service.Get<ContentSchedulerService>();
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			eventChannel.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			Initialize();
			return false;
		}

		private void Initialize()
		{
			loadingCounts = new Dictionary<string, LoadingData>();
			if (Service.Get<GameStateController>().IsFTUEComplete)
			{
				checkWithinDateRange();
			}
			else
			{
				eventChannel.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			}
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.Id == "AAC001Q001LeakyShip" && evt.Quest.State == Quest.QuestState.Completed)
			{
				eventChannel.RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
				checkWithinDateRange();
			}
			return false;
		}

		private void checkWithinDateRange()
		{
			if (!contentSchedulerService.IsDuringScheduleEventDates(dateDefinitions[DateDefinitionKey.Id]))
			{
				return;
			}
			if (!hideDuringQuests())
			{
				currentCutsceneEvent = checkSubEvents();
				if (currentCutsceneEvent.CutsceneData == null && currentCutsceneEvent.DecorationData == null)
				{
					handleNoSubEvent();
					allLoadingComplete();
				}
				else
				{
					loadDecorations(onDecorationsLoaded, beginLoadCutscene);
				}
			}
			else
			{
				allLoadingComplete();
			}
		}

		protected virtual EventCutsceneEvent checkSubEvents()
		{
			for (int num = Events.Length - 1; num >= 0; num--)
			{
				if (contentSchedulerService.IsDuringScheduleEventDates(dateDefinitions[Events[num].DateDefinitionKey.Id]))
				{
					return Events[num];
				}
			}
			return default(EventCutsceneEvent);
		}

		private void loadDecorations(additiveLoadDelegate onLoadCallBack, additiveCompleteDelegate onCompleteCallBack)
		{
			ScheduledDecorationData[] decorationData = currentCutsceneEvent.DecorationData;
			foreach (ScheduledDecorationData scheduledDecorationData in decorationData)
			{
				if (!string.IsNullOrEmpty(scheduledDecorationData.DecorationAdditiveScene))
				{
					incrementLoadCount(scheduledDecorationData.DecorationAdditiveScene, true);
					CoroutineRunner.Start(loadAdditiveScene(scheduledDecorationData.DecorationAdditiveScene, onLoadCallBack, onCompleteCallBack), this, "loadAdditiveScene");
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has a Decoration data field with a null scene entry", base.gameObject.GetPath()));
				}
			}
		}

		private void onDecorationsLoaded(string decorationAdditiveScene, additiveCompleteDelegate onCompleteCallBack)
		{
			if (decrementLoadCount(decorationAdditiveScene) <= 0)
			{
				handleDecorationsLoaded(currentCutsceneEvent);
				onCompleteCallBack();
			}
		}

		private void beginLoadCutscene()
		{
			loadCutscene(onCutsceneLoaded, allLoadingComplete);
		}

		private void loadCutscene(additiveLoadDelegate onLoadCallBack, additiveCompleteDelegate onCompleteCallBack)
		{
			ScheduledCutSceneData cutsceneData = currentCutsceneEvent.CutsceneData;
			if (!string.IsNullOrEmpty(currentCutsceneEvent.CutsceneData.CutSceneAdditiveScene))
			{
				string text = createLocalKey(cutsceneData.PlayedKeyName);
				if (string.IsNullOrEmpty(text) || (!string.IsNullOrEmpty(text) && !PlayerPrefs.HasKey(text)))
				{
					incrementLoadCount(cutsceneData.CutSceneAdditiveScene, false);
					CoroutineRunner.Start(loadAdditiveScene(cutsceneData.CutSceneAdditiveScene, onLoadCallBack, onCompleteCallBack), this, "loadAdditiveScene");
				}
				else
				{
					onCompleteCallBack();
				}
			}
			else
			{
				Log.LogError(this, string.Format("Error: {0} has a Cutscene data field with a null scene entry", base.gameObject.GetPath()));
			}
		}

		private void onCutsceneLoaded(string cutsceneAdditiveScene, additiveCompleteDelegate onCompleteCallBack)
		{
			if (decrementLoadCount(cutsceneAdditiveScene) <= 0)
			{
				handleCutsceneLoaded(currentCutsceneEvent);
				onCompleteCallBack();
			}
		}

		protected abstract void handleNoSubEvent();

		protected abstract void handleDecorationsLoaded(EventCutsceneEvent cutsceneEvent);

		protected abstract void handleCutsceneLoaded(EventCutsceneEvent cutsceneEvent);

		private void allLoadingComplete()
		{
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
		}

		private bool onSuspendQuest(QuestEvents.SuspendQuest evt)
		{
			adjustForQuest(false);
			return false;
		}

		private bool onStartQuest(QuestEvents.StartQuest evt)
		{
			adjustForQuest(true);
			return false;
		}

		private bool onReplayQuest(QuestEvents.ReplayQuest evt)
		{
			adjustForQuest(true);
			return false;
		}

		private bool onResumeQuest(QuestEvents.ResumeQuest evt)
		{
			adjustForQuest(true);
			return false;
		}

		private bool onRestartQuest(QuestEvents.RestartQuest evt)
		{
			adjustForQuest(true);
			return false;
		}

		private bool onCompleteQuest(QuestEvents.QuestCompleted evt)
		{
			adjustForQuest(false);
			return false;
		}

		private void adjustForQuest(bool isQuestActive)
		{
			if (isQuestActive)
			{
				removeLoadedScenes();
				return;
			}
			currentCutsceneEvent = checkSubEvents();
			if (currentCutsceneEvent.CutsceneData == null && currentCutsceneEvent.DecorationData == null)
			{
				handleNoSubEvent();
			}
			else
			{
				loadDecorations(onDecorationsLoaded, onAdjustForQuestDecorationLoadComplete);
			}
		}

		protected void removeLoadedScenes()
		{
			List<string> list = new List<string>();
			foreach (string key in loadingCounts.Keys)
			{
				if (loadingCounts[key].Count == 0 && (!loadingCounts[key].IsDecoration || RemoveDecorationsAfterCutscene))
				{
					SceneManager.UnloadSceneAsync(key);
					list.Add(key);
				}
			}
			foreach (string item in list)
			{
				loadingCounts.Remove(item);
			}
		}

		private void onAdjustForQuestDecorationLoadComplete()
		{
			loadCutscene(onCutsceneLoaded, onAdjustForQuestSceneLoadComplete);
		}

		private void onAdjustForQuestSceneLoadComplete()
		{
		}

		private IEnumerator loadAdditiveScene(string additiveScene, additiveLoadDelegate onLoadCallBack, additiveCompleteDelegate onCompleteCallBack)
		{
			if (!string.IsNullOrEmpty(additiveScene))
			{
				Service.Get<LoadingController>().AddLoadingSystem(this);
				additiveSceneTimer = new Stopwatch();
				additiveSceneTimer.Start();
				yield return SceneManager.LoadSceneAsync(additiveScene, LoadSceneMode.Additive);
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				additiveSceneTimer.Stop();
				if (onLoadCallBack != null)
				{
					onLoadCallBack(additiveScene, onCompleteCallBack);
				}
			}
		}

		private bool hideDuringQuests()
		{
			if (HideWhenQuestActive)
			{
				return Service.Get<QuestService>().ActiveQuest != null;
			}
			return false;
		}

		protected bool wasCutsceneWatched(ScheduledCutSceneData cutsceneData)
		{
			if (!string.IsNullOrEmpty(cutsceneData.PlayedKeyName))
			{
				return PlayerPrefs.HasKey(createLocalKey(cutsceneData.PlayedKeyName));
			}
			Log.LogError(this, string.Format("Error: an Object Data field has a null PlayedKeyName entry"));
			return false;
		}

		protected void clearPlayerPrefs()
		{
			int num = Events.Length;
			for (int i = 0; i < num; i++)
			{
				string text = createLocalKey(Events[i].CutsceneData.PlayedKeyName);
				if (!string.IsNullOrEmpty(text) && PlayerPrefs.HasKey(text))
				{
					PlayerPrefs.DeleteKey(text);
				}
			}
		}

		protected bool isInEventScene()
		{
			return Service.Get<ZoneTransitionService>().CurrentZone.ZoneName == EventZone.ZoneName;
		}

		private string createLocalKey(string baseKey)
		{
			if (string.IsNullOrEmpty(baseKey))
			{
				return "";
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			return cPDataEntityCollection.GetComponent<DisplayNameData>(cPDataEntityCollection.LocalPlayerHandle).DisplayName + baseKey;
		}

		private void OnValidate()
		{
			EventName = EventName.Replace(" ", "");
			int num = Events.Length;
			for (int i = 0; i < num; i++)
			{
				Events[i].Name = Events[i].Name.Replace(" ", "");
				string playedKeyName = "";
				if (!string.IsNullOrEmpty(Events[i].CutsceneData.CutSceneAdditiveScene))
				{
					playedKeyName = EventName + "_" + Events[i].Name;
				}
				Events[i].CutsceneData.PlayedKeyName = playedKeyName;
			}
		}

		public virtual void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		private void incrementLoadCount(string resourceName, bool isDecoration)
		{
			if (!loadingCounts.ContainsKey(resourceName))
			{
				loadingCounts.Add(resourceName, new LoadingData(0, isDecoration));
			}
			LoadingData value = loadingCounts[resourceName];
			value.Count++;
			loadingCounts[resourceName] = value;
		}

		private int decrementLoadCount(string resourceName)
		{
			if (loadingCounts.ContainsKey(resourceName))
			{
				LoadingData value = loadingCounts[resourceName];
				value.Count--;
				loadingCounts[resourceName] = value;
				return loadingCounts[resourceName].Count;
			}
			return 0;
		}
	}
}
