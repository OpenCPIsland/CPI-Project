using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class ProfileData : DisplayNameData
	{
		public Profile profile
		{
			get;
			set;
		}

		public List<MarketingItem> marketing
		{
			get;
			set;
		}
	}
}
