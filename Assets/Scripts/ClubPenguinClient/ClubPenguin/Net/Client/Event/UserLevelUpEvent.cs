using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct UserLevelUpEvent
	{
		public long SessionId;

		public int Level;
	}
}
