using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class GetRegistrationTextRequest : BaseUserRequest
	{
		public string LanguageCode;

		public List<string> TextCodes;

		public string CountryCode;

		public string AgeBand;
	}
}
