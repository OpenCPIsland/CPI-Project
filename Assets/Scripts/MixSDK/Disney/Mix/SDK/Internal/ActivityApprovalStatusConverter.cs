namespace Disney.Mix.SDK.Internal
{
	public static class ActivityApprovalStatusConverter
	{
		public static ActivityApprovalStatus Convert(string status)
		{
			switch (status)
			{
			case "PENDING":
				return ActivityApprovalStatus.Pending;
			case "APPROVED":
				return ActivityApprovalStatus.Approved;
			case "DENIED":
				return ActivityApprovalStatus.Denied;
			default:
				return ActivityApprovalStatus.Unknown;
			}
		}

		public static string Convert(ActivityApprovalStatus status)
		{
			switch (status)
			{
			case ActivityApprovalStatus.Pending:
				return "PENDING";
			case ActivityApprovalStatus.Approved:
				return "APPROVED";
			case ActivityApprovalStatus.Denied:
				return "DENIED";
			default:
				return "UNKNOWN";
			}
		}
	}
}
