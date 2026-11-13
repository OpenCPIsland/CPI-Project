using SwrveUnity.Helpers;
using System.Collections.Generic;

namespace SwrveUnity.REST
{
	public class RESTResponse
	{
		public readonly string Body;

		public readonly WwwDeducedError Error = WwwDeducedError.NoError;

		public readonly long ResponseCode;

		public readonly Dictionary<string, string> Headers;

		public RESTResponse(string body)
		{
			Body = body;
		}

		public RESTResponse(string body, Dictionary<string, string> headers)
			: this(body)
		{
			Headers = headers;
		}

		public RESTResponse(WwwDeducedError error)
		{
			Error = error;
		}

		public RESTResponse(long responseCode)
		{
			ResponseCode = responseCode;
		}
	}
}
