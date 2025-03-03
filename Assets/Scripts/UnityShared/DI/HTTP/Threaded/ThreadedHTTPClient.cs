namespace DI.HTTP.Threaded
{
	public class ThreadedHTTPClient : HTTPBaseClientImpl, IHTTPClient
	{
		public ThreadedHTTPClient(IHTTPFactory factory)
			: base(factory)
		{
		}

		public override IHTTPRequest getRequest()
		{
			return new ThreadedHTTPRequest(this);
		}
	}
}
