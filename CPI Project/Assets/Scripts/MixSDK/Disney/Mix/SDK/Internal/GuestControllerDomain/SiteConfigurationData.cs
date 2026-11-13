using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class SiteConfigurationData
	{
		public Compliance compliance
		{
			get;
			set;
		}

		public Dictionary<string, ConfigurationMarketingAgeBand> marketing
		{
			get;
			set;
		}

		public Dictionary<string, LegalGroup> legal
		{
			get;
			set;
		}
	}
}
