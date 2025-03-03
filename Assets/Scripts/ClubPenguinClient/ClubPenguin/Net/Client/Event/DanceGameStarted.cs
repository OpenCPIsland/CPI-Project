using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct DanceGameStarted
	{
		public long gameId;

		public string challenger;

		public string challengee;
	}
}
