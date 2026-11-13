using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IGetRecommendedFriendsResult
	{
		bool Success
		{
			get;
		}

		IEnumerable<IUnidentifiedUser> Users
		{
			get;
		}
	}
}
