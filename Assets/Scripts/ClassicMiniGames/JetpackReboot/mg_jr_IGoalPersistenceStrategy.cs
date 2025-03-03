namespace JetpackReboot
{
	public interface mg_jr_IGoalPersistenceStrategy
	{
		void SaveGoalProgressFor(mg_jr_Goal _goal);

		int LoadLevelFor(mg_jr_Goal _goal);

		float LoadProgresFor(mg_jr_Goal _goal);
	}
}
