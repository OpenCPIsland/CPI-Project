namespace Disney.Mix.SDK.Internal
{
	internal class RestoreLastSessionFailedParentalConsentResult : IRestoreLastSessionFailedParentalConsentResult, IRestoreLastSessionResult
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

		public RestoreLastSessionFailedParentalConsentResult()
		{
			Success = false;
			Session = null;
		}
	}
}
