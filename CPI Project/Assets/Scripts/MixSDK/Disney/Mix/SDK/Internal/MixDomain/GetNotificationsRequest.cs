using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class GetNotificationsRequest : BaseUserRequest
	{
		public long? CreatedAfter;

		public List<long?> ExcludeNotificationIds;
	}
}
