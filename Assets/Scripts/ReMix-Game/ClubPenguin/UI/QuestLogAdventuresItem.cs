using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using ClubPenguin.Tutorial;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestLogAdventuresItem : MonoBehaviour
	{
		private const string COMPLETE_QUEST_TOKEN = "QuestLog.CompleteQuest.Text";

		private const string TIME_LOCKED_TOKEN = "QuestLog.TimeLocked.Text";

		private const string TALK_TO_MASCOT_TOKEN = "GlobalUI.Adventure.Menu.See";

		private const string TALK_TO_MASCOT_REPLAY_TOKEN = "GlobalUI.Adventure.Menu.ReplaySee";

		private const string REPLAY_QUEST_TOKEN = "GlobalUI.Adventure.Menu.Replay";

		public Text HeaderText;

		public Text CoinsText;

		public Text XPText;

		public GameObject CoinsCheck;

		public GameObject XPCheck;

		public GameObject MaxLevelIcon;

		public GameObject ContentPanel;

		public GameObject CompletedPanel;

		public GameObject StartButton;

		public GameObject ReplayButton;

		public GameObject ResumeButton;

		public GameObject ResumeButton2;

		public GameObject RestartButton;

		public GameObject CompletedButton;

		public GameObject GotoButton;

		public GameObject OffTextNextQuest;

		public GameObject OffTextTime;

		public GameObject LockedOverlay;

		public GameObject MemberLockIcon;

		public GameObject LevelLockIcon;

		public NotificationBreadcrumb breadcrumb;

		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		private QuestDefinition questData;

		private EventDispatcher dispatcher;

		private Quest associatedQuest;

		private MascotService mascotService;

		private bool replayQuest;

		private Timer updateTimer;

		private static readonly DPrompt cancelPromptData = new DPrompt("GlobalUI.Prompts.cancelQuestPromptTitleText", "GlobalUI.Prompts.cancelQuestPromptBodyText", DPrompt.ButtonFlags.CANCEL | DPrompt.ButtonFlags.OK);

		public int QuestNumber
		{
			get
			{
				return questData.QuestNumber;
			}
		}

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			mascotService = Service.Get<MascotService>();
			updateTimer = new Timer(1f, true, delegate
			{
				onTimerTick();
			});
		}

		public void OnEnable()
		{
			dispatcher.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
		}

		public void OnDisable()
		{
			dispatcher.RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			updateTimer.Stop();
			CoroutineRunner.StopAllForOwner(this);
		}

		public void LoadQuestData(QuestDefinition questData)
		{
			this.questData = questData;
			int num = 0;
			int num2 = 0;
			Quest.QuestState state = Quest.QuestState.Locked;
			Mascot mascot = Service.Get<MascotService>().GetMascot(questData.Mascot.name);
			foreach (Quest availableQuest in mascot.AvailableQuests)
			{
				if (availableQuest.Definition.name == questData.name)
				{
					state = availableQuest.State;
					associatedQuest = availableQuest;
					break;
				}
			}
			if ((associatedQuest == null || associatedQuest.TimesCompleted == 0) && questData.CompleteReward != null)
			{
				foreach (MascotXPRewardDefinition definition in questData.CompleteReward.GetDefinitions<MascotXPRewardDefinition>())
				{
					num += definition.XP;
				}
				num2 = CoinRewardableDefinition.Coins(questData.CompleteReward);
			}
			HeaderText.text = Service.Get<Localizer>().GetTokenTranslation(questData.Title);
			if (associatedQuest != null && associatedQuest.TimesCompleted > 0)
			{
				ContentPanel.SetActive(false);
				CompletedPanel.SetActive(true);
			}
			else
			{
				XPText.text = num.ToString();
				CoinsText.text = num2.ToString();
			}
			if (!XPCheck.activeSelf)
			{
				bool flag = Service.Get<ProgressionService>().IsMascotMaxLevel(mascot.Name);
				MaxLevelIcon.SetActive(flag);
				XPText.gameObject.SetActive(!flag);
			}
			breadcrumb.SetBreadcrumbId(BreadcrumbType, questData.name);
			ShowState(state);
		}

		private void ShowState(Quest.QuestState state)
		{
			if (associatedQuest != null && !associatedQuest.HasStartedQuest() && state == Quest.QuestState.Suspended)
			{
				state = ((associatedQuest.TimesCompleted != 0) ? Quest.QuestState.Completed : Quest.QuestState.Available);
			}
			if (questData.isMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				LockedOverlay.SetActive(true);
				MemberLockIcon.SetActive(true);
				LevelLockIcon.SetActive(false);
				return;
			}
			switch (state)
			{
			case Quest.QuestState.Active:
				ResumeButton.SetActive(false);
				StartButton.SetActive(false);
				ReplayButton.SetActive(false);
				CompletedButton.SetActive(false);
				GotoButton.SetActive(false);
				break;
			case Quest.QuestState.Available:
				ResumeButton.SetActive(false);
				if (questData.IsReplayable)
				{
					if (associatedQuest.TimesCompleted > 0)
					{
						ReplayButton.SetActive(true);
					}
					else if (Service.Get<MascotService>().ActiveMascot == null)
					{
						replayQuest = true;
						GotoButton.SetActive(true);
					}
					else
					{
						StartButton.SetActive(true);
						GotoButton.SetActive(false);
					}
					CompletedButton.SetActive(false);
				}
				else if (associatedQuest.TimesCompleted == 0)
				{
					if (Service.Get<MascotService>().ActiveMascot == null)
					{
						GotoButton.SetActive(true);
					}
					else
					{
						StartButton.SetActive(true);
						GotoButton.SetActive(false);
					}
					ReplayButton.SetActive(false);
					CompletedButton.SetActive(false);
				}
				else
				{
					StartButton.SetActive(false);
					ReplayButton.SetActive(false);
					CompletedButton.SetActive(true);
					GotoButton.SetActive(false);
				}
				break;
			case Quest.QuestState.Locked:
				ResumeButton.SetActive(false);
				StartButton.SetActive(false);
				ReplayButton.SetActive(false);
				CompletedButton.SetActive(false);
				GotoButton.SetActive(false);
				applyLockedState();
				break;
			case Quest.QuestState.Suspended:
				StartButton.SetActive(false);
				ReplayButton.SetActive(false);
				CompletedButton.SetActive(false);
				GotoButton.SetActive(false);
				ResumeButton.SetActive(true);
				break;
			case Quest.QuestState.Completed:
				ResumeButton.SetActive(false);
				StartButton.SetActive(false);
				if (!questData.IsReplayable)
				{
					CompletedButton.SetActive(true);
					GotoButton.SetActive(false);
				}
				else if (Service.Get<MascotService>().ActiveMascot == null)
				{
					replayQuest = true;
					GotoButton.SetActive(true);
					GotoButton.GetComponentInChildren<Text>().text = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Adventure.Menu.Replay");
				}
				else
				{
					ReplayButton.SetActive(true);
					GotoButton.SetActive(false);
				}
				break;
			}
		}

		public void OnStartClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.StartQuest(associatedQuest));
			Service.Get<ICPSwrveService>().Action("game.quest", "start", associatedQuest.Definition.name);
		}

		public void OnReplayClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.ReplayQuest(associatedQuest));
			Service.Get<ICPSwrveService>().Action("game.quest", "replay", associatedQuest.Definition.name);
		}

		public void OnCancelClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.SuspendQuest(associatedQuest));
			ShowState(Quest.QuestState.Suspended);
			Service.Get<ICPSwrveService>().Action("game.quest", "pause", associatedQuest.Definition.name);
		}

		public void OnResumeClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.ResumeQuest(associatedQuest));
			Service.Get<ICPSwrveService>().Action("game.quest", "resume", associatedQuest.Definition.name);
		}

		public void OnRestartClick()
		{
			Service.Get<PromptManager>().ShowPrompt(cancelPromptData, onPromptButtonPressed);
		}

		public void OnGotoClick()
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(questData.Mascot.name);
			Service.Get<TutorialManager>().SetTutorial(mascot.Definition.QuestReminderTutorialId, true);
			if (mascot == null)
			{
				return;
			}
			PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
			if (component != null)
			{
				SpawnedAction spawnedAction = new SpawnedAction();
				spawnedAction.Quest = associatedQuest;
				if (replayQuest)
				{
					spawnedAction.Action = SpawnedAction.SPAWNED_ACTION.ReplayQuest;
				}
				else
				{
					spawnedAction.Action = SpawnedAction.SPAWNED_ACTION.StartQuest;
				}
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(mascot.Definition.SpawnPlayerNearMascotPosition).Zone(mascot.Definition.Zone).SpawnedAction(spawnedAction).Build());
			}
		}

		private void onPromptButtonPressed(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.OK)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.RestartQuest(associatedQuest));
				ResumeButton2.SetActive(false);
				RestartButton.SetActive(false);
				if (Service.Get<MascotService>().ActiveMascot == null)
				{
					GotoButton.SetActive(true);
				}
				else
				{
					StartButton.SetActive(true);
				}
			}
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (associatedQuest != null && evt.Quest.Id == associatedQuest.Id)
			{
				ShowState(associatedQuest.State);
			}
			return false;
		}

		public void OnLockedStateClick()
		{
			if (MemberLockIcon.activeSelf)
			{
				Service.Get<GameStateController>().ShowAccountSystemMembership("adventures");
			}
		}

		private void applyLockedState()
		{
			int num = Service.Get<ProgressionService>().MascotLevel(questData.Mascot.name);
			if (num < questData.LevelRequirement)
			{
				LockedOverlay.SetActive(true);
				LevelLockIcon.SetActive(true);
				MemberLockIcon.SetActive(false);
				LevelLockIcon.GetComponentInChildren<Text>().text = questData.LevelRequirement.ToString();
				return;
			}
			Mascot mascot = null;
			QuestDefinition questDefinition = null;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < questData.CompletedQuestRequirement.Length; i++)
			{
				questDefinition = questData.CompletedQuestRequirement[i];
				mascot = mascotService.GetMascot(questDefinition.Mascot.name);
				foreach (Quest availableQuest in mascot.AvailableQuests)
				{
					if (availableQuest.Definition.name == questDefinition.name)
					{
						flag = true;
						if (availableQuest.TimesCompleted == 0)
						{
							flag2 = true;
							break;
						}
					}
				}
			}
			if (flag2 || !flag)
			{
				OffTextNextQuest.SetActive(true);
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("QuestLog.CompleteQuest.Text");
				tokenTranslation = string.Format(tokenTranslation, Service.Get<Localizer>().GetTokenTranslation(questDefinition.AbbreviatedTitle));
				OffTextNextQuest.GetComponentInChildren<Text>().text = tokenTranslation;
			}
			else if (associatedQuest != null)
			{
				associatedQuest.UpdateLockedState();
				if (associatedQuest.State != Quest.QuestState.Locked)
				{
					ShowState(associatedQuest.State);
					return;
				}
				OffTextTime.SetActive(true);
				onTimerTick();
				CoroutineRunner.Start(updateTimer.Start(), this, "QuestLogItemTimer");
			}
		}

		private void onTimerTick()
		{
			DateTime d = Service.Get<INetworkServicesManager>().GameTimeMilliseconds.MsToDateTime();
			DateTime d2 = associatedQuest.UnlockedTimeMilliseconds.MsToDateTime();
			long num = (long)(d2 - d).TotalMilliseconds;
			if (num > 0)
			{
				updateTimerText(num);
				return;
			}
			updateTimer.Stop();
			CoroutineRunner.StopAllForOwner(this);
			OffTextTime.SetActive(false);
			associatedQuest.UpdateLockedState();
			ShowState(associatedQuest.State);
		}

		private void updateTimerText(long time)
		{
			string arg = time.MsToDateTime().ToString("HH:mm:ss");
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("QuestLog.TimeLocked.Text");
			tokenTranslation = string.Format(tokenTranslation, arg);
			OffTextTime.GetComponentInChildren<Text>().text = tokenTranslation;
		}
	}
}
