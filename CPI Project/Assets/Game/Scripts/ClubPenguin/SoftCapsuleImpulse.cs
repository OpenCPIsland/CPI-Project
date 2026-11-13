using ClubPenguin.Locomotion;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(CapsuleCollider))]
	public class SoftCapsuleImpulse : MonoBehaviour
	{
		public float Magnitude;

		[Tooltip("Only apply impulses when actor isn't locomoting on land.")]
		public bool IgnoreIfRunController;

		private CapsuleCollider capsCollider;

		private List<Collider> colliders;

		public void Awake()
		{
			colliders = new List<Collider>();
			capsCollider = GetComponent<CapsuleCollider>();
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
				if (collider == null)
				{
					colliders.RemoveAt(i);
					i--;
					continue;
				}
				Vector3 direction = Vector3.up;
				if (capsCollider.direction == 0)
				{
					direction = Vector3.right;
				}
				else if (capsCollider.direction == 2)
				{
					direction = Vector3.forward;
				}
				direction = base.transform.TransformDirection(direction);
				float num = capsCollider.height;
				float radius = capsCollider.radius;
				if (num < radius * 2f)
				{
					num = radius * 2f;
				}
				Vector3 a = base.transform.TransformPoint(capsCollider.center);
				Vector3 vector = a - direction * (num * 0.5f);
				Vector3 vector2 = vector + direction * Mathf.Max(0f, num - radius);
				Vector3 position = collider.transform.position;
				Vector3 vector3 = position - vector;
				float magnitude = Vector3.Cross(direction, vector3).magnitude;
				float num2 = Vector3.Dot(Vector3.Project(vector3 + vector, direction) - vector2, direction);
				float num3 = 0f;
				if (num2 > 0f)
				{
					Debug.DrawLine(position, vector2, Color.yellow);
					num3 = (position - vector2).magnitude / radius;
				}
				else
				{
					Debug.DrawLine(position, vector, Color.yellow);
					num3 = magnitude / radius;
				}
				num3 = Mathf.Clamp01(1f - num3) * Magnitude;
				Vector3 vector4 = direction * num3;
				Rigidbody attachedRigidbody = collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					attachedRigidbody.AddForce(vector4, ForceMode.Impulse);
					continue;
				}
				LocomotionController currentController = LocomotionHelper.GetCurrentController(collider.gameObject);
				if (currentController != null && (!IgnoreIfRunController || !(currentController is RunController)))
				{
					currentController.AddForce(vector4, base.gameObject);
				}
			}
		}
	}
}
