using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct MembershipRights
	{
		public string swid;

		public bool member;

		public bool recurring;

		public long recurDate;

		public long expireDate;
	}
}
