using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class PermissionRequester
	{
		public static void RequestPermission(AbstractLogger logger, IGuestControllerClient guestControllerClient, string activityCode, Action<IPermissionResult> callback)
		{
			try
			{
				guestControllerClient.RequestPermission(new RequestPermissionRequest
				{
					activityCode = activityCode
				}, delegate(GuestControllerResult<PermissionResponse> r)
				{
					HandleResult(activityCode, callback, r);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(MakeGenericFailure());
			}
		}

		public static void RequestPermissionForChild(AbstractLogger logger, IGuestControllerClient guestControllerClient, string activityCode, string childSwid, Action<IPermissionResult> callback)
		{
			try
			{
				guestControllerClient.RequestPermissionForChild(new RequestPermissionRequest
				{
					activityCode = activityCode
				}, childSwid, delegate(GuestControllerResult<PermissionResponse> r)
				{
					HandleResult(activityCode, callback, r);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(MakeGenericFailure());
			}
		}

		private static void HandleResult(string activityCode, Action<IPermissionResult> callback, GuestControllerResult<PermissionResponse> result)
		{
			if (!result.Success)
			{
				callback(MakeGenericFailure());
				return;
			}
			PermissionResponse response = result.Response;
			if (response.error != null || response.data == null)
			{
				callback(GuestControllerErrorParser.GetPermissionResult(response.error) ?? MakeGenericFailure());
				return;
			}
			ActivityApprovalStatus activityApprovalStatus = ActivityApprovalStatusConverter.Convert(response.data.approvalStatus);
			if (response.data.activityCode != activityCode || activityApprovalStatus == ActivityApprovalStatus.Unknown)
			{
				callback(MakeGenericFailure());
			}
			else
			{
				callback(new PermissionResult(true, activityApprovalStatus));
			}
		}

		private static IPermissionResult MakeGenericFailure()
		{
			return new PermissionResult(false, ActivityApprovalStatus.Unknown);
		}
	}
}
