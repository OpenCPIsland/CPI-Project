using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct DispensableEvent
	{
		public long SessionId;

		public string Type;
	}
}
