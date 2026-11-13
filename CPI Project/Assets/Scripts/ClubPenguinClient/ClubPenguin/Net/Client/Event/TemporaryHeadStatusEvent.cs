using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct TemporaryHeadStatusEvent
	{
		public long SessionId;

		public int Type;
	}
}
