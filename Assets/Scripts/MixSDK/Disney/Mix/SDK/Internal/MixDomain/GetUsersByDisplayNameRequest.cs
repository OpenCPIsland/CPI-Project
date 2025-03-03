using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class GetUsersByDisplayNameRequest : BaseUserRequest
	{
		public List<string> DisplayNames;
	}
}
