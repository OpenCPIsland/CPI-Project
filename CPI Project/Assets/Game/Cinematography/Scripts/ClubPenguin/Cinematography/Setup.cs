using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public struct Setup
	{
		public readonly Transform Focus;

		public readonly Transform Camera;

		public readonly Vector3 FocusVelocity;

		public Vector3 UnconstrainedGoal;

		public Vector3 Goal;

		public Quaternion Glance;

		public Vector3 LookAt;

		public bool IsAxisLocked;

		public Vector3 LockedAxis;

		public Vector3 LockedAxisValues;

		public Setup(Setup other)
			: this(other.Focus, other.Camera, other.FocusVelocity)
		{
		}

		public Setup(Transform focus, Transform camera, Vector3 focusVelocity)
		{
			Focus = focus;
			Camera = camera;
			FocusVelocity = focusVelocity;
			Goal = camera.position;
			UnconstrainedGoal = camera.position;
			LookAt = focus.position;
			Glance = Quaternion.identity;
			IsAxisLocked = false;
			LockedAxis = Vector3.zero;
			LockedAxisValues = Vector3.zero;
		}

		public override string ToString()
		{
			return string.Format("[Setup] Focus {0}, Camera {1}, FocusVelocity {2}, UnconstrainedGoal {3}, Goal {4}, Glance {5}, LookAt {6}", Focus, Camera, FocusVelocity, UnconstrainedGoal, Goal, Glance, LookAt);
		}
	}
}
