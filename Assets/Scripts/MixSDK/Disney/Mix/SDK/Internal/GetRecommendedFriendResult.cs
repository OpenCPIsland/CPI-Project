using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class GetRecommendedFriendResult : IGetRecommendedFriendsResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public IEnumerable<IUnidentifiedUser> Users
		{
			get;
			private set;
		}

		public GetRecommendedFriendResult(bool success, IEnumerable<IUnidentifiedUser> users)
		{
			Success = success;
			Users = users;
		}
	}
}
