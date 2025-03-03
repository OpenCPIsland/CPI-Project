using ClubPenguin.Core;
using ClubPenguin.Progression;
using ClubPenguin.UI;
using ClubPenguin.World;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[RequireComponent(typeof(Animator))]
	public class MascotController : MonoBehaviour
	{
		public enum States
		{
			Idle,
			Dialog,
			GivingQuest,
			WaitingForSplashscreen
		}

		private static readonly int DialogAnimHash = Animator.StringToHash("Dialog");

		private static readonly int IdleAnimHash = Animator.StringToHash("PreIdle");

		public string MascotName;

		public string QuestIndicatorContentKey = "Prefabs/ActionIndicators/ActionIndicatorNpc";

		public Vector3 QuestIndicatorOffset = Vector3.up;

		public PlayMakerFSM TalkPlayermakerFSMOverride;

		private Animator animator;

		private EventDispatcher dispatcher;

		private QuestService questService;

		private MascotService mascotService;

		private DCinematicSpeech speechData;

		private DActionIndicator indicatorData;

		private bool isShowingIndicator = true;

		private bool hasCheckedIndicator = false;

		private string currentAudioEvent;

		private bool autoStopDialogAnim;

		private string overrideStopAnimationName;

		private string overrideIdleAnimationName;

		private DDialogPanel[] dialogPanels;

		private int activePanelIndex;

		private DDialogPanel[] dynamicPanels = new DDialogPanel[1];

		private States state;

		public Mascot Mascot
		{
			get;
			private set;
		}

		public MascotDefinition Definition
		{
			get
			{
				return Mascot.Definition;
			}
		}

		public bool IsGivingQuest
		{
			get
			{
				return state == States.GivingQuest;
			}
		}

		public bool AutoStopDialog
		{
			get
			{
				return autoStopDialogAnim;
			}
			set
			{
				autoStopDialogAnim = value;
			}
		}

		public void Awake()
		{
			animator = GetComponent<Animator>();
			dispatcher = Service.Get<EventDispatcher>();
			questService = Service.Get<QuestService>();
			mascotService = Service.Get<MascotService>();
			indicatorData = new DActionIndicator();
			indicatorData.TargetTransform = base.transform;
			indicatorData.IndicatorContentKey = QuestIndicatorContentKey;
			indicatorData.TargetOffset = QuestIndicatorOffset;
			speechData = new DCinematicSpeech();
		}

		public void OnEnable()
		{
			if (Mascot == null)
			{
				Mascot = mascotService.GetMascot(MascotName);
				if (Mascot == null)
				{
					UnityEngine.Object.Destroy(this);
					return;
				}
			}
			dispatcher.AddListener<CinematicSpeechEvents.SpeechCompleteEvent>(onSpeechComplete);
			dispatcher.AddListener<CinematicSpeechEvents.PlayMascotAnimation>(onPlayMascotAnimation);
			dispatcher.AddListener<CinematicSpeechEvents.SetMascotIdleDialogOverride>(onSetMascotIdleDialogOverride);
			dispatcher.AddListener<ButtonEvents.ClickEvent>(onButtonClicked);
			dispatcher.AddListener<QuestEvents.StartQuest>(onStartQuest);
			dispatcher.AddListener<QuestEvents.ReplayQuest>(onReplayQuest);
			dispatcher.AddListener<QuestEvents.ResumeQuest>(onResumeQuest);
			dispatcher.AddListener<SplashScreenEvents.SplashScreenOpened>(onSplashScreenOpened);
			dispatcher.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			Service.Get<MascotService>().RegisterMascotObject(Mascot.Name, base.gameObject);
		}

		public void OnDisable()
		{
			dispatcher.RemoveListener<CinematicSpeechEvents.SpeechCompleteEvent>(onSpeechComplete);
			dispatcher.RemoveListener<CinematicSpeechEvents.PlayMascotAnimation>(onPlayMascotAnimation);
			dispatcher.RemoveListener<CinematicSpeechEvents.SetMascotIdleDialogOverride>(onSetMascotIdleDialogOverride);
			dispatcher.RemoveListener<ButtonEvents.ClickEvent>(onButtonClicked);
			dispatcher.RemoveListener<QuestEvents.StartQuest>(onStartQuest);
			dispatcher.RemoveListener<QuestEvents.ReplayQuest>(onReplayQuest);
			dispatcher.RemoveListener<QuestEvents.ResumeQuest>(onResumeQuest);
			dispatcher.RemoveListener<SplashScreenEvents.SplashScreenOpened>(onSplashScreenOpened);
			dispatcher.RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			if (Mascot != null)
			{
				Service.Get<MascotService>().UnregisterMascotObject(Mascot.Name);
			}
		}

		public void Start()
		{
			speechData.MascotName = Mascot.Name;
			indicatorData.IndicatorId = Definition.AbbreviatedName + "Indicator";
			dynamicPanels[0] = new DDialogPanel();
		}

		private void setState(States newState)
		{
			if (state == newState)
			{
				return;
			}
			switch (newState)
			{
			case States.Idle:
				if (state == States.GivingQuest)
				{
					hideDialog();
				}
				if (Mascot.InteractionBehaviours.RestoreTray)
				{
					dispatcher.DispatchEvent(new TrayEvents.SelectTrayScreen("ControlsScreen"));
					Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowCellPhoneHud));
				}
				Mascot.IsTalking = false;
				mascotService.ActiveMascot = null;
				dispatcher.DispatchEvent(default(InWorldUIEvents.EnableInWorldText));
				PlayIdleAnimation();
				break;
			case States.Dialog:
				dispatcher.DispatchEvent(new TrayEvents.CloseTray(false, false));
				dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
				dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
				dispatcher.DispatchEvent(default(InWorldUIEvents.DisableInWorldText));
				mascotService.ActiveMascot = Mascot;
				break;
			case States.GivingQuest:
			{
				mascotService.ActiveMascot = Mascot;
				showGivingQuestDialog(Mascot);
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
				component.SendEvent(new ExternalEvent("Root", "maxnpc"));
				component.SendEvent(new ExternalEvent("ScreenContainerContent", "quest"));
				dispatcher.DispatchEvent(default(InWorldUIEvents.DisableInWorldText));
				break;
			}
			case States.WaitingForSplashscreen:
				hideDialog();
				Mascot.IsTalking = false;
				dispatcher.DispatchEvent(new TrayEvents.SelectTrayScreen("ControlsScreen"));
				break;
			}
			state = newState;
		}

		public void Update()
		{
			if (Mascot == null)
			{
				return;
			}
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (Mascot.IsQuestGiver && activeQuest == null && Mascot.HasAvailableQuests() && Service.Get<QuestService>().IsQuestActionIndicatorsEnabled)
			{
				if ((!hasCheckedIndicator || !isShowingIndicator) && Mascot.InteractionBehaviours.ShowIndicator)
				{
					dispatcher.DispatchEvent(new ActionIndicatorEvents.AddActionIndicator(indicatorData));
					isShowingIndicator = true;
				}
			}
			else if (!hasCheckedIndicator || isShowingIndicator)
			{
				dispatcher.DispatchEvent(new ActionIndicatorEvents.RemoveActionIndicator(indicatorData));
				isShowingIndicator = false;
			}
			if (state == States.Dialog && !Mascot.IsTalking)
			{
				setState(States.Idle);
			}
			hasCheckedIndicator = true;
		}

		public void StartInteraction()
		{
			if (!Mascot.IsDefaultInteractDisabled)
			{
				if (TalkPlayermakerFSMOverride != null && Mascot.WantsToTalk)
				{
					sendChatEventOverride();
				}
				else if (questService.ActiveQuest != null)
				{
					if (Mascot.WantsToTalk)
					{
						questService.ActiveQuest.SendEvent("TalkTo" + MascotName);
					}
					else
					{
						if (Mascot.ActiveQuestDialog != null)
						{
							showDialog(Mascot.ActiveQuestDialog.SelectRandom(), true);
						}
						else
						{
							showDialog(Definition.RandomQuestDialog.SelectRandom(), true);
						}
						setState(States.Dialog);
					}
				}
				else if (Mascot.IsQuestGiver)
				{
					setState(States.GivingQuest);
				}
				else
				{
					DialogList dialogList = (Mascot.RandomWorldDialogOverride != null) ? Mascot.RandomWorldDialogOverride : Definition.RandomWorldDialog;
					showDialog(dialogList.SelectRandom(), true);
					setState(States.Dialog);
				}
			}
			dispatcher.DispatchEvent(new QuestEvents.OnMascotInteract(Mascot));
		}

		private void sendChatEventOverride()
		{
			string fsmEventName = "TalkTo" + MascotName;
			List<Fsm> subFsmList = TalkPlayermakerFSMOverride.Fsm.SubFsmList;
			if (subFsmList != null)
			{
				for (int i = 0; i < subFsmList.Count; i++)
				{
					subFsmList[i].Event(fsmEventName);
				}
			}
			TalkPlayermakerFSMOverride.Fsm.Event(fsmEventName);
			setState(States.Dialog);
		}

		public bool IsInteractionDone()
		{
			return state == States.Idle;
		}

		private void PlayIdleAnimation()
		{
			if (!string.IsNullOrEmpty(overrideIdleAnimationName))
			{
				playDialogAnimation(Animator.StringToHash(overrideIdleAnimationName));
			}
			else
			{
				playDialogAnimation(IdleAnimHash);
			}
		}

		public void StartDialogAnimation(string audioEventName, string overrideAnimationName, bool autoStopAnimation, string overrideStopAnimationName)
		{
			currentAudioEvent = audioEventName;
			if (string.IsNullOrEmpty(overrideAnimationName))
			{
				playDialogAnimation(DialogAnimHash);
			}
			else
			{
				playDialogAnimation(Animator.StringToHash(overrideAnimationName));
			}
			autoStopDialogAnim = autoStopAnimation;
			this.overrideStopAnimationName = overrideStopAnimationName;
		}

		public void dialogAudioCallback(EventNotificationType type, string eventName, object info, GameObject gameObject)
		{
			if (autoStopDialogAnim && eventName == currentAudioEvent && type == EventNotificationType.OnFinished)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(QuestEvents.MascotAudioComplete));
				if (string.IsNullOrEmpty(overrideStopAnimationName))
				{
					PlayIdleAnimation();
				}
				else
				{
					playDialogAnimation(Animator.StringToHash(overrideStopAnimationName));
				}
			}
		}

		private void showGivingQuestDialog(Mascot mascot)
		{
			DialogList dialogList = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			QuestDefinition questDefinition = null;
			Quest quest = null;
			for (int i = 0; i < mascot.KnownQuests.Length; i++)
			{
				questDefinition = mascot.KnownQuests[i];
				if (questDefinition.Prototyped)
				{
					continue;
				}
				for (int j = 0; j < mascot.AvailableQuests.Count; j++)
				{
					if (mascot.AvailableQuests[j].Definition.Title == questDefinition.Title)
					{
						quest = mascot.AvailableQuests[j];
						break;
					}
				}
				if (quest != null && quest.TimesCompleted == 0)
				{
					flag3 = false;
					if (quest.State != Quest.QuestState.Locked && (!questDefinition.isMemberOnly || Service.Get<CPDataEntityCollection>().IsLocalPlayerMember()))
					{
						dialogList = Definition.QuestGiverDialog;
						break;
					}
				}
				if (quest == null || quest.State == Quest.QuestState.Locked)
				{
					int num = Service.Get<ProgressionService>().MascotLevel(mascot.Definition.name);
					if (questDefinition.LevelRequirement > num)
					{
						flag2 = true;
					}
					else
					{
						flag = true;
					}
					flag3 = false;
				}
				quest = null;
			}
			if (dialogList == null && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				dialogList = Definition.QuestGiverDialogMemberLocked;
			}
			else if (flag3)
			{
				dialogList = Definition.QuestGiverDialogAllComplete;
			}
			else if (dialogList == null)
			{
				if (flag)
				{
					dialogList = Definition.QuestGiverDialogTimeLocked;
				}
				else if (flag2)
				{
					dialogList = Definition.QuestGiverDialogLevelLocked;
				}
			}
			if (dialogList == null || dialogList.Entries.Length == 0)
			{
				dialogList = Definition.QuestGiverDialog;
			}
			showDialog(dialogList.SelectRandom(), false);
		}

		public void ShowDialog(params DDialogPanel[] panels)
		{
			if (Mascot.IsTalking)
			{
				throw new InvalidOperationException("You cannot show a dialog whilst a mascot is talking!");
			}
			showPanels(panels);
			setState(States.Dialog);
		}

		private void showDialog(DialogList.Entry dialog, bool clickToClose = true, params string[] args)
		{
			dynamicPanels[0].Dialog = dialog;
			dynamicPanels[0].ClickToClose = clickToClose;
			dynamicPanels[0].i18nTextArgs = args;
			showPanels(dynamicPanels);
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
		}

		private void showPanels(params DDialogPanel[] panels)
		{
			Mascot.IsTalking = true;
			dialogPanels = panels;
			activePanelIndex = 0;
			updatePanel();
		}

		private void hideDialog()
		{
			dialogPanels = null;
			dispatcher.DispatchEvent(new CinematicSpeechEvents.HideSpeechEvent(speechData));
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowCellPhoneHud));
		}

		private bool onSpeechComplete(CinematicSpeechEvents.SpeechCompleteEvent evt)
		{
			if (evt.SpeechData == speechData && dialogPanels != null)
			{
				activePanelIndex++;
				if (activePanelIndex < dialogPanels.Length)
				{
					updatePanel();
				}
				else if (state == States.Dialog)
				{
					setState(States.Idle);
				}
			}
			return false;
		}

		private void updatePanel()
		{
			Localizer localizer = Service.Get<Localizer>();
			speechData.Text = localizer.GetTokenTranslationFormatted(dialogPanels[activePanelIndex].Dialog.ContentToken, dialogPanels[activePanelIndex].i18nTextArgs);
			speechData.ClickToClose = dialogPanels[activePanelIndex].ClickToClose;
			speechData.Buttons = null;
			dispatcher.DispatchEvent(new CinematicSpeechEvents.ShowSpeechEvent(speechData));
			DialogList.Entry dialog = dialogPanels[activePanelIndex].Dialog;
			if (!string.IsNullOrEmpty(dialog.AudioEventName))
			{
				EventAction eventAction = EventAction.PlaySound;
				if (dialog.AdvanceSequence)
				{
					eventAction = EventAction.AdvanceSequence;
				}
				EventManager.Instance.PostEventNotify(dialog.AudioEventName, eventAction, base.gameObject, dialogAudioCallback);
				StartDialogAnimation(dialog.AudioEventName, dialog.DialogAnimationTrigger, true, dialog.DialogAnimationEndTrigger);
			}
		}

		private bool onStartQuest(QuestEvents.StartQuest evt)
		{
			if (state == States.GivingQuest)
			{
				setState(States.WaitingForSplashscreen);
			}
			return false;
		}

		private bool onReplayQuest(QuestEvents.ReplayQuest evt)
		{
			if (state == States.GivingQuest)
			{
				setState(States.WaitingForSplashscreen);
			}
			return false;
		}

		private bool onResumeQuest(QuestEvents.ResumeQuest evt)
		{
			if (evt.Quest.Mascot == Mascot && state == States.GivingQuest)
			{
				setState(States.WaitingForSplashscreen);
			}
			return false;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State != Quest.QuestState.Active)
			{
				playDialogAnimation(IdleAnimHash);
			}
			hasCheckedIndicator = false;
			return false;
		}

		private bool onSplashScreenOpened(SplashScreenEvents.SplashScreenOpened evt)
		{
			if (state == States.WaitingForSplashscreen)
			{
				setState(States.Idle);
			}
			return false;
		}

		private bool onButtonClicked(ButtonEvents.ClickEvent evt)
		{
			if (state == States.GivingQuest && evt.ButtonData.Id == "QuestLogAdventureExit")
			{
				setState(States.Idle);
			}
			return false;
		}

		private void playDialogAnimation(int animHash)
		{
			if (animator != null)
			{
				if (animHash == DialogAnimHash)
				{
					animator.ResetTrigger(IdleAnimHash);
				}
				else if (animHash == IdleAnimHash)
				{
					animator.ResetTrigger(DialogAnimHash);
				}
				animator.SetTrigger(animHash);
			}
		}

		private bool onPlayMascotAnimation(CinematicSpeechEvents.PlayMascotAnimation evt)
		{
			if (evt.Mascot == this)
			{
				PlayAnimation(evt.AnimationTrigger);
			}
			return false;
		}

		private bool onSetMascotIdleDialogOverride(CinematicSpeechEvents.SetMascotIdleDialogOverride evt)
		{
			if (Definition.name == evt.MascotName)
			{
				overrideIdleAnimationName = evt.AnimationTrigger;
			}
			return false;
		}

		public void PlayAnimation(string animationTrigger)
		{
			autoStopDialogAnim = false;
			animator.SetTrigger(animationTrigger);
		}
	}
}
