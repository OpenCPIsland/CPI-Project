namespace JetpackReboot
{
	public interface mg_jr_IActiveGoalPersistenceStrategy
	{
		void Save(mg_jr_GoalManager _goalManager);

		mg_jr_Goal.GoalType LoadGoalType(int _index);

		mg_jr_Goal.GoalDuration LoadGoalDuration(int _index);
	}
}
