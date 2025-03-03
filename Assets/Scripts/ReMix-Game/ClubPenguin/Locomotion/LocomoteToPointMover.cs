using ClubPenguin.Core;
using ClubPenguin.Net.Client.Event;
using System;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[DisallowMultipleComponent]
	public class LocomoteToPointMover : MonoBehaviour
	{
		private const float DEFAULT_TIMEOUT_TIME = 15f;

		private RunController runController;

		private RunController.ControllerBehaviour oldRunBehaviour;

		private bool runControllerBehaviourWasSet;

		private float actorRadius;

		private float actorHalfHeight;

		private bool done;

		private Vector3 dest;

		private float distThresholdSq;

		private float elapsedTime;

		private float timeoutTime;

		private bool autoDestroy;

		private Transform targetDestination;

		public event Action OnComplete;

		public void MoveToTarget(Transform target, float distanceThreshold = 0.15f, PlayerLocoStyle.Style locomotionStyle = PlayerLocoStyle.Style.Walk, float timeoutTime = 15f, bool autoDestroy = true)
		{
			targetDestination = target;
			actorRadius = 0f;
			actorHalfHeight = 0.1f;
			this.timeoutTime = timeoutTime;
			this.autoDestroy = autoDestroy;
			CharacterController component = GetComponent<CharacterController>();
			if (component != null)
			{
				actorRadius = component.radius;
				actorHalfHeight = component.height / 2f;
			}
			LocomotionTracker component2 = GetComponent<LocomotionTracker>();
			if (component2.SetCurrentController<RunController>())
			{
				runController = GetComponent<RunController>();
				if (!runControllerBehaviourWasSet)
				{
					oldRunBehaviour = runController.Behaviour;
					runController.Behaviour = new RunController.ControllerBehaviour
					{
						IgnoreCollisions = false,
						IgnoreGravity = false,
						IgnoreRotation = false,
						IgnoreTranslation = false,
						IgnoreJumpRequests = true,
						IgnoreStickInput = true,
						Style = locomotionStyle
					};
					runControllerBehaviourWasSet = true;
				}
				runController.ResetMomentum();
				dest = targetDestination.transform.position;
				Vector3 lhs = dest - base.transform.position;
				if (lhs == Vector3.zero)
				{
					lhs = base.transform.forward;
				}
				distThresholdSq = distanceThreshold * distanceThreshold;
				lhs.y = 0f;
				elapsedTime = 0f;
				done = false;
			}
			if (!CanReachWaypoint())
			{
				if (this.OnComplete != null)
				{
					this.OnComplete();
				}
				runController.Steer(Vector3.zero);
				runController.SnapToPosition(dest);
				runController.SnapToFacing(targetDestination.transform.forward);
				if (autoDestroy)
				{
					UnityEngine.Object.Destroy(this);
				}
			}
		}

		private void OnDestroy()
		{
			if (runControllerBehaviourWasSet)
			{
				runController.Behaviour = oldRunBehaviour;
			}
		}

		private void Update()
		{
			if (!(runController != null) || !runController.enabled)
			{
				return;
			}
			Vector3 vector = dest - base.transform.position;
			vector.y = 0f;
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude <= distThresholdSq)
			{
				done = true;
			}
			else
			{
				runController.Steer(vector.normalized);
			}
			elapsedTime += Time.deltaTime;
			if (elapsedTime > timeoutTime)
			{
				done = true;
			}
			if (done)
			{
				runController.Steer(Vector3.zero);
				runController.SnapToPosition(dest);
				runController.SnapToFacing(targetDestination.transform.forward);
				if (this.OnComplete != null)
				{
					this.OnComplete();
				}
				if (autoDestroy)
				{
					UnityEngine.Object.Destroy(this);
				}
			}
		}

		private bool CanReachWaypoint()
		{
			bool result = true;
			Vector3 vector = base.transform.position + Vector3.up * actorHalfHeight;
			Vector3 a = targetDestination.transform.position + Vector3.up * actorHalfHeight;
			Vector3 direction = a - vector;
			float magnitude = direction.magnitude;
			Vector3 normalized = new Vector3(0f - direction.z, 0f, direction.x).normalized;
			int playerLayerCollisionMask = LayerConstants.GetPlayerLayerCollisionMask();
			if (Physics.Raycast(vector + normalized * actorRadius, direction, magnitude, playerLayerCollisionMask) || Physics.Raycast(vector - normalized * actorRadius, direction, magnitude, playerLayerCollisionMask))
			{
				result = false;
			}
			return result;
		}
	}
}
