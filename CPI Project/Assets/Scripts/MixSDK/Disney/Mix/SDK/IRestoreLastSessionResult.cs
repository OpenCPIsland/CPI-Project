namespace Disney.Mix.SDK
{
	public interface IRestoreLastSessionResult
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
