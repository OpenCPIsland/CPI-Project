using System.Collections.Generic;

namespace ClubPenguin.Tests
{
	public class RegisterRequest
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
