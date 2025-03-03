using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class OutOfBoundsWarper : MonoBehaviour
	{
		[Tooltip("Show the out of bounds limit in the scene view in the editor. Currently red.")]
		public bool DebugDrawBounds = false;

		public Bounds Limits;

		[Tooltip("Explicitly set the bounds using a collider. This component will destroy the collider at runtime on startup")]
		public Collider OutOfBoundsCollider;

		private Quaternion startingRotation;

		private Vector3 startingPosition;

		public Vector3 StartingPosition
		{
			get
			{
				return startingPosition;
			}
		}

		public void Start()
		{
			if (OutOfBoundsCollider != null)
			{
				Limits.Encapsulate(OutOfBoundsCollider.bounds);
				Object.Destroy(OutOfBoundsCollider);
			}
			else if (Limits.extents == Vector3.zero)
			{
				Renderer[] array = Object.FindObjectsOfType<Renderer>();
				foreach (Renderer renderer in array)
				{
					Limits.Encapsulate(renderer.bounds);
				}
			}
			startingRotation = base.transform.rotation;
			startingPosition = base.transform.position;
			startingPosition.y += 1f;
		}

		public void Update()
		{
			if (!Limits.Contains(base.transform.position))
			{
				ResetPlayer();
			}
		}

		public void ResetPlayer()
		{
			LocomotionTracker component = GetComponent<LocomotionTracker>();
			if (component != null)
			{
				component.SetCurrentController<RunController>();
			}
			base.transform.position = startingPosition;
			base.transform.rotation = startingRotation;
			Service.Get<EventDispatcher>().DispatchEvent(new OutOfBoundsWarperEvents.ResetPlayer(base.gameObject));
		}

		private void OnDrawGizmos()
		{
			if (DebugDrawBounds)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawCube(Limits.center, Limits.size);
			}
		}
	}
}
