using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class LinkedGuardiansGetter
	{
		public static void GetGuardians(AbstractLogger logger, IGuestControllerClient guestControllerClient, IMixWebCallFactory mixWebCallFactory, Action<IGetLinkedUsersResult> callback)
		{
			try
			{
				guestControllerClient.GetGuardians(delegate(GuestControllerResult<GuardiansResponse> r)
				{
					HandleGetGuardiansResult(logger, mixWebCallFactory, callback, r);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(MakeGenericFailure());
			}
		}

		private static void HandleGetGuardiansResult(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory, Action<IGetLinkedUsersResult> callback, GuestControllerResult<GuardiansResponse> result)
		{
			try
			{
				if (!result.Success || result.Response.error != null)
				{
					callback(MakeGenericFailure());
				}
				else
				{
					List<Profile> list = (result.Response.data == null) ? null : result.Response.data.guardians;
					if (list == null)
					{
						callback(MakeGenericFailure());
					}
					else
					{
						LinkedUsersGetter.Get(logger, mixWebCallFactory, list, delegate(LinkedUser[] users)
						{
							callback((users == null) ? MakeGenericFailure() : new GetLinkedUsersResult(true, users));
						});
					}
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
