namespace Disney.Mix.SDK.Internal
{
	public class SystemStopwatchFactory : IStopwatchFactory
	{
		public IStopwatch Create()
		{
			return new SystemStopwatch();
		}
	}
}
