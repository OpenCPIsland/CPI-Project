namespace Disney.Mix.SDK.Internal
{
	public class LoginFailedGatedLocationResult : ILoginFailedGatedLocationResult, ILoginResult
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
