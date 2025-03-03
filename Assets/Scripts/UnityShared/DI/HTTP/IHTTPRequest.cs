using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DI.HTTP
{
	public interface IHTTPRequest
	{
		IHTTPClient getClient();

		void setMethod(HTTPMethod method);

		HTTPMethod getMethod();

		void setUrl(string url);

		string getUrl();

		void setListener(IHTTPListener listener);

		IHTTPListener getListener();

		IDictionary<string, IList<string>> getRequestHeaders();

		IHTTPDocument getDocument();

		void setDocument(IHTTPDocument document);

		bool validateCertificate(X509Certificate certificate, SslPolicyErrors sslPolicyError);

		IHTTPResponse performSync();

		void performAsync();
	}
}
