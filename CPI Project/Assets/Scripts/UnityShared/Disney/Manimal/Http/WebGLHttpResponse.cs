using Disney.Manimal.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Disney.Manimal.Http
{
	public class WebGLHttpResponse : IHttpResponse
	{
		private string _content;

		private ResponseStatus _responseStatus = ResponseStatus.None;

		public string ContentType
		{
			get;
			set;
		}

		public long ContentLength
		{
			get;
			set;
		}

		public string ContentEncoding
		{
			get;
			set;
		}

		public string Content
		{
			get
			{
				return _content ?? (_content = RawBytes.AsString());
			}
		}

		public HttpStatusCode StatusCode
		{
			get;
			set;
		}

		public string StatusDescription
		{
			get;
			set;
		}

		public byte[] RawBytes
		{
			get;
			set;
		}

		public Uri ResponseUri
		{
			get;
			set;
		}

		public string Server
		{
			get;
			set;
		}

		public IList<HttpHeader> Headers
		{
			get;
			private set;
		}

		public IList<HttpCookie> Cookies
		{
			get;
			private set;
		}

		public ResponseStatus ResponseStatus
		{
			get
			{
				return _responseStatus;
			}
			set
			{
				_responseStatus = value;
			}
		}

		public string ErrorMessage
		{
			get;
			set;
		}

		public Exception ErrorException
		{
			get;
			set;
		}

		public uint RequestId
		{
			get;
			set;
		}

		public WebGLHttpResponse()
		{
			Headers = new List<HttpHeader>();
			Cookies = new List<HttpCookie>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("HttpResponse ").Append(RequestId).AppendLine();
			stringBuilder.Append("Status: ").Append(StatusCode).AppendLine();
			stringBuilder.Append("Uri: ").AppendLine(ResponseUri.AbsoluteUri);
			stringBuilder.AppendLine("Headers:");
			foreach (HttpHeader header in Headers)
			{
				stringBuilder.Append(header.Name).Append(": ").AppendLine(header.Value);
			}
			if (Content != null)
			{
				stringBuilder.AppendLine("Content:");
				stringBuilder.AppendLine(Content);
			}
			return stringBuilder.ToString();
		}
	}
}
