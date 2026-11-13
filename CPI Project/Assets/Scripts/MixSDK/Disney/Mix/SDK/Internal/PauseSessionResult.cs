namespace Disney.Mix.SDK.Internal
{
	internal class PauseSessionResult : IPauseSessionResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public PauseSessionResult(bool success)
		{
			Success = success;
		}
	}
}
