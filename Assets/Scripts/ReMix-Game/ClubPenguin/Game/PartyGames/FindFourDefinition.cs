using ClubPenguin.PartyGames;
using Newtonsoft.Json;
using System;

namespace ClubPenguin.Game.PartyGames
{
	[Serializable]
	[JsonConverter(typeof(PartyGameDataDefinitionJsonConverter))]
	public class FindFourDefinition : PartyGameDataDefinition
	{
		public int IntroTimeInSeconds;

		public int OutroTimeInSeconds;

		public int TurnTimeInSeconds;

		public int ChipPlaceTimeInSeconds;

		public int PlayerProp;

		public int SequenceCountToWin;

		public int GameBoardWidth;

		public int GameBoardHeight;

		public int DefaultColumn = 0;
	}
}
