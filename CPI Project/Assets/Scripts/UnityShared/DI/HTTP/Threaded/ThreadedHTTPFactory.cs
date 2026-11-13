using DI.HTTP.Security.Pinning;

namespace DI.HTTP.Threaded
{
	public class ThreadedHTTPFactory : IHTTPFactory
	{
		public static bool LogAllRequests = false;

		public static bool VerboseLogging = false;

		private static ThreadedHTTPFactory factory = null;

		private IPinset pinset = null;

		protected ThreadedHTTPFactory()
		{
			IPinsetFactory pinsetFactory = DefaultPinsetFactory.getFactory();
			pinset = pinsetFactory.getPinset();
		}

		public static ThreadedHTTPFactory getFactory()
		{
			if (factory == null)
			{
				factory = new ThreadedHTTPFactory();
			}
			return factory;
		}

		public IPinset getPinset()
		{
			return pinset;
		}

		protected void setPinset(IPinset pinset)
		{
			this.pinset = pinset;
		}

		public IHTTPClient getClient()
		{
			return new ThreadedHTTPClient(this);
		}
	}
}
