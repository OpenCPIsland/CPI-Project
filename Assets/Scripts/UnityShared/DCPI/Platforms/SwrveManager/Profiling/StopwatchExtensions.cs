using System.Diagnostics;

namespace DCPI.Platforms.SwrveManager.Profiling
{
	public static class StopwatchExtensions
	{
		public delegate void TestFunction();

		public static long RunTest(this Stopwatch stopwatch, TestFunction testFunction)
		{
			stopwatch.Reset();
			stopwatch.Start();
			testFunction();
			return stopwatch.ElapsedMilliseconds;
		}
	}
}
