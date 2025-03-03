using ClubPenguin.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[RequireComponent(typeof(CameraController))]
	public abstract class BaseCamera : MonoBehaviour
	{
		private struct TransitionBlendState
		{
			public Vector3 FirstFramePos;

			public Quaternion FirstFrameRot;

			public Quaternion SmoothedAim;

			public float MaxSpeed;

			public bool DampenMovingTarget;

			public bool DoubleDampenMovingTargetAim;

			public AnimationCurve Curve;

			public float Duration;

			public float ElapsedTime;

			public bool IsBlending;

			public void Abort()
			{
				IsBlending = false;
				Curve = null;
			}
		}

		public Action Moved;

		private static readonly float cameraArriveDistance = 0.01f;

		private static readonly float cameraArriveDistanceSq = cameraArriveDistance * cameraArriveDistance;

		private static readonly float cameraDampTime = 0.2f;

		[SerializeField]
		private Vector3 desiredGoal;

		[SerializeField]
		private Vector3 constrainedGoal;

		[SerializeField]
		private Quaternion glance;

		[SerializeField]
		private Vector3 lookat;

		[SerializeField]
		private Vector3 prevTargetPosition;

		[SerializeField]
		private Vector3 prevTargetVelocity;

		private bool dirty;

		private bool snapMove;

		private bool snapAim;

		private int snapFrameCount;

		private bool snapLock;

		private bool isRotationLocked;

		private Vector3 lockedAxis;

		private Vector3 lockedAxisValues;

		[SerializeField]
		private Transform target;

		[SerializeField]
		private List<CameraController> controllers = new List<CameraController>();

		private List<Vector3> velocityFilterSamples;

		private int maxVelocityFilterSamples = 5;

		[SerializeField]
		private CameraController currentGoalPlannerController;

		[SerializeField]
		private CameraController currentFramerController;

		[SerializeField]
		private Vector3 moveVelocity;

		[SerializeField]
		private Vector3 aimVelocity;

		private float maxMoveSpeed;

		private float maxAimSpeed;

		private bool forceCutTransition;

		private TransitionBlendState moveBlendState;

		private TransitionBlendState aimBlendState;

		public Transform Target
		{
			get
			{
				return target;
			}
			set
			{
				if (target != value)
				{
					target = value;
					prevTargetPosition = target.position;
					prevTargetVelocity = Vector3.zero;
					dirty = true;
				}
			}
		}

		public CameraController debug_FirstController
		{
			get
			{
				return controllers[0];
			}
		}

		public int debug_ControllerCount
		{
			get
			{
				return controllers.Count;
			}
		}

		protected abstract void Move(Vector3 goal, bool snap = false);

		protected abstract void Aim(Quaternion aim, bool snap = false);

		protected abstract void Aim(ref Quaternion from, Quaternion aim);

		internal virtual void Awake()
		{
			SceneRefs.Set(Camera.main);
			SceneRefs.Set(this);
			Set(GetComponent<CameraController>());
			desiredGoal = base.transform.position;
			constrainedGoal = base.transform.position;
			lookat = base.transform.position + base.transform.forward;
			glance = Quaternion.identity;
			velocityFilterSamples = new List<Vector3>();
			maxMoveSpeed = -1f;
			maxAimSpeed = -1f;
			moveBlendState = default(TransitionBlendState);
			aimBlendState = default(TransitionBlendState);
			Snap();
		}

		protected void OnDestroy()
		{
			Moved = null;
		}

		public CameraController GetTopmostCameraController()
		{
			if (controllers.Count > 1)
			{
				return controllers[controllers.Count - 1];
			}
			return null;
		}

		internal void Set(CameraController controller, bool forceCutTransition = false)
		{
			if (controllers.Contains(controller))
			{
				return;
			}
			if (controllers.Count > 1)
			{
				for (int num = controllers.Count - 1; num > 0; num--)
				{
					if (controller.Priority >= controllers[num].Priority)
					{
						if (num == controllers.Count - 1)
						{
							controllers.Add(controller);
						}
						else
						{
							controllers.Insert(num + 1, controller);
						}
						break;
					}
					if (num == 1)
					{
						controllers.Insert(num, controller);
					}
				}
			}
			else
			{
				controllers.Add(controller);
			}
			this.forceCutTransition = forceCutTransition;
		}

		internal void Clear(CameraController controller)
		{
			if (controllers.Count > 1)
			{
				if (controllers.Contains(controller))
				{
					dirty = controllers.Remove(controller);
					controller.Deactivate();
				}
				if (!dirty)
				{
				}
			}
		}

		internal void Reset()
		{
			if (controllers.Count > 0)
			{
				controllers.RemoveRange(1, controllers.Count - 1);
				dirty = true;
			}
		}

		internal void SoftReset()
		{
			if (controllers.Count <= 1)
			{
				return;
			}
			for (int i = 1; i < controllers.Count; i++)
			{
				if (controllers[i].IsScripted)
				{
					controllers.RemoveAt(i);
					i--;
				}
			}
			dirty = true;
		}

		private CameraController getCurrentGoalPlannerController()
		{
			CameraController result = null;
			for (int num = controllers.Count - 1; num >= 0; num--)
			{
				if (controllers[num].GoalPlanners != null && controllers[num].GoalPlanners.Length > 0)
				{
					result = controllers[num];
					break;
				}
			}
			return result;
		}

		private CameraController getCurrentFramerController()
		{
			CameraController result = null;
			for (int num = controllers.Count - 1; num >= 0; num--)
			{
				if (controllers[num].Framer != null)
				{
					result = controllers[num];
					break;
				}
			}
			return result;
		}

		private void addGoalPlannerSetupInfluence(ref Setup setup, CameraController goalPlannerController, float influence = 1f)
		{
			Setup setup2 = new Setup(setup);
			if (goalPlannerController.GoalPlanners != null)
			{
				for (int i = 0; i < goalPlannerController.GoalPlanners.Length; i++)
				{
					goalPlannerController.GoalPlanners[i].enabled = true;
					goalPlannerController.GoalPlanners[i].Plan(ref setup2);
				}
			}
			setup2.UnconstrainedGoal = setup2.Goal;
			if (goalPlannerController.Constraints != null)
			{
				for (int i = 0; i < goalPlannerController.Constraints.Length; i++)
				{
					if (goalPlannerController.Constraints[i].Condition == null || goalPlannerController.Constraints[i].Condition.OnOff)
					{
						goalPlannerController.Constraints[i].enabled = true;
						goalPlannerController.Constraints[i].Apply(ref setup2);
					}
				}
			}
			setup.UnconstrainedGoal = Vector3.Lerp(setup.UnconstrainedGoal, setup2.UnconstrainedGoal, influence);
			setup.Goal = Vector3.Lerp(setup.Goal, setup2.Goal, influence);
			setup.IsAxisLocked = setup2.IsAxisLocked;
			setup.LockedAxis = setup2.LockedAxis;
			setup.LockedAxisValues = setup2.LockedAxisValues;
		}

		private void addFramerSetupInfluence(ref Setup setup, CameraController framerController, float influence = 1f)
		{
			Setup setup2 = new Setup(setup);
			if (framerController.Glancers.Length > 0)
			{
				for (int i = 0; i < framerController.Glancers.Length; i++)
				{
					framerController.Glancers[i].enabled = true;
					if (framerController.Glancers[i].Aim(ref setup2))
					{
						break;
					}
				}
			}
			if (framerController.Framer != null)
			{
				framerController.Framer.enabled = true;
				framerController.Framer.Aim(ref setup2);
			}
			setup.LookAt = Vector3.Lerp(setup.LookAt, setup2.LookAt, influence);
			setup.Glance = Quaternion.Lerp(setup.Glance, setup2.Glance, influence);
		}

		internal void GetNextSetup(ref Vector3 position, ref Quaternion rotation)
		{
			updateActiveCameraControllers();
			Setup setup = new Setup(Target, base.transform, Vector3.zero);
			addGoalPlannerSetupInfluence(ref setup, currentGoalPlannerController);
			addFramerSetupInfluence(ref setup, currentFramerController);
			position = setup.Goal;
			rotation = setup.Glance * Quaternion.LookRotation(setup.LookAt - position);
		}

		private Setup getSetup(Vector3 targetVelocity, CameraController goalPlannerController, CameraController framerController)
		{
			Setup setup = new Setup(Target, base.transform, targetVelocity);
			addGoalPlannerSetupInfluence(ref setup, goalPlannerController);
			addFramerSetupInfluence(ref setup, framerController);
			return setup;
		}

		private Vector3 GetFilteredVelocity(Vector3 currentVelocity)
		{
			Vector3 zero = Vector3.zero;
			velocityFilterSamples.Add(currentVelocity);
			while (velocityFilterSamples.Count > maxVelocityFilterSamples)
			{
				velocityFilterSamples.RemoveAt(0);
			}
			for (int i = 0; i < velocityFilterSamples.Count; i++)
			{
				zero += velocityFilterSamples[i];
			}
			return zero / velocityFilterSamples.Count;
		}

		private bool updateActiveCameraControllers()
		{
			bool result = false;
			CameraController cameraController = getCurrentGoalPlannerController();
			if (currentGoalPlannerController == null)
			{
				currentGoalPlannerController = cameraController;
				currentGoalPlannerController.Activate();
				result = true;
			}
			else if (cameraController != currentGoalPlannerController)
			{
				CameraControllerTransition transition = getTransition(currentGoalPlannerController, cameraController);
				snapMove |= forceCutTransition;
				currentGoalPlannerController.Deactivate();
				currentGoalPlannerController = cameraController;
				currentGoalPlannerController.Activate();
				moveBlendState.Abort();
				if (transition == null || snapMove)
				{
					maxMoveSpeed = -1f;
				}
				else if (transition.TransitionType == CameraControllerTransition.CameraTransitionType.CUT)
				{
					maxMoveSpeed = -1f;
					bool snapMove2 = snapMove;
					snapMove = true;
				}
				else if (transition.TransitionType == CameraControllerTransition.CameraTransitionType.MAX_SPEED)
				{
					if (transition.LimitMoveSpeed)
					{
						maxMoveSpeed = transition.MaxMoveSpeed;
					}
				}
				else if (transition.TransitionType == CameraControllerTransition.CameraTransitionType.CURVE)
				{
					if (transition.Duration <= 0f)
					{
						maxMoveSpeed = -1f;
						bool snapMove3 = snapMove;
						snapMove = true;
					}
					else
					{
						moveBlendState.FirstFramePos = base.transform.position;
						moveBlendState.Curve = transition.Curve;
						moveBlendState.Duration = transition.Duration;
						moveBlendState.ElapsedTime = 0f;
						moveBlendState.IsBlending = true;
						moveBlendState.DampenMovingTarget = transition.DampenMovingTarget;
						moveBlendState.MaxSpeed = (moveBlendState.DampenMovingTarget ? transition.MaxMoveSpeed : (-1f));
						maxMoveSpeed = ((moveBlendState.DampenMovingTarget && transition.MaxMoveSpeed > -1f) ? 0f : (-1f));
					}
				}
				result = true;
			}
			CameraController cameraController2 = getCurrentFramerController();
			if (currentFramerController == null)
			{
				currentFramerController = cameraController2;
				currentFramerController.Activate();
				result = true;
			}
			else if (cameraController2 != currentFramerController)
			{
				CameraControllerTransition transition = getTransition(currentFramerController, cameraController2);
				currentFramerController.Deactivate();
				currentFramerController = cameraController2;
				currentFramerController.Activate();
				aimBlendState.Abort();
				if (transition == null || snapAim)
				{
					maxAimSpeed = -1f;
				}
				else if (transition.TransitionType == CameraControllerTransition.CameraTransitionType.CUT)
				{
					maxAimSpeed = -1f;
					bool snapAim2 = snapAim;
					snapAim = true;
				}
				else if (transition.TransitionType == CameraControllerTransition.CameraTransitionType.MAX_SPEED)
				{
					if (transition.LimitAimSpeed)
					{
						maxAimSpeed = transition.MaxAimSpeed;
					}
				}
				else if (transition.TransitionType == CameraControllerTransition.CameraTransitionType.CURVE)
				{
					if (transition.Duration <= 0f)
					{
						maxAimSpeed = -1f;
						bool snapAim3 = snapAim;
						snapAim = true;
					}
					else
					{
						aimBlendState.FirstFrameRot = base.transform.rotation;
						aimBlendState.SmoothedAim = base.transform.rotation;
						aimBlendState.Curve = transition.Curve;
						aimBlendState.Duration = transition.Duration;
						aimBlendState.ElapsedTime = 0f;
						aimBlendState.IsBlending = true;
						aimBlendState.DampenMovingTarget = transition.DampenMovingTarget;
						aimBlendState.MaxSpeed = (aimBlendState.DampenMovingTarget ? transition.MaxAimSpeed : (-1f));
						aimBlendState.DoubleDampenMovingTargetAim = transition.DoubleDampenMovingTargetAim;
						maxAimSpeed = aimBlendState.MaxSpeed;
					}
				}
				result = true;
			}
			return result;
		}

		private CameraControllerTransition getTransition(CameraController fromController, CameraController toController)
		{
			CameraControllerTransition cameraControllerTransition = fromController.GetTransitionToOtherController(toController);
			if (cameraControllerTransition == null)
			{
				cameraControllerTransition = toController.DefaultControllerTransitionIn;
				CameraControllerTransition defaultControllerTransitionOut = fromController.DefaultControllerTransitionOut;
				if (cameraControllerTransition == null)
				{
					cameraControllerTransition = defaultControllerTransitionOut;
				}
				else if (defaultControllerTransitionOut != null && defaultControllerTransitionOut.DefaultTransitionOutPriority > cameraControllerTransition.DefaultTransitionInPriority)
				{
					cameraControllerTransition = defaultControllerTransitionOut;
				}
			}
			return cameraControllerTransition;
		}

		internal void LateUpdate()
		{
			if (Target == null)
			{
				return;
			}
			Vector3 filteredVelocity = GetFilteredVelocity((Target.position - prevTargetPosition) / Time.deltaTime);
			dirty |= updateActiveCameraControllers();
			dirty |= isGoalPlannerControllerDirty(currentGoalPlannerController);
			dirty |= isFramerControllerDirty(currentFramerController);
			dirty |= (Target.position != prevTargetPosition || filteredVelocity != prevTargetVelocity);
			dirty |= (snapAim | aimBlendState.IsBlending);
			dirty |= (snapMove | moveBlendState.IsBlending);
			if (dirty)
			{
				Setup setup = getSetup(filteredVelocity, currentGoalPlannerController, currentFramerController);
				if (maxMoveSpeed >= 0f)
				{
					constrainedGoal = Vector3.SmoothDamp(constrainedGoal, setup.Goal, ref moveVelocity, cameraDampTime, maxMoveSpeed);
				}
				else
				{
					constrainedGoal = setup.Goal;
				}
				if (maxAimSpeed >= 0f)
				{
					lookat = Vector3.SmoothDamp(lookat, setup.LookAt, ref aimVelocity, cameraDampTime, maxAimSpeed);
					glance = setup.Glance;
				}
				else
				{
					lookat = setup.LookAt;
					glance = setup.Glance;
				}
				desiredGoal = setup.UnconstrainedGoal;
				isRotationLocked = setup.IsAxisLocked;
				lockedAxis = setup.LockedAxis;
				lockedAxisValues = setup.LockedAxisValues;
				Vector3 vector = constrainedGoal - setup.Goal;
				Vector3 vector2 = lookat - setup.LookAt;
				bool flag = false;
				if (moveBlendState.IsBlending)
				{
					if (moveBlendState.ElapsedTime >= moveBlendState.Duration)
					{
						moveBlendState.IsBlending = false;
						maxMoveSpeed = -1f;
					}
					else
					{
						flag = true;
					}
				}
				else if (vector.sqrMagnitude > cameraArriveDistanceSq)
				{
					flag = true;
				}
				else
				{
					maxMoveSpeed = -1f;
				}
				if (aimBlendState.IsBlending)
				{
					if (aimBlendState.ElapsedTime >= aimBlendState.Duration)
					{
						aimBlendState.IsBlending = false;
						maxAimSpeed = -1f;
					}
					else
					{
						flag = true;
					}
				}
				else if (vector2.sqrMagnitude < cameraArriveDistance * cameraArriveDistance)
				{
					flag = true;
				}
				else
				{
					maxAimSpeed = -1f;
				}
				dirty = flag;
			}
			prevTargetPosition = Target.position;
			prevTargetVelocity = filteredVelocity;
			if (moveBlendState.IsBlending)
			{
				moveBlendState.ElapsedTime += Time.deltaTime;
				moveBlendState.ElapsedTime = Mathf.Clamp(moveBlendState.ElapsedTime, 0f, moveBlendState.Duration);
				float t = moveBlendState.Curve.Evaluate(moveBlendState.ElapsedTime / moveBlendState.Duration);
				base.transform.position = Vector3.Lerp(moveBlendState.FirstFramePos, constrainedGoal, t);
				if (moveBlendState.DampenMovingTarget && moveBlendState.MaxSpeed > -1f)
				{
					maxMoveSpeed = Mathf.Lerp(0f, moveBlendState.MaxSpeed, t);
				}
				else
				{
					maxMoveSpeed = -1f;
				}
				Move(base.transform.position, true);
			}
			else if (base.transform.position != constrainedGoal)
			{
				Move(constrainedGoal, snapMove);
			}
			float magnitude = (Target.position - base.transform.position).magnitude;
			float nearClipPlane = Camera.main.nearClipPlane;
			if (magnitude < nearClipPlane)
			{
				float t = nearClipPlane / magnitude;
				base.transform.position = Target.position + (base.transform.position - Target.position) * t;
			}
			Quaternion quaternion = glance * Quaternion.LookRotation(lookat - base.transform.position);
			if (aimBlendState.IsBlending)
			{
				aimBlendState.ElapsedTime += Time.deltaTime;
				aimBlendState.ElapsedTime = Mathf.Clamp(aimBlendState.ElapsedTime, 0f, aimBlendState.Duration);
				float t = aimBlendState.Curve.Evaluate(aimBlendState.ElapsedTime / aimBlendState.Duration);
				if (aimBlendState.DoubleDampenMovingTargetAim)
				{
					Aim(ref aimBlendState.SmoothedAim, quaternion);
					base.transform.rotation = Quaternion.Slerp(aimBlendState.FirstFrameRot, aimBlendState.SmoothedAim, t);
				}
				else
				{
					base.transform.rotation = Quaternion.Slerp(aimBlendState.FirstFrameRot, quaternion, t);
				}
			}
			else if (base.transform.rotation != quaternion)
			{
				Aim(quaternion, snapAim);
			}
			Vector3 forward = base.transform.forward;
			Vector3 rhs = Vector3.Cross(Vector3.up, forward);
			Vector3 upwards = Vector3.Cross(forward, rhs);
			base.transform.rotation = Quaternion.LookRotation(forward, upwards);
			if (isRotationLocked)
			{
				applyLockedRotation();
			}
			if (++snapFrameCount > 10)
			{
				snapMove = false;
				snapAim = false;
				snapLock = false;
			}
		}

		private void applyLockedRotation()
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			localEulerAngles.x = localEulerAngles.x * lockedAxis.x + lockedAxisValues.x;
			localEulerAngles.y = localEulerAngles.y * lockedAxis.y + lockedAxisValues.y;
			localEulerAngles.z = localEulerAngles.z * lockedAxis.z + lockedAxisValues.z;
			base.transform.localEulerAngles = localEulerAngles;
		}

		private bool isGoalPlannerControllerDirty(CameraController controller)
		{
			bool flag = false;
			if (currentGoalPlannerController.GoalPlanners != null)
			{
				for (int i = 0; i < currentGoalPlannerController.GoalPlanners.Length; i++)
				{
					flag |= currentGoalPlannerController.GoalPlanners[i].Dirty;
					currentGoalPlannerController.GoalPlanners[i].Dirty = false;
				}
			}
			flag |= currentGoalPlannerController.AreConstraintsDirty;
			currentGoalPlannerController.AreConstraintsDirty = false;
			return flag;
		}

		private bool isFramerControllerDirty(CameraController controller)
		{
			bool flag = false;
			if (currentFramerController.Framer != null)
			{
				flag |= currentFramerController.Framer.Dirty;
				currentFramerController.Framer.Dirty = false;
			}
			if (currentFramerController.Glancers.Length > 0)
			{
				for (int i = 0; i < currentFramerController.Glancers.Length; i++)
				{
					flag |= currentFramerController.Glancers[i].Dirty;
					currentFramerController.Glancers[i].Dirty = false;
				}
			}
			return flag;
		}

		public void Snap(bool position = true, bool rotation = true)
		{
			if (!snapLock)
			{
				snapMove = position;
				snapAim = rotation;
				snapFrameCount = 0;
			}
		}

		public void LockSnap(bool position, bool rotation)
		{
			if (!snapLock)
			{
				snapLock = true;
				snapMove = position;
				snapAim = rotation;
				snapFrameCount = 0;
			}
		}

		internal void OnDrawGizmos()
		{
			Gizmos.DrawIcon(lookat, "Cinematography/LookAt.png");
			Gizmos.DrawIcon(constrainedGoal, "Cinematography/ConstrainedGoal.png");
			Gizmos.DrawIcon(desiredGoal, "Cinematography/DesiredGoal.png");
		}

		internal void DrawGUI()
		{
			CameraController y = getCurrentGoalPlannerController();
			CameraController y2 = getCurrentFramerController();
			GUILayout.Label("Camera: " + base.name);
			GUILayout.BeginVertical();
			for (int i = 0; i < controllers.Count; i++)
			{
				string text = controllers[i].name;
				if (controllers[i] == y)
				{
					text += " (GP)";
				}
				if (controllers[i] == y2)
				{
					text += " (F)";
				}
				GUILayout.Box(text);
			}
			GUILayout.EndVertical();
		}
	}
}
