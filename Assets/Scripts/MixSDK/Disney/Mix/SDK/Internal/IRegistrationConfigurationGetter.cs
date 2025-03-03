using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IRegistrationConfigurationGetter
	{
		void Get(Action<IInternalGetRegistrationConfigurationResult> callback);

		void Get(string countryCode, Action<IInternalGetRegistrationConfigurationResult> callback);
	}
}
