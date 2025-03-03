using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct DanceGameResponse
	{
		public long gameId;

		public string challenger;

		public string challengee;

		public string moves;

		public bool success;
	}
}
