namespace DI.HTTP.Coroutine
{
	public class WWWBasedHTTPClient : HTTPBaseClientImpl, IHTTPClient
	{
		public WWWBasedHTTPClient(WWWBasedHTTPFactory factory)
			: base(factory)
		{
		}

		public override IHTTPRequest getRequest()
		{
			return new WWWBasedHTTPRequest(this);
		}
	}
}
