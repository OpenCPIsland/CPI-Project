namespace Disney.Mix.SDK.Internal
{
	internal class RestoreLastSessionFailedMultipleAccountsResult : IRestoreLastSessionFailedMultipleAccountsResult, IRestoreLastSessionResult
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

		public RestoreLastSessionFailedMultipleAccountsResult()
		{
			Success = false;
			Session = null;
		}
	}
}
