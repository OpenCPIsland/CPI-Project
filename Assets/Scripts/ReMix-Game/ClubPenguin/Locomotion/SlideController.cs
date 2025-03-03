using ClubPenguin.Actions;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion.Primitives;
using ClubPenguin.Tubes;
using Fabric;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(MotionTracker))]
	public class SlideController : LocomotionController, ISlideControllerAdapter
	{
		public enum Mode
		{
			Animated,
			PhysicsDriven
		}

		[Tooltip("Chest bone used for posture adjustment.")]
		public Transform ChestBone;

		[Tooltip("Dictates how often a remote player snap can occur.")]
		public float snapTimeThreshold = 4f;

		private static readonly float hackFixedUpdateChangeMultipler = 3.33333325f;

		private static readonly Vector3 VECTOR3_ZERO = Vector3.zero;

		protected static int waterLayer;

		protected static int raycastLayerMask;

		public SlideControllerData MasterData;

		protected ForceAccumulatorPrimitive impulses;

		protected MotionTracker motion;

		protected Transform thisTransform;

		private GameObject sled;

		private Transform sledTransform;

		protected GameObject pilot;

		protected Rigidbody pilotBody;

		protected Transform pilotTransform;

		protected Mode mode;

		protected float elapsedTime;

		protected Vector3 steerVel;

		protected float curSpringAccel;

		protected float curSpringVel;

		private ParticleSystem particles;

		protected float minSpeedForFastLoopAnim;

		protected float minMagnitudeForBumpSq;

		private Vector3 momentumAtStart;

		private bool wasInAirAtStart;

		protected bool isFloatingOnWater;

		private float waterRippleT;

		private float prevWaterRippleAmp;

		private ParticleSystem waterRipples;

		private float maxSpeedOnWaterWhenSteering;

		private float curSpeedOnWater;

		protected float originalDrag;

		private float lastSplashTime;

		private Vector3 prevPilotPos = VECTOR3_ZERO;

		private Vector3 extrapPilotOffset = VECTOR3_ZERO;

		private float accumulatedVarTime = 0f;

		protected ActionRequest jumpRequest;

		protected bool isLocalPlayer;

		private bool isAirBorne;

		private float timeRemoteSnapIgnore = 0f;

		private bool _isSupported;

		protected virtual SlideControllerData mutableData
		{
			get;
			set;
		}

		public Mode CurrentMode
		{
			get
			{
				return mode;
			}
		}

		public bool ToRaceController
		{
			get;
			set;
		}

		public bool IsSliding
		{
			get;
			protected set;
		}

		public GameObject Sled
		{
			get
			{
				return sled;
			}
			protected set
			{
				sled = value;
			}
		}

		public Transform SledTransform
		{
			get
			{
				return sledTransform;
			}
			protected set
			{
				sledTransform = value;
			}
		}

		public GameObject Pilot
		{
			get
			{
				return pilot;
			}
			protected set
			{
				pilot = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return base.enabled;
			}
			private set
			{
				base.enabled = value;
			}
		}

		public bool IsAirborne
		{
			get
			{
				return isAirBorne;
			}
			protected set
			{
				isAirBorne = value;
			}
		}

		private bool isSupportedByWater
		{
			get
			{
				return _isSupported;
			}
			set
			{
				if (isLocalPlayer && _isSupported != value && mutableData != null)
				{
					if (value)
					{
						if (!string.IsNullOrEmpty(mutableData.Audio.AirborneEventName))
						{
							EventManager.Instance.PostEvent(mutableData.Audio.AirborneEventName, EventAction.StopSound, base.gameObject);
						}
						if (!string.IsNullOrEmpty(mutableData.Audio.SupportedEventName))
						{
							EventManager.Instance.PostEvent(mutableData.Audio.SupportedEventName, EventAction.PlaySound, base.gameObject);
						}
						if (!string.IsNullOrEmpty(mutableData.Audio.LandedEventName))
						{
							EventManager.Instance.PostEvent(mutableData.Audio.LandedEventName, EventAction.PlaySound, base.gameObject);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(mutableData.Audio.AirborneEventName))
						{
							EventManager.Instance.PostEvent(mutableData.Audio.AirborneEventName, EventAction.PlaySound, base.gameObject);
						}
						if (!string.IsNullOrEmpty(mutableData.Audio.SupportedEventName))
						{
							EventManager.Instance.PostEvent(mutableData.Audio.SupportedEventName, EventAction.StopSound, base.gameObject);
						}
					}
				}
				_isSupported = value;
			}
		}

		public void OnValidate()
		{
		}

		protected override void awake()
		{
			waterLayer = LayerMask.NameToLayer(LayerConstants.WaterLayer);
			raycastLayerMask = LayerConstants.GetTubeLayerCollisionMask();
			motion = GetComponent<MotionTracker>();
			isLocalPlayer = base.gameObject.CompareTag("Player");
			thisTransform = base.transform;
			ToRaceController = false;
			mutableData = instantiateData();
			minSpeedForFastLoopAnim = Mathf.Max(mutableData.MinSpeedForSlowLoopAnim, mutableData.MinSpeedForFastLoopAnim);
			minMagnitudeForBumpSq = mutableData.MinMagnitudeForBump * mutableData.MinMagnitudeForBump;
			jumpRequest = new ActionRequest(PenguinUserControl.DefaultActionRequestBufferTime, animator, AnimationHashes.Params.Jump);
			base.enabled = false;
		}

		protected virtual SlideControllerData instantiateData()
		{
			return Object.Instantiate(MasterData);
		}

		private void Update()
		{
			animator.SetBool(AnimationHashes.Params.Slide, true);
		}

		protected virtual void OnEnable()
		{
			if (canSlideFromCurrentState())
			{
				animator.SetBool(AnimationHashes.Params.Slide, true);
				wasInAirAtStart = LocomotionUtils.IsInAir(LocomotionUtils.GetAnimatorStateInfo(animator));
				IsSliding = false;
				isFloatingOnWater = false;
				isSupportedByWater = false;
				lastSplashTime = -1f;
				impulses = base.gameObject.AddComponent<ForceAccumulatorPrimitive>();
				impulses.SetData(mutableData.ImpulseProperties);
				impulses.enabled = true;
				mutableData.WaterProperties = MasterData.WaterProperties;
				maxSpeedOnWaterWhenSteering = mutableData.WaterProperties.MaxSpeed;
				curSpeedOnWater = motion.Velocity.magnitude;
				curSpeedOnWater = Mathf.Clamp(curSpeedOnWater, 0f, maxSpeedOnWaterWhenSteering);
				momentumAtStart = motion.Velocity;
				if (mutableData.WaterRipples != null)
				{
					waterRipples = Object.Instantiate(mutableData.WaterRipples);
					waterRipples.Stop();
					CameraCullingMaskHelper.SetLayerIncludingChildren(waterRipples.transform, LayerMask.LayerToName(base.gameObject.layer));
				}
				base.Broadcaster.OnInteractionPreStartedEvent += onInteractionPreStartedEvent;
			}
			else
			{
				base.enabled = false;
			}
		}

		private void onInteractionPreStartedEvent(GameObject trigger)
		{
			if (trigger.GetComponent<StayOnTubeAction>() == null)
			{
				GetComponent<LocomotionTracker>().SetCurrentController<RunController>();
			}
		}

		protected virtual void OnDisable()
		{
			base.Broadcaster.OnInteractionPreStartedEvent -= onInteractionPreStartedEvent;
			if (impulses != null)
			{
				Object.Destroy(impulses);
			}
			if (!ToRaceController)
			{
				if (pilot != null)
				{
					Object.Destroy(pilot);
				}
				if (sled != null)
				{
					Object.Destroy(sled);
				}
				IsSliding = false;
				if (animator != null)
				{
					animator.SetBool(AnimationHashes.Params.Slide, false);
				}
			}
			if (particles != null)
			{
				Object.Destroy(particles.gameObject);
			}
			if (waterRipples != null)
			{
				Object.Destroy(waterRipples.gameObject);
			}
			if (!string.IsNullOrEmpty(mutableData.Audio.SupportedEventName) && EventManager.Instance != null)
			{
				EventManager.Instance.PostEvent(mutableData.Audio.SupportedEventName, EventAction.StopSound, base.gameObject);
			}
			if (!string.IsNullOrEmpty(mutableData.Audio.AirborneEventName) && EventManager.Instance != null)
			{
				EventManager.Instance.PostEvent(mutableData.Audio.AirborneEventName, EventAction.StopSound, base.gameObject);
			}
		}

		protected virtual bool canSlideFromCurrentState()
		{
			AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
			return LocomotionUtils.IsIdling(animatorStateInfo) || LocomotionUtils.IsLocomoting(animatorStateInfo) || LocomotionUtils.IsInAir(animatorStateInfo) || LocomotionUtils.IsLanding(animatorStateInfo);
		}

		public override bool IsFullbodyLocked()
		{
			return true;
		}

		public void AnimEvent_ApplyImpulse()
		{
			if (!IsSliding && base.enabled)
			{
				mode = Mode.Animated;
				elapsedTime = 0f;
				if (wasInAirAtStart)
				{
					impulses.SetForce(momentumAtStart);
				}
				else
				{
					Vector3 force = momentumAtStart;
					force.y += mutableData.HopSpeed;
					impulses.SetForce(force);
				}
				IsSliding = true;
			}
		}

		public void AnimEvent_SpawnSled()
		{
			if (base.enabled && sled == null)
			{
				if (!IsSliding)
				{
					AnimEvent_ApplyImpulse();
				}
				sled = Object.Instantiate(mutableData.SledPrefab, thisTransform.position, thisTransform.rotation);
				sledTransform = sled.transform;
				sled.GetComponent<TubeLoader>().SetDataHandle(GetComponent<AvatarDataHandle>(), this);
				CameraCullingMaskHelper.SetLayerIncludingChildren(sledTransform, LayerMask.LayerToName(base.gameObject.layer));
			}
		}

		public void AnimEvent_TubeJump()
		{
			if (base.enabled && IsSliding && mode == Mode.PhysicsDriven && !IsAirborne && isSupportedByWater)
			{
				accumulatedVarTime = 0f;
				Vector3 velocity = pilotBody.velocity;
				velocity.y += mutableData.JumpSpeed;
				pilotBody.velocity = velocity;
				isSupportedByWater = false;
				jumpRequest.Reset();
			}
		}

		protected virtual void switchToPhysics()
		{
			pilot = Object.Instantiate(mutableData.PilotPrefab, thisTransform.position, thisTransform.rotation);
			pilotTransform = pilot.transform;
			pilotBody = pilot.GetComponent<Rigidbody>();
			SlideControllerListener component = pilotBody.GetComponent<SlideControllerListener>();
			if (component != null)
			{
				component.SlideController = this;
			}
			Rigidbody component2 = pilot.GetComponent<Rigidbody>();
			Vector3 wsVelocity = impulses.GetOutput().wsVelocity;
			component2.AddForce(wsVelocity, ForceMode.VelocityChange);
			mode = Mode.PhysicsDriven;
			elapsedTime = 0f;
			curSpringVel = 0f;
			originalDrag = pilotBody.drag;
			IsAirborne = false;
			animator.SetBool(AnimationHashes.Params.SlideAirborne, IsAirborne);
			if (!isLocalPlayer)
			{
				return;
			}
			if (isSupportedByWater)
			{
				if (!string.IsNullOrEmpty(mutableData.Audio.SupportedEventName))
				{
					EventManager.Instance.PostEvent(mutableData.Audio.SupportedEventName, EventAction.PlaySound, base.gameObject);
				}
				if (!string.IsNullOrEmpty(mutableData.Audio.AirborneEventName))
				{
					EventManager.Instance.PostEvent(mutableData.Audio.AirborneEventName, EventAction.StopSound, base.gameObject);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(mutableData.Audio.SupportedEventName))
				{
					EventManager.Instance.PostEvent(mutableData.Audio.SupportedEventName, EventAction.StopSound, base.gameObject);
				}
				if (!string.IsNullOrEmpty(mutableData.Audio.AirborneEventName))
				{
					EventManager.Instance.PostEvent(mutableData.Audio.AirborneEventName, EventAction.PlaySound, base.gameObject);
				}
			}
			spawnParticles();
		}

		protected void spawnParticles()
		{
			if (mutableData.ParticlesPrefab != null)
			{
				particles = Object.Instantiate(mutableData.ParticlesPrefab);
				particles.Stop();
				CameraCullingMaskHelper.SetLayerIncludingChildren(particles.transform, LayerMask.LayerToName(base.gameObject.layer));
				Vector3 localPosition = new Vector3(particles.transform.position.x, particles.transform.position.y, particles.transform.position.z);
				Quaternion localRotation = new Quaternion(particles.transform.rotation.x, particles.transform.rotation.y, particles.transform.rotation.z, particles.transform.rotation.w);
				particles.transform.parent = base.transform;
				particles.transform.localPosition = localPosition;
				particles.transform.localRotation = localRotation;
			}
		}

		public override Vector3 GetPosition()
		{
			if (pilotTransform != null)
			{
				return pilotTransform.position + extrapPilotOffset;
			}
			return base.transform.position;
		}

		public override void RemoteSetPosition(Vector3 newPos)
		{
			if (pilotBody != null && pilotTransform != null)
			{
				newPos.y = pilotTransform.position.y + extrapPilotOffset.y;
				pilotTransform.position = newPos;
				pilotBody.velocity = new Vector3(0f, pilotBody.velocity.y, 0f);
				sledTransform.position = pilotTransform.position + mutableData.SledOffsetFromPilot;
			}
			else
			{
				base.transform.position = newPos;
			}
		}

		public override void RemoteSnapPosition(Vector3 newPos)
		{
			float num = Time.time - timeRemoteSnapIgnore;
			if (num > snapTimeThreshold)
			{
				if (pilotBody != null && pilotTransform != null)
				{
					newPos.y += extrapPilotOffset.y;
					pilotTransform.position = newPos;
					pilotBody.velocity = VECTOR3_ZERO;
					sledTransform.position = pilotTransform.position + mutableData.SledOffsetFromPilot;
				}
				else
				{
					base.transform.position = newPos;
				}
				timeRemoteSnapIgnore = Time.time;
			}
		}

		public override void Steer(Vector2 steerInput)
		{
			if (mode == Mode.PhysicsDriven)
			{
				steerVel = LocomotionUtils.StickInputToWorldSpaceTransform(steerInput, LocomotionUtils.AxisIndex.Z);
				steerVel *= (isFloatingOnWater ? mutableData.ImpulseScaleOnWater : mutableData.ImpulseScale);
				base.Broadcaster.BroadcastOnStickDirectionEvent(steerInput);
				base.Broadcaster.BroadcastOnSteerDirectionEvent(steerVel);
			}
		}

		public override void Steer(Vector3 wsSteerInput)
		{
			steerVel = wsSteerInput * (isFloatingOnWater ? mutableData.ImpulseScaleOnWater : mutableData.ImpulseScale);
			base.Broadcaster.BroadcastOnSteerDirectionEvent(steerVel);
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
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.Action1:
			case LocomotionAction.Action2:
			case LocomotionAction.Action3:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.Jump:
				if (IsSliding && mode == Mode.PhysicsDriven && !jumpRequest.Active)
				{
					jumpRequest.Set();
					base.Broadcaster.BroadcastOnDoAction(action, userData);
				}
				break;
			}
		}

		protected virtual void FixedUpdate()
		{
			if (!IsSliding || mode != Mode.PhysicsDriven || !(pilotBody != null))
			{
				return;
			}
			if (isFloatingOnWater)
			{
				if (steerVel != VECTOR3_ZERO)
				{
					pilotBody.velocity += steerVel * hackFixedUpdateChangeMultipler;
					CapVelocity();
				}
			}
			else if (pilotBody.velocity.magnitude <= mutableData.MaxManualSpeedBeforeMomentumTakeover)
			{
				pilotBody.velocity += steerVel * hackFixedUpdateChangeMultipler;
			}
			else
			{
				pilotBody.velocity = (pilotBody.velocity + steerVel * hackFixedUpdateChangeMultipler).normalized * pilotBody.velocity.magnitude;
			}
		}

		private void transitionToWater()
		{
			isFloatingOnWater = true;
			pilotBody.drag = mutableData.WaterProperties.Drag;
			Vector3 velocity = pilotBody.velocity;
			velocity.y = 0f;
			pilotBody.velocity = velocity;
			curSpeedOnWater = Mathf.Min(velocity.magnitude, maxSpeedOnWaterWhenSteering);
			CapVelocity();
			isSupportedByWater = true;
		}

		private void transitionToLand()
		{
			pilotBody.drag = originalDrag;
			isFloatingOnWater = false;
		}

		public void OnCollisionEnter(Collision collision)
		{
			if (!base.enabled)
			{
				return;
			}
			if (collision.gameObject.layer == waterLayer)
			{
				if (collision.contacts.Length > 0 && mutableData.WaterSplash != null && pilotBody != null && Time.time - lastSplashTime >= mutableData.SplashCooldown)
				{
					GameObject gameObject = Object.Instantiate(mutableData.WaterSplash);
					if (gameObject != null)
					{
						Vector3 position = pilotBody.transform.position;
						position.y = collision.contacts[0].point.y + mutableData.SplashOffset;
						gameObject.transform.position = position;
						CameraCullingMaskHelper.SetLayerIncludingChildren(gameObject.transform, LayerMask.LayerToName(base.gameObject.layer));
					}
					lastSplashTime = Time.time;
				}
				transitionToWater();
			}
			if (LocomotionUtils.GetAnimatorStateInfo(animator).tagHash == AnimationHashes.Tags.ReactingToHit || !(collision.impulse.sqrMagnitude >= minMagnitudeForBumpSq) || !(Mathf.Abs(collision.impulse.y) < 0.707f))
			{
				return;
			}
			Vector3 vector = thisTransform.position - collision.contacts[0].point;
			vector.y = 0f;
			if (vector.x != 0f && vector.z != 0f)
			{
				Vector3 forward = thisTransform.forward;
				forward.y = 0f;
				if (forward.x != 0f && forward.z != 0f)
				{
					float num = LocomotionUtils.SignedAngle(forward.normalized, vector.normalized);
					num = ((num < -157.5f) ? 180f : ((num < -112.5f) ? (-135f) : ((num < -67.5f) ? (-90f) : ((num < -22.5f) ? (-45f) : ((num < 22.5f) ? 0f : ((num < 67.5f) ? 45f : ((num < 112.5f) ? 90f : ((!(num < 157.5f)) ? 180f : 135f))))))));
					animator.SetFloat(AnimationHashes.Params.Angle, num);
					animator.SetTrigger(AnimationHashes.Params.Bump);
				}
			}
		}

		private void updateActionRequests()
		{
			jumpRequest.Update();
		}

		protected virtual void LateUpdate()
		{
			if (GetComponent<ForceInteractionAction>() != null)
			{
				timeRemoteSnapIgnore = Time.time;
			}
			if (mode == Mode.Animated)
			{
				Vector3 vector = impulses.GetOutput().wsVelocity * Time.deltaTime;
				characterController.Move(vector);
			}
			if (!IsSliding)
			{
				return;
			}
			updateActionRequests();
			elapsedTime += Time.deltaTime;
			if (mode == Mode.Animated)
			{
				if (sled != null)
				{
					switchToPhysics();
					sledTransform.position = thisTransform.position + mutableData.SledOffsetFromPilot;
					sledTransform.rotation = getRotationOnSurface();
				}
			}
			else
			{
				Vector3 position = pilotTransform.position;
				if (position != prevPilotPos)
				{
					prevPilotPos = position;
					extrapPilotOffset = VECTOR3_ZERO;
					accumulatedVarTime = 0f;
				}
				else
				{
					accumulatedVarTime += Time.deltaTime;
					extrapPilotOffset += pilotBody.velocity * accumulatedVarTime;
					position += pilotBody.velocity * accumulatedVarTime;
				}
				Vector3 position2 = position;
				float num = isFloatingOnWater ? mutableData.WaterProperties.SurfaceOffset : 0f;
				if (elapsedTime > mutableData.BuildMomentumTime)
				{
					position2.y = thisTransform.position.y - num;
					float num2 = position.y - position2.y;
					if (num2 < 0f)
					{
						if (!IsAirborne)
						{
							if (num2 < -0.05f)
							{
								IsAirborne = true;
								animator.SetBool(AnimationHashes.Params.SlideAirborne, IsAirborne);
							}
						}
						else if (num2 > -0.025f)
						{
							IsAirborne = false;
							animator.SetBool(AnimationHashes.Params.SlideAirborne, IsAirborne);
						}
						curSpringAccel += mutableData.SpringAccel * Time.deltaTime;
						if (curSpringAccel > LocomotionUtils.DefaultGravity)
						{
							curSpringAccel = LocomotionUtils.DefaultGravity;
						}
						curSpringVel -= curSpringAccel * Time.deltaTime;
						position2.y += curSpringVel * Time.deltaTime;
						if (position2.y < position.y)
						{
							position2.y = position.y;
							curSpringVel = pilotBody.velocity.y;
							curSpringAccel = mutableData.StartingSpringAccel;
						}
					}
					else
					{
						if (IsAirborne)
						{
							IsAirborne = false;
							animator.SetBool(AnimationHashes.Params.SlideAirborne, IsAirborne);
						}
						position2.y = position.y;
						curSpringVel = pilotBody.velocity.y;
						curSpringAccel = mutableData.StartingSpringAccel;
					}
				}
				position2.y += num;
				position.y += num;
				thisTransform.position = position2;
				updateRotation();
				if (sled != null)
				{
					sledTransform.position = position + mutableData.SledOffsetFromPilot;
					sledTransform.rotation = thisTransform.rotation;
					updateVisualFX();
					updateAudioFX();
				}
				if (isFloatingOnWater)
				{
					updateMaxSpeedOnWater(Time.deltaTime);
				}
				float magnitude = pilotBody.velocity.magnitude;
				magnitude = Mathf.Lerp(b: (!(magnitude < mutableData.MinSpeedForSlowLoopAnim)) ? Mathf.Clamp01((magnitude - mutableData.MinSpeedForSlowLoopAnim) / (minSpeedForFastLoopAnim - mutableData.MinSpeedForSlowLoopAnim)) : 0f, a: animator.GetFloat(AnimationHashes.Params.NormSpeed), t: mutableData.IdleLoopTransitionSmoothing * Time.deltaTime);
				animator.SetFloat(AnimationHashes.Params.NormSpeed, magnitude);
			}
			applyIK();
		}

		private void updateMaxSpeedOnWater(float deltaTime)
		{
			float b = (steerVel != VECTOR3_ZERO) ? maxSpeedOnWaterWhenSteering : mutableData.WaterProperties.MaxSpeed;
			curSpeedOnWater = Mathf.Lerp(curSpeedOnWater, b, mutableData.WaterProperties.Drag * deltaTime);
		}

		private void updateVisualFX()
		{
			if (waterRipples != null)
			{
				if (isFloatingOnWater && Mathf.Abs(pilotBody.velocity.y) < 0.01f)
				{
					if (waterRipples.isStopped)
					{
						waterRipples.Play();
					}
					Vector3 position = sledTransform.position;
					position.y += mutableData.WaterRippleOffset;
					waterRipples.transform.position = position;
				}
				else if (waterRipples.isPlaying)
				{
					waterRipples.Stop();
				}
			}
			if (!(particles != null))
			{
				return;
			}
			if (isFloatingOnWater || IsAirborne)
			{
				if (particles.isPlaying)
				{
					particles.Stop();
				}
			}
			else if (!particles.isPlaying)
			{
				particles.Play();
			}
		}

		private void updateAudioFX()
		{
			if (!isLocalPlayer)
			{
				return;
			}
			if (!string.IsNullOrEmpty(mutableData.Audio.VelocityMagEventName))
			{
				float num = pilotBody.velocity.magnitude / 10f;
				if (num > 2f)
				{
					num = 2f;
				}
				EventManager.Instance.SetParameter(mutableData.Audio.VelocityMagEventName, mutableData.Audio.VelocityMagRTP, num, base.gameObject);
			}
			if (!string.IsNullOrEmpty(mutableData.Audio.YVelocityEventName))
			{
				EventManager.Instance.SetParameter(mutableData.Audio.YVelocityEventName, mutableData.Audio.YVelocityRTP, pilotBody.velocity.y, base.gameObject);
			}
		}

		private void updateRotation()
		{
			thisTransform.rotation = getRotationOnSurface();
			if (isFloatingOnWater)
			{
				waterRippleT += mutableData.WaterProperties.RippleRate * Time.deltaTime;
				waterRippleT -= (int)waterRippleT;
				float num = prevWaterRippleAmp = Mathf.Lerp(prevWaterRippleAmp, mutableData.WaterProperties.RippleAmplitude, mutableData.WaterRippleSmoothing * Time.deltaTime);
				thisTransform.Rotate(mutableData.WaveCurve.Evaluate(waterRippleT) * num, 0f, 0f, Space.World);
			}
		}

		private Quaternion getRotationOnSurface()
		{
			Vector3 vector = Vector3.up;
			RaycastHit hitInfo;
			if (isFloatingOnWater)
			{
				isSupportedByWater = Physics.Raycast(thisTransform.position, Vector3.down, out hitInfo, mutableData.GroundedDistance, raycastLayerMask);
				if (isSupportedByWater && hitInfo.collider.gameObject.layer != LayerMask.NameToLayer(LayerConstants.WaterLayer))
				{
					transitionToLand();
				}
			}
			else
			{
				vector = thisTransform.up;
				if (Physics.Raycast(thisTransform.position, Vector3.down, out hitInfo, mutableData.GroundedDistance, raycastLayerMask))
				{
					isSupportedByWater = true;
					if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer(LayerConstants.WaterLayer))
					{
						transitionToWater();
					}
					vector = hitInfo.normal;
				}
				else
				{
					isSupportedByWater = false;
				}
				if (mutableData.SurfaceSmoothing > 0f)
				{
					vector = Vector3.Slerp(thisTransform.up, vector, mutableData.SurfaceSmoothing * Time.deltaTime);
				}
			}
			Vector3 vECTOR3_ZERO = VECTOR3_ZERO;
			Vector3 vECTOR3_ZERO2 = VECTOR3_ZERO;
			Vector3 forward = thisTransform.forward;
			Vector3 b = (!(steerVel == VECTOR3_ZERO)) ? steerVel.normalized : pilotBody.velocity.normalized;
			forward = Vector3.Lerp(forward, b, Time.deltaTime * mutableData.RotationSmoothing);
			vECTOR3_ZERO = ((!(vector != forward) || !(vector != -forward)) ? Vector3.Cross(vector, base.transform.up) : Vector3.Cross(forward, vector));
			vECTOR3_ZERO2 = Vector3.Cross(vector, vECTOR3_ZERO);
			return Quaternion.LookRotation(vECTOR3_ZERO2, vector);
		}

		private void CapVelocity()
		{
			if (isFloatingOnWater)
			{
				Vector3 vector = pilotBody.velocity;
				vector.y = 0f;
				vector = Vector3.ClampMagnitude(vector, curSpeedOnWater);
				vector.y = pilotBody.velocity.y;
				pilotBody.velocity = vector;
			}
		}

		public override void AddForce(Vector3 wsForce, GameObject pusher = null)
		{
			if (IsSliding && mode == Mode.PhysicsDriven && (!isFloatingOnWater || !(steerVel != VECTOR3_ZERO)))
			{
				pilotBody.velocity += wsForce;
				CapVelocity();
			}
		}

		public override void SetForce(Vector3 wsForce, GameObject pusher = null)
		{
			if (IsSliding && mode == Mode.PhysicsDriven && (!isFloatingOnWater || !(steerVel != VECTOR3_ZERO) || (!(pusher == null) && !(pusher.GetComponent<Action>() == null))))
			{
				pilotBody.velocity = wsForce;
				CapVelocity();
			}
		}

		private void OnTriggerEnter(Collider trigger)
		{
			if (base.enabled && pilotBody != null)
			{
				WaterProperties component = trigger.gameObject.GetComponent<WaterProperties>();
				if (component != null)
				{
					mutableData.WaterProperties = component.properties;
					pilotBody.drag = mutableData.WaterProperties.Drag;
				}
			}
		}

		private void applyIK()
		{
			ChestBone.Rotate(mutableData.ChestBoneRotation);
		}
	}
}
