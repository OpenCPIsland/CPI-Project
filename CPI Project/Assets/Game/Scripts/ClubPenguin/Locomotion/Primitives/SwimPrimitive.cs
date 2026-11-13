using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.Locomotion.Primitives
{
	public class SwimPrimitive : LocomotionPrimitive
	{
		private SwimPrimitiveData mutableData;

		private static readonly int torpedoAnimTrigger = Animator.StringToHash("SwimTorpedo");

		private ActionSequencer sequencer;

		private Transform cameraTransform;

		private Animator anim;

		private EventDispatcher dispatcher;

		private Vector3 wsSteerDir;

		private Vector3 wsLastSteerDir;

		private Vector3 wsUpDir;

		private float speedMultAnimParam;

		private Vector3 prevPos;

		private bool isInTorpedoState;

		private Vector3 curFacing;

		private Vector3 desiredFacing;

		private bool snapToDesiredFacing;

		private float curSpin;

		private float targetSpin;

		private bool isInteracting;

		private ParticleSystem breathBubbles;

		private ParticleSystem swimBubbles;

		private ParticleSystem torpedoBubbles;

		private bool bubblesEnabled;

		private float stickMagnitude;

		private LocomotionEventBroadcaster broadcaster;

		public Vector3 Momentum
		{
			get;
			private set;
		}

		public float SurfaceHeight
		{
			get;
			set;
		}

		public bool IsInShallowWater
		{
			get;
			set;
		}

		protected LocomotionEventBroadcaster Broadcaster
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

		private void Awake()
		{
			cameraTransform = Camera.main.transform;
			anim = GetComponent<Animator>();
			base.enabled = false;
		}

		public void SetData(SwimPrimitiveData data)
		{
			mutableData = UnityEngine.Object.Instantiate(data);
			mutableData.MinSpeedMult = Mathf.Max(0.01f, mutableData.MinSpeedMult);
			if (mutableData.BreathBubbles != null)
			{
				breathBubbles = UnityEngine.Object.Instantiate(mutableData.BreathBubbles);
				breathBubbles.transform.SetParent(base.transform, false);
				breathBubbles.Stop();
			}
			if (mutableData.SwimBubbles != null)
			{
				swimBubbles = UnityEngine.Object.Instantiate(mutableData.SwimBubbles);
				swimBubbles.transform.SetParent(base.transform, false);
				swimBubbles.Stop();
			}
			if (mutableData.TorpedoBubbles != null)
			{
				torpedoBubbles = UnityEngine.Object.Instantiate(mutableData.TorpedoBubbles);
				torpedoBubbles.transform.SetParent(base.transform, false);
				torpedoBubbles.Stop();
			}
		}

		private void OnEnable()
		{
			ResetState();
		}

		public override void ResetState()
		{
			prevPos = base.transform.position;
			curSpin = 0f;
			targetSpin = 0f;
			isInTorpedoState = false;
			Momentum = Vector3.zero;
			wsSteerDir = Vector3.zero;
			wsUpDir = Vector3.zero;
			wsLastSteerDir = Vector3.zero;
			speedMultAnimParam = mutableData.MinSpeedMult;
			IsInShallowWater = false;
			desiredFacing = base.transform.forward;
			snapToDesiredFacing = false;
			base.ResetState();
		}

		private void OnDisable()
		{
			if (breathBubbles != null)
			{
				breathBubbles.Stop();
			}
			if (swimBubbles != null)
			{
				swimBubbles.Stop();
			}
			if (torpedoBubbles != null)
			{
				torpedoBubbles.Stop();
			}
			bubblesEnabled = false;
		}

		private void OnDestroy()
		{
			if (breathBubbles != null)
			{
				UnityEngine.Object.Destroy(breathBubbles.gameObject);
			}
			if (swimBubbles != null)
			{
				UnityEngine.Object.Destroy(swimBubbles.gameObject);
			}
			if (torpedoBubbles != null)
			{
				UnityEngine.Object.Destroy(torpedoBubbles.gameObject);
			}
		}

		public void Torpedo()
		{
			anim.SetTrigger(torpedoAnimTrigger);
		}

		public override void Steer(Vector2 steerInput)
		{
			stickMagnitude = steerInput.magnitude;
			Vector3 wsForward;
			Vector3 wsUp;
			LocomotionUtils.StickInputToWorldSpaceTransform(steerInput, out wsForward, out wsUp, (!IsInShallowWater) ? LocomotionUtils.AxisIndex.Y : LocomotionUtils.AxisIndex.Z);
			applyWorldSpaceSteering(ref wsForward, ref wsUp);
		}

		public override void Steer(Vector3 wsSteerInput)
		{
			Vector3 _wsUpDir = LocomotionUtils.GetUpVector(ref wsSteerInput, (!IsInShallowWater) ? LocomotionUtils.AxisIndex.Y : LocomotionUtils.AxisIndex.Z);
			applyWorldSpaceSteering(ref wsSteerInput, ref _wsUpDir);
		}

		private void applyWorldSpaceSteering(ref Vector3 _wsSteerDir, ref Vector3 _wsUpDir)
		{
			Vector3 v = wsSteerDir;
			wsSteerDir = _wsSteerDir;
			wsUpDir = _wsUpDir;
			targetSpin = 0f;
			if (wsSteerDir != Vector3.zero)
			{
				wsLastSteerDir = wsSteerDir;
				if (isInTorpedoState)
				{
					float num = LocomotionUtils.SignedAngle(v, wsSteerDir);
					if (num > 0f)
					{
						targetSpin = 180f;
					}
				}
			}
			Broadcaster.BroadcastOnSteerDirectionEvent(wsSteerDir);
		}

		public override void SteerRotation(Vector2 steerInput)
		{
			Vector3 _wsSteerRotation = LocomotionUtils.StickInputToWorldSpaceTransform(steerInput, (!IsInShallowWater) ? LocomotionUtils.AxisIndex.Y : LocomotionUtils.AxisIndex.Z);
			if (snapToDesiredFacing || (int)Vector3.Angle(desiredFacing, _wsSteerRotation) > mutableData.RotationDegreesOffsetThreshold)
			{
				applyWorldSpaceSteerRotation(ref _wsSteerRotation);
			}
		}

		public override void SteerRotation(Vector3 wsSteerInput)
		{
			applyWorldSpaceSteerRotation(ref wsSteerInput);
		}

		private void applyWorldSpaceSteerRotation(ref Vector3 _wsSteerRotation)
		{
			if (desiredFacing != _wsSteerRotation)
			{
				desiredFacing = _wsSteerRotation;
				snapToDesiredFacing = true;
				Broadcaster.BroadcastOnSteerRotationDirectionEvent(desiredFacing);
			}
		}

		private void lerpSpin()
		{
			curSpin = Mathf.LerpAngle(curSpin, targetSpin, mutableData.SpinSmoothing * Time.deltaTime);
		}

		private bool isDeepEnoughForBubbles()
		{
			if (IsInShallowWater)
			{
				return false;
			}
			float num = SurfaceHeight - base.transform.position.y;
			return num >= mutableData.MinDistFromSurfaceForBubbles;
		}

		private void updateFX()
		{
			if (!bubblesEnabled && isDeepEnoughForBubbles())
			{
				if (breathBubbles != null)
				{
					breathBubbles.Play();
				}
				if (swimBubbles != null)
				{
					swimBubbles.Play();
				}
				bubblesEnabled = true;
			}
			else if (bubblesEnabled && !isDeepEnoughForBubbles())
			{
				if (breathBubbles != null)
				{
					breathBubbles.Stop();
				}
				if (swimBubbles != null)
				{
					swimBubbles.Stop();
				}
				bubblesEnabled = false;
			}
		}

		private void Update()
		{
			AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);
			curFacing = base.transform.forward;
			curFacing.y = 0f;
			curFacing.Normalize();
			updateFX();
			bool flag = LocomotionUtils.IsIdling(animatorStateInfo);
			if (isInTorpedoState)
			{
				lerpSpin();
				if (!LocomotionUtils.IsTurboing(animatorStateInfo))
				{
					isInTorpedoState = false;
					wsLastSteerDir = wsSteerDir;
					torpedoBubbles.Stop();
				}
			}
			else if (LocomotionUtils.IsTurboing(animatorStateInfo))
			{
				isInTorpedoState = true;
				curSpin = 0f;
				if (!IsInShallowWater)
				{
					torpedoBubbles.Play();
				}
			}
			if (wsSteerDir != Vector3.zero)
			{
				speedMultAnimParam += mutableData.Accel * Time.deltaTime;
			}
			else if (flag)
			{
				speedMultAnimParam = mutableData.MinSpeedMult;
			}
			if (flag)
			{
				Momentum = Vector3.Lerp(Momentum, Vector3.zero, mutableData.DragSmoothing * Time.deltaTime);
				if ((double)Momentum.magnitude < 0.25)
				{
					Momentum = Vector3.zero;
				}
			}
			else
			{
				Momentum = (base.transform.position - prevPos) / Time.deltaTime;
				desiredFacing = wsSteerDir;
			}
			speedMultAnimParam = Mathf.Clamp(speedMultAnimParam, mutableData.MinSpeedMult, 1f);
			anim.SetFloat(AnimationHashes.Params.SwimSpeedMult, speedMultAnimParam);
			anim.SetFloat(AnimationHashes.Params.SwimSpeed, wsSteerDir.magnitude);
			prevPos = base.transform.position;
		}

		private void OnAnimatorMove()
		{
			AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);
			if (wsSteerDir != Vector3.zero)
			{
				Quaternion identity = Quaternion.identity;
				if (LocomotionUtils.IsTurboing(animatorStateInfo))
				{
					identity = Quaternion.LookRotation(wsLastSteerDir, wsUpDir);
					identity = Quaternion.AngleAxis(curSpin, wsLastSteerDir) * identity;
				}
				else
				{
					identity = Quaternion.LookRotation(wsSteerDir);
				}
				Output.wsRotation = Quaternion.Slerp(base.transform.rotation, identity, mutableData.RotationSmoothing * Time.deltaTime);
			}
			else if (!IsInShallowWater)
			{
				Quaternion identity = Quaternion.LookRotation(-cameraTransform.forward, cameraTransform.up);
				Output.wsRotation = Quaternion.Slerp(base.transform.rotation, identity, mutableData.RotationSmoothing * Time.deltaTime);
			}
			else if (snapToDesiredFacing)
			{
				if (Vector3.Angle(curFacing, desiredFacing) < 1f)
				{
					snapToDesiredFacing = false;
				}
				TurnToDesiredFacing(mutableData.RotationSmoothing);
			}
			else
			{
				Output.wsRotation = base.transform.rotation;
			}
			if (LocomotionUtils.IsIdling(animatorStateInfo))
			{
				Output.wsDeltaPos = Momentum * Time.deltaTime;
			}
			else if (LocomotionUtils.IsTurboing(animatorStateInfo))
			{
				Output.wsDeltaPos = anim.deltaPosition;
			}
			else
			{
				Output.wsDeltaPos = anim.deltaPosition * stickMagnitude;
			}
		}

		private void TurnToDesiredFacing(float smoothing, float angleCap = float.PositiveInfinity)
		{
			if (desiredFacing != Vector3.zero)
			{
				if (Math.Abs(smoothing) < float.Epsilon)
				{
					Output.wsRotation = Quaternion.LookRotation(desiredFacing);
					return;
				}
				Quaternion b = default(Quaternion);
				b.SetLookRotation(desiredFacing);
				Quaternion to = Quaternion.Slerp(base.transform.rotation, b, smoothing * Time.deltaTime);
				Output.wsRotation = Quaternion.RotateTowards(base.transform.rotation, to, angleCap * Time.deltaTime);
			}
		}
	}
}
