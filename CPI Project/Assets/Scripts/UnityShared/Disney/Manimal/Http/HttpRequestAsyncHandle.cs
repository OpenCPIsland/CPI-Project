using System.Net;

namespace Disney.Manimal.Http
{
	public class HttpRequestAsyncHandle : IHttpRequestAsyncHandle
	{
		private readonly HttpWebRequest _webRequest;

		public HttpRequestAsyncHandle(HttpWebRequest webRequest)
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
