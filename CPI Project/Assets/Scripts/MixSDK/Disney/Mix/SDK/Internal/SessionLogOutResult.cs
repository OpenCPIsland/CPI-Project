namespace Disney.Mix.SDK.Internal
{
	internal class SessionLogOutResult : ISessionLogOutResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SessionLogOutResult(bool success)
		{
			Success = success;
		}
	}
}
