using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct MembershipRightsRefresh
	{
		public bool isMember;

		public long expireDate;

		public string vendor;

		public string productId;
	}
}
