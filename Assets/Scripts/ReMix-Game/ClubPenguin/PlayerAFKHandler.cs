using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(LocomotionTracker))]
	public class PlayerAFKHandler : MonoBehaviour
	{
		private LocomotionTracker locomotionTracker;

		private void Awake()
		{
			locomotionTracker = GetComponent<LocomotionTracker>();
		}

		private void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<AwayFromKeyboardEvent>(onAFKEvent);
		}

		private void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<AwayFromKeyboardEvent>(onAFKEvent);
		}

		private bool onAFKEvent(AwayFromKeyboardEvent evt)
		{
			if (evt.FaceCamera)
			{
				locomotionTracker.GetCurrentController().SteerRotation(Vector2.down);
			}
			LocomotionEventBroadcaster component = GetComponent<LocomotionEventBroadcaster>();
			if (component != null)
			{
				component.BroadcastOnSteerRotationFlushEvent();
			}
			Service.Get<INetworkServicesManager>().PlayerStateService.SetAwayFromKeyboard((int)evt.Type);
			return false;
		}
	}
}
