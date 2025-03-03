using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct ConsumableEvent
	{
		public long SessionId;

		public string Type;
	}
}
