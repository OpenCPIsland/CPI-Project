namespace Disney.Mix.SDK
{
	public interface IOfflineLastSessionResult
	{
		bool Success
		{
			get;
		}

		ISession Session
		{
			get;
		}
	}
}
