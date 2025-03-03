namespace Disney.Mix.SDK.Internal
{
	public class ReuseExistingGuestControllerLoginResult : IReuseExistingGuestControllerLoginResult
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

		public ReuseExistingGuestControllerLoginResult(bool success, ISession session)
		{
			Success = success;
			Session = session;
		}
	}
}
