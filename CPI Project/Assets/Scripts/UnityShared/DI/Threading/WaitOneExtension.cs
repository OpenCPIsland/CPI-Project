using System;
using System.Threading;

namespace DI.Threading
{
	public static class WaitOneExtension
	{
		public static bool InterWaitOne(this ManualResetEvent that, int ms)
		{
			return that.WaitOne(ms, false);
		}

		public static bool InterWaitOne(this ManualResetEvent that, TimeSpan duration)
		{
			return that.WaitOne(duration, false);
		}
	}
}
