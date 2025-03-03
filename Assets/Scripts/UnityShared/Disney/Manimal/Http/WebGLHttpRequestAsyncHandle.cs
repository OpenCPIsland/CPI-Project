namespace Disney.Manimal.Http
{
	public class WebGLHttpRequestAsyncHandle : IHttpRequestAsyncHandle
	{
		private readonly XmlHttpRequest _webRequest;

		public WebGLHttpRequestAsyncHandle(XmlHttpRequest webRequest)
		{
			_webRequest = webRequest;
		}

		public void Abort()
		{
			if (_webRequest != null)
			{
				_webRequest.Abort();
			}
		}
	}
}
