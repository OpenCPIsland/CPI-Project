using ClubPenguin.Core;
using ClubPenguin.Participation;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class TrayInputButtonDisabler : UIElementDisabler
	{
		private TrayInputButton inputButton;

		private EventChannel eventChannel;

		private bool isLockedByActionSequence = false;

		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			inputButton = GetComponentInChildren<TrayInputButton>();
		}

		protected override void start()
		{
			base.start();
			eventChannel.AddListener<ActionSequencerEvents.ActionSequenceStarted>(onActionSequenceStarted);
			eventChannel.AddListener<ActionSequencerEvents.ActionSequenceCompleted>(onActionSequenceCompleted);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (!cPDataEntityCollection.LocalPlayerHandle.IsNull)
			{
				ParticipationData component = cPDataEntityCollection.GetComponent<ParticipationData>(cPDataEntityCollection.LocalPlayerHandle);
				if (component != null && ParticipationState.Retained == component.CurrentParticipationState)
				{
					disableAndLockForActionSequence();
				}
			}
		}

		public override void DisableElement(bool hide)
		{
			inputButton.Lock(TrayInputButton.ButtonState.Disabled);
			changeVisibility(!hide);
			isLockedByActionSequence = false;
			isEnabled = false;
		}

		public override void EnableElement()
		{
			inputButton.Unlock();
			changeVisibility(true);
			isLockedByActionSequence = false;
			isEnabled = true;
		}

		private bool onActionSequenceStarted(ActionSequencerEvents.ActionSequenceStarted evt)
		{
			disableAndLockForActionSequence(evt.actionGameObject);
			return false;
		}

		private bool onActionSequenceCompleted(ActionSequencerEvents.ActionSequenceCompleted evt)
		{
			enableAndUnlockForActionSequence();
			return false;
		}

		private void disableAndLockForActionSequence(GameObject overrideObject = null)
		{
			bool flag = true;
			if (overrideObject != null)
			{
				ButtonDisablerOverride component = overrideObject.GetComponent<ButtonDisablerOverride>();
				if (component != null && component.buttonsToOverride.Length > 0)
				{
					TrayInputButtonDisabler componentInParent = GetComponentInParent<TrayInputButtonDisabler>();
					string[] buttonsToOverride = component.buttonsToOverride;
					foreach (string a in buttonsToOverride)
					{
						if (a == componentInParent.UIElementID)
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag && !inputButton.IsLocked)
			{
				inputButton.Lock(TrayInputButton.ButtonState.Disabled);
				isLockedByActionSequence = true;
			}
		}

		private void enableAndUnlockForActionSequence()
		{
			if (isLockedByActionSequence)
			{
				inputButton.Unlock();
				isLockedByActionSequence = false;
			}
		}

		protected override void onDestroy()
		{
			base.onDestroy();
			eventChannel.RemoveAllListeners();
			if (inputButton != null && isLockedByActionSequence)
			{
				inputButton.Unlock();
			}
		}
	}
}
