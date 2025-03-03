namespace Disney.Mix.SDK.Internal
{
	internal class RestoreLastSessionFailedProfileDisabledResult : IRestoreLastSessionFailedProfileDisabledResult, IRestoreLastSessionResult
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

		public RestoreLastSessionFailedProfileDisabledResult()
		{
			Success = false;
			Session = null;
		}
	}
}
