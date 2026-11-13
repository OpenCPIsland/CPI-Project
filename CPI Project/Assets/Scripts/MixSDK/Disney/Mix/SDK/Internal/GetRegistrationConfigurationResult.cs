namespace Disney.Mix.SDK.Internal
{
	public class GetRegistrationConfigurationResult : IInternalGetRegistrationConfigurationResult, IGetRegistrationConfigurationResult
	{
		public bool Success
		{
			get;
			private set;
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
			get;
			private set;
		}

		public GetRegistrationConfigurationResult(bool success, IInternalRegistrationConfiguration configuration)
		{
			Success = success;
			InternalConfiguration = configuration;
		}
	}
}
