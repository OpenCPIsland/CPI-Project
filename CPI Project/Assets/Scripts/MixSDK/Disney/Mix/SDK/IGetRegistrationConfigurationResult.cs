namespace Disney.Mix.SDK
{
	public interface IGetRegistrationConfigurationResult
	{
		bool Success
		{
			get;
		}

		IRegistrationConfiguration Configuration
		{
			get;
		}
	}
}
