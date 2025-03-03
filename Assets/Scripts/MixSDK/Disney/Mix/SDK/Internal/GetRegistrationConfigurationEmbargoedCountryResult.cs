namespace Disney.Mix.SDK.Internal
{
	public class GetRegistrationConfigurationEmbargoedCountryResult : IInternalGetRegistrationConfigurationResult, IGetRegistrationConfigurationEmbargoedCountryResult, IGetRegistrationConfigurationResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}

		public IRegistrationConfiguration Configuration
		{
			get
			{
				return InternalConfiguration;
			}
		}

		public IInternalRegistrationConfiguration InternalConfiguration
		{
			get
			{
				return null;
			}
		}
	}
}
