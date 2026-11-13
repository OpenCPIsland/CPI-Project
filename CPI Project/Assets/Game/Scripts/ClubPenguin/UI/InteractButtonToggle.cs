using ClubPenguin.Core;
using ClubPenguin.Participation;
using ClubPenguin.Props;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InteractButtonToggle : MonoBehaviour
	{
		private TrayInputButton trayInputButton;

		private MemberLockedTrayInputButton memberLockedButton;

		private PropService propService;

		private CPDataEntityCollection dataEntityCollection;

		private ParticipationData participation;

		private ParticipationData participationData
		{
			get
			{
				if (participation == null && dataEntityCollection != null && !dataEntityCollection.LocalPlayerHandle.IsNull)
				{
					participation = dataEntityCollection.GetComponent<ParticipationData>(dataEntityCollection.LocalPlayerHandle);
				}
				return participation;
			}
		}

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			propService = Service.Get<PropService>();
			trayInputButton = GetComponentInParent<TrayInputButton>();
			memberLockedButton = GetComponent<MemberLockedTrayInputButton>();
			trayInputButton.OnStateChanged += onTrayInputButtonStateChanged;
		}

		private void onTrayInputButtonStateChanged(TrayInputButton.ButtonState state)
		{
		}

		private void Update()
		{
			refreshButtonState();
		}

		private void refreshButtonState()
		{
			if (participationData != null)
			{
				if (!participationData.IsInteractButtonAvailable)
				{
					setButtonState(false);
					return;
				}
				bool flag = propService.LocalPlayerPropUser == null || propService.LocalPlayerPropUser.IsPropUseCompleted;
				bool buttonState = participationData.CurrentParticipationState == ParticipationState.Pending && flag;
				setButtonState(buttonState);
			}
		}

		private void setButtonState(bool pulsate)
		{
			if (trayInputButton != null)
			{
				memberLockedButton.IsLocked = (!memberLockedButton.IsPlayerAMember && isInteractingObjectMemberOnly());
				if (pulsate)
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Pulsing);
				}
				else
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Disabled);
				}
			}
		}

		private bool isInteractingObjectMemberOnly()
		{
			if (participationData.ParticipatingGO != null && participationData.ParticipatingGO.Value != null)
			{
				MemberAccess component = participationData.ParticipatingGO.Value.GetComponent<MemberAccess>();
				if (component != null)
				{
					return component.IsMemberLocked;
				}
			}
			return false;
		}

		private void OnDestroy()
		{
			if (trayInputButton != null)
			{
				trayInputButton.OnStateChanged -= onTrayInputButtonStateChanged;
			}
		}
	}
}
