namespace Disney.Mix.SDK.Internal
{
	public class VerifyAdultFailedMissingInfoResult : IVerifyAdultFailedMissingInfoResult, IVerifyAdultResult
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
				return false;
			}
		}
	}
}
