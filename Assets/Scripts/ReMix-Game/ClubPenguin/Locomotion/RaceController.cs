using ClubPenguin.Adventure;
using ClubPenguin.Cinematography;
using ClubPenguin.Cinematography.Cameras;
using ClubPenguin.Core;
using ClubPenguin.SledRacer;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(MotionTracker))]
	public class RaceController : SlideController
	{
		public enum RaceControllerMode
		{
			Countdown,
			Launch,
			Race
		}

		private const string QUEST_EVENT_START_RACE = "Start{0}Race";

		public RaceControllerData RaceMasterData;

		private RaceControllerData myMutableData;

		private EventDispatcher dispatcher;

		private PhysicMaterial physicMaterial;

		private PhysicMaterialCombine physicMaterialCombine;

		private float bounciness;

		private GameObject speedLines;

		private ChaseCamera chaseCamera;

		private float fixedSpeed;

		private float prevFixedSpeed;

		private float interpolatedFixedSpeed;

		private float fixedSpeedInterpolationTime;

		private float maxSpeed;

		private float prevMaxSpeed;

		private float interpolatedMaxSpeed;

		private float maxSpeedInterpolationTime;

		private Vector3 trackDir;

		private Vector3 prevTrackDir;

		private Vector3 interpolatedTrackDir;

		private bool firstTrackDir = false;

		private float trackDirInterpolationTime;

		private Quaternion lateralRotation = Quaternion.Euler(0f, 90f, 0f);

		private float constantForwardThrustThresholdSquared;

		private bool visualizeTrackSegment;

		private GameObject trackDirObject;

		private LineRenderer trackDirRenderer;

		private GameObject steeringObject;

		private LineRenderer steeringObjectRenderer;

		private RaceControllerMode raceControllerMode;

		private RaceGameController raceGameController;

		private int delayUI;

		private static readonly float TERMINAL_VELOCITY = -10f;

		public bool FromSlideController
		{
			get;
			set;
		}

		protected override void awake()
		{
			base.awake();
			visualizeTrackSegment = myMutableData.VisualizeTrackSegment;
			constantForwardThrustThresholdSquared = myMutableData.ConstantForwardThrustThreshold * myMutableData.ConstantForwardThrustThreshold;
			delayUI = 0;
		}

		protected override SlideControllerData instantiateData()
		{
			myMutableData = Object.Instantiate(RaceMasterData);
			return myMutableData;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus && LocomotionHelper.IsCurrentControllerOfType<RaceController>(base.gameObject))
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				PausedStateData component;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					OutOfBoundsWarper component2 = GetComponent<OutOfBoundsWarper>();
					component.Position = component2.StartingPosition;
				}
			}
		}

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<RaceGameEvents.Launch>(launch);
			dispatcher.AddListener<OutOfBoundsWarperEvents.ResetPlayer>(onOutOfBoundsReset);
		}

		private void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.RemoveListener<RaceGameEvents.Launch>(launch);
				dispatcher.RemoveListener<OutOfBoundsWarperEvents.ResetPlayer>(onOutOfBoundsReset);
			}
		}

		public void SetInitialTrackDir(Vector3 direction)
		{
			trackDir = direction;
			interpolatedTrackDir = trackDir;
		}

		public void InitializeRace(RaceGameController raceGameController)
		{
			this.raceGameController = raceGameController;
			fixedSpeed = 0f;
			interpolatedFixedSpeed = fixedSpeed;
			maxSpeed = myMutableData.RaceTrackProperties.MaxSpeed;
			interpolatedMaxSpeed = maxSpeed;
			firstTrackDir = false;
			chaseCamera = null;
			raceControllerMode = RaceControllerMode.Countdown;
			delayUI = 2;
		}

		private bool launch(RaceGameEvents.Launch e)
		{
			if (isLocalPlayer)
			{
				raceControllerMode = RaceControllerMode.Launch;
				jumpRequest.Set();
				steerVel = trackDir * myMutableData.LaunchImpulse;
				speedLines.SetActive(true);
				ChaseCamera chaseCamera = ClubPenguin.Core.SceneRefs.Get<ChaseCamera>();
				if (chaseCamera != null)
				{
					chaseCamera.Enable(base.gameObject);
				}
				setControlsEnabled(false);
			}
			return false;
		}

		public override void AddForce(Vector3 wsForce, GameObject pusher = null)
		{
			if (base.IsSliding && mode == Mode.PhysicsDriven)
			{
				Vector3 vector = pilotBody.velocity.normalized * wsForce.magnitude;
				pilotBody.velocity += vector;
			}
		}

		public override void SetForce(Vector3 wsForce, GameObject pusher = null)
		{
			if (base.IsSliding && mode == Mode.PhysicsDriven)
			{
				if (prevFixedSpeed == 0f && pilotBody != null)
				{
					prevFixedSpeed = pilotBody.velocity.magnitude;
				}
				else
				{
					prevFixedSpeed = fixedSpeed;
				}
				fixedSpeed = wsForce.magnitude;
				fixedSpeedInterpolationTime = 0f;
			}
		}

		protected override void switchToPhysics()
		{
			if (FromSlideController)
			{
				pilotTransform = pilot.transform;
				pilotBody = pilot.GetComponent<Rigidbody>();
				SlideControllerListener component = pilotBody.GetComponent<SlideControllerListener>();
				if (component != null)
				{
					component.SlideController = this;
				}
				mode = Mode.PhysicsDriven;
				elapsedTime = 0f;
				curSpringVel = 0f;
				originalDrag = pilotBody.drag;
				pilotBody.velocity = Vector3.zero;
				spawnParticles();
			}
			else
			{
				base.switchToPhysics();
			}
			pilot.layer = LayerMask.NameToLayer("NoncollidingTube");
			pilotBody.drag = myMutableData.RaceTrackProperties.Drag;
			Collider component2 = pilot.GetComponent<Collider>();
			if (component2 != null)
			{
				physicMaterial = component2.material;
				if (physicMaterial != null)
				{
					bounciness = physicMaterial.bounciness;
					physicMaterialCombine = physicMaterial.bounceCombine;
					physicMaterial.bounciness = myMutableData.Bounciness;
					physicMaterial.bounceCombine = myMutableData.BounceCombine;
				}
			}
		}

		public bool InitializeFromSlideController(SlideController slideController)
		{
			base.Sled = slideController.Sled;
			base.SledTransform = slideController.SledTransform;
			base.Pilot = slideController.Pilot;
			if (base.Sled == null || base.SledTransform == null || base.Pilot == null)
			{
				return false;
			}
			return true;
		}

		protected override void OnEnable()
		{
			AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
			LocomotionController currentController = GetComponent<LocomotionTracker>().GetCurrentController();
			if (currentController is SlideController)
			{
				base.Sled = ((SlideController)currentController).Sled;
				base.SledTransform = ((SlideController)currentController).SledTransform;
				base.Pilot = ((SlideController)currentController).Pilot;
				if (base.Sled == null || base.SledTransform == null || base.Pilot == null || LocomotionUtils.IsInAir(animatorStateInfo) || LocomotionUtils.IsLanding(animatorStateInfo) || ((SlideController)currentController).CurrentMode == Mode.Animated)
				{
					base.enabled = false;
					return;
				}
				((SlideController)currentController).ToRaceController = true;
				FromSlideController = true;
			}
			else if (!LocomotionUtils.IsIdling(animatorStateInfo) && !LocomotionUtils.IsLocomoting(animatorStateInfo))
			{
				base.enabled = false;
				return;
			}
			base.OnEnable();
			if (myMutableData.SpeedLinesTubeRacePrefab != null)
			{
				speedLines = Object.Instantiate(myMutableData.SpeedLinesTubeRacePrefab);
				CameraCullingMaskHelper.SetLayerIncludingChildren(speedLines.transform, LayerMask.LayerToName(base.gameObject.layer));
				Vector3 localPosition = new Vector3(speedLines.transform.position.x, speedLines.transform.position.y, speedLines.transform.position.z);
				Quaternion localRotation = new Quaternion(speedLines.transform.rotation.x, speedLines.transform.rotation.y, speedLines.transform.rotation.z, speedLines.transform.rotation.w);
				speedLines.transform.parent = base.transform;
				speedLines.transform.localPosition = localPosition;
				speedLines.transform.localRotation = localRotation;
				speedLines.SetActive(false);
			}
			if (visualizeTrackSegment)
			{
				steeringObject = new GameObject();
				steeringObjectRenderer = steeringObject.AddComponent<LineRenderer>();
				steeringObjectRenderer.transform.parent = base.gameObject.transform;
				steeringObjectRenderer.useWorldSpace = true;
				trackDirObject = new GameObject();
				trackDirRenderer = trackDirObject.AddComponent<LineRenderer>();
				trackDirObject.transform.parent = base.gameObject.transform;
				trackDirRenderer.useWorldSpace = true;
			}
			if (FromSlideController)
			{
				mode = Mode.Animated;
				base.IsSliding = true;
			}
			steerVel = Vector3.zero;
		}

		protected override void OnDisable()
		{
			if (physicMaterial != null)
			{
				physicMaterial.bounciness = bounciness;
				physicMaterial.bounceCombine = physicMaterialCombine;
			}
			if (visualizeTrackSegment)
			{
				Object.Destroy(steeringObject);
				Object.Destroy(trackDirObject);
			}
			if (speedLines != null)
			{
				Object.Destroy(speedLines.gameObject);
			}
			if (FromSlideController)
			{
				FromSlideController = false;
				GetComponent<SlideController>().ToRaceController = false;
			}
			base.OnDisable();
		}

		protected override bool canSlideFromCurrentState()
		{
			if (FromSlideController)
			{
				return true;
			}
			return base.canSlideFromCurrentState();
		}

		public override void Steer(Vector2 steerInput)
		{
			if (raceControllerMode == RaceControllerMode.Launch)
			{
				raceControllerMode = RaceControllerMode.Race;
			}
			else
			{
				if (mode != Mode.PhysicsDriven)
				{
					return;
				}
				if (raceControllerMode == RaceControllerMode.Countdown)
				{
					steerVel = lateralRotation * (steerInput.x * trackDir * myMutableData.LateralThrustScale);
					steerVel += myMutableData.ConstantForwardThrust * interpolatedTrackDir;
					steerVel *= (isFloatingOnWater ? myMutableData.ImpulseScaleOnWater : myMutableData.ImpulseScale);
				}
				else if (firstTrackDir)
				{
					steerVel = lateralRotation * (steerInput.x * trackDir * myMutableData.LateralThrustScale);
					if (pilotBody.velocity.sqrMagnitude < constantForwardThrustThresholdSquared)
					{
						steerVel += myMutableData.ConstantForwardThrust * interpolatedTrackDir;
					}
					steerVel *= (isFloatingOnWater ? myMutableData.ImpulseScaleOnWater : myMutableData.ImpulseScale);
				}
				else
				{
					steerVel = myMutableData.ConstantForwardThrust * trackDir;
					steerVel *= (isFloatingOnWater ? myMutableData.ImpulseScaleOnWater : myMutableData.ImpulseScale);
				}
				base.Broadcaster.BroadcastOnStickDirectionEvent(steerInput);
				base.Broadcaster.BroadcastOnSteerDirectionEvent(steerVel);
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!(pilotBody == null))
			{
				if (base.IsSliding && mode == Mode.PhysicsDriven)
				{
					pilotBody.velocity += steerVel;
				}
				Vector3 velocity = pilotBody.velocity;
				if (velocity.y < TERMINAL_VELOCITY)
				{
					velocity.y = TERMINAL_VELOCITY;
					pilotBody.velocity = velocity;
				}
			}
		}

		protected void Update()
		{
			if (isLocalPlayer && delayUI > 0)
			{
				setControlsEnabled(false);
				if (--delayUI == 1)
				{
				}
				if (--delayUI == 0)
				{
					raceGameController.StartRace();
					Service.Get<QuestService>().SendEvent(string.Format("Start{0}Race", raceGameController.TrackId));
				}
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (raceControllerMode == RaceControllerMode.Countdown || !isLocalPlayer)
			{
				return;
			}
			if (chaseCamera != null)
			{
				if (chaseCamera.enabled && trackDir != chaseCamera.TrackDir && chaseCamera.TrackDir != Vector3.zero && Vector3.Dot(thisTransform.position - chaseCamera.TrackPos, chaseCamera.TrackDir) >= 0f)
				{
					prevTrackDir = trackDir;
					trackDir = chaseCamera.TrackDir;
					trackDirInterpolationTime = 0f;
					if (!firstTrackDir)
					{
						firstTrackDir = true;
						prevTrackDir = trackDir;
						trackDirInterpolationTime = 1f;
						raceGameController.ActivateGate();
					}
				}
				if (interpolatedMaxSpeed != maxSpeed)
				{
					maxSpeedInterpolationTime += Time.deltaTime;
					interpolatedMaxSpeed = Mathf.Lerp(prevMaxSpeed, maxSpeed, maxSpeedInterpolationTime * 0.75f);
					prevMaxSpeed = interpolatedMaxSpeed;
				}
				if (interpolatedFixedSpeed != fixedSpeed)
				{
					fixedSpeedInterpolationTime += Time.deltaTime;
					interpolatedFixedSpeed = Mathf.Lerp(prevFixedSpeed, fixedSpeed, fixedSpeedInterpolationTime * 0.75f);
				}
				if (pilotBody != null)
				{
					Vector3 vector = pilotBody.velocity;
					vector.y = 0f;
					vector = ((fixedSpeed == 0f) ? Vector3.ClampMagnitude(vector, maxSpeed) : (vector.normalized * interpolatedFixedSpeed));
					vector.y = pilotBody.velocity.y;
					pilotBody.velocity = vector;
				}
				if (firstTrackDir)
				{
					trackDirInterpolationTime += Time.deltaTime;
					interpolatedTrackDir = Vector3.Lerp(prevTrackDir, trackDir, trackDirInterpolationTime);
					interpolatedTrackDir.Normalize();
					if (visualizeTrackSegment)
					{
						trackDirRenderer.material.color = Color.black;
						trackDirRenderer.positionCount = 2;
						trackDirRenderer.startWidth = 0.05f;
						trackDirRenderer.endWidth = 0.05f;
						trackDirRenderer.SetPosition(0, base.gameObject.transform.position);
						trackDirRenderer.SetPosition(1, base.gameObject.transform.position + interpolatedTrackDir * 10f);
						Vector3 a = lateralRotation * trackDir;
						steeringObjectRenderer.material.color = Color.red;
						steeringObjectRenderer.positionCount = 2;
						steeringObjectRenderer.startWidth = 0.05f;
						steeringObjectRenderer.endWidth = 0.05f;
						steeringObjectRenderer.SetPosition(0, base.gameObject.transform.position - a * 3f);
						steeringObjectRenderer.SetPosition(1, base.gameObject.transform.position + a * 3f);
					}
				}
			}
			else
			{
				chaseCamera = ClubPenguin.Core.SceneRefs.Get<ChaseCamera>();
			}
		}

		public void setControlsEnabled(bool enabled)
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			if (enabled)
			{
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ChatButtons"));
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
				return;
			}
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ChatButtons"));
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("CellphoneButton"));
			StateMachineContext component = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponent<StateMachineContext>();
			if (component != null)
			{
				component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
			}
		}

		private bool onOutOfBoundsReset(OutOfBoundsWarperEvents.ResetPlayer evt)
		{
			if (base.gameObject.activeSelf && chaseCamera != null)
			{
				chaseCamera.Disable();
				setControlsEnabled(true);
				RaceGameController component = evt.Player.GetComponent<RaceGameController>();
				if (component != null)
				{
					component.RemoveLocalPlayerRaceData();
					Object.Destroy(component);
				}
			}
			return false;
		}
	}
}
