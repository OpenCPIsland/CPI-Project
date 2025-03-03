using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class ClearAlertsRequest : BaseUserRequest
	{
		public List<long?> AlertIds;
	}
}
