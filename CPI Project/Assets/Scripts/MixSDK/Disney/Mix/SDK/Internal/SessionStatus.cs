namespace Disney.Mix.SDK.Internal
{
	public class SessionStatus : ISessionStatus
	{
		public bool IsPaused
		{
			get;
			set;
		}

		public SessionStatus(bool isPaused)
		{
			IsPaused = isPaused;
		}
	}
}
