using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class GetLinkedUsersResult : IGetLinkedUsersResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public IEnumerable<ILinkedUser> LinkedUsers
		{
			get;
			private set;
		}

		public GetLinkedUsersResult(bool success, IEnumerable<ILinkedUser> users)
		{
			Success = success;
			LinkedUsers = users;
		}
	}
}
