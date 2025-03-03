using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class DialogFramer : Framer
	{
		public Transform Target;

		public float Weight = 0.75f;

		public Vector3 Offset;

		public override void Aim(ref Setup setup)
		{
			Vector3 position = setup.Focus.position;
			Vector3 position2 = Target.position;
			setup.LookAt = Vector3.Lerp(position, position2, Weight) + Offset;
		}
	}
}
