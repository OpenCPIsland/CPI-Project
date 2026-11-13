using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestHud : MonoBehaviour
	{
		private enum QuestHudState
		{
			Open,
			Closed
		}

		private const string ANIMATOR_BOOL_MAXIMIZED = "isMaximized";

		private const string ANIMATOR_BOOL_COMMUNICATOR_OPEN = "isCommunicatorOpen";

		private const string ANIMATOR_BOOL_MESSAGE_OPEN = "isMessageOpen";

		private const string ANIMATOR_BOOL_UPDATE = "isTextUpdated";

		private const string ANIMATOR_BOOL_SUBTASKS = "isSubtasksOpen";

		private const string ANIMATOR_TRIGGER_SHOWSUBTASKS = "showSubtasks";

		private const string ANIMATOR_TRIGGER_HIDESUBTASKS = "hideSubtasks";

		public QuestCommunicatorHud QuestCommunicatorHud;

		public QuestMessageHud QuestMessageHud;

		public GameObject QuestPointerPanel;

		public GameObject CommunicatorPanel;

		public ParticleSystem QuestHUDParticleSystem;

		public int ParticlesToEmit = 50;

		private Animator animator;

		private EventChannel eventChannel;

		private bool showCommunicatorPending = false;

		private bool showMessagePending = false;

		private QuestHudState currentState;

		private bool isCommunicatorOpening = false;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			QuestCommunicatorHud.SubtasksChanged += onSubtasksChanged;
			QuestCommunicatorHud.CommunicatorSuppressed += onCommunicatorSuppressed;
			QuestCommunicatorHud.CommunicatorUnsuppressed += onCommunicatorUnsuppressed;
			QuestMessageHud.MessageClosed += onQuestMessageClosed;
			QuestMessageHud.MascotImageLoaded += onMascotImageLoaded;
		}

		private void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<HudEvents.ResetQuestNotifier>(onResetQuestNotifier);
			eventChannel.AddListener<HudEvents.SetObjectiveText>(onObjectiveTextSet);
			eventChannel.AddListener<HudEvents.ShowHideQuestNotifier>(onShowHideQuestNotifier);
			eventChannel.AddListener<CinematicSpeechEvents.ShowSpeechEvent>(onShowSpeech);
			eventChannel.AddListener<CinematicSpeechEvents.SpeechCompleteEvent>(onSpeechComplete);
			eventChannel.AddListener<SplashScreenEvents.SplashScreenClosed>(onSplashScreenClosed);
			toggleCommunicatorHudEnabled(false);
			toggleMessageHudEnabled(false);
			QuestPointerPanel.SetActive(false);
		}

		private void OnEnable()
		{
			if (QuestCommunicatorHud.IsOpen && !animator.GetBool("isCommunicatorOpen"))
			{
				showCommunicatorHud();
			}
			if (QuestMessageHud.IsOpen)
			{
				OnMessageCloseAnimationComplete();
			}
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			CoroutineRunner.StopAllForOwner(this);
		}

		public void OnHiddenStateMachineBehaviourEnter()
		{
			setState(QuestHudState.Closed);
			if (!animator.GetBool("isMessageOpen") && !animator.GetBool("isCommunicatorOpen"))
			{
				CoroutineRunner.Start(disableAnimatorAfterFrame(), this, "QuestHud.disableAnimatorAfterFrame");
			}
		}

		public void OnHiddenStateMachineBehaviourExit()
		{
			setState(QuestHudState.Open);
		}

		private bool onResetQuestNotifier(HudEvents.ResetQuestNotifier evt)
		{
			CoroutineRunner.StopAllForOwner(this);
			hideCommunicatorHud(false);
			hideMessageHud();
			toggleCommunicatorHudEnabled(false);
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowCellPhoneHud));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
			setState(QuestHudState.Closed);
			return false;
		}

		private bool onShowSpeech(CinematicSpeechEvents.ShowSpeechEvent evt)
		{
			if (currentState == QuestHudState.Open && evt.HideQuestHud)
			{
				hideCommunicatorHud();
				hideMessageHud();
			}
			return false;
		}

		private bool onSpeechComplete(CinematicSpeechEvents.SpeechCompleteEvent evt)
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null)
			{
				showCommunicatorHud();
			}
			return false;
		}

		public void OnCommunicatorCloseAnimationComplete()
		{
			toggleCommunicatorHudEnabled(false);
			QuestCommunicatorHud.SetOpen(false);
		}

		public void OnMessageCloseAnimationComplete()
		{
			if (!animator.GetBool("isMessageOpen"))
			{
				QuestMessageHud.ResetMessage();
				Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.QuestMessageComplete(QuestMessageHud.SpeechData));
				CoroutineRunner.Start(enableCommunicatorPanel(), this, "");
				toggleMessageHudEnabled(false);
				if (showCommunicatorPending)
				{
					showCommunicatorPending = false;
					showCommunicatorHud();
				}
			}
		}

		public void OnMessageOpenAnimationComplete()
		{
			QuestMessageHud.OnMessageOpenAnimationComplete();
		}

		public void OnCommunicatorOutStaticAnimationComplete()
		{
			if (animator.GetBool("isCommunicatorOpen") && !animator.GetBool("isMessageOpen") && !animator.GetBool("showSubtasks") && !animator.GetBool("hideSubtasks"))
			{
				disableAnimator();
			}
			isCommunicatorOpening = false;
		}

		private void disableAnimator()
		{
			if (!QuestMessageHud.IsOpen && !QuestMessageHud.IsOpening && !isCommunicatorOpening && animator.enabled)
			{
				animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 1f);
				animator.enabled = false;
			}
		}

		private IEnumerator disableAnimatorAfterFrame()
		{
			yield return new WaitForEndOfFrame();
			disableAnimator();
		}

		private void enableAnimator()
		{
			if (!animator.enabled)
			{
				animator.enabled = true;
			}
		}

		private bool onObjectiveTextSet(HudEvents.SetObjectiveText evt)
		{
			if (!string.IsNullOrEmpty(evt.ObjectiveText))
			{
				Service.Get<QuestService>().CurrentObjectiveText = evt.ObjectiveText;
				if (QuestCommunicatorHud.SetObjectiveText(evt.ObjectiveText) || currentState != 0)
				{
					if (!QuestMessageHud.IsOpen && !QuestMessageHud.IsOpening)
					{
						showCommunicatorHud();
						CoroutineRunner.Start(playTextUpdatedAnimation(), this, "playTextUpdatedAnimation");
					}
					else
					{
						showCommunicatorPending = true;
					}
				}
			}
			else
			{
				hideCommunicatorHud();
			}
			return false;
		}

		private IEnumerator playTextUpdatedAnimation()
		{
			if (currentState == QuestHudState.Open)
			{
				enableAnimator();
				animator.SetBool("isTextUpdated", true);
				yield return null;
				animator.SetBool("isTextUpdated", false);
			}
		}

		private bool onShowHideQuestNotifier(HudEvents.ShowHideQuestNotifier evt)
		{
			if (evt.Show)
			{
				showCommunicatorHud();
			}
			else
			{
				hideCommunicatorHud();
			}
			return false;
		}

		private void showCommunicatorHud()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			QuestPointerPanel.SetActive(false);
			if (Service.Get<QuestService>().IsSplashScreenOpen)
			{
				showCommunicatorPending = true;
			}
			else if (!QuestCommunicatorHud.IsSuppressed && !QuestCommunicatorHud.IsPermanentlySuppressed)
			{
				isCommunicatorOpening = true;
				QuestCommunicatorHud.SetOpen(true);
				toggleCommunicatorHudEnabled(true);
				if (!animator.GetBool("isCommunicatorOpen"))
				{
					enableAnimator();
					animator.SetBool("isCommunicatorOpen", true);
				}
				showSubtasks(true);
				Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowCellPhoneHud));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
			}
		}

		private void hideCommunicatorHud(bool hidePhone = true)
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			isCommunicatorOpening = false;
			QuestCommunicatorHud.SetOpen(false);
			if (currentState == QuestHudState.Open)
			{
				enableAnimator();
				showCommunicatorPending = false;
				if (hidePhone)
				{
					CoroutineRunner.Start(hideCellPhone(), this, "hideCellPhone");
				}
			}
			else if (hidePhone)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("CellphoneButton"));
			}
			animator.SetBool("isCommunicatorOpen", false);
		}

		private IEnumerator hideCellPhone()
		{
			yield return new WaitForEndOfFrame();
			float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(animTime);
			if (!animator.GetBool("isCommunicatorOpen"))
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("CellphoneButton"));
			}
		}

		private void showMessageHud()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
			if (Service.Get<QuestService>().IsSplashScreenOpen)
			{
				showMessagePending = true;
				return;
			}
			enableAnimator();
			toggleMessageHudEnabled(true);
			if (!animator.GetBool("isMessageOpen"))
			{
				CoroutineRunner.Start(disableCommunicatorPanel(), this, "");
				animator.SetBool("isMessageOpen", true);
				QuestPointerPanel.SetActive(true);
			}
			if (QuestCommunicatorHud.IsOpen)
			{
				showCommunicatorPending = true;
				animator.SetBool("isCommunicatorOpen", false);
				CoroutineRunner.Start(hideCellPhone(), this, "hideCellPhone");
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
			}
		}

		private IEnumerator disableCommunicatorPanel()
		{
			yield return new WaitForEndOfFrame();
			CommunicatorPanel.SetActive(false);
		}

		private IEnumerator enableCommunicatorPanel()
		{
			yield return new WaitForEndOfFrame();
			CommunicatorPanel.SetActive(true);
		}

		private void hideMessageHud()
		{
			enableAnimator();
			if (animator.GetBool("isMessageOpen"))
			{
				animator.SetBool("isMessageOpen", false);
			}
			showMessagePending = false;
		}

		private void showSubtasks(bool forceOpen = false)
		{
			if (QuestCommunicatorHud.SubtaskCount > 0)
			{
				if (forceOpen || !animator.GetBool("isSubtasksOpen"))
				{
					enableAnimator();
					animator.SetBool("isSubtasksOpen", true);
					animator.SetTrigger("showSubtasks");
					animator.ResetTrigger("hideSubtasks");
				}
			}
			else
			{
				enableAnimator();
				animator.SetBool("isSubtasksOpen", false);
			}
		}

		private void hideSubtasks()
		{
			if (animator.GetBool("isSubtasksOpen"))
			{
				enableAnimator();
				animator.SetBool("isSubtasksOpen", false);
				animator.SetTrigger("hideSubtasks");
				animator.ResetTrigger("showSubtasks");
			}
		}

		private void onSubtasksChanged()
		{
			if (QuestCommunicatorHud.SubtaskCount == 0 && currentState == QuestHudState.Open)
			{
				enableAnimator();
				animator.SetBool("isSubtasksOpen", false);
				if (animator.GetBool("isCommunicatorOpen"))
				{
					animator.SetTrigger("hideSubtasks");
				}
				animator.ResetTrigger("showSubtasks");
			}
			else if (QuestCommunicatorHud.IsOpen)
			{
				showSubtasks();
			}
		}

		private void onCommunicatorSuppressed()
		{
			hideCommunicatorHud();
		}

		private void onCommunicatorUnsuppressed()
		{
			showCommunicatorHud();
		}

		private void onQuestMessageClosed()
		{
			hideMessageHud();
		}

		private void onMascotImageLoaded()
		{
			showMessageHud();
		}

		private void toggleCommunicatorHudEnabled(bool enabled)
		{
			QuestHUDParticleSystem.gameObject.SetActive(enabled);
		}

		private void toggleMessageHudEnabled(bool enabled)
		{
			QuestMessageHud.ToggleActive(enabled);
		}

		private bool onSplashScreenClosed(SplashScreenEvents.SplashScreenClosed evt)
		{
			if (showCommunicatorPending)
			{
				showCommunicatorPending = false;
				showCommunicatorHud();
			}
			if (showMessagePending)
			{
				showMessagePending = false;
				showMessageHud();
			}
			return false;
		}

		private void fireParticleEffect()
		{
			if (QuestHUDParticleSystem != null)
			{
				QuestHUDParticleSystem.Emit(ParticlesToEmit);
			}
		}

		private void setState(QuestHudState newState)
		{
			currentState = newState;
		}
	}
}
