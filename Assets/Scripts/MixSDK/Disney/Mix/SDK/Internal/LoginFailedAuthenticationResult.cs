namespace Disney.Mix.SDK.Internal
{
	public class LoginFailedAuthenticationResult : ILoginFailedAuthenticationResult, ILoginResult
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
