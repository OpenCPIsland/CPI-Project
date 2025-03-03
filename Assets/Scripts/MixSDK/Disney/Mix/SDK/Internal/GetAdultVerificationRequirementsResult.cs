namespace Disney.Mix.SDK.Internal
{
	public class GetAdultVerificationRequirementsResult : IGetAdultVerificationRequirementsResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public bool IsRequired
		{
			get;
			private set;
		}

		public bool IsAvailable
		{
			get;
			private set;
		}

		public GetAdultVerificationRequirementsResult(bool success, bool isRequired, bool isAvailable)
		{
			Success = success;
			IsRequired = isRequired;
			IsAvailable = isAvailable;
		}
	}
}
