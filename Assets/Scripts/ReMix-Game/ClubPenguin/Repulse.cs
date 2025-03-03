using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin
{
	public class Repulse : MonoBehaviour
	{
		public float Magnitude;

		public void OnCollisionEnter(Collision collision)
		{
			OnTriggerEnter(collision.collider);
		}

		public void OnTriggerEnter(Collider collider)
		{
			Vector3 vector = (collider.transform.position - base.transform.position).normalized * Magnitude;
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody != null)
			{
				attachedRigidbody.AddForce(vector, ForceMode.VelocityChange);
				return;
			}
			LocomotionController currentController = LocomotionHelper.GetCurrentController(collider.gameObject);
			if (currentController != null)
			{
				currentController.SetForce(vector, base.gameObject);
			}
		}
	}
}
