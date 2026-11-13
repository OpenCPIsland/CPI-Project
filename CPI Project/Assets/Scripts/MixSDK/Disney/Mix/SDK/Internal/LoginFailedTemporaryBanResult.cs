namespace Disney.Mix.SDK.Internal
{
	internal class LoginFailedTemporaryBanResult : ILoginFailedTemporaryBanResult, ILoginResult
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

		public LoginFailedTemporaryBanResult()
		{
			Success = false;
			Session = null;
		}
	}
}
