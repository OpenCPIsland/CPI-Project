namespace Disney.Mix.SDK.Internal
{
	public class VerifyAdultFailedMaximumAttemptsResult : IVerifyAdultFailedMaximumAttemptsResult, IVerifyAdultResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}

		public bool MaxAttempts
		{
			get
			{
				return true;
			}
		}
	}
}
