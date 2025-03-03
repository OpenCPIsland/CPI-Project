namespace ClubPenguin.Cinematography
{
	public class HeightOffsetGoalPlanner : GoalPlanner
	{
		public float Height;

		public override void Plan(ref Setup setup)
		{
			setup.Goal.y = setup.Focus.position.y + Height;
		}
	}
}
