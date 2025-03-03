using System.Collections.Generic;

namespace DI.HTTP
{
	public interface IHTTPResponse
	{
		IHTTPRequest getRequest();

		int getStatusCode();

		string getReasonPhrase();

		IHTTPDocument getDocument();

		IDictionary<string, IList<string>> getResponseHeaders();
	}
}
