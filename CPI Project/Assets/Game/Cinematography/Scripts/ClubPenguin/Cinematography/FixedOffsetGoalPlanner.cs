using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class FixedOffsetGoalPlanner : GoalPlanner
	{
		public enum Scopes
		{
			Global,
			Local,
			FocusLocal
		}

		public Vector3 Offset;

		public Scopes Scope;

		public Vector3 ScopedOffset
		{
			get;
			private set;
		}

		public override void Plan(ref Setup setup)
		{
			switch (Scope)
			{
			case Scopes.Local:
				ScopedOffset = base.transform.TransformDirection(Offset);
				break;
			case Scopes.FocusLocal:
				ScopedOffset = setup.Focus.TransformDirection(Offset);
				break;
			default:
				ScopedOffset = Offset;
				break;
			}
			setup.Goal = setup.Focus.position + ScopedOffset;
		}
	}
}
