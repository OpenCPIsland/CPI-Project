using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class WwwCallFactory : IWwwCallFactory
	{
		private readonly AbstractLogger logger;

		private readonly ICoroutineManager coroutineManager;

		private readonly IStopwatchFactory stopwatchFactory;

		private readonly IWwwFactory wwwFactory;

		private static int nextRequestId;

		public WwwCallFactory(AbstractLogger logger, ICoroutineManager coroutineManager, IStopwatchFactory stopwatchFactory, IWwwFactory wwwFactory)
		{
			this.logger = logger;
			this.coroutineManager = coroutineManager;
			this.stopwatchFactory = stopwatchFactory;
			this.wwwFactory = wwwFactory;
		}

		public IWwwCall Create(Uri uri, HttpMethod method, byte[] body, Dictionary<string, string> headers, long latencyWwwCallTimeout, long maxWwwCallTimeout)
		{
			nextRequestId++;
			int requestId = nextRequestId;
			IStopwatch stopwatch = stopwatchFactory.Create();
			return new WwwCall(logger, requestId, uri, method, body, headers, coroutineManager, stopwatch, wwwFactory, latencyWwwCallTimeout, maxWwwCallTimeout);
		}
	}
}
