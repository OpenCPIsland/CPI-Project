using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PurchaseResponse
	{
		public bool success
		{
			get;
			private set;
		}

		public int responseCode
		{
			get;
			private set;
		}

		public SignedResponse<MembershipRightsRefresh> rights
		{
			get;
			private set;
		}
	}
}
