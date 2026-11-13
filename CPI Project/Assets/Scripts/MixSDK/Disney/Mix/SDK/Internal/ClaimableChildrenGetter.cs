using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class ClaimableChildrenGetter
	{
		public static void GetChildren(AbstractLogger logger, IGuestControllerClient guestControllerClient, IMixWebCallFactory mixWebCallFactory, Action<IGetLinkedUsersResult> callback)
		{
			try
			{
				guestControllerClient.GetClaimableChildren(delegate(GuestControllerResult<ChildrenResponse> r)
				{
					HandleGetClaimableChildrenResult(logger, mixWebCallFactory, callback, r);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(MakeGenericFailure());
			}
		}

		private static void HandleGetClaimableChildrenResult(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory, Action<IGetLinkedUsersResult> callback, GuestControllerResult<ChildrenResponse> result)
		{
			try
			{
				if (!result.Success || result.Response.error != null || result.Response.data == null)
				{
					callback(MakeGenericFailure());
				}
				else
				{
					LinkedUsersGetter.Get(logger, mixWebCallFactory, result.Response.data.children, delegate(LinkedUser[] users)
					{
						callback((users == null) ? MakeGenericFailure() : new GetLinkedUsersResult(true, users));
					});
				}
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(MakeGenericFailure());
			}
		}

		private static IGetLinkedUsersResult MakeGenericFailure()
		{
			return new GetLinkedUsersResult(false, Enumerable.Empty<ILinkedUser>());
		}
	}
}
