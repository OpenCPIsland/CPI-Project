namespace DI.HTTP
{
	public abstract class HTTPBaseClientImpl : HTTPListenerHelper, IHTTPClient
	{
		public static bool LogAllRequests = false;

		public static bool VerboseLogging = false;

		private IHTTPFactory factory = null;

		public HTTPBaseClientImpl(IHTTPFactory factory)
		{
			this.factory = factory;
		}

		public IHTTPFactory getFactory()
		{
			return factory;
		}

		public abstract IHTTPRequest getRequest();
	}
}
