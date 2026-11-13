using System;

namespace Disney.Mix.SDK
{
	public interface IRegistrationConfiguration
	{
		void GetRegistrationAgeBand(int age, string languageCode, Action<IGetAgeBandResult> callback);

		void GetUpdateAgeBand(int age, string languageCode, Action<IGetAgeBandResult> callback);
	}
}
