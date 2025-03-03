using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class GetNotificationsSinceSequenceRequest : BaseUserRequest
	{
		public long? SequenceNumber;

		public List<long?> ExcludeNotificationSequenceNumbers;
	}
}
