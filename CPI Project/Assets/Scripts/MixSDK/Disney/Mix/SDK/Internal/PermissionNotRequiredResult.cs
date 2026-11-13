namespace Disney.Mix.SDK.Internal
{
	public class PermissionNotRequiredResult : IPermissionNotRequiredResult, IPermissionResult
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
