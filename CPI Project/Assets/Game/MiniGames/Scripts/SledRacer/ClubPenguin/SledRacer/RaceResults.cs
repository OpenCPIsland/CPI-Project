namespace ClubPenguin.SledRacer
{
	public class RaceResults
	{
		public enum RaceResultsCategory
		{
			Incomplete,
			Bronze,
			Silver,
			Gold,
			Legendary
		}

		public string trackId;

		public long StartTime;

		public long CompletionTime;

		public RaceResultsCategory raceResultsCategory;
	}
}
