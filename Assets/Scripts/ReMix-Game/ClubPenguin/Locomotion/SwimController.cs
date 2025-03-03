using ClubPenguin.Avatar;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Interactables;
using ClubPenguin.Locomotion.Primitives;
using ClubPenguin.Props;
using ClubPenguin.UI;
using ClubPenguin.World.Activities.Diving;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(LocomotionTracker))]
	[RequireComponent(typeof(Rig))]
	public class SwimController : LocomotionController
	{
		public enum State
		{
			None,
			Resurfacing,
			QuickResurfacing,
			ExitingWater,
			Interacting,
			ReactingToHit,
			PostResurfacing
		}

		public class IKBoneEntry
		{
			public Transform Bone;

			public float StartAngle;

			public float CurAngle;

			public float TargetAngle;

			public IKBoneEntry()
			{
				StartAngle = 0f;
				CurAngle = 0f;
				TargetAngle = 0f;
			}
		}

		public float remoteSnapTime = 0.5f;

		private InputButtonGroupContentKey divingButtonsDefinitionContentKey = new InputButtonGroupContentKey("Definitions/ControlsScreen/Diving/DivingGroupDefinition");

		private InputButtonGroupContentKey swimmingButtonsDefinitionContentKey = new InputButtonGroupContentKey("Definitions/ControlsScreen/Swimming/SwimmingGroupDefinition");

		private static readonly float defaultSwimAnimIndex = 0f;

		private static readonly float gaspingSwimAnimIndex = 1f;

		public SwimControllerData MasterData;

		private SwimControllerData mutableData;

		private float logicalSurfaceHeight;

		private float visibleSurfaceHeight;

		private SwimPrimitive swim;

		private ForceAccumulatorPrimitive impulses;

		private EventDispatcher dispatcher;

		private State curState;

		private ParticleSystem waterRipples;

		private Vector3 waterRipplePos;

		private Transform cameraTransform;

		private LocomotionTracker tracker;

		private Rig rig;

		private float elapsedLowAirWarningTime;

		private float lowAirWarningDuration;

		private float prevAirSupply;

		private float desiredAnimIndex;

		private bool isInShallowWater;

		private PlayMakerFSM swimFSM;

		private List<IKBoneEntry> ikBones;

		private GameObject curPropGO;

		private bool isHoldingProp;

		private PropUser propUser;

		private PropIK propIK;

		private BaseCamera mainCamera;

		private bool _active = true;

		private List<SurfaceSwimProperties> volumeProperties = new List<SurfaceSwimProperties>();

		private Dictionary<Collider, SurfaceSwimProperties> volumeTriggerProperties = new Dictionary<Collider, SurfaceSwimProperties>();

		public InputButtonGroupContentKey DivingButtonsDefinitionContentKey
		{
			get
			{
				return divingButtonsDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey SwimmingButtonsDefinitionContentKey
		{
			get
			{
				return swimmingButtonsDefinitionContentKey;
			}
		}

		public bool TriggerResurface
		{
			get;
			set;
		}

		public bool TriggerQuickResurface
		{
			get;
			set;
		}

		public bool TriggerSnapToSurface
		{
			get;
			set;
		}

		public State CurState
		{
			get
			{
				return curState;
			}
		}

		public bool IsInShallowWater
		{
			get
			{
				return isInShallowWater;
			}
		}

		public bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
			}
		}

		protected override void awake()
		{
			mutableData = UnityEngine.Object.Instantiate(MasterData);
			tracker = GetComponent<LocomotionTracker>();
			rig = GetComponent<Rig>();
			cameraTransform = Camera.main.transform;
			dispatcher = Service.Get<EventDispatcher>();
			ikBones = new List<IKBoneEntry>();
			base.enabled = false;
		}

		public void Start()
		{
			mainCamera = ClubPenguin.Core.SceneRefs.Get<BaseCamera>();
		}

		private void OnEnable()
		{
			animator.SetTrigger(AnimationHashes.Params.Swim);
			swim = base.gameObject.AddComponent<SwimPrimitive>();
			swim.SetData(mutableData.SwimProperties);
			swim.enabled = true;
			swim.SurfaceHeight = logicalSurfaceHeight;
			swim.IsInShallowWater = isInShallowWater;
			impulses = base.gameObject.AddComponent<ForceAccumulatorPrimitive>();
			impulses.SetData(mutableData.ImpulseProperties);
			impulses.enabled = true;
			curState = State.None;
			Vector3 position = base.transform.position;
			position.y = logicalSurfaceHeight;
			if (isCloseToSurface())
			{
				base.transform.position = position;
			}
			dispatcher.AddListener<DivingEvents.AirSupplyUpdated>(onAirSupplyUpdated);
			if (CompareTag("Player"))
			{
				LoadContextualControlsLayout();
			}
			resetIK(null);
		}

		public override void RemoteSnapPosition(Vector3 newPos)
		{
			Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			base.gameObject.GetComponentInChildren<Canvas>().enabled = false;
			iTween.MoveTo(base.gameObject, iTween.Hash("position", newPos, "time", remoteSnapTime, "oncomplete", "OnCompleteSnap"));
		}

		public void OnCompleteSnap()
		{
			if (!base.gameObject.IsDestroyed())
			{
				Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
				base.gameObject.GetComponentInChildren<Canvas>().enabled = true;
			}
		}

		private void LoadContextualControlsLayout()
		{
			PropUser localPlayerPropUser = Service.Get<PropService>().LocalPlayerPropUser;
			if (localPlayerPropUser != null && localPlayerPropUser.Prop != null)
			{
				PropControlsOverride propControls = localPlayerPropUser.Prop.PropControls;
				if (!isInShallowWater && propControls.DivingControls != null && !string.IsNullOrEmpty(propControls.DivingControls.Key))
				{
					dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(propControls.DivingControls));
				}
				else
				{
					dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(propControls.SwimControls));
				}
			}
			else if (GetComponentInChildren<InvitationalItemExperience>() != null)
			{
				InvitationalItemExperience componentInChildren = GetComponentInChildren<InvitationalItemExperience>();
				if (!isInShallowWater && componentInChildren.DivingControlLayout != null && !string.IsNullOrEmpty(componentInChildren.DivingControlLayout.Key))
				{
					dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(componentInChildren.DivingControlLayout));
				}
				else
				{
					dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(componentInChildren.SwimControlLayout));
				}
			}
			else
			{
				LoadControlsLayout();
			}
		}

		public override void LoadControlsLayout()
		{
			if (isInShallowWater)
			{
				dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(swimmingButtonsDefinitionContentKey));
			}
			else
			{
				dispatcher.DispatchEvent(new ControlsScreenEvents.SetRightOption(divingButtonsDefinitionContentKey));
			}
		}

		public override void ResetState()
		{
		}

		private void OnDisable()
		{
			dispatcher.RemoveListener<DivingEvents.AirSupplyUpdated>(onAirSupplyUpdated);
			if (waterRipples != null)
			{
				UnityEngine.Object.Destroy(waterRipples.gameObject);
			}
			if (swim != null)
			{
				UnityEngine.Object.Destroy(swim);
			}
			if (impulses != null)
			{
				UnityEngine.Object.Destroy(impulses);
			}
			volumeProperties.Clear();
			volumeTriggerProperties.Clear();
			animator.ResetTrigger(AnimationHashes.Params.Swim);
		}

		public void SetSurfaceHeight(float height)
		{
			logicalSurfaceHeight = height;
			visibleSurfaceHeight = height;
			if (swim != null)
			{
				swim.SurfaceHeight = logicalSurfaceHeight;
			}
		}

		public float GetSurfaceHeight()
		{
			return logicalSurfaceHeight;
		}

		public override bool AllowTriggerOnStay()
		{
			return false;
		}

		public override void Steer(Vector2 steerInput)
		{
			if (curState == State.None)
			{
				swim.Steer(steerInput);
				base.Broadcaster.BroadcastOnStickDirectionEvent(steerInput);
			}
		}

		public override void Steer(Vector3 wsSteerInput)
		{
			if (curState == State.None)
			{
				swim.Steer(wsSteerInput);
			}
		}

		public override void SteerRotation(Vector2 steerInput)
		{
			if (curState == State.None)
			{
				swim.SteerRotation(steerInput);
			}
		}

		public override void SteerRotation(Vector3 wsSteerInput)
		{
			if (curState == State.None)
			{
				swim.SteerRotation(wsSteerInput);
			}
		}

		public override void AddForce(Vector3 wsForce, GameObject pusher = null)
		{
			if (curState == State.None)
			{
				impulses.AddForce(wsForce);
			}
		}

		public override void SetForce(Vector3 wsForce, GameObject pusher = null)
		{
			if (curState == State.None || curState == State.Interacting)
			{
				impulses.SetForce(wsForce);
				if (pusher != null && pusher.GetComponent<Repulse>() != null)
				{
					onReactToHit();
				}
			}
		}

		public override void DoAction(LocomotionAction action, object userData = null)
		{
			switch (action)
			{
			case LocomotionAction.Torpedo:
				if (curState == State.None)
				{
					animator.ResetTrigger(AnimationHashes.Params.AbortTorpedo);
					swim.Torpedo();
					base.Broadcaster.BroadcastOnDoAction(action, userData);
				}
				break;
			case LocomotionAction.Interact:
			case LocomotionAction.Action3:
				DefaultDoAction(action, userData);
				break;
			}
		}

		public override void OnBlockingInteractionStarted()
		{
			curState = State.Interacting;
		}

		private bool isCloseToSurface()
		{
			float num = logicalSurfaceHeight - base.transform.position.y;
			return num <= mutableData.MaxDistToConsiderNearSurface;
		}

		public bool IsOnSurface()
		{
			float num = logicalSurfaceHeight - base.transform.position.y;
			return num <= mutableData.MaxDistToConsiderOnSurface;
		}

		private void onReactToHit()
		{
			dispatcher.DispatchEvent(new DivingEvents.CollisionEffects(base.gameObject.tag));
			playAudioEvent(mutableData.CollisionAudioEvent);
		}

		private void onTriggerResurface()
		{
			TriggerResurface = false;
			if (curState == State.None)
			{
				curState = State.Resurfacing;
				animator.SetBool(AnimationHashes.Params.Resurface, true);
			}
		}

		public void ResurfaceAccepted()
		{
			TriggerQuickResurface = false;
			if (curState == State.None)
			{
				curState = State.QuickResurfacing;
				animator.SetBool(AnimationHashes.Params.QuickResurface, true);
			}
		}

		private bool onAirSupplyUpdated(DivingEvents.AirSupplyUpdated evt)
		{
			float num = evt.AirSupply * DivingAirPanel.AirSupplySlice;
			if (num > 0f)
			{
				if (prevAirSupply > num)
				{
					for (int i = 0; i < mutableData.LowAirThresholds.Length; i++)
					{
						if (Mathf.Abs(num - mutableData.LowAirThresholds[i].AirSupplyThreshold) < Mathf.Epsilon)
						{
							lowAirWarningDuration = mutableData.LowAirThresholds[i].AnimDuration;
							elapsedLowAirWarningTime = 0f;
							desiredAnimIndex = gaspingSwimAnimIndex;
							break;
						}
					}
				}
				else
				{
					if (prevAirSupply < num)
					{
						desiredAnimIndex = defaultSwimAnimIndex;
						lowAirWarningDuration = 0f;
					}
					else if (lowAirWarningDuration > 0f)
					{
						elapsedLowAirWarningTime += Time.deltaTime;
						if (elapsedLowAirWarningTime >= lowAirWarningDuration)
						{
							desiredAnimIndex = defaultSwimAnimIndex;
							lowAirWarningDuration = 0f;
						}
					}
					float @float = animator.GetFloat(AnimationHashes.Params.LowAirAnimChooser);
					@float = Mathf.Lerp(@float, desiredAnimIndex, mutableData.LowAirAnimSmoothing * Time.deltaTime);
					@float = Mathf.Clamp01(@float);
					animator.SetFloat(AnimationHashes.Params.LowAirAnimChooser, @float);
				}
				prevAirSupply = num;
			}
			else
			{
				desiredAnimIndex = defaultSwimAnimIndex;
			}
			return false;
		}

		private void updateFX()
		{
			if (!(waterRipples != null))
			{
				return;
			}
			if (IsOnSurface())
			{
				if (waterRipples.isStopped)
				{
					waterRipples.Play();
				}
				waterRipplePos.Set(base.transform.position.x, logicalSurfaceHeight + mutableData.RippleHeightOffset, base.transform.position.z);
				waterRipples.transform.position = waterRipplePos;
			}
			else if (waterRipples.isPlaying)
			{
				waterRipples.Stop();
			}
		}

		public void FixedUpdate()
		{
			if (curState == State.PostResurfacing)
			{
				curState = State.None;
			}
		}

		private void updateResurfacing()
		{
			if (curState == State.PostResurfacing)
			{
				return;
			}
			AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
			if (TriggerSnapToSurface || (animatorStateInfo.normalizedTime > 0.9f && animatorStateInfo.tagHash == AnimationHashes.Tags.Resurfacing) || (animatorStateInfo.normalizedTime > 0.8f && animatorStateInfo.tagHash == AnimationHashes.Tags.QuickResurfacing))
			{
				mainCamera.Snap();
				if (curState == State.QuickResurfacing)
				{
					base.transform.position = mutableData.QuickResurfacingTransform.position;
				}
				else
				{
					base.transform.position = mutableData.ResurfaceTransform.position;
				}
				curState = State.PostResurfacing;
				animator.SetBool(AnimationHashes.Params.Resurface, false);
				animator.SetBool(AnimationHashes.Params.QuickResurface, false);
				desiredAnimIndex = defaultSwimAnimIndex;
				animator.SetFloat(AnimationHashes.Params.LowAirAnimChooser, desiredAnimIndex);
				TriggerSnapToSurface = false;
				base.transform.rotation = Quaternion.LookRotation(-cameraTransform.forward, cameraTransform.up);
				swim.ResetState();
				impulses.ResetState();
			}
		}

		private void Update()
		{
			switch (curState)
			{
			case State.Resurfacing:
			case State.QuickResurfacing:
			case State.PostResurfacing:
				updateResurfacing();
				break;
			case State.ExitingWater:
				animator.SetTrigger(AnimationHashes.Params.SwimToWalk);
				base.transform.rotation = Quaternion.LookRotation(new Vector3(base.transform.forward.x, 0f, base.transform.forward.z), Vector3.up);
				LocomotionHelper.SetCurrentController<RunController>(base.gameObject);
				break;
			case State.Interacting:
				if (SceneRefs.ActionSequencer.GetTrigger(base.gameObject) == null)
				{
					curState = State.None;
					swim.ResetState();
				}
				break;
			case State.ReactingToHit:
				if (!LocomotionUtils.IsReactingToHit(LocomotionUtils.GetAnimatorStateInfo(animator)))
				{
					curState = State.None;
					swim.ResetState();
				}
				break;
			default:
				if (LocomotionUtils.IsReactingToHit(LocomotionUtils.GetAnimatorStateInfo(animator)))
				{
					curState = State.ReactingToHit;
				}
				if (isWaterTooShallow(mutableData.MaxShallowWaterDepth))
				{
					curState = State.ExitingWater;
				}
				updateFX();
				break;
			}
			if (TriggerResurface)
			{
				onTriggerResurface();
			}
			if (TriggerQuickResurface)
			{
				pauseHealth();
				TriggerQuickResurface = false;
			}
		}

		private bool isWaterTooShallow(float maxDepth)
		{
			if (isInShallowWater)
			{
				Vector3 position = base.transform.position;
				position.y = visibleSurfaceHeight;
				if (Physics.Raycast(position, Vector3.down, maxDepth, LayerConstants.GetPlayerLayerCollisionMask()))
				{
					return true;
				}
			}
			return false;
		}

		private Quaternion getSmoothFaceCameraRotation()
		{
			Quaternion b = Quaternion.LookRotation(-cameraTransform.forward, cameraTransform.up);
			return Quaternion.Slerp(base.transform.rotation, b, mutableData.HitReactRotationSmoothing * Time.deltaTime);
		}

		private void LateUpdate()
		{
			if (!Active)
			{
				return;
			}
			if (curState == State.Resurfacing || curState == State.QuickResurfacing)
			{
				base.transform.position += animator.deltaPosition;
				base.transform.rotation = getSmoothFaceCameraRotation();
			}
			else
			{
				if (curState == State.Interacting)
				{
					return;
				}
				if (curState != State.ExitingWater && !LocomotionUtils.IsSwimming(LocomotionUtils.GetAnimatorStateInfo(animator)))
				{
					animator.SetTrigger(AnimationHashes.Params.Swim);
				}
				LocomotionPrimitive.PrimitiveOutput output = swim.GetOutput();
				Vector3 vector = output.wsDeltaPos;
				Quaternion rotation = output.wsRotation;
				if (isInShallowWater)
				{
					vector *= mutableData.ShallowWaterSwimSpeedMultiplier;
				}
				if (curState == State.ExitingWater)
				{
					rotation = Quaternion.LookRotation(new Vector3(base.transform.forward.x, 0f, base.transform.forward.z), Vector3.up);
					rotation = Quaternion.Slerp(base.transform.rotation, rotation, 5f * Time.deltaTime);
					vector = animator.deltaPosition;
				}
				else
				{
					if (curState == State.ReactingToHit)
					{
						rotation = getSmoothFaceCameraRotation();
					}
					vector += impulses.GetOutput().wsVelocity * Time.deltaTime;
					if (!isInShallowWater && mutableData.FreezeAxis != SwimControllerData.FreezeAxisType.None)
					{
						vector[(int)mutableData.FreezeAxis] = mutableData.FreezeDist;
					}
				}
				Vector3 position = base.transform.position;
				base.transform.rotation = rotation;
				if (vector != Vector3.zero)
				{
					characterController.Move(vector);
				}
				if (curState != 0)
				{
					return;
				}
				Vector3 position2;
				if (isInShallowWater)
				{
					position2 = base.transform.position;
					position2.y = logicalSurfaceHeight;
					base.transform.position = position2;
				}
				else
				{
					if (mutableData.FreezeAxis != SwimControllerData.FreezeAxisType.None && !isCloseToSurface())
					{
						position2 = base.transform.position;
						position2[(int)mutableData.FreezeAxis] = Mathf.Lerp(position2[(int)mutableData.FreezeAxis], mutableData.FreezeDist, 5f * Time.deltaTime);
						if (Mathf.Abs(position2[(int)mutableData.FreezeAxis] - mutableData.FreezeDist) < 0.01f)
						{
							position2[(int)mutableData.FreezeAxis] = mutableData.FreezeDist;
						}
						base.transform.position = position2;
					}
					if (base.transform.position.y >= logicalSurfaceHeight && curState != State.PostResurfacing)
					{
						position2 = base.transform.position;
						position2.y = logicalSurfaceHeight;
						base.transform.position = position2;
						if (LocomotionUtils.IsTurboing(animator.GetCurrentAnimatorStateInfo(0)))
						{
							animator.SetTrigger(AnimationHashes.Params.AbortTorpedo);
						}
					}
				}
				applyIK();
			}
		}

		private void enableWaterEffects()
		{
			if (mutableData.Ripples != null && waterRipples == null)
			{
				waterRipples = UnityEngine.Object.Instantiate(mutableData.Ripples);
				waterRipples.Stop();
				CameraCullingMaskHelper.SetLayerIncludingChildren(waterRipples.transform, LayerMask.LayerToName(base.gameObject.layer));
			}
			if (mutableData.Splash != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(mutableData.Splash);
				gameObject.transform.position = new Vector3(base.transform.position.x, logicalSurfaceHeight + mutableData.SplashHeightOffset, base.transform.position.z);
				CameraCullingMaskHelper.SetLayerIncludingChildren(gameObject.transform, LayerMask.LayerToName(base.gameObject.layer));
			}
		}

		private void addVolumeProperties(SurfaceSwimProperties properties)
		{
			if (properties != null && !volumeProperties.Contains(properties))
			{
				volumeProperties.Add(properties);
				setVolumeProperties(properties);
			}
		}

		private void removeVolumeProperties(SurfaceSwimProperties properties)
		{
			if (!(properties != null) || !volumeProperties.Contains(properties))
			{
				return;
			}
			if (volumeProperties.IndexOf(properties) == volumeProperties.Count - 1)
			{
				volumeProperties.Remove(properties);
				if (volumeProperties.Count > 0)
				{
					setVolumeProperties(volumeProperties[volumeProperties.Count - 1]);
				}
			}
			else
			{
				volumeProperties.Remove(properties);
			}
			if (volumeProperties.Count == 0)
			{
				setVolumeProperties(null);
			}
		}

		private SurfaceSwimProperties getVolumeTriggerProperties(Collider trigger)
		{
			SurfaceSwimProperties value;
			if (!volumeTriggerProperties.TryGetValue(trigger, out value))
			{
				value = trigger.GetComponent<SurfaceSwimProperties>();
				volumeTriggerProperties.Add(trigger, value);
				addVolumeProperties(value);
			}
			return value;
		}

		private SurfaceSwimProperties removeVolumeTriggerProperties(Collider trigger)
		{
			SurfaceSwimProperties value = null;
			if (volumeTriggerProperties.TryGetValue(trigger, out value))
			{
				volumeTriggerProperties.Remove(trigger);
				removeVolumeProperties(value);
			}
			return value;
		}

		private void setVolumeProperties(SurfaceSwimProperties properties)
		{
			if (properties != null)
			{
				isInShallowWater = (properties.Type == SurfaceSwimProperties.VolumeType.SurfaceSwimming);
				visibleSurfaceHeight = (properties.SpecifyValuesInLocalSpace ? (properties.VisibleSurfaceHeight + properties.ObjectOrigin.position.y) : properties.VisibleSurfaceHeight);
				logicalSurfaceHeight = visibleSurfaceHeight + properties.SinkOffset;
				if (swim != null)
				{
					swim.IsInShallowWater = isInShallowWater;
				}
			}
		}

		private void OnTriggerEnter(Collider trigger)
		{
			OnTriggerStay(trigger);
		}

		private void OnTriggerStay(Collider trigger)
		{
			SurfaceSwimProperties surfaceSwimProperties = getVolumeTriggerProperties(trigger);
			if (!base.enabled && Active && (tracker.IsCurrentControllerOfType<RunController>() || (tracker.IsCurrentControllerOfType<SlideController>() && (tracker.GetCurrentController() as SlideController).CurrentMode == SlideController.Mode.Animated)) && surfaceSwimProperties != null && (surfaceSwimProperties.Type == SurfaceSwimProperties.VolumeType.Diving || !isWaterTooShallow(mutableData.MaxShallowWaterDepth + mutableData.ShallowWaterDepthHysteresis)))
			{
				tracker.SetCurrentController<SwimController>();
				enableWaterEffects();
			}
		}

		private void OnTriggerExit(Collider trigger)
		{
			SurfaceSwimProperties x = removeVolumeTriggerProperties(trigger);
			if (curState != State.Resurfacing && curState != State.QuickResurfacing && volumeProperties.Count == 0 && (x != null || trigger.CompareTag(LocomotionUtils.WaterVolumeTag)))
			{
				curState = State.ExitingWater;
			}
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (!isInShallowWater && LocomotionUtils.IsTurboing(animator.GetCurrentAnimatorStateInfo(0)))
			{
				animator.SetTrigger(AnimationHashes.Params.AbortTorpedo);
			}
		}

		private void pauseHealth()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (!(localPlayerGameObject != null))
			{
				return;
			}
			PlayMakerFSM[] components = localPlayerGameObject.GetComponents<PlayMakerFSM>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].FsmName == "PenguinSwimFSM")
				{
					swimFSM = components[i];
					break;
				}
			}
			if (swimFSM != null)
			{
				swimFSM.FsmVariables.GetFsmString("DivingStatus").Value = "invincible";
			}
		}

		private void resetIK(GameObject newPropGO)
		{
			curPropGO = newPropGO;
			propIK = null;
			isHoldingProp = false;
			for (int i = 0; i < ikBones.Count; i++)
			{
				ikBones[i].StartAngle = ikBones[i].CurAngle;
				ikBones[i].TargetAngle = 0f;
			}
			if (newPropGO != null)
			{
				isHoldingProp = true;
				propIK = newPropGO.GetComponent<PropIK>();
				if (propIK != null && propIK.IKModifiers != null && propIK.IKModifiers.Length > 0)
				{
					findIKBones();
				}
			}
		}

		private void applyIK()
		{
			if (propUser == null)
			{
				propUser = GetComponent<PropUser>();
				if (propUser == null)
				{
					return;
				}
			}
			if (curPropGO == null)
			{
				if (propUser.Prop != null)
				{
					resetIK(propUser.Prop.gameObject);
				}
				else
				{
					InvitationalItemController componentInChildren = GetComponentInChildren<InvitationalItemController>();
					if (componentInChildren != null)
					{
						curPropGO = componentInChildren.gameObject;
						resetIK(curPropGO);
					}
				}
			}
			if (curPropGO == null && isHoldingProp)
			{
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(1);
				if (!LocomotionUtils.IsUsing(currentAnimatorStateInfo))
				{
					resetIK(null);
				}
			}
			if (propIK != null && ikBones.Count > 0)
			{
				AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
				for (int i = 0; i < ikBones.Count; i++)
				{
					if (swim.IsInShallowWater)
					{
						if (LocomotionUtils.IsIdling(animatorStateInfo))
						{
							ikBones[i].TargetAngle = propIK.IKModifiers[i].IdleZRot;
						}
						else
						{
							ikBones[i].TargetAngle = propIK.IKModifiers[i].SwimMoveZRot;
						}
					}
					else if (LocomotionUtils.IsIdling(animatorStateInfo))
					{
						ikBones[i].TargetAngle = propIK.IKModifiers[i].IdleZRot;
					}
					else
					{
						ikBones[i].TargetAngle = propIK.IKModifiers[i].DiveMoveZRot;
					}
				}
			}
			blendIK();
		}

		private void blendIK()
		{
			bool flag = false;
			bool flag2 = false;
			int fullPathHash = animator.GetCurrentAnimatorStateInfo(1).fullPathHash;
			if (animator.IsInTransition(1))
			{
				int fullPathHash2 = animator.GetNextAnimatorStateInfo(1).fullPathHash;
				if (fullPathHash == AnimationHashes.States.TorsoIdle)
				{
					flag = true;
				}
				else if (curPropGO == null && isHoldingProp && fullPathHash2 == AnimationHashes.States.TorsoIdle && base.gameObject.GetComponent<RunController>().DefaultAnimatorController == animator.runtimeAnimatorController)
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				float normalizedTime = animator.GetAnimatorTransitionInfo(1).normalizedTime;
				for (int i = 0; i < ikBones.Count; i++)
				{
					ikBones[i].CurAngle = Mathf.Lerp(ikBones[i].StartAngle, ikBones[i].TargetAngle, normalizedTime);
					ikBones[i].Bone.Rotate(0f, 0f, ikBones[i].CurAngle);
				}
			}
			else if (flag2)
			{
				float normalizedTime = animator.GetAnimatorTransitionInfo(1).normalizedTime;
				for (int i = 0; i < ikBones.Count; i++)
				{
					ikBones[i].CurAngle = Mathf.Lerp(ikBones[i].StartAngle, 0f, normalizedTime);
					ikBones[i].Bone.Rotate(0f, 0f, ikBones[i].CurAngle);
				}
			}
			else if (fullPathHash != AnimationHashes.States.TorsoIdle)
			{
				float t = mutableData.SwimWithPropIKSmoothing * Time.deltaTime;
				for (int i = 0; i < ikBones.Count; i++)
				{
					ikBones[i].CurAngle = Mathf.Lerp(ikBones[i].CurAngle, ikBones[i].TargetAngle, t);
					ikBones[i].StartAngle = ikBones[i].CurAngle;
					ikBones[i].Bone.Rotate(0f, 0f, ikBones[i].CurAngle);
				}
			}
		}

		private void findIKBones()
		{
			ikBones.Clear();
			for (int i = 0; i < propIK.IKModifiers.Length; i++)
			{
				try
				{
					IKBoneEntry iKBoneEntry = new IKBoneEntry();
					iKBoneEntry.Bone = rig[propIK.IKModifiers[i].BoneName];
					ikBones.Add(iKBoneEntry);
				}
				catch (ArgumentException)
				{
					Log.LogErrorFormatted(base.gameObject, "Could not find bone '{0}', needed for IK on prop '{1}', in gameobject '{2}'!", propIK.IKModifiers[i].BoneName, propIK.gameObject.name, base.gameObject.name);
				}
			}
		}

		private void playAudioEvent(string audioEvent)
		{
			if (!string.IsNullOrEmpty(audioEvent))
			{
				EventManager.Instance.PostEvent(audioEvent, EventAction.PlaySound, base.gameObject);
			}
		}
	}
}
