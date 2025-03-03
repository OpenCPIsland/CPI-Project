using ClubPenguin.Net.Domain;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class LocomotionEventBroadcaster : MonoBehaviour
	{
		public delegate void StickDirectionHandler(Vector2 steer);

		public delegate void SteerDirectionHandler(Vector3 steer);

		public delegate void SteerRotationDirectionHandler(Vector3 steer);

		public delegate void SteerRotationFlushHandler();

		public delegate void DoActionHandler(LocomotionController.LocomotionAction action, object userData = null);

		public delegate void InteractionPreStartedHandler(GameObject trigger);

		public delegate void InteractionStartedHandler(GameObject trigger);

		public delegate void ControllerChangedHandler(LocomotionController newController);

		public delegate void LocomotionStateHandler(LocomotionState state);

		public delegate void LandedJumpHandler();

		public delegate void ControlsLocked();

		public delegate void ControlsUnlocked();

		public event StickDirectionHandler OnStickDirectionEvent;

		public event SteerDirectionHandler OnSteerDirectionEvent;

		public event SteerRotationDirectionHandler OnSteerRotationDirectionEvent;

		public event SteerRotationFlushHandler OnSteerRotationFlushEvent;

		public event DoActionHandler OnDoActionEvent;

		public event InteractionPreStartedHandler OnInteractionPreStartedEvent;

		public event InteractionStartedHandler OnInteractionStartedEvent;

		public event ControllerChangedHandler OnControllerChangedEvent;

		public event LocomotionStateHandler OnLocomotionStateChangedEvent;

		public event LandedJumpHandler OnLandedJumpEvent;

		public event ControlsLocked OnControlsLocked;

		public event ControlsUnlocked OnControlsUnLocked;

		public void OnDestroy()
		{
			this.OnStickDirectionEvent = null;
			this.OnSteerDirectionEvent = null;
			this.OnSteerRotationDirectionEvent = null;
			this.OnSteerRotationFlushEvent = null;
			this.OnDoActionEvent = null;
			this.OnInteractionPreStartedEvent = null;
			this.OnInteractionStartedEvent = null;
			this.OnControllerChangedEvent = null;
			this.OnLocomotionStateChangedEvent = null;
			this.OnLandedJumpEvent = null;
			this.OnControlsLocked = null;
			this.OnControlsUnLocked = null;
		}

		public void BroadcastOnStickDirectionEvent(Vector2 steer)
		{
			if (this.OnStickDirectionEvent != null)
			{
				this.OnStickDirectionEvent(steer);
			}
		}

		public void BroadcastOnSteerDirectionEvent(Vector3 steer)
		{
			if (this.OnSteerDirectionEvent != null)
			{
				this.OnSteerDirectionEvent(steer);
			}
		}

		public void BroadcastOnSteerRotationDirectionEvent(Vector3 steer)
		{
			if (this.OnSteerRotationDirectionEvent != null)
			{
				this.OnSteerRotationDirectionEvent(steer);
			}
		}

		public void BroadcastOnSteerRotationFlushEvent()
		{
			if (this.OnSteerRotationFlushEvent != null)
			{
				this.OnSteerRotationFlushEvent();
			}
		}

		public void BroadcastOnDoAction(LocomotionController.LocomotionAction action, object userData = null)
		{
			if (this.OnDoActionEvent != null)
			{
				this.OnDoActionEvent(action, userData);
			}
		}

		public void BroadcastSetLocomotionState(LocomotionState state)
		{
			if (this.OnLocomotionStateChangedEvent != null)
			{
				this.OnLocomotionStateChangedEvent(state);
			}
		}

		public void BroadcastOnInteractionPreStarted(GameObject trigger)
		{
			if (this.OnInteractionPreStartedEvent != null)
			{
				this.OnInteractionPreStartedEvent(trigger);
			}
		}

		public void BroadcastOnInteractionStarted(GameObject trigger)
		{
			if (this.OnInteractionStartedEvent != null)
			{
				this.OnInteractionStartedEvent(trigger);
			}
		}

		public void BroadcastOnControllerChanged(LocomotionController newController)
		{
			if (this.OnControllerChangedEvent != null)
			{
				this.OnControllerChangedEvent(newController);
			}
		}

		public void BroadcastOnLandedJump()
		{
			if (this.OnLandedJumpEvent != null)
			{
				this.OnLandedJumpEvent();
			}
		}

		public void BroadcastOnControlsLocked()
		{
			if (this.OnControlsLocked != null)
			{
				this.OnControlsLocked();
			}
		}

		public void BroadcastOnControlsUnLocked()
		{
			if (this.OnControlsUnLocked != null)
			{
				this.OnControlsUnLocked();
			}
		}
	}
}
