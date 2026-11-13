using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[RequireComponent(typeof(CameraController))]
	public class CameraTrigger : MonoBehaviour
	{
		private EventDispatcher dispatcher;

		private CameraController controller;

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			controller = GetComponent<CameraController>();
		}

		public void OnTriggerEnter(Collider col)
		{
			CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
			evt.Controller = controller;
			dispatcher.DispatchEvent(evt);
		}

		public void OnTriggerExit(Collider col)
		{
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			evt.Controller = controller;
			dispatcher.DispatchEvent(evt);
		}
	}
}
