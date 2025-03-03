using System;

namespace ClubPenguin.MiniGames.Fishing
{
	[Serializable]
	public class FishPatternData
	{
		public FishingFish.Rarities rarity;

		public float speed = 1f;

		public bool isEmpty = true;
	}
}
