namespace ClubPenguin.Cinematography
{
	public class FixedPositionGoalPlanner : GoalPlanner
	{
		public override void Plan(ref Setup setup)
		{
			setup.Goal = base.transform.position;
		}
	}
}
