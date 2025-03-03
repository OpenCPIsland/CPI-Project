using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct DanceGameEnded
	{
		public long gameId;

		public string winner;

		public int winnerScore;

		public string loser;

		public int loserScore;
	}
}
