namespace Disney.Mix.SDK.Internal
{
	internal class LoginFailedMultipleAccountsResult : ILoginFailedMultipleAccountsResult, ILoginResult
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

		public LoginFailedMultipleAccountsResult()
		{
			Success = false;
			Session = null;
		}
	}
}
