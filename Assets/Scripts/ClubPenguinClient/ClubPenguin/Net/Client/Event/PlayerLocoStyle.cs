using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PlayerLocoStyle
	{
		public enum Style : byte
		{
			Walk = 1,
			Run
		}

		public long sessionId;

		public Style style;
	}
}
