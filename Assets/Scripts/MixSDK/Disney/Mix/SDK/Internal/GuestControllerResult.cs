using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class GuestControllerResult<TResponse>
	{
		public bool Success
		{
			get;
			private set;
		}

		public TResponse Response
		{
			get;
			private set;
		}

		public Dictionary<string, string> ResponseHeaders
		{
			get;
			private set;
		}

		public GuestControllerResult(bool success, TResponse response, Dictionary<string, string> responseHeaders)
		{
			Success = success;
			Response = response;
			ResponseHeaders = responseHeaders;
		}
	}
}
