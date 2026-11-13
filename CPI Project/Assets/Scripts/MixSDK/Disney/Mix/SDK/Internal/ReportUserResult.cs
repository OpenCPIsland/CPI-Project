namespace Disney.Mix.SDK.Internal
{
	internal class ReportUserResult : IReportUserResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public ReportUserResult(bool success)
		{
			Success = success;
		}
	}
}
