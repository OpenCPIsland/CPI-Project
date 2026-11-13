namespace Disney.Mix.SDK.Internal
{
	public interface IStopwatch
	{
		long ElapsedMilliseconds
		{
			get;
		}

		void Stop();

		void Start();

		void Reset();
	}
}
