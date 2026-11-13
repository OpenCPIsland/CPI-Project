using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class RegisterRequest : AbstractGuestControllerWebCallRequest
	{
		public string password
		{
			get;
			set;
		}

		public BaseRegisterProfile profile
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
