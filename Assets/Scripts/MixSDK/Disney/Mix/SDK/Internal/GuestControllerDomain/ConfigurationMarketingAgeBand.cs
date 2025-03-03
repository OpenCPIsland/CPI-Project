using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class ConfigurationMarketingAgeBand
	{
		public Dictionary<string, ConfigurationMarketingItem> CREATE
		{
			get;
			set;
		}

		public Dictionary<string, ConfigurationMarketingItem> UPDATE
		{
			get;
			set;
		}

		public Dictionary<string, ConfigurationMarketingItem> PARTIAL
		{
			get;
			set;
		}
	}
}
