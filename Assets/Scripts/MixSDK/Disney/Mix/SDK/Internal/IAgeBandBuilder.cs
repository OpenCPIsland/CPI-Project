using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IAgeBandBuilder
	{
		void Build(SiteConfigurationData siteConfig, int age, string languageCode, bool registration, Action<IGetAgeBandResult> callback);

		void Build(SiteConfigurationData siteConfig, string ageBandKey, int age, string languageCode, bool registration, Action<IGetAgeBandResult> callback);
	}
}
