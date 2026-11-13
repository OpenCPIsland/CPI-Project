namespace Disney.Mix.SDK.Internal
{
	public class VerifyAdultFailedInvalidDataResult : IVerifyAdultFailedInvalidDataResult, IVerifyAdultResult
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
