namespace Disney.Mix.SDK
{
	public interface ILoginResult
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
