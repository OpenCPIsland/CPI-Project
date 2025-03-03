using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class MaxDistanceConstraint : Constraint
	{
		public float Distance;

		public override void Apply(ref Setup setup)
		{
			float magnitude = (setup.Goal - setup.Focus.position).magnitude;
			if (magnitude > Distance)
			{
				float t = Distance / magnitude;
				setup.Goal = Vector3.Lerp(setup.Focus.position, setup.Goal, t);
			}
		}
	}
}
