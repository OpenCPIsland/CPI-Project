using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class UpdateProfileRequest : AbstractGuestControllerWebCallRequest
	{
		public string etag
		{
			get;
			set;
		}

		public Dictionary<string, string> profile
		{
			get;
			set;
		}

		public List<MarketingItem> marketing
		{
			get;
			set;
		}

		public RegisterDisplayName displayName
		{
			get;
			set;
		}

		public List<string> legalAssertions
		{
			get;
			set;
		}
	}
}
