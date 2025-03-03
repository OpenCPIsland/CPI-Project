namespace ClubPenguin.Game.PartyGames
{
	public struct PartyGameEndGamePlayerData
	{
		public long PlayerId;

		public int Placement;

		public int PlayerNum;

		public bool IsLocalPlayer;

		public int Score;

		public bool HasScore;

		public PartyGameEndGamePlayerData(long playerId, int placement, int playerNum, bool isLocalPlayer, int score = 0, bool hasScore = false)
		{
			PlayerId = playerId;
			Placement = placement;
			PlayerNum = playerNum;
			IsLocalPlayer = isLocalPlayer;
			Score = score;
			HasScore = hasScore;
		}
	}
}
