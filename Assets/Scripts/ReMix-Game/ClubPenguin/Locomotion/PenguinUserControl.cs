#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(LocomotionTracker))]
	public class PenguinUserControl : MonoBehaviour
	{
		public static float DefaultActionRequestBufferTime = 0.5f;

		private static readonly float minTimeBetweenSlideToggles = 1.5f;

		private LocomotionTracker tracker;

		private float lastSlideToggleTime;

		private bool togglingSlide;

		private EventChannel eventChannel;

		private LocomotionEventBroadcaster locoEventBroadcaster;

		public void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			tracker = GetComponent<LocomotionTracker>();
			locoEventBroadcaster = GetComponent<LocomotionEventBroadcaster>();
			Assert.IsNotNull(eventChannel, "EventChannel is null");
			Assert.IsNotNull(tracker, "Tracker is null");
		}

		public void OnEnable()
		{
			eventChannel.AddListener<InputEvents.MoveEvent>(OnMove);
			eventChannel.AddListener<InputEvents.RotateEvent>(onRotate);
			eventChannel.AddListener<InputEvents.ActionEvent>(OnAction);
			eventChannel.AddListener<InputEvents.LocomotionStateEvent>(OnLocomotionStateChange);
			eventChannel.AddListener<InputEvents.ChargeActionEvent>(OnChargeAction);
			eventChannel.AddListener<InputEvents.SwitchChangeEvent>(OnSwitchChange);
			eventChannel.AddListener<InputEvents.CycleChangeEvent>(OnCycleChange);
			eventChannel.AddListener<InputEvents.ActionEnabledEvent>(OnActionEnabled);
			locoEventBroadcaster.BroadcastOnControlsUnLocked();
		}

		public void OnDisable()
		{
			eventChannel.RemoveAllListeners();
			locoEventBroadcaster.BroadcastOnControlsLocked();
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			LocomotionController currentController = tracker.GetCurrentController();
			if (currentController != null)
			{
				currentController.Steer(Vector3.zero);
			}
		}

		private bool OnMove(InputEvents.MoveEvent evt)
		{
			if (SceneRefs.ActionSequencer != null)
			{
				SceneRefs.ActionSequencer.UserInputReceived();
			}
			LocomotionController currentController = tracker.GetCurrentController();
			if (currentController != null)
			{
				currentController.Steer(evt.Direction);
			}
			return false;
		}

		private bool onRotate(InputEvents.RotateEvent evt)
		{
			LocomotionController currentController = tracker.GetCurrentController();
			if (currentController != null)
			{
				currentController.SteerRotation(evt.Direction);
			}
			return false;
		}

		private bool OnAction(InputEvents.ActionEvent evt)
		{
			LocomotionController currentController = tracker.GetCurrentController();
			if (currentController != null)
			{
				switch (evt.Action)
				{
				case InputEvents.Actions.Jump:
					if (SceneRefs.ActionSequencer != null)
					{
						SceneRefs.ActionSequencer.UserInputReceived();
					}
					currentController.DoAction(LocomotionController.LocomotionAction.Jump);
					break;
				case InputEvents.Actions.Torpedo:
					if (SceneRefs.ActionSequencer != null)
					{
						SceneRefs.ActionSequencer.UserInputReceived();
					}
					currentController.DoAction(LocomotionController.LocomotionAction.Torpedo);
					break;
				case InputEvents.Actions.Interact:
					if (!Service.Get<PropService>().IsLocalUserUsingProp() || Service.Get<PropService>().CanActionsBeUsedWithProp())
					{
						if (SceneRefs.ActionSequencer != null)
						{
							SceneRefs.ActionSequencer.UserInputReceived();
						}
						currentController.DoAction(LocomotionController.LocomotionAction.Interact);
					}
					break;
				case InputEvents.Actions.Action1:
					if (SceneRefs.ActionSequencer != null)
					{
						SceneRefs.ActionSequencer.UserInputReceived();
					}
					currentController.DoAction(LocomotionController.LocomotionAction.Action1);
					break;
				case InputEvents.Actions.Action2:
					if (SceneRefs.ActionSequencer != null)
					{
						SceneRefs.ActionSequencer.UserInputReceived();
					}
					currentController.DoAction(LocomotionController.LocomotionAction.Action2);
					break;
				case InputEvents.Actions.Action3:
					if (SceneRefs.ActionSequencer != null)
					{
						SceneRefs.ActionSequencer.UserInputReceived();
					}
					currentController.DoAction(LocomotionController.LocomotionAction.Action3);
					break;
				}
			}
			return false;
		}

		private bool OnLocomotionStateChange(InputEvents.LocomotionStateEvent evt)
		{
			if (SceneRefs.ActionSequencer != null)
			{
				SceneRefs.ActionSequencer.UserInputReceived();
			}
			LocomotionController currentController = tracker.GetCurrentController();
			if (currentController != null)
			{
				currentController.SetState(evt.LocomotionState);
			}
			return false;
		}

		private bool OnChargeAction(InputEvents.ChargeActionEvent evt)
		{
			LocomotionController currentController = tracker.GetCurrentController();
			if (currentController != null && evt.Action == InputEvents.ChargeActions.Snowball)
			{
				if (evt.ButtonState)
				{
					currentController.DoAction(LocomotionController.LocomotionAction.ChargeThrow);
				}
				else
				{
					currentController.DoAction(LocomotionController.LocomotionAction.LaunchThrow, evt.HoldTime);
				}
			}
			return false;
		}

		private bool OnSwitchChange(InputEvents.SwitchChangeEvent evt)
		{
			if (evt.Switch == InputEvents.Switches.Tube && !togglingSlide)
			{
				CoroutineRunner.Start(toggleSlide(), this, "toggleSlide");
			}
			return false;
		}

		private bool OnCycleChange(InputEvents.CycleChangeEvent evt)
		{
			return false;
		}

		private bool OnActionEnabled(InputEvents.ActionEnabledEvent evt)
		{
			LocomotionController currentController = tracker.GetCurrentController();
			if (currentController != null)
			{
				InputEvents.Actions action = evt.Action;
				if (action == InputEvents.Actions.Snowball)
				{
					currentController.EnableAction(LocomotionController.LocomotionAction.ChargeThrow, evt.Enabled);
				}
			}
			return false;
		}

		private IEnumerator toggleSlide()
		{
			togglingSlide = true;
			float timeRemainingBeforeAllowingToggle = minTimeBetweenSlideToggles - (Time.time - lastSlideToggleTime);
			if (timeRemainingBeforeAllowingToggle > 0f)
			{
				yield return new WaitForSeconds(timeRemainingBeforeAllowingToggle);
			}
			if (SceneRefs.ActionSequencer != null)
			{
				SceneRefs.ActionSequencer.UserInputReceived();
			}
			lastSlideToggleTime = Time.time;
			if (tracker.IsCurrentControllerOfType<SlideController>())
			{
				tracker.SetCurrentController<RunController>();
			}
			else if (tracker.IsCurrentControllerOfType<RunController>())
			{
				tracker.SetCurrentController<SlideController>();
			}
			togglingSlide = false;
		}
	}
}
