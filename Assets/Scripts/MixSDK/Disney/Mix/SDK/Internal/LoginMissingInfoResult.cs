namespace Disney.Mix.SDK.Internal
{
	internal class LoginMissingInfoResult : ILoginMissingInfoResult, ILoginResult
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

		public LoginMissingInfoResult()
		{
			Success = false;
			Session = null;
		}

		public LoginMissingInfoResult(bool success, ISession session)
		{
			Success = success;
			Session = session;
		}
	}
}
