using System;

namespace ZenFulcrum.EmbeddedBrowser
{
	internal class PredicateWait2
	{
		public Func<TimeData, bool> predicate;

		public float timeStarted;

		public IPendingPromise2 pendingPromise;

		public TimeData timeData;
	}
}
