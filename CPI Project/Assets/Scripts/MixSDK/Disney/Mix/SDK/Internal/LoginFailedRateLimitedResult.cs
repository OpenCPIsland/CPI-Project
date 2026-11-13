namespace Disney.Mix.SDK.Internal
{
	internal class LoginFailedRateLimitedResult : ILoginFailedRateLimitedResult, ILoginResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}

		public ISession Session
		{
			get
			{
				return null;
			}
		}
	}
}
