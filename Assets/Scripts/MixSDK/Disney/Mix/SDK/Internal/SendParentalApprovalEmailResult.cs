namespace Disney.Mix.SDK.Internal
{
	internal class SendParentalApprovalEmailResult : ISendParentalApprovalEmailResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendParentalApprovalEmailResult(bool success)
		{
			Success = success;
		}
	}
}
