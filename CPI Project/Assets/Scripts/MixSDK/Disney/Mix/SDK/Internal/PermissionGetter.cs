using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class PermissionGetter
	{
		public static void GetPermission(AbstractLogger logger, IGuestControllerClient guestControllerClient, string activityCode, string childSwid, Action<IPermissionResult> callback)
		{
			if (string.IsNullOrEmpty(childSwid) || string.IsNullOrEmpty(activityCode))
			{
				callback(new PermissionFailedInvalidResult());
			}
			else
			{
				try
				{
					guestControllerClient.GetPermission(childSwid, delegate(GuestControllerResult<GetPermissionsResponse> r)
					{
						if (!r.Success)
						{
							callback(MakeGenericFailure());
						}
						else if (r.Response.error != null || r.Response.data == null)
						{
							callback(MakeGenericFailure());
						}
						else
						{
							Permission permission = r.Response.data.activityPermissions.FirstOrDefault((Permission ap) => ap.activityCode == activityCode);
							ActivityApprovalStatus status = (permission == null) ? ActivityApprovalStatus.Unknown : ActivityApprovalStatusConverter.Convert(permission.approvalStatus);
							callback(new PermissionResult(true, status));
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
	}
}
