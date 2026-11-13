namespace Disney.Mix.SDK.Internal
{
	internal class RestoreLastSessionNotFoundResult : IRestoreLastSessionNotFoundResult, IRestoreLastSessionResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public ISession Session
		{
			get;
			private set;
		}
	}
}
