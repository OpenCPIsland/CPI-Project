using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Disney.Manimal.Http
{
	public interface IHttpRequest
	{
		Action<Stream> ResponseWriter
		{
			get;
			set;
		}

		CookieContainer CookieContainer
		{
			get;
			set;
		}

		ICredentials Credentials
		{
			get;
			set;
		}

		bool AlwaysMultipartFormData
		{
			get;
			set;
		}

		string UserAgent
		{
			get;
			set;
		}

		int Timeout
		{
			get;
			set;
		}

		int ReadWriteTimeout
		{
			get;
			set;
		}

		bool FollowRedirects
		{
			get;
			set;
		}

		X509CertificateCollection ClientCertificates
		{
			get;
			set;
		}

		int? MaxRedirects
		{
			get;
			set;
		}

		bool UseDefaultCredentials
		{
			get;
			set;
		}

		Encoding Encoding
		{
			get;
			set;
		}

		IList<HttpHeader> Headers
		{
			get;
		}

		IList<HttpParameter> Parameters
		{
			get;
		}

		IList<HttpFile> Files
		{
			get;
		}

		IList<HttpCookie> Cookies
		{
			get;
		}

		string RequestBody
		{
			get;
			set;
		}

		string RequestContentType
		{
			get;
			set;
		}

		byte[] RequestBodyBytes
		{
			get;
			set;
		}

		Uri Url
		{
			get;
			set;
		}

		bool PreAuthenticate
		{
			get;
			set;
		}

		IWebProxy Proxy
		{
			get;
			set;
		}

		IHttpRequestAsyncHandle AsPostAsync(Action<IHttpResponse> action, string httpMethod);

		IHttpRequestAsyncHandle AsGetAsync(Action<IHttpResponse> action, string httpMethod);
	}
}
