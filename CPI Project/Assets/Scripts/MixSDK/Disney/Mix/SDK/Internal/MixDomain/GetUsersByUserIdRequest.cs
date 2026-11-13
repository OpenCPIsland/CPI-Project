using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class GetUsersByUserIdRequest : BaseUserRequest
	{
		public List<string> UserIds;
	}
}
