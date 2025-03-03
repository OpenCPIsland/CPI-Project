using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Input
{
	public abstract class PenguinControlsInputHandlerLib<TResult> : InputMapHandler<TResult> where TResult : new()
	{
		protected EventDispatcher eventDispatcher;

		private UIElementDisablerManager disablerManager;

		private ExitTransitionStateHandler exitStateHandler;

		private LocomotionTracker locomotionTracker;

		protected override void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			disablerManager = Service.Get<UIElementDisablerManager>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			GameObjectReferenceData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component) && component.GameObject != null)
			{
				identifiedLocalPlayer(component.GameObject);
			}
			base.Awake();
		}

		protected virtual void identifiedLocalPlayer(GameObject localPlayer)
		{
			locomotionTracker = localPlayer.GetComponent<LocomotionTracker>();
		}

		private void Start()
		{
			exitStateHandler = GetComponentInParent<ExitTransitionStateHandler>();
			addControlsExitTriggeredCallback();
		}

		protected override void OnEnable()
		{
			addControlsExitTriggeredCallback();
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			exitStateHandler.OnExitTriggered -= onControlsExiting;
			base.OnDisable();
		}

		private void addControlsExitTriggeredCallback()
		{
			if (exitStateHandler != null)
			{
				exitStateHandler.OnExitTriggered += onControlsExiting;
			}
		}

		private void onControlsExiting()
		{
			OnDisable();
		}

		protected void handleLocomotionInput(LocomotionInputResult locomotionInputResult)
		{
			bool flag = disablerManager.IsUIElementDisabled("Joystick");
			eventDispatcher.DispatchEvent(new InputEvents.MoveEvent(flag ? Vector2.zero : locomotionInputResult.Direction));
			if (!flag && locomotionInputResult.Rotation.sqrMagnitude > 0f)
			{
				eventDispatcher.DispatchEvent(new InputEvents.RotateEvent(locomotionInputResult.Rotation));
			}
			if (!flag && locomotionInputResult.Direction.sqrMagnitude > 0f)
			{
				logLocomotionAction(locomotionInputResult.LogType);
			}
		}

		private void logLocomotionAction(string locomotionType)
		{
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			string tier = (currentController != null) ? currentController.GetType().Name.Replace("Controller", "") : "undefined";
			string callID = string.Format("locomotion_{0}", locomotionType);
			Service.Get<ICPSwrveService>().ActionSingular(callID, locomotionType, tier);
		}
	}
}
