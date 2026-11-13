namespace Disney.Mix.SDK.Internal
{
	public interface IInternalGetRegistrationConfigurationResult : IGetRegistrationConfigurationResult
	{
		IInternalRegistrationConfiguration InternalConfiguration
		{
			get;
		}
	}
}
