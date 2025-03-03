using UnityEngine;

namespace ClubPenguin
{
	public class RotateTransform : ProximityBroadcaster
	{
		[Header("Axis to rotate on.")]
		[Tooltip("If you do not wish it to rotate on an axis make the speed 0.")]
		public float XAxisRotationSpeed = 0f;

		[Tooltip("If you do not wish it to rotate on an axis make the speed 0.")]
		public float YAxisRotationSpeed = 0f;

		[Tooltip("If you do not wish it to rotate on an axis make the speed 0.")]
		public float ZAxisRotationSpeed = 0f;

		public bool isActive = false;

		private void Update()
		{
			if (isActive)
			{
				base.transform.Rotate(XAxisRotationSpeed * Time.deltaTime, YAxisRotationSpeed * Time.deltaTime, ZAxisRotationSpeed * Time.deltaTime);
			}
		}

		public override void OnProximityEnter(ProximityListener other)
		{
			isActive = true;
		}

		public override void OnProximityExit(ProximityListener other)
		{
			isActive = false;
		}
	}
}
