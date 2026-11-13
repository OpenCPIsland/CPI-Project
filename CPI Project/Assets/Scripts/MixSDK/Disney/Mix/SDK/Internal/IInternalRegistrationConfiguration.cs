using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IInternalRegistrationConfiguration : IRegistrationConfiguration
	{
		void GetRegistrationAgeBand(string ageBandKey, int age, string languageCode, Action<IGetAgeBandResult> callback);

		void GetUpdateAgeBand(string ageBandKey, int age, string languageCode, Action<IGetAgeBandResult> callback);
	}
}
