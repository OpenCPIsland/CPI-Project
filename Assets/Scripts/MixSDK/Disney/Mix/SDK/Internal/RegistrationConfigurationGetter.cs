using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class RegistrationConfigurationGetter : IRegistrationConfigurationGetter
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClientFactory guestControllerClientFactory;

		private readonly IAgeBandBuilder ageBandBuilder;

		public RegistrationConfigurationGetter(AbstractLogger logger, IGuestControllerClientFactory guestControllerClientFactory, IAgeBandBuilder ageBandBuilder)
		{
			this.logger = logger;
			this.guestControllerClientFactory = guestControllerClientFactory;
			this.ageBandBuilder = ageBandBuilder;
		}

		public void Get(Action<IInternalGetRegistrationConfigurationResult> callback)
		{
			try
			{
				IGuestControllerClient guestControllerClient = guestControllerClientFactory.Create("NoSWID");
				guestControllerClient.GetSiteConfiguration(delegate(GuestControllerResult<SiteConfigurationResponse> r)
				{
					HandleResult(r, callback);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new GetRegistrationConfigurationResult(false, null));
			}
		}

		public void Get(string countryCode, Action<IInternalGetRegistrationConfigurationResult> callback)
		{
			try
			{
				IGuestControllerClient guestControllerClient = guestControllerClientFactory.Create("NoSWID");
				guestControllerClient.GetSiteConfiguration(countryCode, delegate(GuestControllerResult<SiteConfigurationResponse> r)
				{
					HandleResult(r, callback);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new GetRegistrationConfigurationResult(false, null));
			}
		}

		private void HandleResult(GuestControllerResult<SiteConfigurationResponse> gcResult, Action<IInternalGetRegistrationConfigurationResult> callback)
		{
			SiteConfigurationResponse response = gcResult.Response;
			if (!gcResult.Success)
			{
				callback(new GetRegistrationConfigurationResult(false, null));
				return;
			}
			IInternalGetRegistrationConfigurationResult regConfigResult = GuestControllerErrorParser.GetRegConfigResult(response.error);
			if (regConfigResult != null)
			{
				callback(regConfigResult);
				return;
			}
			if (!ValidateResponse(response))
			{
				callback(new GetRegistrationConfigurationResult(false, null));
				return;
			}
			RegistrationConfiguration configuration = new RegistrationConfiguration(response.data, ageBandBuilder);
			callback(new GetRegistrationConfigurationResult(true, configuration));
		}

		private static bool ValidateResponse(SiteConfigurationResponse response)
		{
			return ValidateResponseNotNull(response.data) && ValidateAgeBandKey(response.data.compliance.defaultAgeBand) && GetDefaultAgeBand(response.data.compliance) != null && GetDefaultLegalGroup(response.data) != null;
		}

		private static bool ValidateResponseNotNull(SiteConfigurationData siteConfig)
		{
			return siteConfig != null && siteConfig.compliance != null && siteConfig.compliance.ageBands != null && siteConfig.compliance.defaultAgeBand != null && siteConfig.legal != null;
		}

		private static bool ValidateAgeBandKey(string key)
		{
			return AgeBandTypeConverter.Convert(key) != AgeBandType.Unknown;
		}

		private static ConfigurationAgeBand GetDefaultAgeBand(Compliance compliance)
		{
			return DictionaryUtils.TryGetValue(compliance.ageBands, compliance.defaultAgeBand);
		}

		private static LegalGroup GetDefaultLegalGroup(SiteConfigurationData siteConfig)
		{
			return DictionaryUtils.TryGetValue(siteConfig.legal, siteConfig.compliance.defaultAgeBand);
		}
	}
}
