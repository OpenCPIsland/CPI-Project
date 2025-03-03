using ClubPenguin.PartyGames;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameSessionFactory : IPartyGameSessionFactory
	{
		public IPartyGameSession getPartyGameSession(PartyGameDefinition.GameTypes type)
		{
			switch (type)
			{
			case PartyGameDefinition.GameTypes.SCAVENGER_HUNT:
				return new ScavengerHunt();
			case PartyGameDefinition.GameTypes.FISH_BUCKET:
				return new FishBucket();
			case PartyGameDefinition.GameTypes.FIND_FOUR:
				return new FindFour();
			case PartyGameDefinition.GameTypes.DANCE_BATTLE:
				return new DanceBattle();
			case PartyGameDefinition.GameTypes.TUBE_RACE_RED:
			case PartyGameDefinition.GameTypes.TUBE_RACE_BLUE:
				return new TubeRacePartyGameSession();
			default:
				return new ScavengerHunt();
			}
		}
	}
}
