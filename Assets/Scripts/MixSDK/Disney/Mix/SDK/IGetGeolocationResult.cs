namespace Disney.Mix.SDK
{
	public interface IGetGeolocationResult
	{
		bool Success
		{
			get;
		}

		string CountryCode
		{
			get;
		}
	}
}
