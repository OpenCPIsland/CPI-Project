namespace Disney.Mix.SDK.Internal
{
	public class ReuseExistingGuestControllerLoginCorruptionDetectedResult : IReuseExistingGuestControllerLoginCorruptionDetectedResult, IReuseExistingGuestControllerLoginResult
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
	}
}
