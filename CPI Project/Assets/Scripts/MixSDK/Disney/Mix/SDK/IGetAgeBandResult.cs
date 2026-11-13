namespace Disney.Mix.SDK
{
	public interface IGetAgeBandResult
	{
		bool Success
		{
			get;
		}

		IAgeBand AgeBand
		{
			get;
		}
	}
}
