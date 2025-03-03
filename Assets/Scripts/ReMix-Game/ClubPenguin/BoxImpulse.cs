using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin
{
	public class BoxImpulse : MonoBehaviour
	{
		public enum ImpulseType
		{
			SnapVelocity,
			AddVelocity
		}

		public ImpulseType Behaviour = ImpulseType.SnapVelocity;

		public Transform ZDirection;

		public float Magnitude;

		public bool EveryFrame;

		[Tooltip("Only apply impulses when actor isn't locomoting on land.")]
		public bool IgnoreIfRunController;

		public void OnCollisionEnter(Collision collision)
		{
			applyImpulse(collision.collider);
		}

		public void OnCollisionStay(Collision collision)
		{
			if (EveryFrame)
			{
				applyImpulse(collision.collider);
			}
		}

		public void OnTriggerEnter(Collider collider)
		{
			applyImpulse(collider);
		}

		public void OnTriggerStay(Collider collider)
		{
			if (EveryFrame)
			{
				applyImpulse(collider);
			}
		}

		private void applyImpulse(Collider collider)
		{
			Vector3 vector = (ZDirection != null) ? (ZDirection.forward * Magnitude) : (base.transform.forward * Magnitude);
			LocomotionController locomotionController = LocomotionHelper.GetCurrentController(collider.gameObject);
			if (collider.gameObject.layer == LayerMask.NameToLayer(LayerConstants.TubeLayer))
			{
				SlideControllerListener component = collider.GetComponent<SlideControllerListener>();
				if (component != null)
				{
					locomotionController = component.SlideController;
				}
			}
			if (locomotionController != null)
			{
				if (!IgnoreIfRunController || !(locomotionController is RunController))
				{
					if (Behaviour == ImpulseType.AddVelocity)
					{
						locomotionController.AddForce(vector * Time.fixedDeltaTime, base.gameObject);
					}
					else
					{
						locomotionController.SetForce(vector, base.gameObject);
					}
				}
				return;
			}
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody != null)
			{
				if (Behaviour == ImpulseType.SnapVelocity)
				{
					attachedRigidbody.velocity = vector;
				}
				else
				{
					attachedRigidbody.AddForce(vector, ForceMode.VelocityChange);
				}
			}
		}
	}
}
