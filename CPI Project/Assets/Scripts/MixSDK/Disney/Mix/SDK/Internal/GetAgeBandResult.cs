namespace Disney.Mix.SDK.Internal
{
	public class GetAgeBandResult : IGetAgeBandResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public IAgeBand AgeBand
		{
			get;
			private set;
		}

		public GetAgeBandResult(bool success, IAgeBand ageBand)
		{
			Success = success;
			AgeBand = ageBand;
		}
	}
}
