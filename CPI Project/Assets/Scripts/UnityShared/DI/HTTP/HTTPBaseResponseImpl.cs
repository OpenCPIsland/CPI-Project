using System.Collections.Generic;

namespace DI.HTTP
{
	public class HTTPBaseResponseImpl : IHTTPResponse
	{
		private IHTTPRequest request = null;

		private int statusCode = 0;

		private string reasonPhrase = "";

		private IDictionary<string, IList<string>> headers = null;

		private IHTTPDocument document = null;

		public HTTPBaseResponseImpl(IHTTPRequest request)
		{
			this.request = request;
		}

		public IHTTPRequest getRequest()
		{
			return request;
		}

		public int getStatusCode()
		{
			return statusCode;
		}

		public void setStatusCode(int statusCode)
		{
			this.statusCode = statusCode;
		}

		public string getReasonPhrase()
		{
			return reasonPhrase;
		}

		public void setReasonPhrase(string reasonPhrase)
		{
			this.reasonPhrase = reasonPhrase;
		}

		public IDictionary<string, IList<string>> getResponseHeaders()
		{
			return headers;
		}

		public void setResponseHeaders(IDictionary<string, IList<string>> headers)
		{
			this.headers = headers;
		}

		public void setDocument(IHTTPDocument document)
		{
			this.document = document;
		}

		public IHTTPDocument getDocument()
		{
			return document;
		}
	}
}
