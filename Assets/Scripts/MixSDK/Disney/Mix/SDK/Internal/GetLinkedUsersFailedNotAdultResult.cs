using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class GetLinkedUsersFailedNotAdultResult : IGetLinkedUsersFailedNotAdultResult, IGetLinkedUsersResult
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
