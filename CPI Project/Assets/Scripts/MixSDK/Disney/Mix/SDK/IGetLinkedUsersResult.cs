using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IGetLinkedUsersResult
	{
		bool Success
		{
			get;
		}

		IEnumerable<ILinkedUser> LinkedUsers
		{
			get;
		}
	}
}
