namespace Disney.Mix.SDK.Internal
{
	internal class LoginCorruptionDetectedResult : ILoginCorruptionDetectedResult, ILoginResult
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
