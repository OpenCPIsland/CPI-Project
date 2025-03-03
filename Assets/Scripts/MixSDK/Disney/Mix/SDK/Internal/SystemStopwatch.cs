using System.Diagnostics;

namespace Disney.Mix.SDK.Internal
{
	public class SystemStopwatch : IStopwatch
	{
		private readonly Stopwatch stopwatch;

		public long ElapsedMilliseconds
		{
			get
			{
				return stopwatch.ElapsedMilliseconds;
			}
		}

		public SystemStopwatch()
		{
			stopwatch = new Stopwatch();
		}

		public void Stop()
		{
			stopwatch.Stop();
		}

		public void Start()
		{
			stopwatch.Start();
		}

		public void Reset()
		{
			stopwatch.Reset();
		}
	}
}
