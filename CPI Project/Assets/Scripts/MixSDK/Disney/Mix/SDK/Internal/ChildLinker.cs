using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class ChildLinker
	{
		public static void LinkChild(AbstractLogger logger, IGuestControllerClient guestControllerClient, string childSwid, string childAccessToken, Action<ILinkChildResult> callback)
		{
			try
			{
				LinkChildRequest linkChildRequest = new LinkChildRequest();
				linkChildRequest.authZToken = childAccessToken;
				LinkChildRequest request = linkChildRequest;
				guestControllerClient.LinkChild(request, childSwid, delegate(GuestControllerResult<GuestControllerWebCallResponse> r)
				{
					if (!r.Success)
					{
						callback(MakeGenericFailure());
					}
					else if (r.Response.error != null)
					{
						callback(ParseError(r.Response));
					}
					else
					{
						callback(new LinkChildResult(true));
					}
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(MakeGenericFailure());
			}
		}

		public static void LinkClaimableChildren(AbstractLogger logger, IGuestControllerClient guestControllerClient, IEnumerable<string> childSwids, Action<ILinkChildResult> callback)
		{
			try
			{
				LinkClaimableChildrenRequest linkClaimableChildrenRequest = new LinkClaimableChildrenRequest();
				linkClaimableChildrenRequest.swids = childSwids.ToArray();
				LinkClaimableChildrenRequest request = linkClaimableChildrenRequest;
				guestControllerClient.LinkClaimableChildren(request, delegate(GuestControllerResult<GuestControllerWebCallResponse> r)
				{
					if (!r.Success)
					{
						callback(MakeGenericFailure());
					}
					else if (r.Response.error != null)
					{
						callback(ParseError(r.Response));
					}
					else
					{
						callback(new LinkChildResult(true));
					}
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(MakeGenericFailure());
			}
		}

		private static ILinkChildResult MakeGenericFailure()
		{
			return new LinkChildResult(false);
		}

		private static ILinkChildResult ParseError(GuestControllerWebCallResponse response)
		{
			ILinkChildResult linkChildResult = GuestControllerErrorParser.GetLinkChildResult(response.error);
			return linkChildResult ?? MakeGenericFailure();
		}
	}
}
