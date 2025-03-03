namespace Disney.Mix.SDK.Internal
{
	public class VerifyAdultFailedNotAdultResult : IVerifyAdultFailedNotAdultResult, IVerifyAdultResult
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
