namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleScoreData
	{
		public float Team1Score;

		public float Team2Score;

		public long StartTimeInSeconds;

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			DanceBattleScoreData danceBattleScoreData = other as DanceBattleScoreData;
			if (danceBattleScoreData == null)
			{
				return false;
			}
			return danceBattleScoreData.StartTimeInSeconds == StartTimeInSeconds;
		}

		public override int GetHashCode()
		{
			return (int)StartTimeInSeconds;
		}
	}
}
