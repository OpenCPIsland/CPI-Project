using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class MinDistanceConstraint : Constraint
	{
		public float Distance = 1f;

		public bool XZ = true;

		public override void Apply(ref Setup setup)
		{
			Vector3 a = setup.Goal - setup.Focus.position;
			if (XZ)
			{
				a.y = 0f;
			}
			float magnitude = a.magnitude;
			if (magnitude < Distance)
			{
				float d = Distance / magnitude;
				setup.Goal = setup.Focus.position + a * d;
			}
		}
	}
}
