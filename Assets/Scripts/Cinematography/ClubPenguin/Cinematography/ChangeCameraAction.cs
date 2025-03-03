using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Cinematography")]
	public class ChangeCameraAction : FsmStateAction
	{
		private static readonly int defaultAdventureCameraPriority = 10;

		[RequiredField]
		public string ControllerName;

		public string TargetName;

		public FsmVector3 StoreOriginalCameraPosition;

		public FsmVector3 CameraPosition;

		[UnityEngine.Tooltip("By default, all adventure cameras are set to high priority (10). Check this box to use the camera's pre-defined priority instead.")]
		public bool PreserveCameraPriority;

		[UnityEngine.Tooltip("Most cameras are considered finished immediately.  Dolly cameras are only finished after they have completed their motion.")]
		public bool WaitForCameraToComplete = false;

		public bool ResetOnExit;

		public bool ForceCutTransition;

		private EventDispatcher dispatcher;

		private CameraController controller;

		private GameObject controllerGameObject;

		private int originalCameraPriority;

		[ActionSection("Platform Specific Settings")]
		public ChangeCameraActionSettings[] OverrideSettings;

		public override void OnEnter()
		{
			ChangeCameraActionSettings changeCameraActionSettings = PlatformUtils.FindAspectRatioSettings(OverrideSettings);
			if (changeCameraActionSettings != null)
			{
				applySettings(changeCameraActionSettings);
			}
			dispatcher = Service.Get<EventDispatcher>();
			controllerGameObject = GameObject.Find(ControllerName);
			if ((bool)controllerGameObject)
			{
				controller = controllerGameObject.GetComponent<CameraController>();
			}
			else
			{
				Disney.LaunchPadFramework.Log.LogError(this, "Unable to find Camera Setup called " + ControllerName);
			}
			if (controller == null)
			{
				Disney.LaunchPadFramework.Log.LogError(this, "Provided GameObject " + ControllerName + " does not contain a controller, but one is required.");
				Finish();
			}
			else if (!WaitForCameraToComplete && ResetOnExit)
			{
				Finish();
			}
			else
			{
				CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
				originalCameraPriority = controller.Priority;
				if (!PreserveCameraPriority)
				{
					controller.Priority = defaultAdventureCameraPriority;
				}
				controller.IsScripted = true;
				evt.Controller = controller;
				evt.DisablePostEffects = true;
				evt.ForceCutTransition = ForceCutTransition;
				dispatcher.DispatchEvent(evt);
			}
			if (StoreOriginalCameraPosition != null && StoreOriginalCameraPosition.Value != Vector3.zero)
			{
				StoreOriginalCameraPosition.Value = controllerGameObject.transform.position;
			}
			if (CameraPosition != null && CameraPosition.Value != Vector3.zero)
			{
				controllerGameObject.transform.position = CameraPosition.Value;
			}
			if (!string.IsNullOrEmpty(TargetName))
			{
				GameObject gameObject = GameObject.Find(TargetName);
				if (gameObject != null)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.ChangeCameraTarget(gameObject.transform));
				}
			}
			if (!WaitForCameraToComplete)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (WaitForCameraToComplete && controller.IsFinished)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (ResetOnExit && controller != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				controller.Priority = originalCameraPriority;
				controller.IsScripted = false;
				evt.Controller = controller;
				dispatcher.DispatchEvent(evt);
			}
		}

		private void applySettings(ChangeCameraActionSettings settings)
		{
			ControllerName = settings.ControllerName;
			TargetName = settings.TargetName;
			StoreOriginalCameraPosition = settings.StoreOriginalCameraPosition;
			CameraPosition = settings.CameraPosition;
			PreserveCameraPriority = settings.PreserveCameraPriority;
			WaitForCameraToComplete = settings.WaitForCameraToComplete;
			ResetOnExit = settings.ResetOnExit;
		}
	}
}
