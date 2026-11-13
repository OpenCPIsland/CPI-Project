namespace Disney.Mix.SDK.Internal
{
	public class RestoreLastSessionCorruptionDetectedResult : IRestoreLastSessionCorruptionDetectedResult, IRestoreLastSessionResult
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
