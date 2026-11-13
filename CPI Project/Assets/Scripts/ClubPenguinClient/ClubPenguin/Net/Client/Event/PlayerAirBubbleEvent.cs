using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PlayerAirBubbleEvent
	{
		public AirBubble AirBubble;

		public long SessionId
		{
			get;
			set;
		}
	}
}
