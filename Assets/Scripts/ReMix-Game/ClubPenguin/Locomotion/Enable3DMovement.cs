using ClubPenguin.Net.Locomotion;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(Collider))]
	public class Enable3DMovement : MonoBehaviour
	{
		private void OnTriggerEnter(Collider trigger)
		{
			LocomotionReceiver component = trigger.GetComponent<LocomotionReceiver>();
			if (component != null)
			{
				component.Allow3DMovement = true;
			}
		}

		private void OnTriggerExit(Collider trigger)
		{
			LocomotionReceiver component = trigger.GetComponent<LocomotionReceiver>();
			if (component != null)
			{
				component.Allow3DMovement = false;
			}
		}
	}
}
