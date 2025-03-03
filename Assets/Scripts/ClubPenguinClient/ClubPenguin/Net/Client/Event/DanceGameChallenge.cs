using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct DanceGameChallenge
	{
		public long gameId;

		public string challenger;

		public string challengee;

		public string moves;
	}
}
