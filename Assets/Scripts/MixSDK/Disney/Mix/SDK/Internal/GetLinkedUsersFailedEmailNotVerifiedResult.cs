using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class GetLinkedUsersFailedEmailNotVerifiedResult : IGetLinkedUsersFailedEmailNotVerifiedResult, IGetLinkedUsersResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}

		public IEnumerable<ILinkedUser> LinkedUsers
		{
			get
			{
				return Enumerable.Empty<ILinkedUser>();
			}
		}
	}
}
