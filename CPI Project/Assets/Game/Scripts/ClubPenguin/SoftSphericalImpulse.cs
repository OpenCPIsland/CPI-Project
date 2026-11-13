using ClubPenguin.Locomotion;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class SoftSphericalImpulse : MonoBehaviour
	{
		public float Magnitude;

		public float MaxDist;

		[Tooltip("Only apply impulses when actor isn't locomoting on land.")]
		public bool IgnoreIfRunController;

		private List<Collider> colliders;

		public void Awake()
		{
			colliders = new List<Collider>();
		}

		public void OnDisable()
		{
			colliders.Clear();
		}

		public void OnTriggerEnter(Collider collider)
		{
			if (!colliders.Contains(collider))
			{
				colliders.Add(collider);
			}
		}

		public void OnTriggerStay(Collider collider)
		{
			OnTriggerEnter(collider);
		}

		public void OnTriggerExit(Collider collider)
		{
			if (colliders.Contains(collider))
			{
				colliders.Remove(collider);
			}
		}

		private void Update()
		{
			for (int i = 0; i < colliders.Count; i++)
			{
				Collider collider = colliders[i];
				Vector3 vector = collider.transform.position - base.transform.position;
				float magnitude = vector.magnitude;
				float d = Magnitude;
				if (MaxDist > 0f)
				{
					d = Mathf.Clamp01(1f - magnitude / MaxDist) * Magnitude;
				}
				Vector3 vector2 = vector.normalized * d;
				Rigidbody attachedRigidbody = collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					attachedRigidbody.AddForce(vector2, ForceMode.Impulse);
					continue;
				}
				LocomotionController currentController = LocomotionHelper.GetCurrentController(collider.gameObject);
				if (currentController != null && (!IgnoreIfRunController || !(currentController is RunController)))
				{
					currentController.AddForce(vector2, base.gameObject);
				}
			}
		}
	}
}
