namespace Disney.Mix.SDK
{
	public interface IPermissionResult
	{
		bool Success
		{
			get;
		}

		ActivityApprovalStatus Status
		{
			get;
		}
	}
}
