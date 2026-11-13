namespace Disney.Mix.SDK.Internal
{
	internal class RestoreLastSessionSuccessRequiresLegalMarketingUpdateResult : IRestoreLastSessionSuccessRequiresLegalMarketingUpdateResult, IRestoreLastSessionResult
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

		public RestoreLastSessionSuccessRequiresLegalMarketingUpdateResult()
		{
			Success = true;
			Session = null;
		}

		public RestoreLastSessionSuccessRequiresLegalMarketingUpdateResult(bool success, ISession session)
		{
			Success = success;
			Session = session;
		}
	}
}
