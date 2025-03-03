using ClubPenguin.Cinematography;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class FixedDistanceTowardsCameraGoalPlanner : GoalPlanner
	{
		public float Distance;

		public override void Plan(ref Setup setup)
		{
			Vector3 vector = setup.Focus.position - setup.Camera.position;
			vector.y = 0f;
			float d = vector.magnitude - Distance;
			float y = setup.Goal.y;
			setup.Goal = setup.Camera.position + vector.normalized * d;
			setup.Goal.y = y;
		}
	}
}
