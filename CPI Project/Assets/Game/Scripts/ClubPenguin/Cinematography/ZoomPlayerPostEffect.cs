using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class ZoomPlayerPostEffect : MonoBehaviour
	{
		[Serializable]
		public struct SwitchInfo
		{
			public Switch ActivationSwitch;

			public float ZoomPercentOnMove;

			public float ZoomPercentOnIdle;

			public float HeightOffset;

			public float ZoomOutDelay;

			public float MinDist;
		}

		private static readonly float activationDelayTime = 1f;

		public AnimationCurve Curve = new AnimationCurve();

		public float Duration = 1f;

		public SwitchInfo[] Switches = new SwitchInfo[0];

		private GameObject localPlayer;

		private Animator anim;

		private bool isZoomActive;

		private bool isSuspended;

		private float startPercent;

		private float curPercent;

		private float desiredPercent;

		private float targetDesiredPercent;

		private float startHeightOffset;

		private float curHeightOffset;

		private float desiredHeightOffset;

		private float targetDesiredHeightOffset;

		private float elapsedZoomOutDelay;

		private float zoomOutDelay;

		private bool zoomOutDelayActive;

		private float elapsedTime;

		private float desiredDuration;

		private EventDispatcher dispatcher;

		private bool useCustomZoom;

		private bool wasCustomZoom;

		private SwitchInfo customZoom;

		private Quaternion lastKnownUnzoomedRotation;

		private Quaternion lastKnownZoomedRotation;

		private float activationDelay;

		private bool isPlayerSpawned;

		private CameraController previousController;

		private ConsumingPropSwitch consumingPropSwitch;

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			isPlayerSpawned = false;
			activationDelay = activationDelayTime;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (!localPlayerHandle.IsNull && cPDataEntityCollection.HasComponent<PresenceData>(localPlayerHandle))
			{
				isPlayerSpawned = true;
			}
			else
			{
				dispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			}
		}

		public void Start()
		{
			setup();
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			isPlayerSpawned = true;
			dispatcher.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			return false;
		}

		public void setup()
		{
			if (localPlayer == null)
			{
				localPlayer = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				anim = localPlayer.GetComponent<Animator>();
				reset();
			}
		}

		private void reset()
		{
			if (Duration <= 0f)
			{
				Duration = 1f;
			}
			elapsedTime = 0f;
			startPercent = 0f;
			curPercent = 0f;
			desiredPercent = 0f;
			startHeightOffset = 0f;
			curHeightOffset = 0f;
			desiredHeightOffset = 0f;
			zoomOutDelay = 0f;
			elapsedZoomOutDelay = 0f;
			zoomOutDelayActive = false;
			useCustomZoom = false;
			wasCustomZoom = false;
			targetDesiredPercent = 0f;
			lastKnownUnzoomedRotation = base.transform.rotation;
			lastKnownZoomedRotation = base.transform.rotation;
		}

		public void OnEnable()
		{
			dispatcher.AddListener<CinematographyEvents.CameraLogicChangeEvent>(onCameraLogicChangeEvent);
			dispatcher.AddListener<CinematographyEvents.CameraLogicResetEvent>(onCameraLogicResetEvent);
			dispatcher.AddListener<CinematographyEvents.ZoomCameraEvent>(onZoomCameraEvent);
		}

		public void OnDisable()
		{
			dispatcher.RemoveListener<CinematographyEvents.CameraLogicChangeEvent>(onCameraLogicChangeEvent);
			dispatcher.RemoveListener<CinematographyEvents.CameraLogicResetEvent>(onCameraLogicResetEvent);
			dispatcher.RemoveListener<CinematographyEvents.ZoomCameraEvent>(onZoomCameraEvent);
		}

		private bool onCameraLogicChangeEvent(CinematographyEvents.CameraLogicChangeEvent evt)
		{
			isSuspended |= evt.DisablePostEffects;
			if (isSuspended)
			{
				reset();
			}
			return false;
		}

		private bool onCameraLogicResetEvent(CinematographyEvents.CameraLogicResetEvent evt)
		{
			isSuspended = false;
			return false;
		}

		private bool onZoomCameraEvent(CinematographyEvents.ZoomCameraEvent evt)
		{
			bool flag = false;
			Director director = ClubPenguin.Core.SceneRefs.Get<Director>();
			if (director != null)
			{
				flag = director.InCinematicContext;
			}
			if (!flag)
			{
				useCustomZoom = evt.State;
				if (useCustomZoom)
				{
					customZoom.ZoomOutDelay = evt.ZoomOutDelay;
					customZoom.ZoomPercentOnIdle = evt.ZoomPercentOnIdle;
					customZoom.ZoomPercentOnMove = evt.ZoomPercentOnMove;
					customZoom.HeightOffset = evt.HeightOffset;
					customZoom.MinDist = evt.MinDist;
				}
				else
				{
					wasCustomZoom = true;
				}
			}
			return false;
		}

		private bool isCameraInValidState()
		{
			CameraController topmostCameraController = ClubPenguin.Core.SceneRefs.Get<BaseCamera>().GetTopmostCameraController();
			if (topmostCameraController != previousController)
			{
				consumingPropSwitch = ((topmostCameraController != null) ? topmostCameraController.GetComponent<ConsumingPropSwitch>() : null);
				previousController = topmostCameraController;
			}
			return consumingPropSwitch == null;
		}

		private void updateDesiredPercent()
		{
			int num = -1;
			bool flag = isZoomActive;
			isZoomActive = false;
			if (!isSuspended && isCameraInValidState())
			{
				if (useCustomZoom)
				{
					isZoomActive = true;
				}
				else
				{
					for (int i = 0; i < Switches.Length; i++)
					{
						if (Switches[i].ActivationSwitch != null && Switches[i].ActivationSwitch.enabled && Switches[i].ActivationSwitch.OnOff)
						{
							isZoomActive = true;
							num = i;
							break;
						}
					}
				}
			}
			if (isZoomActive)
			{
				AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(AnimationHashes.Layers.Base);
				if (LocomotionUtils.IsLocomoting(currentAnimatorStateInfo) || LocomotionHelper.IsCurrentControllerOfType<SlideController>(localPlayer) || (LocomotionHelper.IsCurrentControllerOfType<SwimController>(localPlayer) && !LocomotionUtils.IsSwimmingIdle(currentAnimatorStateInfo)))
				{
					targetDesiredPercent = (useCustomZoom ? customZoom.ZoomPercentOnMove : Switches[num].ZoomPercentOnMove);
				}
				else
				{
					targetDesiredPercent = (useCustomZoom ? customZoom.ZoomPercentOnIdle : Switches[num].ZoomPercentOnIdle);
				}
				if (useCustomZoom)
				{
					zoomOutDelay = customZoom.ZoomOutDelay;
					targetDesiredHeightOffset = customZoom.HeightOffset;
				}
				else
				{
					zoomOutDelay = Switches[num].ZoomOutDelay;
					targetDesiredHeightOffset = Switches[num].HeightOffset;
				}
				zoomOutDelayActive = false;
			}
			else if (flag && zoomOutDelay > 0f)
			{
				zoomOutDelayActive = true;
				elapsedZoomOutDelay = 0f;
			}
			else if (!zoomOutDelayActive)
			{
				targetDesiredPercent = 0f;
			}
			if (zoomOutDelayActive)
			{
				elapsedZoomOutDelay += Time.deltaTime;
				if (elapsedZoomOutDelay >= zoomOutDelay)
				{
					elapsedZoomOutDelay = 0f;
					zoomOutDelayActive = false;
					targetDesiredPercent = 0f;
				}
			}
			if (targetDesiredPercent != desiredPercent || targetDesiredHeightOffset != desiredHeightOffset)
			{
				elapsedTime = 0f;
				startPercent = curPercent;
				desiredPercent = targetDesiredPercent;
				startHeightOffset = curHeightOffset;
				desiredHeightOffset = targetDesiredHeightOffset;
			}
			elapsedTime += Time.deltaTime;
			if (elapsedTime > Duration)
			{
				elapsedTime = Duration;
			}
		}

		public void LateUpdate()
		{
			if (!isPlayerSpawned)
			{
				return;
			}
			activationDelay -= Time.deltaTime;
			if (activationDelay > 0f)
			{
				return;
			}
			setup();
			updateDesiredPercent();
			float num = Curve.Evaluate(elapsedTime / Duration);
			curPercent = Mathf.Lerp(startPercent, desiredPercent, num);
			curHeightOffset = Mathf.Lerp(startHeightOffset, desiredHeightOffset, num);
			Vector3 position = localPlayer.transform.position;
			position.y += curHeightOffset;
			base.transform.position = Vector3.Lerp(base.transform.parent.position, position, curPercent);
			if (useCustomZoom || wasCustomZoom)
			{
				Quaternion b = Quaternion.LookRotation(localPlayer.transform.position - base.transform.position);
				if (useCustomZoom)
				{
					base.transform.rotation = Quaternion.Lerp(lastKnownUnzoomedRotation, b, num);
					lastKnownZoomedRotation = base.transform.rotation;
					return;
				}
				base.transform.rotation = Quaternion.Lerp(lastKnownZoomedRotation, b, num);
				lastKnownUnzoomedRotation = base.transform.rotation;
				if (num >= 1f)
				{
					wasCustomZoom = false;
				}
			}
			else
			{
				lastKnownUnzoomedRotation = base.transform.rotation;
			}
		}
	}
}
