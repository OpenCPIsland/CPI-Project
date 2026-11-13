using ClubPenguin.Locomotion;
using ClubPenguin.Net.Domain;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	internal class InteractionZoneRemotePlayerDanceController : MonoBehaviour
	{
		private const int IDLE_DANCE_MODE = 0;

		private Animator animator;

		private AvatarLocomotionStateSetter locomotionStateSetter;

		private RunController runController;

		public void Start()
		{
			animator = base.gameObject.GetComponent<Animator>();
			runController = base.gameObject.GetComponent<RunController>();
			if (runController != null)
			{
				runController.OnSteer += OnStickDirectionEvent;
			}
			locomotionStateSetter = base.gameObject.GetComponent<AvatarLocomotionStateSetter>();
			if (locomotionStateSetter != null)
			{
				locomotionStateSetter.ActionButtonInvoked += OnActionButtonClicked;
			}
		}

		private void OnStickDirectionEvent(Vector3 steer)
		{
			bool danceMode = steer == Vector3.zero;
			SetDanceMode(danceMode);
		}

		private void OnActionButtonClicked(LocomotionAction action)
		{
			if (animator != null)
			{
				int value = 1;
				switch (action)
				{
				case LocomotionAction.Action1:
					value = 1;
					break;
				case LocomotionAction.Action2:
					value = 2;
					break;
				case LocomotionAction.Action3:
					value = 3;
					break;
				}
				SetDanceMode(true);
				animator.SetInteger("DanceMove", value);
			}
		}

		private void SetDanceMode(bool enable)
		{
			animator.SetBool("Dancing", enable);
		}

		public void OnDestroy()
		{
			if (animator != null)
			{
				animator.SetInteger("DanceMove", 0);
				SetDanceMode(false);
			}
			if (locomotionStateSetter != null)
			{
				locomotionStateSetter.ActionButtonInvoked -= OnActionButtonClicked;
			}
			if (runController != null)
			{
				runController.OnSteer -= OnStickDirectionEvent;
			}
		}
	}
}
