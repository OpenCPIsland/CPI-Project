using ClubPenguin.Core;
using ClubPenguin.Interactables;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(CharacterController))]
	public class RunController : LocomotionController
	{
		public delegate void SteerDelegate(Vector3 steer);

		public enum LocoMode
		{
			Idle,
			Walk,
			Jog,
			Sprint
		}

		public struct ControllerBehaviour
		{
			public bool IgnoreTranslation;

			public bool IgnoreRotation;

			public bool IgnoreCollisions;

			public bool IgnoreGravity;

			public bool IgnoreJumpRequests;

			public bool IgnoreStickInput;

			public PlayerLocoStyle.Style Style;

			public Component LastModifier;

			public ControllerBehaviour(ControllerBehaviour newBehaviour)
			{
				this = newBehaviour;
			}

			public void Reset()
			{
				IgnoreTranslation = false;
				IgnoreRotation = false;
				IgnoreCollisions = false;
				IgnoreGravity = false;
				IgnoreJumpRequests = false;
				IgnoreStickInput = false;
				Style = PlayerLocoStyle.Style.Run;
				LastModifier = null;
			}

			public void SetStyle(PlayerLocoStyle.Style style)
			{
				Style = style;
			}
		}

		private const float MAX_PHYSICS_COLLISION_CHECK_THRESHOLD = 1f;

		private InputButtonGroupContentKey runButtonsDefinitionContentKey = new InputButtonGroupContentKey("Definitions/ControlsScreen/Locomotion/LocomotionGroupDefinition");

		public RunControllerData MasterData;

		private RunControllerData mutableData;

		private EventDispatcher dispatcher;

		private LocoMode _curLocoMode;

		private float elapsedMoveTime;

		private Vector3 wsSteerDir;

		private Vector3 curFacing;

		private Vector3 desiredFacing;

		private bool snapToDesiredFacing;

		private Vector3 impulseVel;

		private Vector3 inAirVel = Vector3.zero;

		private Vector3 prevVel = Vector3.zero;

		private Vector3 prevPosition = Vector3.zero;

		private Vector3 gravityVector = Vector3.zero;

		private float timeSinceLastPhysicsCheck;

		private bool isInAir = false;

		private bool isWalking = false;

		private bool isLanding = false;

		private bool isJogging = false;

		private bool isSprinting = false;

		private bool isStopping = false;

		private bool isPivoting = false;

		private int raycastLayerMask;

		private ActionRequest jumpRequest;

		private int prevAnimState;

		private AnimatorStateInfo prevStateInfo;

		private AnimatorStateInfo curStateInfo;

		private LocomotionTracker tracker;

		private LocomotionController prevLocoController;

		[HideInInspector]
		public ControllerBehaviour Behaviour;

		private LocoMode curLocoMode
		{
			get
			{
				return _curLocoMode;
			}
			set
			{
				if (_curLocoMode != value)
				{
					switch (value)
					{
					case LocoMode.Walk:
						mutableData.WalkParams.speedMult = mutableData.WalkParams.IntoSpeedMult;
						break;
					case LocoMode.Jog:
						mutableData.JogParams.speedMult = mutableData.JogParams.IntoSpeedMult;
						break;
					case LocoMode.Sprint:
						mutableData.SprintParams.speedMult = mutableData.SprintParams.IntoSpeedMult;
						break;
					}
				}
				_curLocoMode = value;
			}
		}

		public RuntimeAnimatorController DefaultAnimatorController
		{
			get;
			private set;
		}

		public event SteerDelegate OnSteer;

		protected override void awake()
		{
			mutableData = Object.Instantiate(MasterData);
			tracker = GetComponent<LocomotionTracker>();
			raycastLayerMask = LayerConstants.GetPlayerLayerCollisionMask();
			DefaultAnimatorController = animator.runtimeAnimatorController;
			dispatcher = Service.Get<EventDispatcher>();
			jumpRequest = new ActionRequest(PenguinUserControl.DefaultActionRequestBufferTime, animator, AnimationHashes.Params.Jump);
		}

		private void OnEnable()
		{
			prevLocoController = tracker.GetCurrentController();
			ResetState();
			if (CompareTag("Player"))
			{
				LoadContextualControlsLayout();
			}
		}

		private void LoadContextualControlsLayout()
		{
			PropUser localPlayerPropUser = Service.Get<PropService>().LocalPlayerPropUser;
			if (localPlayerPropUser != null && localPlayerPropUser.Prop != null)
			{
				dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(localPlayerPropUser.Prop.PropControls.DefaultControls));
			}
			else if (GetComponentInChildren<InvitationalItemExperience>() != null)
			{
				InvitationalItemExperience componentInChildren = GetComponentInChildren<InvitationalItemExperience>();
				dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(componentInChildren.ControlLayout));
			}
			else if (!(prevLocoController is SlideController))
			{
				LoadControlsLayout();
			}
		}

		public override void LoadControlsLayout()
		{
			dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(runButtonsDefinitionContentKey));
		}

		public override void ResetState()
		{
			Behaviour.Reset();
			ResetMomentum();
			inAirVel = Vector3.zero;
			impulseVel = Vector3.zero;
			curLocoMode = LocoMode.Idle;
			desiredFacing = base.transform.forward;
			curLocoMode = LocoMode.Idle;
			wsSteerDir = Vector3.zero;
			jumpRequest.Reset();
			snapToDesiredFacing = false;
			curStateInfo = (animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0));
			prevStateInfo = curStateInfo;
			prevAnimState = AnimationHashes.States.Idle;
			mutableData.WalkParams.ElapsedWalkTime = 0f;
			mutableData.JogParams.ElapsedJogTime = 0f;
		}

		public override void Steer(Vector2 steerInput)
		{
			if (!Behaviour.IgnoreStickInput)
			{
				float magnitude = steerInput.magnitude;
				wsSteerDir = LocomotionUtils.StickInputToWorldSpaceTransform(steerInput, LocomotionUtils.AxisIndex.Z);
				wsSteerDir = Vector3.ClampMagnitude(wsSteerDir, magnitude);
				base.Broadcaster.BroadcastOnStickDirectionEvent(steerInput);
				base.Broadcaster.BroadcastOnSteerDirectionEvent(wsSteerDir);
			}
		}

		public override void Steer(Vector3 wsSteerInput)
		{
			wsSteerDir = wsSteerInput;
			base.Broadcaster.BroadcastOnSteerDirectionEvent(wsSteerDir);
			if (this.OnSteer != null)
			{
				this.OnSteer(wsSteerInput);
			}
		}

		public override void SteerRotation(Vector2 steerInput)
		{
			if (!Behaviour.IgnoreStickInput)
			{
				Vector3 wsSteerInput = LocomotionUtils.StickInputToWorldSpaceTransform(steerInput, LocomotionUtils.AxisIndex.Z);
				if (snapToDesiredFacing || (int)Vector3.Angle(desiredFacing, wsSteerInput) > mutableData.IdleParams.RotationDegreesOffsetThreshold)
				{
					applySteerRotation(ref wsSteerInput);
				}
			}
		}

		public override void SteerRotation(Vector3 wsSteerInput)
		{
			applySteerRotation(ref wsSteerInput);
		}

		private void applySteerRotation(ref Vector3 wsSteerInput)
		{
			if (desiredFacing != wsSteerInput)
			{
				desiredFacing = wsSteerInput;
				snapToDesiredFacing = true;
				base.Broadcaster.BroadcastOnSteerRotationDirectionEvent(desiredFacing);
			}
		}

		public override void DoAction(LocomotionAction action, object userData = null)
		{
			switch (action)
			{
			case LocomotionAction.Torpedo:
			case LocomotionAction.SlideTrick:
				break;
			case LocomotionAction.ChargeThrow:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.LaunchThrow:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.Interact:
			case LocomotionAction.Action1:
			case LocomotionAction.Action2:
			case LocomotionAction.Action3:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.Jump:
				if (!Behaviour.IgnoreJumpRequests && !jumpRequest.Active)
				{
					jumpRequest.Set();
					base.Broadcaster.BroadcastOnDoAction(action, userData);
				}
				break;
			}
		}

		public override void SetState(LocomotionState state)
		{
			base.Broadcaster.BroadcastSetLocomotionState(state);
		}

		public void ClearAllVelocityInputs()
		{
			desiredFacing = curFacing;
			curLocoMode = LocoMode.Idle;
			impulseVel = Vector3.zero;
			wsSteerDir = Vector3.zero;
			inAirVel = Vector3.zero;
		}

		private void Update()
		{
			curStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
			isInAir = LocomotionUtils.IsInAir(curStateInfo);
			isWalking = LocomotionUtils.IsWalking(curStateInfo);
			isLanding = LocomotionUtils.IsLanding(curStateInfo);
			isJogging = LocomotionUtils.IsJogging(curStateInfo);
			isSprinting = LocomotionUtils.IsSprinting(curStateInfo);
			isStopping = LocomotionUtils.IsStopping(curStateInfo);
			isPivoting = LocomotionUtils.IsPivoting(curStateInfo);
			curFacing = base.transform.forward;
			curFacing.y = 0f;
			curFacing.Normalize();
			animator.SetFloat(AnimationHashes.Params.GroundFriction, mutableData.GroundFriction);
			gravityVector.y = (0f - mutableData.Gravity) * Time.deltaTime;
			updateActionRequests();
			if (wsSteerDir != Vector3.zero)
			{
				if (Behaviour.Style == PlayerLocoStyle.Style.Walk)
				{
					curLocoMode = LocoMode.Walk;
				}
				else if (mutableData.JogParams.ElapsedJogTime >= mutableData.SprintParams.MinTimeToStartSprinting)
				{
					curLocoMode = LocoMode.Sprint;
				}
				else if (isWalking || (isLanding && curLocoMode == LocoMode.Walk))
				{
					float num = mutableData.JogParams.MinSteerMag * mutableData.JogParams.MinSteerMag;
					if (wsSteerDir.sqrMagnitude >= num && mutableData.WalkParams.ElapsedWalkTime >= mutableData.JogParams.MinTimeToStartJogging)
					{
						curLocoMode = LocoMode.Jog;
					}
					else
					{
						curLocoMode = LocoMode.Walk;
					}
				}
				else if (isJogging || (isLanding && curLocoMode == LocoMode.Jog))
				{
					float num = mutableData.JogParams.MinSteerMag - 0.1f;
					num *= num;
					if (wsSteerDir.sqrMagnitude < num)
					{
						curLocoMode = LocoMode.Walk;
					}
					else
					{
						curLocoMode = LocoMode.Jog;
					}
				}
				else
				{
					curLocoMode = LocoMode.Walk;
				}
				desiredFacing = wsSteerDir;
				elapsedMoveTime += Time.deltaTime;
			}
			else
			{
				curLocoMode = LocoMode.Idle;
				if (elapsedMoveTime > 0f)
				{
					snapToDesiredFacing = true;
				}
				else if (!snapToDesiredFacing)
				{
					desiredFacing = curFacing;
				}
				elapsedMoveTime = 0f;
			}
			Debug.DrawRay(base.transform.position, curFacing, Color.blue, 0f, false);
			Debug.DrawRay(base.transform.position, desiredFacing, Color.red, 0f, false);
			float normalizedTime = curStateInfo.normalizedTime;
			animator.SetInteger(AnimationHashes.Params.LoopCount, Mathf.FloorToInt(normalizedTime));
			animator.SetFloat(AnimationHashes.Params.NormTime, normalizedTime - Mathf.Floor(normalizedTime));
			animator.SetInteger(AnimationHashes.Params.LocoMode, (int)curLocoMode);
			animator.SetBool(AnimationHashes.Params.Shuffle, snapToDesiredFacing);
			int fullPathHash = curStateInfo.fullPathHash;
			if (prevAnimState != fullPathHash)
			{
				onStateTransitioned();
			}
			if (isInAir)
			{
				updateInAirState();
			}
			else
			{
				updateOnGroundState();
			}
			mutableData.JogParams.speedMult = Mathf.Clamp(mutableData.JogParams.speedMult, 0f, 1f);
			animator.SetFloat(AnimationHashes.Params.JogSpeedMult, mutableData.JogParams.speedMult);
			mutableData.WalkParams.speedMult = Mathf.Clamp(mutableData.WalkParams.speedMult, 0f, 1f);
			animator.SetFloat(AnimationHashes.Params.WalkSpeedMult, mutableData.WalkParams.speedMult);
			mutableData.SprintParams.speedMult = Mathf.Clamp(mutableData.SprintParams.speedMult, 0f, 1f);
			animator.SetFloat(AnimationHashes.Params.SprintSpeedMult, mutableData.SprintParams.speedMult);
			prevAnimState = fullPathHash;
			prevStateInfo = curStateInfo;
		}

		private void onStateTransitioned()
		{
			if (isPivoting)
			{
				animator.SetFloat(AnimationHashes.Params.PivotAngle, 0f);
				mutableData.SprintParams.ElapsedLeanTime = 0f;
			}
			if (isLanding)
			{
				impulseVel = Vector3.zero;
				inAirVel = Vector3.zero;
				animator.ResetTrigger(AnimationHashes.Params.Freefall);
				base.Broadcaster.BroadcastOnLandedJump();
			}
			if (curLocoMode != 0 && isLanding)
			{
				mutableData.WalkParams.speedMult = 1f;
				mutableData.JogParams.speedMult = 1f;
				mutableData.SprintParams.speedMult = 1f;
			}
			else if (!isJogging && !isSprinting && !isInAir)
			{
				mutableData.WalkParams.ElapsedWalkTime = 0f;
				mutableData.JogParams.ElapsedJogTime = 0f;
				mutableData.SprintParams.ElapsedLeanTime = 0f;
			}
		}

		private void updateInAirState()
		{
			float num = float.PositiveInfinity;
			float value = float.PositiveInfinity;
			if (impulseVel.y < 0f)
			{
				RaycastHit hitInfo;
				if (characterController.isGrounded)
				{
					num = 0f;
					value = 0f;
				}
				else if (Physics.Raycast(base.transform.position, Vector3.down, out hitInfo, mutableData.InAirParams.LandingGroundCheckDistance, raycastLayerMask))
				{
					num = hitInfo.distance;
					value = num / (0f - characterController.velocity.y);
					value = ((!(num <= mutableData.InAirParams.LandingTriggerDistance)) ? float.PositiveInfinity : 0f);
				}
			}
			else if (characterController.isGrounded)
			{
				num = 0f;
				value = 0f;
			}
			updateVelocityInAir();
			animator.SetFloat(AnimationHashes.Params.LandingDistance, num);
			animator.SetFloat(AnimationHashes.Params.LandingTime, value);
			if (!Behaviour.IgnoreGravity)
			{
				impulseVel.y -= mutableData.Gravity * Time.deltaTime;
			}
		}

		private void updateVelocityInAir()
		{
			if (curLocoMode != 0)
			{
				inAirVel += desiredFacing * mutableData.InAirParams.Acceleration * Time.deltaTime;
			}
		}

		private void updateOnGroundState()
		{
			if (!isLanding && !isInAir && characterController.velocity.y < mutableData.InAirParams.MinVelocityYToStartFreefall && (characterController.collisionFlags & CollisionFlags.Below) == 0 && !Physics.Raycast(base.transform.position + Vector3.up * 0.2f, Vector3.down, mutableData.InAirParams.StartFreefallGroundCheckDistance + 0.2f, raycastLayerMask))
			{
				animator.SetTrigger(AnimationHashes.Params.Freefall);
			}
			if (characterController.isGrounded)
			{
				if (!Behaviour.IgnoreGravity)
				{
					impulseVel.y = (0f - mutableData.Gravity) * Time.deltaTime;
				}
			}
			else if (!Behaviour.IgnoreGravity)
			{
				impulseVel.y -= mutableData.Gravity * Time.deltaTime;
			}
			if (isSprinting)
			{
				if (mutableData.SprintParams.LeanDuration > 0f)
				{
					float num = mutableData.SprintParams.ElapsedLeanTime / mutableData.SprintParams.LeanDuration;
					if (num < 1f)
					{
						float value = Mathf.Clamp01(mutableData.SprintParams.LeanCurve.Evaluate(num));
						animator.SetFloat(AnimationHashes.Params.SprintLean, value);
					}
					else
					{
						animator.SetFloat(AnimationHashes.Params.SprintLean, 0f);
					}
					mutableData.SprintParams.ElapsedLeanTime += Time.deltaTime;
				}
				mutableData.SprintParams.speedMult += mutableData.SprintParams.Acceleration * Time.deltaTime;
			}
			else if (isWalking)
			{
				mutableData.WalkParams.speedMult += mutableData.WalkParams.Acceleration * Time.deltaTime;
				mutableData.WalkParams.ElapsedWalkTime += Time.deltaTime;
			}
			else if (isJogging)
			{
				mutableData.JogParams.speedMult += mutableData.JogParams.Acceleration * Time.deltaTime;
				mutableData.JogParams.ElapsedJogTime += Time.deltaTime;
			}
			if (isSprinting)
			{
				float num2 = Vector3.Angle(curFacing, desiredFacing);
				if (num2 >= mutableData.MinFacingAngleToPivot)
				{
					animator.SetFloat(AnimationHashes.Params.PivotAngle, Mathf.Sign(num2));
				}
			}
		}

		private void updateActionRequests()
		{
			jumpRequest.Update();
		}

		public void SnapToPosition(Vector3 pos)
		{
			if (Behaviour.IgnoreCollisions)
			{
				base.transform.position = pos;
			}
			else
			{
				characterController.Move(pos - base.transform.position);
			}
			prevVel = Vector3.zero;
		}

		public void SnapToFacing(Vector3 facing)
		{
			desiredFacing = facing;
			base.transform.rotation = Quaternion.LookRotation(desiredFacing);
		}

		public void SnapToFacing(Vector3 facing, Vector3 up)
		{
			desiredFacing = facing;
			base.transform.rotation = Quaternion.LookRotation(desiredFacing, up);
		}

		private void LateUpdate()
		{
			AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
			LocoMode integer = (LocoMode)animator.GetInteger(AnimationHashes.Params.LocoMode);
			Vector3 vector = animator.deltaPosition / Time.deltaTime;
			Vector3 vector2 = animator.GetBool(AnimationHashes.Params.Scripted) ? vector : Vector3.zero;
			vector.y = 0f;
			if (isWalking || (isLanding && integer == LocoMode.Walk))
			{
				if (Vector3.Angle(desiredFacing, curFacing) >= mutableData.MinFacingAngleToResetMomentum * Time.deltaTime)
				{
					mutableData.WalkParams.speedMult = mutableData.WalkParams.TurnSpeedMult;
					mutableData.WalkParams.ElapsedWalkTime = 0f;
				}
				vector2 = vector * mutableData.WalkParams.Speed;
				TurnToDesiredFacing(mutableData.WalkParams.TurnSmoothing, mutableData.WalkParams.MaxTurnDegreesPerSec);
			}
			else if (isJogging || (isLanding && integer == LocoMode.Jog))
			{
				if (Vector3.Angle(desiredFacing, curFacing) >= mutableData.MinFacingAngleToResetMomentum * Time.deltaTime)
				{
					mutableData.JogParams.speedMult = mutableData.JogParams.TurnSpeedMult;
					mutableData.JogParams.ElapsedJogTime = 0f;
				}
				vector2 = desiredFacing * mutableData.JogParams.Speed * mutableData.JogParams.speedMult;
				TurnToDesiredFacing(mutableData.JogParams.TurnSmoothing, mutableData.JogParams.MaxTurnDegreesPerSec);
			}
			else if (isSprinting || (isLanding && integer == LocoMode.Sprint))
			{
				if (Vector3.Angle(desiredFacing, curFacing) >= mutableData.MinFacingAngleToResetMomentum * Time.deltaTime)
				{
					mutableData.SprintParams.speedMult = mutableData.SprintParams.TurnSpeedMult;
				}
				vector2 = desiredFacing * mutableData.SprintParams.Speed * mutableData.SprintParams.speedMult;
				TurnToDesiredFacing(mutableData.SprintParams.TurnSmoothing, mutableData.SprintParams.MaxTurnDegreesPerSec);
			}
			else if (isStopping)
			{
				vector2 = vector * mutableData.StopAnimVelMultiplier;
			}
			else if (isPivoting)
			{
				float normalizedTime = animatorStateInfo.normalizedTime;
				vector2 = Vector3.Lerp(prevVel, Vector3.zero, normalizedTime * normalizedTime * normalizedTime);
				if (animatorStateInfo.normalizedTime > 0.2f)
				{
					float num = normalizedTime * normalizedTime * normalizedTime;
					TurnToDesiredFacing(num / Time.deltaTime, mutableData.SprintParams.MaxTurnDegreesPerSec);
				}
			}
			else if (isInAir)
			{
				vector2 = inAirVel;
				TurnToDesiredFacing(mutableData.InAirParams.TurnSmoothing, mutableData.InAirParams.MaxTurnDegreesPerSec);
			}
			if (snapToDesiredFacing)
			{
				if (Vector3.Angle(curFacing, desiredFacing) < 1f)
				{
					snapToDesiredFacing = false;
				}
				TurnToDesiredFacing(mutableData.IdleParams.TurnSmoothing, mutableData.IdleParams.MaxTurnDegreesPerSec);
			}
			if (!Behaviour.IgnoreTranslation)
			{
				prevVel = vector2;
				prevVel.y = 0f;
				vector2 += impulseVel;
				if (Behaviour.IgnoreCollisions)
				{
					base.transform.position += vector2 * Time.deltaTime;
					return;
				}
				if (vector2 == gravityVector && characterController.isGrounded && prevPosition == base.transform.position)
				{
					timeSinceLastPhysicsCheck += Time.deltaTime;
					if (timeSinceLastPhysicsCheck < 1f)
					{
						return;
					}
				}
				timeSinceLastPhysicsCheck = 0f;
				prevPosition = base.transform.position;
				characterController.Move(vector2 * Time.deltaTime);
			}
			else
			{
				prevVel = Vector3.zero;
			}
		}

		private void TurnToDesiredFacing(float smoothing, float angleCap = float.PositiveInfinity)
		{
			if (!Behaviour.IgnoreRotation && desiredFacing != Vector3.zero)
			{
				if (smoothing == 0f)
				{
					base.transform.rotation = Quaternion.LookRotation(desiredFacing);
					return;
				}
				Quaternion b = default(Quaternion);
				b.SetLookRotation(desiredFacing);
				Quaternion to = Quaternion.Slerp(base.transform.rotation, b, smoothing * Time.deltaTime);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, angleCap * Time.deltaTime);
			}
		}

		public void ApplyImpulse(Transform startTransform, Vector3 instantVelocity)
		{
			base.transform.position = startTransform.position;
			base.transform.rotation = startTransform.rotation;
			SetForce(instantVelocity);
		}

		public override void AddForce(Vector3 wsForce, GameObject pusher = null)
		{
			impulseVel += wsForce;
		}

		public override void SetForce(Vector3 wsForce, GameObject pusher = null)
		{
			impulseVel = wsForce;
		}

		public void ResetMomentum()
		{
			impulseVel = Vector3.zero;
		}

		public void AnimEvent_JumpStarted()
		{
			Vector3 wsForce = new Vector3(0f, mutableData.JumpSpeed, 0f);
			SetForce(wsForce);
			inAirVel = prevVel;
			inAirVel.y = 0f;
			jumpRequest.Reset();
		}
	}
}
