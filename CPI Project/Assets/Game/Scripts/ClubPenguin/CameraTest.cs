using ClubPenguin.Cinematography;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class CameraTest : MonoBehaviour
	{
		private BaseCamera baseCamera;

		private bool lastFramePassed = true;

		public void Start()
		{
			base.enabled = false;
		}

		public void Update()
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			if (!baseCamera.enabled)
			{
				if (lastFramePassed)
				{
					lastFramePassed = false;
					eventDispatcher.DispatchEvent(new ApplicationService.Error("CameraError", "Camera is not enabled!"));
				}
				return;
			}
			if (baseCamera.debug_ControllerCount < 1)
			{
				if (lastFramePassed)
				{
					lastFramePassed = false;
					eventDispatcher.DispatchEvent(new ApplicationService.Error("CameraError", "Camera has no controllers!"));
				}
				return;
			}
			if (baseCamera.Target == null)
			{
				if (lastFramePassed)
				{
					lastFramePassed = false;
					eventDispatcher.DispatchEvent(new ApplicationService.Error("CameraError", "Camera has no target!"));
				}
				return;
			}
			CameraController component = baseCamera.GetComponent<CameraController>();
			CameraController debug_FirstController = baseCamera.debug_FirstController;
			if (component != debug_FirstController)
			{
				if (lastFramePassed)
				{
					lastFramePassed = false;
					eventDispatcher.DispatchEvent(new ApplicationService.Error("CameraError", "Camera's first controller is not default controller!\n" + debug_FirstController));
				}
			}
			else
			{
				lastFramePassed = true;
			}
		}
	}
}
