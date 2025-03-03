#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Net.Locomotion
{
	[RequireComponent(typeof(LocomotionTracker))]
	public class LocomotionReceiver : MonoBehaviour
	{
		private enum SyncState
		{
			IDLE,
			WAITING,
			INTERPOLATION,
			EXTRAPOLATION
		}

		public bool Allow3DMovement = false;

		public long MaxQueueTimeMS = 8000L;

		public long WarningQueueTimeMS = 4000L;

		public float SnapThreshold = 0.5f;

		public float SnapHeightThreshold = 5f;

		public long InterpolationDelay = 600L;

		public long MaxExtrapolationTime = 1000L;

		private LocomotionTracker locomotionTracker;

		private AvatarLocomotionStateSetter locomotionStateSetter;

		private AvatarDataHandle playerDataHandle;

		private RemotePenguinSnowballThrower remoteSnowballLauncher;

		private long playerSessionId;

		private PositionTimeline positionTimeline;

		private Stopwatch timer;

		private long motionSequenceStartTime;

		private Vector3 startPosition;

		private Vector3 startFacing;

		private LocomotionActionEvent desiredStartEvent;

		private LocomotionActionEvent desiredTargetEvent;

		private SyncState state = SyncState.IDLE;

		private Vector3 lastJumpPosition;

		private Vector3 lastStickDirection;

		private EventChannel eventChannel;

		private CPDataEntityCollection dataEntityCollection;

		private LocomotionData locomotionData;

		private bool receivingEnabled = true;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		private bool pendingSlide;

		private bool pendingRun;

		public event Action<LocomotionActionEvent> OnTriggerActionEvent;

		public void EnableReceiving(bool enabled = true)
		{
			receivingEnabled = enabled;
		}

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			locomotionTracker = GetComponent<LocomotionTracker>();
			locomotionStateSetter = GetComponent<AvatarLocomotionStateSetter>();
			playerDataHandle = GetComponent<AvatarDataHandle>();
			positionTimeline = new PositionTimeline(MaxQueueTimeMS, WarningQueueTimeMS);
			remoteSnowballLauncher = GetComponent<RemotePenguinSnowballThrower>();
			timer = new Stopwatch();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerActionServiceEvents.LocomotionActionReceived>(onLocomotionAction);
			locomotionEventBroadcaster = GetComponent<LocomotionEventBroadcaster>();
			locomotionEventBroadcaster.OnControlsLocked += onControlsLocked;
			locomotionEventBroadcaster.OnControlsUnLocked += onControlsUnLocked;
		}

		public void Init()
		{
			Assert.IsTrue(playerDataHandle != null, "AvatarDataHandle is null");
			if (dataEntityCollection.TryGetComponent(playerDataHandle.Handle, out locomotionData))
			{
				SubscribeLocomotionStateChange();
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<LocomotionData>>(onLocomotionDataAdded);
			}
		}

		private void SubscribeLocomotionStateChange()
		{
			if (locomotionData.LocomotionStateIsInitialized)
			{
				locomotionStateChanged(locomotionData.LocoState);
			}
			locomotionData.PlayerLocoStateChanged += locomotionStateChanged;
		}

		private bool onLocomotionDataAdded(DataEntityEvents.ComponentAddedEvent<LocomotionData> evt)
		{
			if (evt.Handle == playerDataHandle.Handle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<LocomotionData>>(onLocomotionDataAdded);
				locomotionData = evt.Component;
				SubscribeLocomotionStateChange();
			}
			return false;
		}

		private void OnDestroy()
		{
			if (locomotionData != null)
			{
				locomotionData.PlayerLocoStateChanged -= locomotionStateChanged;
			}
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			locomotionEventBroadcaster.OnControlsLocked -= onControlsLocked;
			locomotionEventBroadcaster.OnControlsUnLocked -= onControlsUnLocked;
			this.OnTriggerActionEvent = null;
		}

		private bool onLocomotionAction(PlayerActionServiceEvents.LocomotionActionReceived evt)
		{
			if (!base.gameObject.IsDestroyed() && isThisPlayer(evt.SessionId))
			{
				switch (evt.Action.Type)
				{
				case LocomotionAction.Interact:
				case LocomotionAction.ChargeThrow:
				case LocomotionAction.LaunchThrow:
				case LocomotionAction.CancelThrow:
					addEvent(evt.Action, receivingEnabled);
					break;
				default:
					addEvent(evt.Action);
					break;
				}
			}
			return false;
		}

		private bool isThisPlayer(long sessionId)
		{
			if (playerSessionId > 0)
			{
				return playerSessionId == sessionId;
			}
			DataEntityHandle handle;
			SessionIdData component;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && dataEntityCollection.TryGetComponent(handle, out component))
			{
				playerSessionId = component.SessionId;
				return playerSessionId == sessionId;
			}
			return false;
		}

		private void locomotionStateChanged(LocomotionState state)
		{
			switch (state)
			{
			case LocomotionState.Slide:
				if (receivingEnabled)
				{
					locomotionTracker.SetCurrentController<SlideController>();
					break;
				}
				pendingSlide = true;
				pendingRun = false;
				break;
			case LocomotionState.Racing:
				if (receivingEnabled)
				{
					if (playerDataHandle.IsLocalPlayer)
					{
						locomotionTracker.SetCurrentController<RaceController>();
					}
					else if (LocomotionHelper.IsCurrentControllerOfType<SlideController>(base.gameObject))
					{
						locomotionTracker.SetCurrentController<RaceController>();
					}
					else
					{
						locomotionTracker.SetCurrentController<SlideController>();
					}
				}
				else
				{
					pendingSlide = true;
					pendingRun = false;
				}
				break;
			case LocomotionState.Default:
				if (receivingEnabled)
				{
					locomotionTracker.SetCurrentController<RunController>();
					break;
				}
				pendingSlide = false;
				pendingRun = true;
				break;
			default:
				pendingSlide = false;
				pendingRun = false;
				break;
			}
		}

		private void addEvent(LocomotionActionEvent evt, bool force = false)
		{
			if (evt.Type == LocomotionAction.SnapToPosition)
			{
				snapToPosition(evt.Position);
				return;
			}
			switch (state)
			{
			case SyncState.IDLE:
				addToTimeline(evt);
				break;
			case SyncState.WAITING:
				if (addToTimeline(evt) == 0)
				{
					desiredTargetEvent = evt;
					snapIfNeeded(desiredTargetEvent.Position);
				}
				break;
			case SyncState.INTERPOLATION:
				if (!force && desiredStartEvent.Timestamp != 0 && timeForPosition(desiredStartEvent, evt))
				{
					break;
				}
				if (force && timeForPosition(desiredStartEvent, evt))
				{
					triggerEventAction(evt);
					break;
				}
				if (evt.Timestamp < desiredTargetEvent.Timestamp)
				{
					LocomotionActionEvent locomotionActionEvent = evt;
					evt = desiredTargetEvent;
					desiredTargetEvent = locomotionActionEvent;
				}
				addToTimeline(evt);
				break;
			case SyncState.EXTRAPOLATION:
				if (!force && evt.Timestamp < desiredTargetEvent.Timestamp)
				{
					break;
				}
				if (force && evt.Timestamp < desiredTargetEvent.Timestamp)
				{
					triggerEventAction(evt);
					break;
				}
				if (timeForPosition(desiredTargetEvent, evt))
				{
				}
				addToTimeline(evt);
				break;
			}
		}

		private int addToTimeline(LocomotionActionEvent evt)
		{
			bool queueTooLong;
			int result = positionTimeline.Enqueue(evt, out queueTooLong);
			if (queueTooLong || (desiredStartEvent.Timestamp != 0 && evt.Timestamp - desiredStartEvent.Timestamp > MaxQueueTimeMS))
			{
				timer.Reset();
				timer.Start();
				motionSequenceStartTime = positionTimeline.Peek().Timestamp;
				desiredTargetEvent.Timestamp = motionSequenceStartTime;
			}
			return result;
		}

		private void Update()
		{
			if (!receivingEnabled)
			{
				return;
			}
			switch (state)
			{
			case SyncState.IDLE:
				if (positionTimeline.Count > 0)
				{
					desiredTargetEvent = positionTimeline.Peek();
					snapIfNeeded(desiredTargetEvent.Position);
					timer.Reset();
					timer.Start();
					state = SyncState.WAITING;
				}
				break;
			case SyncState.WAITING:
				if (timer.ElapsedMilliseconds > InterpolationDelay || positionTimeline.PeekLast().Timestamp - desiredTargetEvent.Timestamp >= InterpolationDelay)
				{
					timer.Reset();
					timer.Start();
					switchToNextPosition();
					motionSequenceStartTime = desiredStartEvent.Timestamp;
				}
				break;
			case SyncState.INTERPOLATION:
				if (!timeForPosition(desiredStartEvent, desiredTargetEvent))
				{
					break;
				}
				triggerEventAction(desiredTargetEvent);
				if (positionTimeline.Count == 0)
				{
					LocomotionController currentController = locomotionTracker.GetCurrentController();
					if (currentController != null && !(currentController is SlideController))
					{
						currentController.Steer(lastStickDirection.normalized);
					}
					if (lastStickDirection == Vector3.zero)
					{
						timer.Stop();
						state = SyncState.IDLE;
					}
					else
					{
						state = SyncState.EXTRAPOLATION;
					}
				}
				else
				{
					switchToNextPosition();
				}
				break;
			case SyncState.EXTRAPOLATION:
			{
				LocomotionController currentController = locomotionTracker.GetCurrentController();
				if (positionTimeline.Count == 0)
				{
					if (timer.ElapsedMilliseconds > MaxExtrapolationTime)
					{
						if (currentController != null && !(currentController is SlideController))
						{
							currentController.Steer(Vector3.zero);
						}
						timer.Stop();
						lastStickDirection = Vector3.zero;
						state = SyncState.IDLE;
					}
				}
				else
				{
					desiredTargetEvent.Position = ((currentController != null) ? currentController.GetPosition() : base.transform.position);
					desiredTargetEvent.Timestamp = motionSequenceStartTime + timer.ElapsedMilliseconds;
					switchToNextPosition();
				}
				break;
			}
			}
		}

		private void LateUpdate()
		{
			SyncState syncState = state;
			if (syncState != SyncState.INTERPOLATION || desiredStartEvent.Timestamp == desiredTargetEvent.Timestamp)
			{
				return;
			}
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			float t = (float)(motionSequenceStartTime + timer.ElapsedMilliseconds - desiredStartEvent.Timestamp) / (float)(desiredTargetEvent.Timestamp - desiredStartEvent.Timestamp);
			Vector3 vector = Vector3.Lerp(startPosition, desiredTargetEvent.Position, t);
			if (!Allow3DMovement)
			{
				vector.y = ((currentController != null) ? currentController.GetPosition().y : base.transform.position.y);
			}
			if (currentController != null)
			{
				currentController.RemoteSetPosition(vector);
				if (currentController is SlideController && desiredTargetEvent.Velocity.HasValue)
				{
					Vector3 newFacing = Vector3.Slerp(startFacing, desiredTargetEvent.Velocity.Value, t);
					currentController.RemoteSetFacing(newFacing);
				}
			}
			else
			{
				base.transform.position = vector;
			}
		}

		private bool timeForPosition(LocomotionActionEvent currentPosition, LocomotionActionEvent nextPosition)
		{
			if (currentPosition.Timestamp == 0)
			{
				return true;
			}
			return motionSequenceStartTime + timer.ElapsedMilliseconds >= nextPosition.Timestamp;
		}

		private void switchToNextPosition()
		{
			LocomotionActionEvent locomotionActionEvent = positionTimeline.Dequeue();
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			Vector3 b = (currentController != null) ? currentController.GetPosition() : base.transform.position;
			Vector3 wsSteerInput = locomotionActionEvent.Position - b;
			wsSteerInput.y = 0f;
			if (locomotionActionEvent.Direction.HasValue)
			{
				wsSteerInput = locomotionActionEvent.Direction.Value;
				if (locomotionActionEvent.Type.IsMovement())
				{
					lastStickDirection = wsSteerInput;
				}
			}
			if (currentController != null && !(currentController is SlideController))
			{
				if (locomotionActionEvent.Type.IsMovement())
				{
					currentController.Steer(wsSteerInput);
				}
				else if (locomotionActionEvent.Type == LocomotionAction.Rotate)
				{
					currentController.SteerRotation(wsSteerInput);
				}
			}
			desiredStartEvent = desiredTargetEvent;
			desiredTargetEvent = locomotionActionEvent;
			startPosition = b;
			startFacing = ((currentController != null) ? currentController.GetFacing() : base.transform.forward);
			state = SyncState.INTERPOLATION;
		}

		private void triggerEventAction(LocomotionActionEvent targetEvent)
		{
			snapIfNeeded(targetEvent.Position);
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			switch (targetEvent.Type)
			{
			case LocomotionAction.Torpedo:
				lastJumpPosition = targetEvent.Position;
				if (currentController != null)
				{
					currentController.DoAction(LocomotionController.LocomotionAction.Torpedo);
				}
				break;
			case LocomotionAction.Jump:
				lastJumpPosition = targetEvent.Position;
				if (currentController != null)
				{
					currentController.DoAction(LocomotionController.LocomotionAction.Jump);
				}
				break;
			case LocomotionAction.Interact:
				snapToGround(targetEvent.Position);
				if (currentController != null)
				{
					currentController.DoAction(LocomotionController.LocomotionAction.Interact);
				}
				break;
			case LocomotionAction.ChargeThrow:
				remoteSnowballLauncher.ChargeSnowball();
				break;
			case LocomotionAction.LaunchThrow:
				remoteSnowballLauncher.LaunchSnowball(targetEvent.Velocity.Value);
				break;
			case LocomotionAction.CancelThrow:
				remoteSnowballLauncher.CancelChargeSnowball();
				break;
			case LocomotionAction.Action1:
			case LocomotionAction.Action2:
			case LocomotionAction.Action3:
				locomotionStateSetter.ActionButton(targetEvent.Type);
				break;
			}
			if (this.OnTriggerActionEvent != null)
			{
				this.OnTriggerActionEvent(targetEvent);
			}
		}

		public void AnimEvent_JumpStarted()
		{
			snapToGround(lastJumpPosition);
		}

		private void onControlsLocked()
		{
			receivingEnabled = false;
		}

		private void onControlsUnLocked()
		{
			receivingEnabled = true;
			if (pendingRun)
			{
				locomotionTracker.SetCurrentController<RunController>();
				pendingRun = false;
			}
			if (pendingSlide)
			{
				locomotionTracker.SetCurrentController<SlideController>();
				pendingSlide = false;
			}
		}

		private void snapIfNeeded(Vector3 target)
		{
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			Vector3 b = (currentController != null) ? currentController.GetPosition() : base.transform.position;
			if (!Allow3DMovement)
			{
				if (Vector2.Distance(new Vector2(target.x, target.z), new Vector2(b.x, b.z)) > SnapThreshold)
				{
					snapToGround(target);
				}
				if (Mathf.Abs(target.y - b.y) > SnapHeightThreshold)
				{
					snapToGround(target);
				}
			}
			else if (Vector3.Distance(target, b) > SnapThreshold)
			{
				snapToPosition(target);
			}
		}

		private void snapToGround(Vector3 target)
		{
			if (!Allow3DMovement)
			{
				float y = target.y;
				RaycastHit hitInfo;
				if (Physics.Raycast(target, Vector3.down, out hitInfo, 100f, RemotePlayerMask.remotePlayerLayerMask))
				{
					y = hitInfo.point.y;
				}
				else if (Physics.Raycast(target, Vector3.up, out hitInfo, 100f, RemotePlayerMask.remotePlayerLayerMask))
				{
					y = hitInfo.point.y;
				}
				target.y = y;
			}
			snapToPosition(target);
		}

		private void snapToPosition(Vector3 newPos)
		{
			LocomotionController currentController = locomotionTracker.GetCurrentController();
			if (currentController != null)
			{
				currentController.RemoteSnapPosition(newPos);
			}
			else
			{
				base.transform.position = newPos;
			}
		}
	}
}
