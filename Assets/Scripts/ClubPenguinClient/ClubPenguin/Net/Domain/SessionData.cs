using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct SessionData
	{
		public long sessionId;

		public long expiriation;

		public string userName;
	}
}
