using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class ValidateDisplayNameResponse : BaseResponse
	{
		public string DisplayNameStatus;

		public List<string> DisplayNames;
	}
}
