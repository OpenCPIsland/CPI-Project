using ClubPenguin.Actions;
using ClubPenguin.Collectibles;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Net.Locomotion
{
	[RequireComponent(typeof(LocomotionTracker))]
	[RequireComponent(typeof(LocomotionEventBroadcaster))]
	public class LocomotionBroadcaster : MonoBehaviour
	{
		public class InteractActionFilterTag
		{
		}

		public float UpdatePeriodSec = 0.2f;

		public float UpdateRotationPeriodSec = 1f;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		private LocomotionTracker locomotionTracker;

		private Vector3 lastPosition = Vector3.zero;

		private Vector3 lastSteerDirection = Vector3.zero;

		private Vector3 newSteerDirection = Vector3.zero;

		private float nextUpdateSendTime = 0f;

		private Vector3 lastSteerRotationDirection = Vector3.zero;

		private Vector3 newSteerRotationDirection = Vector3.zero;

		private float nextUpdateRotationSendTime = 0f;

		private int broadcastingDisabledEvents = 0;

		private INetworkServicesManager networkService;

		private void Start()
		{
			locomotionTracker = GetComponent<LocomotionTracker>();
			locomotionEventBroadcaster = GetComponent<LocomotionEventBroadcaster>();
			locomotionEventBroadcaster.OnDoActionEvent += sendDoActionEvent;
			locomotionEventBroadcaster.OnLocomotionStateChangedEvent += sendLocomotionStateChangedEvent;
			locomotionEventBroadcaster.OnInteractionStartedEvent += filterSendInteractionEvent;
			locomotionEventBroadcaster.OnSteerDirectionEvent += onSteerDirectionChanged;
			locomotionEventBroadcaster.OnSteerRotationDirectionEvent += onSteerRotationDirectionChanged;
			locomotionEventBroadcaster.OnSteerRotationFlushEvent += onSteerRotationFlush;
			locomotionEventBroadcaster.OnControllerChangedEvent += onControllerChanged;
			locomotionEventBroadcaster.OnControlsLocked += onControlsLocked;
			locomotionEventBroadcaster.OnControlsUnLocked += onControlsUnLocked;
			networkService = Service.Get<INetworkServicesManager>();
			Service.Get<EventDispatcher>().AddListener<WorldServiceEvents.SelfWillLeaveRoomEvent>(onUserLeaving);
			Service.Get<EventDispatcher>().AddListener<WorldServiceEvents.SelfRoomJoinedEvent>(onUserJoined);
		}

		private void OnDestroy()
		{
			locomotionEventBroadcaster.OnDoActionEvent -= sendDoActionEvent;
			locomotionEventBroadcaster.OnLocomotionStateChangedEvent -= sendLocomotionStateChangedEvent;
			locomotionEventBroadcaster.OnInteractionStartedEvent -= filterSendInteractionEvent;
			locomotionEventBroadcaster.OnSteerDirectionEvent -= onSteerDirectionChanged;
			locomotionEventBroadcaster.OnSteerRotationDirectionEvent -= onSteerRotationDirectionChanged;
			locomotionEventBroadcaster.OnSteerRotationFlushEvent -= onSteerRotationFlush;
			locomotionEventBroadcaster.OnControllerChangedEvent -= onControllerChanged;
			locomotionEventBroadcaster.OnControlsLocked -= onControlsLocked;
			locomotionEventBroadcaster.OnControlsUnLocked -= onControlsUnLocked;
			Service.Get<EventDispatcher>().RemoveListener<WorldServiceEvents.SelfWillLeaveRoomEvent>(onUserLeaving);
			Service.Get<EventDispatcher>().RemoveListener<WorldServiceEvents.SelfRoomJoinedEvent>(onUserJoined);
		}

		private bool onUserLeaving(WorldServiceEvents.SelfWillLeaveRoomEvent evt)
		{
			broadcastingDisabledEvents++;
			return false;
		}

		private bool onUserJoined(WorldServiceEvents.SelfRoomJoinedEvent evt)
		{
			broadcastingDisabledEvents = 0;
			return false;
		}

		private void LateUpdate()
		{
			if (MovementDirty())
			{
				if (Time.time > nextUpdateSendTime)
				{
					lastPosition = getCurrentPosition();
					lastSteerDirection = newSteerDirection;
					sendLocomotionUpdate(newSteerDirection, LocomotionAction.Move);
					nextUpdateSendTime = Time.time + UpdatePeriodSec;
				}
			}
			else if (RotationDirty() && Time.time > nextUpdateRotationSendTime)
			{
				sendRotation();
			}
		}

		private void sendRotation()
		{
			lastSteerRotationDirection = newSteerRotationDirection;
			sendLocomotionUpdate(newSteerRotationDirection, LocomotionAction.Rotate);
		}

		private Vector3 getCurrentPosition()
		{
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			return (currentController != null) ? currentController.GetPosition() : base.transform.position;
		}

		private void sendDoActionEvent(LocomotionController.LocomotionAction actionType, object userData = null)
		{
			switch (actionType)
			{
			case LocomotionController.LocomotionAction.SlideTrick:
			case LocomotionController.LocomotionAction.ChargeThrow:
			case LocomotionController.LocomotionAction.LaunchThrow:
			case LocomotionController.LocomotionAction.Interact:
				break;
			case LocomotionController.LocomotionAction.Jump:
				setLocomotionActionEvent(LocomotionAction.Jump, true);
				break;
			case LocomotionController.LocomotionAction.Torpedo:
				setLocomotionActionEvent(LocomotionAction.Torpedo, true);
				break;
			case LocomotionController.LocomotionAction.Action1:
				setLocomotionActionEvent(LocomotionAction.Action1);
				break;
			case LocomotionController.LocomotionAction.Action2:
				setLocomotionActionEvent(LocomotionAction.Action2);
				break;
			case LocomotionController.LocomotionAction.Action3:
				setLocomotionActionEvent(LocomotionAction.Action3);
				break;
			}
		}

		private void setLocomotionActionEvent(LocomotionAction locomotionAction, bool setDirection = false)
		{
			LocomotionActionEvent action = default(LocomotionActionEvent);
			action.Type = locomotionAction;
			action.Position = getCurrentPosition();
			if (setDirection)
			{
				action.Direction = newSteerDirection;
			}
			if (broadcastingDisabledEvents == 0)
			{
				networkService.PlayerActionService.LocomotionAction(action);
			}
		}

		private void filterSendInteractionEvent(GameObject trigger)
		{
			Service.Get<ActionConfirmationService>().ConfirmAction(typeof(InteractActionFilterTag), trigger, delegate
			{
				sendInteractionEvent(trigger);
			});
		}

		private void sendInteractionEvent(GameObject trigger)
		{
			ForceInteractionAction component = trigger.GetComponent<ForceInteractionAction>();
			if (component != null && !component.IsInteractionBroadcasted)
			{
				sendLocomotionUpdate(newSteerDirection, LocomotionAction.Move);
				return;
			}
			LocomotionActionEvent action = default(LocomotionActionEvent);
			action.Type = LocomotionAction.Interact;
			action.Position = getCurrentPosition();
			ObjectType type = ObjectType.LOCAL;
			string id = trigger.GetPath();
			string tag = "";
			if (trigger.GetComponent<AvatarDataHandle>() != null)
			{
				type = ObjectType.PLAYER;
				id = Service.Get<CPDataEntityCollection>().GetComponent<SessionIdData>(trigger.GetComponent<AvatarDataHandle>().Handle).SessionId.ToString();
				tag = trigger.tag;
			}
			else if (trigger.GetComponent<NetworkObjectController>() != null)
			{
				CPMMOItemId itemId = trigger.GetComponent<NetworkObjectController>().ItemId;
				if (itemId.Parent == CPMMOItemId.CPMMOItemParent.WORLD)
				{
					type = ObjectType.SERVER;
				}
				id = itemId.Id.ToString();
				tag = trigger.tag;
			}
			else
			{
				Transform parent = trigger.transform.parent;
				while (parent != null && parent.GetComponent<AvatarDataHandle>() == null)
				{
					parent = parent.transform.parent;
				}
				if (parent != null)
				{
					type = ObjectType.PLAYER;
					id = Service.Get<CPDataEntityCollection>().GetComponent<SessionIdData>(parent.GetComponent<AvatarDataHandle>().Handle).SessionId.ToString();
					tag = trigger.tag;
				}
				else if (trigger.transform.parent != null)
				{
					Collectible componentInChildren = trigger.transform.parent.gameObject.GetComponentInChildren<Collectible>();
					bool flag = false;
					if (componentInChildren != null && componentInChildren.RewardDef != null)
					{
						List<CollectibleRewardDefinition> definitions = componentInChildren.RewardDef.GetDefinitions<CollectibleRewardDefinition>();
						if (definitions.Count > 0 && definitions[0].Collectible != null)
						{
							tag = definitions[0].Collectible.CollectibleType;
							flag = true;
						}
					}
					if (!flag)
					{
						tag = trigger.tag;
					}
				}
				else
				{
					tag = trigger.tag;
				}
			}
			action.Object = new ActionedObject(type, id, tag);
			if (broadcastingDisabledEvents == 0)
			{
				networkService.PlayerActionService.LocomotionAction(action);
			}
		}

		private void sendLocomotionStateChangedEvent(LocomotionState state)
		{
			if (broadcastingDisabledEvents == 0)
			{
				networkService.PlayerStateService.SetLocomotionState(state);
			}
		}

		private void sendLocomotionUpdate(Vector3 direction, LocomotionAction type)
		{
			LocomotionActionEvent action = default(LocomotionActionEvent);
			action.Type = type;
			action.Position = getCurrentPosition();
			action.Direction = direction;
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			if (currentController is SlideController)
			{
				action.Velocity = currentController.GetFacing();
			}
			if (broadcastingDisabledEvents == 0)
			{
				networkService.PlayerActionService.LocomotionAction(action, true);
			}
			AvatarDataHandle component = GetComponent<AvatarDataHandle>();
			PositionData component2;
			if (!(component == null) && broadcastingDisabledEvents == 0 && Service.Get<CPDataEntityCollection>().TryGetComponent(component.Handle, out component2))
			{
				component2.Position = action.Position;
				PausedStateData component3;
				if (!Service.Get<CPDataEntityCollection>().TryGetComponent(component.Handle, out component3))
				{
					component3 = Service.Get<CPDataEntityCollection>().AddComponent<PausedStateData>(component.Handle);
				}
				component3.Position = component2.Position;
			}
		}

		private void onControllerChanged(LocomotionController newController)
		{
			if (broadcastingDisabledEvents == 0)
			{
				if (newController is RaceController)
				{
					networkService.PlayerStateService.SetLocomotionState(LocomotionState.Racing);
				}
				else if (newController is SlideController)
				{
					networkService.PlayerStateService.SetLocomotionState(LocomotionState.Slide);
				}
				else if (!(newController is SitController) && !(newController is ZiplineController))
				{
					networkService.PlayerStateService.SetLocomotionState(LocomotionState.Default);
				}
			}
		}

		private void onSteerDirectionChanged(Vector3 stickDir)
		{
			newSteerDirection = stickDir;
			if (newSteerDirection.magnitude > 0f)
			{
				newSteerRotationDirection = Vector3.zero;
			}
		}

		private void onSteerRotationDirectionChanged(Vector3 steer)
		{
			newSteerRotationDirection = steer;
			nextUpdateRotationSendTime = Time.time + UpdateRotationPeriodSec;
		}

		private void onSteerRotationFlush()
		{
			if (RotationDirty())
			{
				sendRotation();
			}
		}

		private void onControlsLocked()
		{
			broadcastingDisabledEvents++;
		}

		private void onControlsUnLocked()
		{
			if (broadcastingDisabledEvents > 0)
			{
				broadcastingDisabledEvents--;
			}
			lastSteerDirection = newSteerDirection + Vector3.one;
			nextUpdateSendTime = Time.time - 1f;
		}

		public bool MovementDirty()
		{
			return lastPosition != getCurrentPosition() || lastSteerDirection != newSteerDirection;
		}

		public bool RotationDirty()
		{
			return lastSteerRotationDirection != newSteerRotationDirection && newSteerRotationDirection != Vector3.zero;
		}
	}
}
