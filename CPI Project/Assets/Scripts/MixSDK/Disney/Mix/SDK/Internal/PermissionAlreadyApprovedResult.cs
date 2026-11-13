namespace Disney.Mix.SDK.Internal
{
	public class PermissionAlreadyApprovedResult : IPermissionResult
	{
		public bool Success
		{
			get
			{
				return true;
			}
		}

		public ActivityApprovalStatus Status
		{
			get
			{
				return ActivityApprovalStatus.Approved;
			}
		}
	}
}
