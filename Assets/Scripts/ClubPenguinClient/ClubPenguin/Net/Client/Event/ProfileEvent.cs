using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct ProfileEvent
	{
		public long SessionId;

		public Profile Profile;
	}
}
