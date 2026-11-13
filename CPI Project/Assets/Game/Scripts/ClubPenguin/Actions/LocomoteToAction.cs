using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Client.Event;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class LocomoteToAction : Action
	{
		public List<Transform> Waypoints = new List<Transform>();

		public PlayerLocoStyle.Style Style = PlayerLocoStyle.Style.Walk;

		public float DistanceThreshold = 0.15f;

		public float AngleThreshold = 1f;

		public bool UseShortestPath;

		public bool SnapRotAtEnd = true;

		public bool DontSnapYPosAtEnd = false;

		private LocomotionTracker tracker;

		private RunController runController;

		private PlayerLocoStyle.Style oldLocoStyle;

		private float actorRadius;

		private float actorHalfHeight;

		private int raycastLayerMask;

		private Transform thisTransform;

		private GameObject actionTarget;

		public bool IsEnabled
		{
			get;
			set;
		}

		protected override void CopyTo(Action _destWarper)
		{
			LocomoteToAction locomoteToAction = _destWarper as LocomoteToAction;
			locomoteToAction.Waypoints = new List<Transform>(Waypoints);
			locomoteToAction.Style = Style;
			locomoteToAction.DistanceThreshold = DistanceThreshold;
			locomoteToAction.AngleThreshold = AngleThreshold;
			locomoteToAction.UseShortestPath = UseShortestPath;
			locomoteToAction.SnapRotAtEnd = SnapRotAtEnd;
			locomoteToAction.DontSnapYPosAtEnd = DontSnapYPosAtEnd;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			if (IsEnabled)
			{
				actionTarget = GetTarget();
				tracker = actionTarget.GetComponent<LocomotionTracker>();
				if (tracker.SetCurrentController<RunController>())
				{
					thisTransform = actionTarget.transform;
					raycastLayerMask = LayerConstants.GetPlayerLayerCollisionMask();
					actorRadius = 0f;
					actorHalfHeight = 0.1f;
					CharacterController component = actionTarget.GetComponent<CharacterController>();
					if (component != null)
					{
						actorRadius = component.radius;
						actorHalfHeight = component.height / 2f;
					}
					runController = actionTarget.GetComponent<RunController>();
					base.OnEnable();
					CoroutineRunner.Start(doMoveTo(), this, "doMoveTo");
				}
			}
			else
			{
				CoroutineRunner.Start(completeAfterFrame(), this, "completeAfterFrame");
			}
		}

		private IEnumerator completeAfterFrame()
		{
			yield return new WaitForFrame(1);
			Completed();
		}

		protected override void Abort()
		{
			CoroutineRunner.StopAllForOwner(this);
			base.Abort();
		}

		private int FindFarthestReachableWaypoint(int curWaypoint)
		{
			for (int num = Waypoints.Count - 1; num > curWaypoint; num--)
			{
				if (CanReachWaypoint(num))
				{
					return num;
				}
			}
			return curWaypoint;
		}

		private bool CanReachWaypoint(int waypointIndex)
		{
			bool result = true;
			Transform transform = Waypoints[waypointIndex];
			if (transform != null)
			{
				Vector3 vector = actionTarget.transform.position + Vector3.up * actorHalfHeight;
				Vector3 a = transform.position + Vector3.up * actorHalfHeight;
				Vector3 direction = a - vector;
				float magnitude = direction.magnitude;
				Vector3 normalized = new Vector3(0f - direction.z, 0f, direction.x).normalized;
				Debug.DrawLine(vector + normalized * actorRadius, vector + normalized * actorRadius + direction.normalized);
				Debug.DrawLine(vector - normalized * actorRadius, vector - normalized * actorRadius + direction.normalized);
				if (Physics.Raycast(vector + normalized * actorRadius, direction, magnitude, raycastLayerMask) || Physics.Raycast(vector - normalized * actorRadius, direction, magnitude, raycastLayerMask))
				{
					result = false;
				}
			}
			return result;
		}

		private IEnumerator doMoveTo()
		{
			if (runController != null && runController.enabled)
			{
				Animator anim = actionTarget.GetComponent<Animator>();
				Transform tempTransform = null;
				RunController.ControllerBehaviour oldRunBehaviour = runController.Behaviour;
				runController.Behaviour = new RunController.ControllerBehaviour
				{
					IgnoreCollisions = false,
					IgnoreGravity = false,
					IgnoreRotation = false,
					IgnoreTranslation = false,
					IgnoreJumpRequests = true,
					IgnoreStickInput = true,
					Style = Style
				};
				bool runControllerBehaviourWasSet = true;
				AnimatorStateInfo animStateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);
				while (!LocomotionUtils.IsLocomoting(animStateInfo) && !LocomotionUtils.IsLanding(animStateInfo) && !LocomotionUtils.IsIdling(animStateInfo))
				{
					yield return null;
					animStateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);
				}
				runController.ResetMomentum();
				if (IncomingUserData != null && IncomingUserData.GetType() == typeof(Vector3))
				{
					Vector3 vector = (Vector3)IncomingUserData;
					Vector3 vector2 = vector - actionTarget.transform.position;
					vector2.y = 0f;
					if (vector2 == Vector3.zero)
					{
						vector2 = base.transform.forward;
					}
					tempTransform = new GameObject().transform;
					tempTransform.rotation = Quaternion.LookRotation(vector2);
					tempTransform.position = vector;
					Waypoints.Clear();
					Waypoints.Add(tempTransform);
				}
				if (Waypoints.Count > 0)
				{
					float distThresholdSq = DistanceThreshold * DistanceThreshold;
					float prevDistSq = float.PositiveInfinity;
					float elapsedTime = 0f;
					bool done = false;
					int curWaypoint2 = 0;
					do
					{
						if (thisTransform.IsDestroyed() || actionTarget.IsDestroyed())
						{
							Log.LogError(this, "Aborting LocomoteToAction as an object has been destroyed");
							break;
						}
						if (UseShortestPath)
						{
							curWaypoint2 = FindFarthestReachableWaypoint(curWaypoint2);
						}
						Vector3 toTarget = Waypoints[curWaypoint2].position - thisTransform.position;
						toTarget.y = 0f;
						float distSq = toTarget.sqrMagnitude;
						if (distSq <= distThresholdSq || distSq > prevDistSq)
						{
							curWaypoint2++;
							if (curWaypoint2 >= Waypoints.Count)
							{
								done = true;
							}
							else
							{
								toTarget = Waypoints[curWaypoint2].position - thisTransform.position;
								toTarget.y = 0f;
								runController.Steer(toTarget.normalized);
							}
						}
						else
						{
							runController.Steer(toTarget.normalized);
						}
						elapsedTime += Time.deltaTime;
						if (elapsedTime > 5f)
						{
							done = true;
						}
						yield return null;
					}
					while (!done);
					curWaypoint2 = Waypoints.Count - 1;
					runController.Steer(Vector3.zero);
					if (DontSnapYPosAtEnd)
					{
						Vector3 position = Waypoints[curWaypoint2].position;
						position.y = thisTransform.position.y;
						runController.SnapToPosition(position);
					}
					else
					{
						runController.SnapToPosition(Waypoints[curWaypoint2].position);
					}
					if (SnapRotAtEnd)
					{
						runController.SnapToFacing(Waypoints[curWaypoint2].forward);
					}
				}
				if (tempTransform != null)
				{
					Object.Destroy(tempTransform.gameObject);
					Waypoints.Clear();
				}
				if (runControllerBehaviourWasSet)
				{
					runController.Behaviour = oldRunBehaviour;
				}
			}
			Completed();
		}
	}
}
