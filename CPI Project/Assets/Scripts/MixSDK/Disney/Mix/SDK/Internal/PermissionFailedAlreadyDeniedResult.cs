namespace Disney.Mix.SDK.Internal
{
	public class PermissionFailedAlreadyDeniedResult : IPermissionFailedAlreadyDeniedResult, IPermissionResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}

		public ActivityApprovalStatus Status
		{
			get
			{
				return ActivityApprovalStatus.Denied;
			}
		}
	}
}
