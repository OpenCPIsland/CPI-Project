using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct HeldObjectEvent
	{
		public long SessionId;

		public string Type;
	}
}
