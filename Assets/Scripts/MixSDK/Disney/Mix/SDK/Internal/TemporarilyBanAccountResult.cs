namespace Disney.Mix.SDK.Internal
{
	public class TemporarilyBanAccountResult : ITemporarilyBanAccountResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public TemporarilyBanAccountResult(bool success)
		{
			Success = success;
		}
	}
}
