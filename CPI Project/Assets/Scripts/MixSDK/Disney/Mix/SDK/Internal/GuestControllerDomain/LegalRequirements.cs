using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class LegalRequirements
	{
		public Dictionary<string, string> cookiePermissions
		{
			get;
			set;
		}

		public Dictionary<string, string> marketingPermissions
		{
			get;
			set;
		}
	}
}
