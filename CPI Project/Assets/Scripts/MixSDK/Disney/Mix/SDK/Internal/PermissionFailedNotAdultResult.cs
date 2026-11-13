namespace Disney.Mix.SDK.Internal
{
	public class PermissionFailedNotAdultResult : IPermissionFailedNotAdultResult, IPermissionResult
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
