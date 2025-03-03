using ClubPenguin.Analytics;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using ClubPenguin.World;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestService
	{
		public readonly PrefabContentKey splashScreenContentKey = new PrefabContentKey("Prefabs/Quest/SplashScreens/SplashScreen_*");

		public Dictionary<string, QuestDefinition> knownQuests = new Dictionary<string, QuestDefinition>();

		private Dictionary<string, Quest> availableQuests = new Dictionary<string, Quest>();

		private Dictionary<string, Mascot> questToMascotMap = new Dictionary<string, Mascot>();

		private List<Quest> timeLockedQuests = new List<Quest>();

		private EventDispatcher dispatcher;

		private EventChannel eventChannel;

		private bool pendingQuestSuspend;

		private Quest pendingQuest;

		private bool pendingQuestStart;

		private bool pendingQuestReplay;

		private readonly PersistentBreadcrumbTypeDefinitionKey breadcrumbType;

		private string lastEndScreenMusicPlayed;

		private SplashScreenController splashScreen;

		private Timer timeLockedCheckTimer;

		private bool isPlayerOutOfWorld = false;

		private Quest activeQuest;

		public bool QuestPlayerIsReady
		{
			get;
			set;
		}

		public string CurrentFishingPrize
		{
			get;
			set;
		}

		public bool IsSplashScreenOpen
		{
			get;
			private set;
		}

		public bool IsQuestDataRecieved
		{
			get;
			private set;
		}

		public IEnumerable<Quest> Quests
		{
			get
			{
				return availableQuests.Values;
			}
		}

		public IEnumerable<QuestDefinition> KnownQuests
		{
			get
			{
				return knownQuests.Values;
			}
		}

		public Dictionary<string, Mascot> QuestToMascotMap
		{
			get
			{
				return questToMascotMap;
			}
		}

		public string CurrentObjectiveText
		{
			get;
			set;
		}

		public bool IsQuestActionIndicatorsEnabled
		{
			get;
			private set;
		}

		public Quest ActiveQuest
		{
			get
			{
				return activeQuest;
			}
			private set
			{
				if (OnPrototypeQuest)
				{
					dispatcher.DispatchEvent(default(PrototypeExperienceEnd));
				}
				bool flag = activeQuest == null;
				activeQuest = value;
				if (flag)
				{
					dispatcher.DispatchEvent(default(QuestEvents.QuestStarted));
				}
				if (OnPrototypeQuest)
				{
					dispatcher.DispatchEvent(default(PrototypeExperienceStart));
				}
			}
		}

		public bool OnPrototypeQuest
		{
			get
			{
				return ActiveQuest != null && ActiveQuest.Definition.Prototyped;
			}
		}

		public bool IsQuestActive()
		{
			return ActiveQuest != null;
		}

		public QuestService(Manifest manifest, PersistentBreadcrumbTypeDefinitionKey breadcrumbType)
		{
			this.breadcrumbType = breadcrumbType;
			dispatcher = Service.Get<EventDispatcher>();
			MascotService mascotService = Service.Get<MascotService>();
			List<QuestDefinition> list = new List<QuestDefinition>();
			ScriptableObject[] assets = manifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(scriptableObject.name);
				Mascot mascot = mascotService.GetMascot(fileNameWithoutExtension);
				ScriptableObject[] assets2 = ((Manifest)scriptableObject).Assets;
				foreach (ScriptableObject scriptableObject2 in assets2)
				{
					QuestDefinition questDefinition = (QuestDefinition)scriptableObject2;
					knownQuests[scriptableObject2.name] = questDefinition;
					questToMascotMap[scriptableObject2.name] = mascot;
					list.Add(questDefinition);
				}
				mascot.KnownQuests = list.ToArray();
				list.Clear();
			}
			eventChannel = new EventChannel(dispatcher);
			eventChannel.AddListener<QuestEvents.SuspendQuest>(onSuspendQuestRequest);
			eventChannel.AddListener<QuestEvents.ResumeQuest>(onResumeQuestRequest);
			eventChannel.AddListener<QuestEvents.StartQuest>(onStartQuestRequest);
			eventChannel.AddListener<QuestEvents.ReplayQuest>(onReplayQuestRequest);
			eventChannel.AddListener<QuestEvents.RestartQuest>(onRestartQuestRequest);
			eventChannel.AddListener<SplashScreenEvents.SplashScreenOpened>(onSplashScreenOpened);
			eventChannel.AddListener<SplashScreenEvents.SplashScreenClosed>(onSplashScreenClosed);
			eventChannel.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			eventChannel.AddListener<QuestEvents.QuestInitializationComplete>(onQuestInitializationComplete);
			eventChannel.AddListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
			eventChannel.AddListener<QuestEvents.SetPlayerOutOfWorld>(onSetPlayerOutOfWorld);
			eventChannel.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
			eventChannel.AddListener<QuestEvents.RegisterQuestSubFsm>(onRegisterQuestSubFsm);
			eventChannel.AddListener<InWorldUIEvents.DisableActionIndicators>(onDisableActionIndicators);
			eventChannel.AddListener<InWorldUIEvents.EnableActionIndicators>(onEnableActionIndicators);
			eventChannel.AddListener<QuestServiceEvents.QuestStatesRecieved>(onQuestStatesReceived);
			eventChannel.AddListener<QuestServiceErrors.QuestProgressionError>(onQuestProgressionError);
			eventChannel.AddListener<ProgressionEvents.LevelUp>(onLevelUp);
			updateQuestGivers(0);
			timeLockedCheckTimer = new Timer(2f, true, delegate
			{
				onTimerTick();
			});
			CoroutineRunner.StartPersistent(timeLockedCheckTimer.Start(), this, "timeLockedQuestTimer");
		}

		private bool onDisableActionIndicators(InWorldUIEvents.DisableActionIndicators evt)
		{
			IsQuestActionIndicatorsEnabled = false;
			return false;
		}

		private bool onEnableActionIndicators(InWorldUIEvents.EnableActionIndicators evt)
		{
			IsQuestActionIndicatorsEnabled = true;
			return false;
		}

		public Quest GetQuest(string questName, bool createIfNotFound = true)
		{
			Quest result = null;
			if (knownQuests.ContainsKey(questName))
			{
				result = GetQuest(knownQuests[questName], null, createIfNotFound);
			}
			return result;
		}

		public Quest GetQuest(QuestDefinition questDef, Mascot mascot = null, bool createIfNotFound = true)
		{
			Quest value;
			if (!availableQuests.TryGetValue(questDef.name, out value) && createIfNotFound)
			{
				if (mascot == null)
				{
					mascot = questToMascotMap[questDef.name];
				}
				trace("Creating quest {0}", questDef.name);
				value = new Quest(questDef, mascot);
				availableQuests[questDef.name] = value;
				mascot.AvailableQuests.Add(value);
				value.StateChanged += onQuestStateChanged;
			}
			return value;
		}

		private void deactivateQuest(Quest.QuestState state, bool doCleanup = true)
		{
			if (ActiveQuest != null)
			{
				trace("Deactivating quest {0} with state {1}", ActiveQuest.Id, state);
				ActiveQuest.Deactivate(state);
				if (state == Quest.QuestState.Suspended)
				{
					Mascot mascot = questToMascotMap[ActiveQuest.Id];
					mascot.ResumableQuests.Add(ActiveQuest);
				}
				ActiveQuest = null;
				Service.Get<ZonePathing>().ClearWaypoint();
			}
			Service.Get<TutorialBreadcrumbController>().RemoveAllBreadcrumbs();
			if (doCleanup)
			{
				dispatcher.DispatchEvent(default(HudEvents.ResetQuestNotifier));
				RemotePlayerVisibilityState.ShowRemotePlayers();
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.ClearGroupCullingOverride));
			setPlayerOutOfWorld(false);
		}

		public void SendEvent(string eventName)
		{
			if (ActiveQuest != null)
			{
				ActiveQuest.SendEvent(eventName);
			}
		}

		public void EndQuest(GameObject player, string questName)
		{
			if (ActiveQuest != null)
			{
				trace("Ending quest {0}", questName);
				if (ActiveQuest.Definition.Prototyped)
				{
					dispatcher.DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.QUEST_COMPLETED, "", ActiveQuest.Definition.CompleteReward.ToReward()));
				}
				if (ActiveQuest.TimesCompleted > 0)
				{
					new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.replay, new Reward()).setMascotName(ActiveQuest.Mascot.Definition.name).Build().Execute();
				}
				pendingQuest = null;
				pendingQuestStart = false;
				pendingQuestReplay = false;
				Service.Get<ICPSwrveService>().EndTimer("questtime");
				dispatcher.DispatchEvent(new QuestEvents.QuestCompleted(ActiveQuest));
				MascotDefinition definition = ActiveQuest.Mascot.Definition;
				EventManager.Instance.PostEvent("MIX/MusicWorldOff", EventAction.PlaySound, null);
				lastEndScreenMusicPlayed = (string.IsNullOrEmpty(ActiveQuest.Definition.RewardPopupMusicOverride) ? definition.QuestEndMusic : ActiveQuest.Definition.RewardPopupMusicOverride);
				EventManager.Instance.PostEvent(lastEndScreenMusicPlayed, EventAction.PlaySound, null);
				deactivateQuest(Quest.QuestState.Completed);
				disableUI();
			}
		}

		private void disableUI()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
			SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "minNPC"));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
		}

		private bool onRewardPopupComplete(RewardEvents.RewardPopupComplete evt)
		{
			if (evt.RewardPopupData.PopupType == DRewardPopup.RewardPopupType.questComplete || evt.RewardPopupData.PopupType == DRewardPopup.RewardPopupType.replay)
			{
				EventManager.Instance.PostEvent(lastEndScreenMusicPlayed, EventAction.StopSound, null);
				EventManager.Instance.PostEvent("MIX/MusicWorldOn", EventAction.PlaySound, null);
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			}
			return false;
		}

		public void SetQuestItemState(GameObject player, string questName, string item, QuestItem.QuestItemState state, int itemCount)
		{
			QuestItem questItem = ActiveQuest.QuestItems[item];
			questItem.State = state;
			questItem.ItemCount = itemCount;
			dispatcher.DispatchEvent(new QuestEvents.QuestUpdated(ActiveQuest));
		}

		public QuestItem.QuestItemState GetQuestItemState(GameObject player, string questName, string item)
		{
			QuestItem questItem = ActiveQuest.QuestItems[item];
			return questItem.State;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State == Quest.QuestState.Active && evt.Quest != ActiveQuest)
			{
				if (ActiveQuest != null)
				{
					deactivateQuest(Quest.QuestState.Suspended);
				}
				ActiveQuest = evt.Quest;
			}
			else if (evt.Quest.State == Quest.QuestState.Completed && evt.Quest == ActiveQuest && evt.Quest.TimesCompleted == 1)
			{
				if (evt.Quest.Definition.name == Service.Get<GameStateController>().FTUEConfig.FtueQuestId)
				{
					foreach (Quest value in availableQuests.Values)
					{
						if (value.State == Quest.QuestState.Available)
						{
							Service.Get<NotificationBreadcrumbController>().AddPersistentBreadcrumb(breadcrumbType, value.Definition.name);
							string breadcrumbId = string.Format("{0}{1}", value.Definition.Mascot.AbbreviatedName, value.Definition.ChapterNumber);
							if (Service.Get<NotificationBreadcrumbController>().GetBreadcrumbCount(breadcrumbId) == 0)
							{
								Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(breadcrumbId);
							}
							string breadcrumbId2 = string.Format("{0}Quest", value.Mascot.AbbreviatedName);
							Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(breadcrumbId2);
						}
					}
				}
				else
				{
					bool flag = false;
					bool flag2 = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
					string name = evt.Quest.Definition.Mascot.name;
					int levelRequirement = evt.Quest.Definition.LevelRequirement;
					foreach (QuestDefinition value2 in knownQuests.Values)
					{
						if (value2.Mascot.name == name && value2.ChapterNumber == evt.Quest.Definition.ChapterNumber + 1 && levelRequirement == value2.LevelRequirement && flag2)
						{
							Service.Get<NotificationBreadcrumbController>().AddPersistentBreadcrumb(breadcrumbType, value2.name);
							string breadcrumbId = string.Format("{0}{1}", value2.Mascot.AbbreviatedName, value2.ChapterNumber);
							if (Service.Get<NotificationBreadcrumbController>().GetBreadcrumbCount(breadcrumbId) == 0)
							{
								Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(breadcrumbId);
							}
							flag = true;
						}
					}
					if (flag)
					{
						Mascot mascot = Service.Get<MascotService>().GetMascot(name);
						string breadcrumbId2 = string.Format("{0}Quest", mascot.AbbreviatedName);
						Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(breadcrumbId2);
					}
				}
			}
			return false;
		}

		private void onQuestStateChanged(Quest quest, Quest.QuestState newState)
		{
			if (newState == Quest.QuestState.Locked && quest.UnlockedTimeMilliseconds != 0 && !timeLockedQuests.Contains(quest))
			{
				timeLockedQuests.Add(quest);
			}
			quest.StateChanged -= onQuestStateChanged;
		}

		private bool onSuspendQuestRequest(QuestEvents.SuspendQuest evt)
		{
			trace("Suspending quest {0}", evt.Quest.Id);
			if (evt.Quest == ActiveQuest)
			{
				CoroutineRunner.Start(ShowSuspendQuestSplashscreen(evt.Quest), this, "Load splash screen");
				pendingQuestSuspend = true;
				Service.Get<ICPSwrveService>().Action("game.quest", "pause", evt.Quest.Mascot.Name, evt.Quest.Id);
			}
			else
			{
				Log.LogError(ActiveQuest, "Request to suspend incorrect quest: " + evt.Quest.Definition.name);
			}
			return false;
		}

		private bool onReplayQuestRequest(QuestEvents.ReplayQuest evt)
		{
			trace("onReplayQuestRequest quest {0}", evt.Quest.Id);
			CoroutineRunner.Start(ShowStartQuestSplashscreen(evt.Quest), this, "Load splash screen");
			pendingQuest = evt.Quest;
			pendingQuestReplay = true;
			return false;
		}

		private bool onStartQuestRequest(QuestEvents.StartQuest evt)
		{
			trace("Starting quest {0}", evt.Quest.Id);
			if (evt.Quest.Id != Service.Get<GameStateController>().FTUEConfig.FtueQuestId)
			{
				CoroutineRunner.Start(ShowStartQuestSplashscreen(evt.Quest), this, "Load splash screen");
				pendingQuest = evt.Quest;
				pendingQuestStart = true;
			}
			Service.Get<ICPSwrveService>().StartTimer("questtime", "quest." + evt.Quest.Definition.name);
			AdventureReminderTutorial.ClearReminderCount(evt.Quest.Mascot.Name);
			return false;
		}

		private bool onResumeQuestRequest(QuestEvents.ResumeQuest evt)
		{
			trace("Resuming quest {0}", evt.Quest.Id);
			CoroutineRunner.Start(ShowStartQuestSplashscreen(evt.Quest), this, "Load splash screen");
			pendingQuest = evt.Quest;
			pendingQuestStart = true;
			AdventureReminderTutorial.ClearReminderCount(evt.Quest.Mascot.Name);
			Service.Get<ICPSwrveService>().Action("game.quest", "resume", evt.Quest.Mascot.Name, evt.Quest.Id);
			return false;
		}

		private bool onRestartQuestRequest(QuestEvents.RestartQuest evt)
		{
			trace("Restarting quest {0}", evt.Quest.Id);
			Service.Get<INetworkServicesManager>().QuestService.RestartQuest(evt.Quest.Id);
			Mascot mascot = questToMascotMap[evt.Quest.Id];
			mascot.ResumableQuests.Remove(pendingQuest);
			AdventureReminderTutorial.ClearReminderCount(evt.Quest.Mascot.Name);
			return false;
		}

		private bool onQuestInitializationComplete(QuestEvents.QuestInitializationComplete evt)
		{
			ActiveQuest.RestartFSM();
			return false;
		}

		private bool onQuestStatesReceived(QuestServiceEvents.QuestStatesRecieved evt)
		{
			trace("Receiving quest states");
			CoroutineRunner.Start(applyReceivedQuestStates(evt), this, "applyReceivedQuestStates");
			return false;
		}

		private IEnumerator applyReceivedQuestStates(QuestServiceEvents.QuestStatesRecieved evt)
		{
			while (!QuestPlayerIsReady)
			{
				yield return null;
			}
			CPDataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
			foreach (QuestState questState in evt.QuestStates)
			{
				Mascot value;
				if (questToMascotMap.TryGetValue(questState.questId, out value))
				{
					QuestDefinition questDefinition = knownQuests[questState.questId];
					if (!questDefinition.Prototyped)
					{
						Quest quest = GetQuest(questDefinition, value);
						if (quest != null)
						{
							if (questState.status == QuestStatus.ACTIVE && (ActiveQuest == null || ActiveQuest != quest) && !quest.IsActivating && pendingQuest != quest && quest.Definition.IsPausable)
							{
								trace(Time.time + ": applyReceivedQuestStates() setting " + quest.Id + " to SUSPENDED");
								Service.Get<INetworkServicesManager>().QuestService.SetStatus(questState.questId, QuestStatus.SUSPENDED);
							}
							else
							{
								trace(Time.time + ": applyReceivedQuestStates() quest.UpdateState()");
								quest.UpdateState(questState);
								if (quest.State == Quest.QuestState.Suspended)
								{
									if (!value.ResumableQuests.Contains(quest))
									{
										value.ResumableQuests.Add(quest);
									}
								}
								else if (quest == ActiveQuest && !quest.IsActivating)
								{
									trace(Time.time + ": applyReceivedQuestStates() calling RestoreAsync()");
									GameObjectReferenceData component = dataEntityCollection.GetComponent<GameObjectReferenceData>(dataEntityCollection.LocalPlayerHandle);
									if (component != null)
									{
										quest.RestoreAsync(component.GameObject);
									}
								}
								if (quest.TimesCompleted == 0 && (quest.State == Quest.QuestState.Available || quest.State == Quest.QuestState.Suspended))
								{
									AdventureReminderTutorial.SetAvailableAdventureTimestamp(value.Name);
								}
							}
						}
					}
				}
			}
			dispatcher.DispatchEvent(default(QuestEvents.QuestSyncCompleted));
			IsQuestDataRecieved = true;
		}

		private bool onQuestProgressionError(QuestServiceErrors.QuestProgressionError evt)
		{
			if (ActiveQuest != null)
			{
				Service.Get<PromptManager>().ShowPrompt("AdventureErrorPrompt", delegate
				{
					if (!Service.Get<GameStateController>().IsFTUEComplete)
					{
						Service.Get<GameStateController>().ExitWorld();
					}
					else
					{
						dispatcher.DispatchEvent(new QuestEvents.SuspendQuest(ActiveQuest));
					}
				});
			}
			return false;
		}

		private bool onSceneTransition(SceneTransitionEvents.TransitionStart evt)
		{
			if (evt.SceneName == "Home")
			{
				forceSuspend();
			}
			return false;
		}

		private void forceSuspend()
		{
			Quest.QuestState state = Quest.QuestState.Suspended;
			if (activeQuest != null)
			{
				EventManager.Instance.PostEvent("MIX/MusicWorldOn", EventAction.PlaySound, null);
				if (!activeQuest.Definition.IsPausable)
				{
					state = Quest.QuestState.Active;
				}
			}
			deactivateQuest(state, false);
			foreach (Mascot value in questToMascotMap.Values)
			{
				value.AvailableQuests.Clear();
			}
			availableQuests.Clear();
			IsQuestDataRecieved = false;
		}

		public IEnumerator ShowStartQuestSplashscreen(Quest quest)
		{
			GameObject HUD = GameObject.FindWithTag(UIConstants.Tags.UI_HUD);
			if (HUD != null && HUD.GetComponentInChildren<PopupManager>() != null)
			{
				AssetRequest<GameObject> splashScreenPrefab = Content.LoadAsync(new PrefabContentKey(splashScreenContentKey, quest.Mascot.Name));
				yield return splashScreenPrefab;
				GameObject splashScreenObject = UnityEngine.Object.Instantiate(splashScreenPrefab.Asset);
				splashScreen = splashScreenObject.GetComponent<SplashScreenController>();
				splashScreen.SetMessage(Service.Get<Localizer>().GetTokenTranslation(quest.Definition.SplashScreenText));
				dispatcher.DispatchEvent(new PopupEvents.ShowPopup(splashScreenObject, false, true, "Accessibility.Popup.Title.QuestStart"));
				dispatcher.DispatchEvent(new TrayEvents.CloseTray(false, false));
				dispatcher.DispatchEvent(new HudEvents.PermanentlySuppressQuestNotifier(false, false));
				IsSplashScreenOpen = true;
				EventManager.Instance.PostEvent("MIX/MusicWorldOff", EventAction.PlaySound, null);
				EventManager.Instance.PostEvent(quest.Mascot.Definition.QuestIntroMusic, EventAction.PlaySound, null);
			}
		}

		public IEnumerator ShowSuspendQuestSplashscreen(Quest quest)
		{
			AssetRequest<GameObject> splashScreenPrefab = Content.LoadAsync(new PrefabContentKey(splashScreenContentKey, quest.Mascot.Name));
			yield return splashScreenPrefab;
			GameObject splashScreenObject = UnityEngine.Object.Instantiate(splashScreenPrefab.Asset);
			splashScreen = splashScreenObject.GetComponent<SplashScreenController>();
			splashScreen.SetMessage(Service.Get<Localizer>().GetTokenTranslation(quest.Definition.SplashScreenText));
			dispatcher.DispatchEvent(new PopupEvents.ShowPopup(splashScreenObject, false, true, "Accessibility.Popup.Title.QuestStop"));
			dispatcher.DispatchEvent(new TrayEvents.CloseTray(false, false));
			IsSplashScreenOpen = true;
			EventManager.Instance.PostEvent("MIX/MusicWorldOn", EventAction.PlaySound, null);
		}

		private bool onSplashScreenOpened(SplashScreenEvents.SplashScreenOpened evt)
		{
			if (pendingQuestSuspend)
			{
				Mascot mascot = questToMascotMap[ActiveQuest.Id];
				mascot.InteractionBehaviours.Reset();
				deactivateQuest(Quest.QuestState.Suspended);
				pendingQuestSuspend = false;
			}
			else if (pendingQuestStart || pendingQuestReplay)
			{
				if (pendingQuestReplay)
				{
					pendingQuest.Replay();
					pendingQuestReplay = false;
				}
				pendingQuest.Activate();
				Mascot mascot = questToMascotMap[pendingQuest.Id];
				mascot.ResumableQuests.Remove(pendingQuest);
				mascot.InteractionBehaviours.Reset();
				pendingQuestStart = false;
				pendingQuest = null;
			}
			dispatcher.DispatchEvent(new TrayEvents.SelectTrayScreen("ControlsScreen"));
			return false;
		}

		private bool onSplashScreenClosed(SplashScreenEvents.SplashScreenClosed evt)
		{
			IsSplashScreenOpen = false;
			return false;
		}

		private void updateQuestGivers(int playerLevel)
		{
			trace("Updating quest givers for playerlevel {0}", playerLevel);
			foreach (KeyValuePair<string, QuestDefinition> knownQuest in knownQuests)
			{
				if (!availableQuests.ContainsKey(knownQuest.Key))
				{
					QuestDefinition value = knownQuest.Value;
					bool flag = playerLevel >= value.LevelRequirement;
					if (flag)
					{
						trace("Checking completed requirements for quest {0}", knownQuest.Key);
						QuestDefinition[] completedQuestRequirement = value.CompletedQuestRequirement;
						foreach (QuestDefinition questDefinition in completedQuestRequirement)
						{
							Quest value2;
							if (!availableQuests.TryGetValue(questDefinition.name, out value2))
							{
								flag = false;
								break;
							}
							if (value2.State != Quest.QuestState.Completed)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						Mascot mascot = questToMascotMap[value.name];
						GetQuest(value, mascot);
					}
				}
			}
		}

		private bool onLevelUp(ProgressionEvents.LevelUp evt)
		{
			bool flag = false;
			bool flag2 = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			foreach (QuestDefinition value in knownQuests.Values)
			{
				if (value.Mascot.name == evt.MascotName)
				{
					if (value.LevelRequirement >= evt.MascotLevel)
					{
						Quest quest = GetQuest(value.name);
						quest.UpdateLockedState();
					}
					if (evt.MascotLevel == value.LevelRequirement && flag2)
					{
						Service.Get<NotificationBreadcrumbController>().AddPersistentBreadcrumb(breadcrumbType, value.name);
						string breadcrumbId = string.Format("{0}{1}", value.Mascot.AbbreviatedName, value.ChapterNumber);
						if (Service.Get<NotificationBreadcrumbController>().GetBreadcrumbCount(breadcrumbId) == 0)
						{
							Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(breadcrumbId);
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				Mascot mascot = Service.Get<MascotService>().GetMascot(evt.MascotName);
				string breadcrumbId2 = string.Format("{0}Quest", mascot.AbbreviatedName);
				Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(breadcrumbId2);
			}
			return false;
		}

		private bool onSetPlayerOutOfWorld(QuestEvents.SetPlayerOutOfWorld evt)
		{
			setPlayerOutOfWorld(evt.IsOutOfWorld);
			return false;
		}

		private void setPlayerOutOfWorld(bool isOutOfWorld)
		{
			if (isPlayerOutOfWorld == isOutOfWorld)
			{
				return;
			}
			if (SceneRefs.ZoneLocalPlayerManager != null)
			{
				GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				if (localPlayerGameObject != null)
				{
					LocomotionEventBroadcaster component = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
					if (isOutOfWorld)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.AwayFromWorld));
						if (component != null)
						{
							component.BroadcastOnControlsLocked();
						}
					}
					else
					{
						Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Here));
						if (component != null)
						{
							component.BroadcastOnControlsUnLocked();
						}
					}
				}
			}
			isPlayerOutOfWorld = isOutOfWorld;
		}

		private bool onRegisterQuestSubFsm(QuestEvents.RegisterQuestSubFsm evt)
		{
			if (ActiveQuest != null)
			{
				ActiveQuest.RegisterQuestSubFsm(evt.QuestFsm);
			}
			return false;
		}

		private void onTimerTick()
		{
			DateTime d = Service.Get<INetworkServicesManager>().GameTimeMilliseconds.MsToDateTime();
			List<Quest> list = new List<Quest>();
			NotificationBreadcrumbController notificationBreadcrumbController = Service.Get<NotificationBreadcrumbController>();
			for (int i = 0; i < timeLockedQuests.Count; i++)
			{
				Quest quest = timeLockedQuests[i];
				DateTime d2 = quest.UnlockedTimeMilliseconds.MsToDateTime();
				long num = (long)(d2 - d).TotalMilliseconds;
				if (num > 0)
				{
					continue;
				}
				quest.UpdateLockedState();
				if (quest.State == Quest.QuestState.Available)
				{
					list.Add(quest);
					notificationBreadcrumbController.AddPersistentBreadcrumb(breadcrumbType, quest.Definition.name);
					string breadcrumbId = string.Format("{0}{1}", quest.Definition.Mascot.AbbreviatedName, quest.Definition.ChapterNumber);
					if (Service.Get<NotificationBreadcrumbController>().GetBreadcrumbCount(breadcrumbId) == 0)
					{
						Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(breadcrumbId);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				string breadcrumbId2 = string.Format("{0}Quest", list[i].Mascot.AbbreviatedName);
				if (notificationBreadcrumbController.GetBreadcrumbCount(breadcrumbId2) == 0)
				{
					notificationBreadcrumbController.AddBreadcrumb(breadcrumbId2);
				}
				timeLockedQuests.Remove(list[i]);
			}
		}

		private void trace(string format, params object[] objs)
		{
		}
	}
}
