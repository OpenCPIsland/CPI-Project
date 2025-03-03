using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct ProductResponse
	{
		public string key
		{
			get;
			private set;
		}

		public string gp_sku
		{
			get;
			private set;
		}

		public string apple_sku
		{
			get;
			private set;
		}

		public string csg_id
		{
			get;
			private set;
		}

		public string duration
		{
			get;
			private set;
		}

		public string trial
		{
			get;
			private set;
		}
	}
}
