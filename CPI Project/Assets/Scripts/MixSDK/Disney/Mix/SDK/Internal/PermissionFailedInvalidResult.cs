namespace Disney.Mix.SDK.Internal
{
	public class PermissionFailedInvalidResult : IPermissionFailedInvalidResult, IPermissionResult
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
				return ActivityApprovalStatus.Unknown;
			}
		}
	}
}
