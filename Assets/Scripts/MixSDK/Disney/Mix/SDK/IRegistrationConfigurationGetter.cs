using System;

namespace Disney.Mix.SDK
{
	public interface IRegistrationConfigurationGetter
	{
		void Get(Action<IGetRegistrationConfigurationResult> callback);

		void Get(string countryCode, Action<IGetRegistrationConfigurationResult> callback);
	}
}
