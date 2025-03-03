using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(LocomotionEventBroadcaster))]
	public class LocomotionController : MonoBehaviour
	{
		public enum LocomotionAction
		{
			Jump,
			Torpedo,
			SlideTrick,
			ChargeThrow,
			LaunchThrow,
			Interact,
			Action1,
			Action2,
			Action3
		}

		protected CharacterController characterController;

		protected Animator animator;

		protected LocalPenguinSnowballThrower snowballThrow;

		private LocomotionEventBroadcaster broadcaster;

		public LocomotionEventBroadcaster Broadcaster
		{
			get
			{
				if (broadcaster == null)
				{
					broadcaster = GetComponent<LocomotionEventBroadcaster>();
				}
				return broadcaster;
			}
			set
			{
			}
		}

		public virtual void Steer(Vector2 steerInput)
		{
		}

		public virtual void Steer(Vector3 wsSteerInput)
		{
		}

		public virtual void SteerRotation(Vector2 steerInput)
		{
		}

		public virtual void SteerRotation(Vector3 wsSteerInput)
		{
		}

		public virtual void AddForce(Vector3 wsForce, GameObject pusher = null)
		{
		}

		public virtual void SetForce(Vector3 wsForce, GameObject pusher = null)
		{
		}

		public virtual void DoAction(LocomotionAction action, object userData = null)
		{
		}

		public virtual void SetState(LocomotionState state)
		{
		}

		public virtual bool AllowTriggerInteractions()
		{
			return true;
		}

		public virtual bool AllowTriggerOnStay()
		{
			return true;
		}

		public virtual bool IsFullbodyLocked()
		{
			return false;
		}

		public virtual Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public virtual Vector3 GetFacing()
		{
			return base.transform.forward;
		}

		public virtual void RemoteSetPosition(Vector3 newPos)
		{
			base.transform.position = newPos;
		}

		public virtual void RemoteSnapPosition(Vector3 newPos)
		{
			base.transform.position = newPos;
		}

		public virtual void RemoteSetFacing(Vector3 newFacing)
		{
			base.transform.rotation = Quaternion.LookRotation(newFacing);
		}

		public virtual void ResetState()
		{
		}

		public virtual void OnBlockingInteractionStarted()
		{
		}

		private void Awake()
		{
			animator = GetComponent<Animator>();
			characterController = GetComponent<CharacterController>();
			snowballThrow = GetComponent<LocalPenguinSnowballThrower>();
			awake();
		}

		protected virtual void awake()
		{
		}

		protected void DefaultDoAction(LocomotionAction action, object userData = null)
		{
			switch (action)
			{
			case LocomotionAction.ChargeThrow:
				if (snowballThrow != null)
				{
					snowballThrow.ChargeSnowball();
				}
				break;
			case LocomotionAction.LaunchThrow:
				if (snowballThrow != null)
				{
					snowballThrow.LaunchSnowball((float)userData);
				}
				break;
			case LocomotionAction.Interact:
			{
				PenguinInteraction component = GetComponent<PenguinInteraction>();
				if (component != null && component.RequestInteraction())
				{
					Broadcaster.BroadcastOnDoAction(action, userData);
				}
				break;
			}
			case LocomotionAction.Action1:
			case LocomotionAction.Action2:
			case LocomotionAction.Action3:
			{
				PenguinInteraction component = GetComponent<PenguinInteraction>();
				if (component != null && !LocomotionUtils.IsInAir(LocomotionUtils.GetAnimatorStateInfo(animator)))
				{
					Broadcaster.BroadcastOnDoAction(action, userData);
				}
				break;
			}
			}
		}

		public virtual void LoadControlsLayout()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ControlsScreenEvents.ReturnToDefaultRightOption));
		}

		public virtual void EnableAction(LocomotionAction action, bool enabled)
		{
			switch (action)
			{
			case LocomotionAction.ChargeThrow:
			case LocomotionAction.LaunchThrow:
			{
				AbstractPenguinSnowballThrower component = GetComponent<AbstractPenguinSnowballThrower>();
				if (component != null)
				{
					component.EnableSnowballThrow(enabled);
				}
				break;
			}
			}
		}
	}
}
