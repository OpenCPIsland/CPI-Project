namespace ClubPenguin.Game.PartyGames
{
	public static class FindFourEvents
	{
		public struct ColorChanged
		{
			public readonly FindFour.FindFourTokenColor Color;

			public ColorChanged(FindFour.FindFourTokenColor color)
			{
				Color = color;
			}
		}
	}
}
