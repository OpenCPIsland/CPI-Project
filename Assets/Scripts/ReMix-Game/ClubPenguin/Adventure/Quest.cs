using ClubPenguin.Core;
using ClubPenguin.Game.Adventure;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Adventure
{
	public class Quest
	{
		public enum QuestState
		{
			Available,
			Active,
			Suspended,
			Completed,
			Locked
		}

		public delegate void StateChangedHandler(Quest quest, QuestState newState);

		private readonly string INITIALIZE_STATE_NAME = "InitializeQuest";

		private readonly string CLEANUP_STATE_NAME = "CleanupQuest";

		private readonly TypedAssetContentKey<FsmTemplate> fsmTemplateKey = new TypedAssetContentKey<FsmTemplate>("Quests/*/QuestFsm.*");

		private readonly TypedAssetContentKey<GameObject> prefabTemplateKey = new TypedAssetContentKey<GameObject>("Quests/*/Zones/*/*");

		private readonly PrefabContentKey audioContentKey = new PrefabContentKey("Quests/*/Audio/QuestAudio.*");

		public readonly QuestDefinition Definition;

		public readonly Mascot Mascot;

		public Dictionary<string, QuestItem> QuestItems = new Dictionary<string, QuestItem>();

		private bool offline = false;

		private QuestState state;

		private long unlockedTimeMilliseconds = 0L;

		private GameObject questAudioInstance;

		private PlayMakerFSM playMakerFsm;

		private FsmTemplate fsmTemplate;

		private Dictionary<string, Fsm> QuestSubFsms;

		private EventDispatcher dispatcher;

		private HashSet<string> allObjectiveNames = new HashSet<string>();

		private List<string> sortedObjectives = new List<string>();

		public Dictionary<string, int> ObjectiveRewardIndexes = new Dictionary<string, int>();

		private HashSet<string> serverControlledAllTimeCompletedObjectives = new HashSet<string>();

		public string CurrentObjectiveName
		{
			get;
			private set;
		}

		public string CurrentObjectiveDescription
		{
			get;
			private set;
		}

		public string Id
		{
			get
			{
				return Definition.name;
			}
		}

		public bool Offline
		{
			get
			{
				return offline;
			}
		}

		public int TimesCompleted
		{
			get;
			private set;
		}

		public bool IsRestoringAsync
		{
			get;
			private set;
		}

		public bool IsActivating
		{
			get;
			private set;
		}

		public QuestState State
		{
			get
			{
				return state;
			}
			private set
			{
				trace("Changing state from {0} to {1}", state, value);
				if (state == value)
				{
					return;
				}
				state = value;
				QuestStatus questStatus = QuestStatus.AVAILABLE;
				switch (state)
				{
				case QuestState.Available:
					questStatus = QuestStatus.AVAILABLE;
					break;
				case QuestState.Active:
					questStatus = QuestStatus.ACTIVE;
					break;
				case QuestState.Suspended:
					questStatus = QuestStatus.SUSPENDED;
					break;
				case QuestState.Completed:
					questStatus = QuestStatus.COMPLETED;
					break;
				case QuestState.Locked:
					questStatus = QuestStatus.LOCKED;
					break;
				}
				if (questStatus != 0 && questStatus != QuestStatus.LOCKED)
				{
					INetworkServicesManager networkServicesManager = Service.Get<INetworkServicesManager>();
					if (networkServicesManager != null && !Definition.Prototyped)
					{
						networkServicesManager.QuestService.SetStatus(Definition.name, questStatus);
					}
				}
				dispatcher.DispatchEvent(new QuestEvents.QuestUpdated(this));
			}
		}

		public long UnlockedTimeMilliseconds
		{
			get
			{
				return unlockedTimeMilliseconds;
			}
		}

		public GameObject PrefabInstance
		{
			get;
			private set;
		}

		public List<string> Objectives
		{
			get
			{
				return sortedObjectives;
			}
		}

		public event StateChangedHandler StateChanged;

		public Quest(QuestDefinition dQuest, Mascot mascot)
		{
			dispatcher = Service.Get<EventDispatcher>();
			Definition = dQuest;
			Mascot = mascot;
			QuestSubFsms = new Dictionary<string, Fsm>();
			state = QuestState.Locked;
			resetQuestItems();
		}

		private void resetQuestItems()
		{
			QuestDefinition.DQuestItem[] questItems = Definition.QuestItems;
			for (int i = 0; i < questItems.Length; i++)
			{
				QuestDefinition.DQuestItem dataModel = questItems[i];
				QuestItems[dataModel.Name] = new QuestItem(dataModel);
			}
		}

		public void SetOffline()
		{
			offline = true;
		}

		public void SetOnline()
		{
			if (offline)
			{
				offline = false;
			}
		}

		public bool HasStartedQuest()
		{
			return serverControlledAllTimeCompletedObjectives.Count > 0;
		}

		public void Replay()
		{
			serverControlledAllTimeCompletedObjectives.Clear();
		}

		public void Activate()
		{
			trace("Activating quest");
			IsActivating = true;
			CoroutineRunner.Start(load(), this, "QuestLoader: " + Definition.name);
			dispatcher.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionRequest);
			addAdventureActionFilter();
		}

		public void Deactivate(QuestState state)
		{
			trace("Deactivating quest");
			IsActivating = false;
			if (playMakerFsm != null)
			{
				FsmState fsmState = playMakerFsm.Fsm.GetState(CLEANUP_STATE_NAME);
				if (fsmState != null)
				{
					RestartFSM(CLEANUP_STATE_NAME);
					playMakerFsm.Fsm.Stop();
				}
			}
			QuestSubFsms.Clear();
			dispatcher.RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionRequest);
			fsmTemplate = null;
			foreach (Mascot mascot in Service.Get<MascotService>().Mascots)
			{
				mascot.ActiveQuestDialog = null;
				mascot.InteractionBehaviours.Reset();
			}
			resetQuestItems();
			if (questAudioInstance != null)
			{
				unloadFabricAudio(questAudioInstance);
				UnityEngine.Object.Destroy(questAudioInstance);
			}
			if (PrefabInstance != null)
			{
				UnityEngine.Object.Destroy(PrefabInstance);
			}
			string questKey = getQuestKey();
			if (state == QuestState.Completed)
			{
				if (Definition.Prototyped)
				{
					PlayerPrefs.DeleteKey(questKey);
				}
				TimesCompleted++;
				CurrentObjectiveName = null;
			}
			else if (Definition.Prototyped)
			{
				PlayerPrefs.SetString(questKey, CurrentObjectiveName);
			}
			State = state;
			removePlayMakerFsm();
			rebootScheduledEvents();
			removeAdventureActionFilter();
		}

		private void unloadFabricAudio(GameObject gameObject)
		{
			AudioComponent[] componentsInChildren = gameObject.GetComponentsInChildren<AudioComponent>(true);
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				AudioComponent audioComponent = componentsInChildren[i];
				audioComponent.Stop();
				audioComponent.UnloadAudio();
				AudioClip audioClip = audioComponent.AudioClip;
				if (audioClip != null && !audioClip.UnloadAudioData())
				{
					Log.LogError(this, "Failed to unload audio data for: " + audioClip.name);
				}
			}
		}

		private string getQuestKey()
		{
			return "Quest." + Definition.name + ".currentFsmState";
		}

		private bool onSceneTransitionRequest(SceneTransitionEvents.TransitionStart evt)
		{
			if (playMakerFsm != null && !string.IsNullOrEmpty(playMakerFsm.Fsm.ActiveStateName))
			{
				trace("Saving root FSM Variables {0}", playMakerFsm.Fsm.Name);
				saveFSMVariables(playMakerFsm.Fsm);
			}
			return false;
		}

		public void StartObjective(string name, string description, bool notifyPlayer)
		{
			trace("Starting objective {0}", name);
			if (allObjectiveNames.Contains(name))
			{
				CurrentObjectiveName = name;
				CurrentObjectiveDescription = description;
				trace("NEW OBJECTIVE: {0}", name);
				dispatcher.DispatchEvent(new QuestEvents.QuestUpdated(this));
				dispatcher.DispatchEvent(default(HudEvents.DestroySubtaskText));
				if (notifyPlayer)
				{
					dispatcher.DispatchEvent(new HudEvents.SetObjectiveText(description));
				}
			}
			Service.Get<MascotService>().ResetInteractionBehaviours();
		}

		public void CompleteObjective(string name, string description)
		{
			trace("Completing objective {0}", name);
			dispatcher.DispatchEvent(new QuestEvents.QuestUpdated(this));
			dispatcher.DispatchEvent(default(HudEvents.DestroySubtaskText));
			dispatcher.DispatchEvent(default(HudEvents.SetObjectiveText));
			completeObjective(name);
			dispatcher.DispatchEvent(default(QuestEvents.CancelQuestHint));
		}

		private void completeObjective(string name)
		{
			bool flag = !serverControlledAllTimeCompletedObjectives.Contains(name);
			INetworkServicesManager networkServicesManager = Service.Get<INetworkServicesManager>();
			if (networkServicesManager != null && allObjectiveNames.Contains(name) && flag && !Definition.Prototyped)
			{
				networkServicesManager.QuestService.CompleteObjective(name);
				serverControlledAllTimeCompletedObjectives.Add(name);
			}
		}

		public bool IsObjectiveComplete(string name)
		{
			return serverControlledAllTimeCompletedObjectives.Contains(name);
		}

		public void UpdateState(ClubPenguin.Net.Domain.QuestState newState)
		{
			trace("Updating state from server {0} -> {1}", state, newState.status);
			serverControlledAllTimeCompletedObjectives.Clear();
			if (newState.completedObjectives != null)
			{
				foreach (string completedObjective in newState.completedObjectives)
				{
					serverControlledAllTimeCompletedObjectives.Add(completedObjective);
				}
			}
			TimesCompleted = newState.timesCompleted;
			if (!IsActivating)
			{
				switch (newState.status)
				{
				case QuestStatus.AVAILABLE:
					state = QuestState.Available;
					break;
				case QuestStatus.ACTIVE:
					state = QuestState.Active;
					break;
				case QuestStatus.SUSPENDED:
					state = QuestState.Suspended;
					break;
				case QuestStatus.COMPLETED:
					state = QuestState.Completed;
					break;
				case QuestStatus.LOCKED:
					state = QuestState.Locked;
					unlockedTimeMilliseconds = newState.unlockTime.Value;
					break;
				}
				if (this.StateChanged != null)
				{
					this.StateChanged(this, state);
				}
			}
		}

		public void RegisterQuestSubFsm(Fsm questFsm)
		{
			if (!QuestSubFsms.ContainsKey(questFsm.Name))
			{
				QuestSubFsms.Add(questFsm.Name, questFsm);
				return;
			}
			QuestSubFsms[questFsm.Name] = questFsm;
			ActiveQuestData sessionPersistentContainer = getSessionPersistentContainer();
			sessionPersistentContainer.LoadFSMVariables(questFsm, questFsm.Name);
		}

		private void saveFSMVariables(Fsm fsm)
		{
			ActiveQuestData sessionPersistentContainer = getSessionPersistentContainer();
			if (sessionPersistentContainer != null)
			{
				sessionPersistentContainer.SaveFSMVariables(fsm, fsm.Name);
				foreach (Fsm value in QuestSubFsms.Values)
				{
					sessionPersistentContainer.SaveFSMVariables(value, value.Name);
				}
			}
		}

		public void RestoreAsync(GameObject player)
		{
			CoroutineRunner.Start(restoreAsync(player), this, "restoreAsync: " + Definition.name);
		}

		private IEnumerator restoreAsync(GameObject player)
		{
			while (IsRestoringAsync)
			{
				yield return null;
			}
			trace("Restoring quest to its last saved objective");
			IsRestoringAsync = true;
			if (questAudioInstance == null)
			{
				AssetRequest<GameObject> assetRequest;
				if (Content.TryLoadAsync(out assetRequest, audioContentKey, Id, Id))
				{
					yield return assetRequest;
					questAudioInstance = UnityEngine.Object.Instantiate(assetRequest.Asset);
					UnityEngine.Object.DontDestroyOnLoad(questAudioInstance);
				}
				else
				{
					Log.LogErrorFormatted(this, "Quest audio prefab not found for quest: {0}", Id);
				}
			}
			trace("attempting to load zone-specific quest prefab");
			AssetRequest<GameObject> prefabResult;
			if (PrefabInstance == null && Content.TryLoadAsync(out prefabResult, prefabTemplateKey, Definition.name, SceneManager.GetActiveScene().name, Definition.name))
			{
				yield return prefabResult;
				trace("Loaded quest prefab {0}", prefabResult.Asset.name);
				PrefabInstance = UnityEngine.Object.Instantiate(prefabResult.Asset);
			}
			if (playMakerFsm == null)
			{
				addPlayMakerFSM(player);
				restoreFromCompletedObjectives();
				ActiveQuestData sessionPersistentContainer = getSessionPersistentContainer();
				sessionPersistentContainer.LoadFSMVariables(playMakerFsm.Fsm, playMakerFsm.Fsm.Name);
				FsmState fsmState = playMakerFsm.Fsm.GetState(INITIALIZE_STATE_NAME);
				if (fsmState == null)
				{
					RestartFSM();
				}
				else
				{
					playMakerFsm.Fsm.Stop();
					playMakerFsm.Fsm.SetState(fsmState.Name);
					playMakerFsm.Fsm.Start();
				}
			}
			rebootScheduledEvents();
			IsRestoringAsync = false;
		}

		private void restoreFromCompletedObjectives()
		{
			if (!Definition.Prototyped)
			{
				foreach (FsmState fsmState in getFsmStates())
				{
					ObjectiveAction objectiveAction = getObjectiveAction(fsmState);
					if (objectiveAction != null)
					{
						if (!serverControlledAllTimeCompletedObjectives.Contains(fsmState.Name))
						{
							trace("Objective name not found: " + fsmState.Name + " " + serverControlledAllTimeCompletedObjectives.Count);
							break;
						}
						CurrentObjectiveName = getNextState(fsmState).Name;
						trace("Setting CurrentObjectiveName = " + CurrentObjectiveName + " " + serverControlledAllTimeCompletedObjectives.Count);
					}
				}
			}
		}

		private void addPlayMakerFSM(GameObject player)
		{
			playMakerFsm = player.AddComponent<PlayMakerFSM>();
			playMakerFsm.Fsm.Name = Definition.name;
			try
			{
				playMakerFsm.SetFsmTemplate(fsmTemplate);
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
			}
			foreach (FsmState fsmState in getFsmStates())
			{
				ObjectiveAction objectiveAction = getObjectiveAction(fsmState);
				if (objectiveAction != null)
				{
					allObjectiveNames.Add(fsmState.Name);
					sortedObjectives.Add(fsmState.Name);
					ObjectiveRewardIndexes[fsmState.Name] = objectiveAction.DefinitionRewardIndex;
					if (CurrentObjectiveName == null)
					{
						CurrentObjectiveName = fsmState.Name;
					}
				}
			}
		}

		private void removePlayMakerFsm()
		{
			if (playMakerFsm != null)
			{
				UnityEngine.Object.Destroy(playMakerFsm);
				playMakerFsm = null;
				allObjectiveNames.Clear();
				sortedObjectives.Clear();
				ObjectiveRewardIndexes.Clear();
				CurrentObjectiveName = null;
			}
		}

		public void RestartFSM(string startState = null)
		{
			if (playMakerFsm != null)
			{
				playMakerFsm.Fsm.Stop();
				if (startState != null)
				{
					trace("FSM init done. Setting StartState = " + startState);
					playMakerFsm.Fsm.StartState = startState;
				}
				else if (!string.IsNullOrEmpty(CurrentObjectiveName))
				{
					trace("Restoring quest root fsm to state {0}", CurrentObjectiveName);
					playMakerFsm.Fsm.StartState = CurrentObjectiveName;
				}
				else
				{
					trace("Restoring quest root FSM to default start state ");
				}
				playMakerFsm.Fsm.Start();
			}
		}

		private IEnumerator load()
		{
			trace("LOADING QUEST DATA...");
			string assetName = Id;
			if (questAudioInstance == null)
			{
				AssetRequest<GameObject> assetRequest;
				if (Content.TryLoadAsync(out assetRequest, audioContentKey, assetName, assetName))
				{
					yield return assetRequest;
					questAudioInstance = UnityEngine.Object.Instantiate(assetRequest.Asset);
					UnityEngine.Object.DontDestroyOnLoad(questAudioInstance);
				}
				else
				{
					Log.LogErrorFormatted(this, "Quest audio prefab not found for quest: {0}", assetName);
				}
			}
			trace("attempting to load zone-specific quest prefab");
			AssetRequest<GameObject> prefabResult;
			if (PrefabInstance == null && Content.TryLoadAsync(out prefabResult, prefabTemplateKey, assetName, SceneManager.GetActiveScene().name, assetName))
			{
				yield return prefabResult;
				trace("Loaded quest prefab {0}", prefabResult.Asset.name);
				PrefabInstance = UnityEngine.Object.Instantiate(prefabResult.Asset);
			}
			List<CoroutineReturn> results = new List<CoroutineReturn>();
			AssetRequest<FsmTemplate> fsmTemplateResult = Content.LoadAsync(fsmTemplateKey, assetName, assetName);
			results.Add(fsmTemplateResult);
			CompositeCoroutineReturn waiter = new CompositeCoroutineReturn(results.ToArray());
			while (!waiter.Finished)
			{
				yield return null;
			}
			fsmTemplate = fsmTemplateResult.Asset;
			if (Definition.Prototyped)
			{
				string questKey = getQuestKey();
				if (PlayerPrefs.HasKey(questKey))
				{
					CurrentObjectiveName = PlayerPrefs.GetString(questKey);
					trace("Getting saved state {0} from playerprefs", CurrentObjectiveName);
				}
			}
			if (State != QuestState.Active)
			{
				State = QuestState.Active;
			}
			else
			{
				dispatcher.DispatchEvent(new QuestEvents.QuestUpdated(this));
			}
			if (Definition.Prototyped || offline)
			{
				RestoreAsync(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject);
			}
			IsActivating = false;
		}

		private static ActiveQuestData getSessionPersistentContainer()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle dataEntityHandle = cPDataEntityCollection.GetEntityByType<ActiveQuestData>();
			if (dataEntityHandle.IsNull)
			{
				dataEntityHandle = cPDataEntityCollection.AddEntity("ActiveQuestData");
				cPDataEntityCollection.AddComponent<ActiveQuestData>(dataEntityHandle);
			}
			return cPDataEntityCollection.GetComponent<ActiveQuestData>(dataEntityHandle);
		}

		public void SendEvent(string evt)
		{
			List<Fsm> subFsmList = playMakerFsm.Fsm.SubFsmList;
			if (subFsmList != null)
			{
				for (int i = 0; i < subFsmList.Count; i++)
				{
					subFsmList[i].Event(evt);
				}
			}
			playMakerFsm.Fsm.Event(evt);
		}

		public void UpdateLockedState()
		{
			bool flag = false;
			if (Definition.isMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				flag = true;
			}
			if (!flag)
			{
				int num = Service.Get<ProgressionService>().MascotLevel(Definition.Mascot.name);
				if (num < Definition.LevelRequirement)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				Mascot mascot = null;
				QuestDefinition questDefinition = null;
				bool flag2 = false;
				bool flag3 = false;
				for (int i = 0; i < Definition.CompletedQuestRequirement.Length; i++)
				{
					questDefinition = Definition.CompletedQuestRequirement[i];
					mascot = Service.Get<MascotService>().GetMascot(questDefinition.Mascot.name);
					foreach (Quest availableQuest in mascot.AvailableQuests)
					{
						if (availableQuest.Definition.name == questDefinition.name)
						{
							flag2 = true;
							if (availableQuest.TimesCompleted == 0)
							{
								flag3 = true;
								break;
							}
						}
					}
				}
				if (flag3 || !flag2)
				{
					flag = true;
				}
			}
			if (!flag && unlockedTimeMilliseconds > Service.Get<INetworkServicesManager>().GameTimeMilliseconds)
			{
				flag = true;
			}
			if (!flag)
			{
				state = QuestState.Available;
			}
		}

		private void rebootScheduledEvents()
		{
			GameObject gameObject = GameObject.Find("ScheduledEvents");
			if (gameObject != null)
			{
				foreach (Transform item in gameObject.transform)
				{
					GameObject gameObject2 = item.gameObject;
					PlayMakerFSM component = gameObject2.GetComponent<PlayMakerFSM>();
					if (component != null)
					{
						component.Fsm.RestartOnEnable = true;
						component.Fsm.Stop();
						component.Fsm.Start();
						component.Fsm.RestartOnEnable = false;
					}
				}
			}
		}

		public override string ToString()
		{
			string[] array = new string[allObjectiveNames.Count];
			int count = 0;
			foreach (string allObjectiveName in allObjectiveNames)
			{
				array[count++] = allObjectiveName;
			}
			return string.Format("[Quest: Id={0}, State={1}, Objectives={2}]", Id, State, string.Join(",", array, 0, count));
		}

		private void trace(string format, params object[] objs)
		{
		}

		private IEnumerable<FsmState> getFsmStates()
		{
			if (fsmTemplate == null || playMakerFsm == null)
			{
				yield break;
			}
			HashSet<FsmState> visitedStates = new HashSet<FsmState>();
			FsmState state = playMakerFsm.Fsm.GetState(fsmTemplate.fsm.StartState);
			do
			{
				if (state != null)
				{
					yield return state;
					visitedStates.Add(state);
					state = getNextState(state);
					continue;
				}
				yield break;
			}
			while (!visitedStates.Contains(state));
			Log.LogError(this, "State loop found in quest " + Id);
		}

		private ObjectiveAction getObjectiveAction(FsmState state)
		{
			for (int i = 0; i < state.Actions.Length; i++)
			{
				if (state.Actions[i] is ObjectiveAction)
				{
					return (ObjectiveAction)state.Actions[i];
				}
			}
			return null;
		}

		private string getObjectiveDescription(string name)
		{
			FsmState fsmState = playMakerFsm.Fsm.GetState(name);
			if (fsmState != null)
			{
				return fsmState.Description;
			}
			return "<UNKNOWN>";
		}

		private FsmState getNextState(FsmState state)
		{
			if (state.Transitions != null && state.Transitions.Length > 0)
			{
				return playMakerFsm.Fsm.GetState(state.Transitions[0].ToState);
			}
			return null;
		}

		private void addAdventureActionFilter()
		{
			Service.Get<ActionConfirmationService>().AddFilter(new AdventureActionConfirmationFilter());
		}

		private void removeAdventureActionFilter()
		{
			Service.Get<ActionConfirmationService>().RemoveFilter("adventure_action_filter");
		}
	}
}
