namespace Disney.Mix.SDK
{
	public interface IVerifyAdultResult
	{
		bool Success
		{
			get;
		}

		bool MaxAttempts
		{
			get;
		}
	}
}
