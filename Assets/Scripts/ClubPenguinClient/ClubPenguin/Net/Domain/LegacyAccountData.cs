using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class LegacyAccountData
	{
		public string username;

		public bool member;

		public long createdDate;
	}
}
