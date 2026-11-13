namespace Disney.Mix.SDK
{
	public interface IGetAdultVerificationRequirementsResult
	{
		bool Success
		{
			get;
		}

		bool IsRequired
		{
			get;
		}

		bool IsAvailable
		{
			get;
		}
	}
}
