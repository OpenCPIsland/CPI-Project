namespace ClubPenguin.PartyGames
{
	public class PartyGameEvents
	{
		public struct PartyGameStarted
		{
			public PartyGameDefinition Definition;

			public PartyGameStarted(PartyGameDefinition definition)
			{
				Definition = definition;
			}
		}

		public struct PartyGameEnded
		{
			public PartyGameDefinition Definition;

			public PartyGameEnded(PartyGameDefinition definition)
			{
				Definition = definition;
			}
		}
	}
}
