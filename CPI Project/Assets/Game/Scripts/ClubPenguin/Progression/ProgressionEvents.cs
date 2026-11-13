namespace ClubPenguin.Progression
{
	public static class ProgressionEvents
	{
		public struct LevelUp
		{
			public readonly string MascotName;

			public readonly int MascotLevel;

			public readonly int OverallLevel;

			public LevelUp(string mascotName, int mascotLevel, int overallLevel)
			{
				MascotName = mascotName;
				MascotLevel = mascotLevel;
				OverallLevel = overallLevel;
			}
		}
	}
}
