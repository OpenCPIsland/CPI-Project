using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class PermissionApprover
	{
		public static void ApprovePermission(AbstractLogger logger, IGuestControllerClient guestControllerClient, string activityCode, string childSwid, ActivityApprovalStatus desiredStatus, Action<IPermissionResult> callback)
		{
			if (string.IsNullOrEmpty(childSwid) || string.IsNullOrEmpty(activityCode) || desiredStatus == ActivityApprovalStatus.Pending || desiredStatus == ActivityApprovalStatus.Unknown)
			{
				callback(new PermissionFailedInvalidResult());
			}
			else
			{
				try
				{
					ApprovePermissionRequest approvePermissionRequest = new ApprovePermissionRequest();
					approvePermissionRequest.activityCode = activityCode;
					approvePermissionRequest.approvalStatus = ActivityApprovalStatusConverter.Convert(desiredStatus);
					approvePermissionRequest.swid = childSwid;
					ApprovePermissionRequest request = approvePermissionRequest;
					guestControllerClient.ApprovePermission(request, childSwid, delegate(GuestControllerResult<PermissionResponse> r)
					{
						if (!r.Success)
						{
							callback(MakeGenericFailure());
						}
						else
						{
							PermissionResponse response = r.Response;
							if (response.error != null || response.data == null)
							{
								callback(ParseError(response));
							}
							else
							{
								string activityCode2 = response.data.activityCode;
								ActivityApprovalStatus activityApprovalStatus = ActivityApprovalStatusConverter.Convert(response.data.approvalStatus);
								if (activityCode2 != activityCode || activityApprovalStatus != desiredStatus)
								{
									callback(new PermissionFailedInvalidResult());
								}
								else
								{
									callback(new PermissionResult(true, activityApprovalStatus));
								}
							}
						}
					});
				}
				catch (Exception arg)
				{
					logger.Critical("Unhandled exception: " + arg);
					callback(MakeGenericFailure());
				}
			}
		}

		private static IPermissionResult MakeGenericFailure()
		{
			return new PermissionResult(false, ActivityApprovalStatus.Unknown);
		}

		private static IPermissionResult ParseError(GuestControllerWebCallResponse response)
		{
			IPermissionResult permissionResult = GuestControllerErrorParser.GetPermissionResult(response.error);
			return permissionResult ?? MakeGenericFailure();
		}
	}
}
