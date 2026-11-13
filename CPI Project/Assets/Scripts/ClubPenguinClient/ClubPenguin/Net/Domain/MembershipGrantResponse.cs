using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct MembershipGrantResponse
	{
		public string swid
		{
			get;
			private set;
		}

		public bool isMember
		{
			get;
			private set;
		}

		public bool isRecurring
		{
			get;
			private set;
		}

		public long recurDate
		{
			get;
			private set;
		}

		public long expireDate
		{
			get;
			private set;
		}
	}
}
