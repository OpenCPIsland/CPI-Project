namespace Disney.Mix.SDK.Internal
{
	internal class OfflineLastSessionNotFoundResult : IOfflineLastSessionNotFoundResult, IInternalOfflineLastSessionResult, IOfflineLastSessionResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public ISession Session
		{
			get
			{
				return InternalSession;
			}
		}

		public IInternalSession InternalSession
		{
			get;
			private set;
		}
	}
}
