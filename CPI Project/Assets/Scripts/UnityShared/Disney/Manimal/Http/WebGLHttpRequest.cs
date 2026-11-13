using Disney.Manimal.Common.Diagnostics;
using Disney.Manimal.Http.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Disney.Manimal.Http
{
	public class WebGLHttpRequest : IHttpRequest
	{
		private class WebGLAsyncResult : IAsyncResult
		{
			private object asyncState;

			public object AsyncState
			{
				get
				{
					return asyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return null;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return true;
				}
			}

			public WebGLAsyncResult(object asyncState)
			{
				this.asyncState = asyncState;
			}
		}

		private const string LineBreak = "\r\n";

		private const string FormBoundary = "-----------------------------28947758029299";

		protected static readonly Disney.Manimal.Common.Diagnostics.ILogger logger = LogManager.GetLogger<HttpRequest>();

		private readonly IDictionary<string, Action<XmlHttpRequest, string>> _restrictedHeaderActions;

		private MonoBehaviour monoBehaviour;

		protected bool HasParameters
		{
			get
			{
				return Parameters.Count > 0;
			}
		}

		protected bool HasCookies
		{
			get
			{
				return Cookies.Count > 0;
			}
		}

		protected bool HasBody
		{
			get
			{
				return RequestBodyBytes != null || !string.IsNullOrEmpty(RequestBody);
			}
		}

		protected bool HasFiles
		{
			get
			{
				return Files.Count > 0;
			}
		}

		public Action<Stream> ResponseWriter
		{
			get;
			set;
		}

		public CookieContainer CookieContainer
		{
			get
			{
				throw new Exception("Operation not supported");
			}
			set
			{
				throw new Exception("Operation not supported");
			}
		}

		public ICredentials Credentials
		{
			get
			{
				throw new Exception("Operation not supported");
			}
			set
			{
				throw new Exception("Operation not supported");
			}
		}

		public bool AlwaysMultipartFormData
		{
			get;
			set;
		}

		public string UserAgent
		{
			get;
			set;
		}

		public int Timeout
		{
			get;
			set;
		}

		public int ReadWriteTimeout
		{
			get;
			set;
		}

		public bool FollowRedirects
		{
			get;
			set;
		}

		public X509CertificateCollection ClientCertificates
		{
			get;
			set;
		}

		public int? MaxRedirects
		{
			get;
			set;
		}

		public bool UseDefaultCredentials
		{
			get;
			set;
		}

		public Encoding Encoding
		{
			get;
			set;
		}

		public IList<HttpHeader> Headers
		{
			get;
			private set;
		}

		public IList<HttpParameter> Parameters
		{
			get;
			private set;
		}

		public IList<HttpFile> Files
		{
			get;
			private set;
		}

		public IList<HttpCookie> Cookies
		{
			get;
			private set;
		}

		public string RequestBody
		{
			get;
			set;
		}

		public string RequestContentType
		{
			get;
			set;
		}

		public byte[] RequestBodyBytes
		{
			get;
			set;
		}

		public Uri Url
		{
			get;
			set;
		}

		public bool PreAuthenticate
		{
			get;
			set;
		}

		public IWebProxy Proxy
		{
			get
			{
				throw new Exception("Operation not supported");
			}
			set
			{
				throw new Exception("Operation not supported");
			}
		}

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				throw new Exception("Operation not supported");
			}
			set
			{
				throw new Exception("Operation not supported");
			}
		}

		public uint RequestId
		{
			get;
			private set;
		}

		public MonoBehaviour MonoBehaviour
		{
			private get
			{
				return monoBehaviour;
			}
			set
			{
				monoBehaviour = value;
			}
		}

		public WebGLHttpRequest()
		{
			Encoding = Encoding.UTF8;
			Headers = new List<HttpHeader>();
			Files = new List<HttpFile>();
			Parameters = new List<HttpParameter>();
			Cookies = new List<HttpCookie>();
			_restrictedHeaderActions = new Dictionary<string, Action<XmlHttpRequest, string>>(StringComparer.OrdinalIgnoreCase);
			AddRestrictedHeaders();
		}

		public IHttpRequestAsyncHandle AsPostAsync(Action<IHttpResponse> action, string httpMethod)
		{
			if (monoBehaviour == null)
			{
				throw new InvalidOperationException("The MonoBehaviour property must be set before invoking WebGLHttpRequest.AsPostAsync()");
			}
			XmlHttpRequest webRequest = PutPostInternalAsync(httpMethod.ToUpperInvariant(), action);
			return new WebGLHttpRequestAsyncHandle(webRequest);
		}

		public IHttpRequestAsyncHandle AsGetAsync(Action<IHttpResponse> action, string httpMethod)
		{
			if (monoBehaviour == null)
			{
				throw new InvalidOperationException("The MonoBehaviour property must be set before invoking WebGLHttpRequest.AsPostAsync()");
			}
			XmlHttpRequest styleMethodInternalAsync = GetStyleMethodInternalAsync(httpMethod.ToUpperInvariant(), action);
			return new WebGLHttpRequestAsyncHandle(styleMethodInternalAsync);
		}

		private void AddRestrictedHeaders()
		{
		}

		private static string GetMultipartFormContentType()
		{
			return string.Format("multipart/form-data; boundary={0}", "-----------------------------28947758029299");
		}

		private static string GetMultipartFileHeader(HttpFile file)
		{
			return string.Format("--{0}{4}Content-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"{4}Content-Type: {3}{4}{4}", "-----------------------------28947758029299", file.Name, file.FileName, file.ContentType ?? "application/octet-stream", "\r\n");
		}

		private string GetMultipartFormData(HttpParameter param)
		{
			string format = (param.Name == RequestContentType) ? "--{0}{3}Content-Type: {1}{3}Content-Disposition: form-data; name=\"{1}\"{3}{3}{2}{3}" : "--{0}{3}Content-Disposition: form-data; name=\"{1}\"{3}{3}{2}{3}";
			return string.Format(format, "-----------------------------28947758029299", param.Name, param.Value, "\r\n");
		}

		private static string GetMultipartFooter()
		{
			return string.Format("--{0}--{1}", "-----------------------------28947758029299", "\r\n");
		}

		private void AppendHeaders(XmlHttpRequest webRequest)
		{
			foreach (HttpHeader header in Headers)
			{
				if (string.Equals(header.Name, "Accept", StringComparison.OrdinalIgnoreCase))
				{
					webRequest.RequestHeaders.Add(header.Name, header.Value + ";q=0.9,*/*;q=0.8");
				}
				else if (_restrictedHeaderActions.ContainsKey(header.Name))
				{
					_restrictedHeaderActions[header.Name](webRequest, header.Value);
				}
				else
				{
					webRequest.RequestHeaders.Add(header.Name, header.Value);
				}
			}
		}

		private void AppendCookies(XmlHttpRequest webRequest)
		{
		}

		private string EncodeParameters()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (HttpParameter parameter in Parameters)
			{
				if (stringBuilder.Length > 1)
				{
					stringBuilder.Append("&");
				}
				stringBuilder.AppendFormat("{0}={1}", parameter.Name.UrlEncode(), parameter.Value.UrlEncode());
			}
			return stringBuilder.ToString();
		}

		private void PreparePostBody(XmlHttpRequest webRequest)
		{
			if (HasFiles || AlwaysMultipartFormData)
			{
				webRequest.ContentType = GetMultipartFormContentType();
			}
			else if (HasParameters)
			{
				webRequest.ContentType = "application/x-www-form-urlencoded";
				RequestBody = EncodeParameters();
			}
			else if (HasBody)
			{
				webRequest.ContentType = RequestContentType;
			}
		}

		private void WriteStringTo(Stream stream, string toWrite)
		{
			byte[] bytes = Encoding.GetBytes(toWrite);
			stream.Write(bytes, 0, bytes.Length);
		}

		private void WriteMultipartFormData(Stream requestStream)
		{
			foreach (HttpParameter parameter in Parameters)
			{
				WriteStringTo(requestStream, GetMultipartFormData(parameter));
			}
			foreach (HttpFile file in Files)
			{
				WriteStringTo(requestStream, GetMultipartFileHeader(file));
				file.Writer(requestStream);
				WriteStringTo(requestStream, "\r\n");
			}
			WriteStringTo(requestStream, GetMultipartFooter());
		}

		private void ExtractResponseData(WebGLHttpResponse response, XmlHttpRequest webResponse)
		{
			if (webResponse.ResponseHeaders.ContainsKey("Content-Encoding"))
			{
				response.ContentEncoding = webResponse.ResponseHeaders["Content-Encoding"];
			}
			if (webResponse.ResponseHeaders.ContainsKey("Server"))
			{
				response.Server = webResponse.ResponseHeaders["Server"];
			}
			if (webResponse.ResponseHeaders.ContainsKey("Content-Type"))
			{
				response.ContentType = webResponse.ResponseHeaders["Content-Type"];
			}
			if (webResponse.ResponseHeaders.ContainsKey("Content-Length"))
			{
				response.ContentLength = Convert.ToInt64(webResponse.ResponseHeaders["Content-Length"]);
			}
			response.RawBytes = webResponse.Response;
			response.StatusCode = (HttpStatusCode)webResponse.Status;
			response.StatusDescription = webResponse.StatusLine;
			response.ResponseUri = webResponse.Url;
			response.ResponseStatus = ResponseStatus.Completed;
			foreach (string key in webResponse.ResponseHeaders.Keys)
			{
				string value = webResponse.ResponseHeaders[key];
				response.Headers.Add(new HttpHeader
				{
					Name = key,
					Value = value
				});
			}
		}

		private void AddRange(XmlHttpRequest r, string range)
		{
			r.RequestHeaders.Add("Range", range);
		}

		private XmlHttpRequest GetStyleMethodInternalAsync(string method, Action<IHttpResponse> callback)
		{
			XmlHttpRequest xmlHttpRequest = null;
			try
			{
				xmlHttpRequest = ConfigureWebRequest(method, Url);
				if (HasBody && (method == "DELETE" || method == "OPTIONS"))
				{
					xmlHttpRequest.ContentType = RequestContentType;
					WriteRequestBodyAsync(xmlHttpRequest, callback);
				}
				else
				{
					LogHttpRequest(xmlHttpRequest);
					xmlHttpRequest.Send();
					monoBehaviour.StartCoroutine(GetResponse(callback, xmlHttpRequest));
				}
			}
			catch (Exception ex)
			{
				ExecuteCallback(CreateErrorResponse(ex), callback);
			}
			return xmlHttpRequest;
		}

		private IEnumerator GetResponse(Action<IHttpResponse> callback, XmlHttpRequest webRequest)
		{
			while (!webRequest.IsComplete)
			{
				yield return null;
			}
			IAsyncResult result = new WebGLAsyncResult(webRequest);
			ResponseCallback(result, callback);
		}

		private WebGLHttpResponse CreateErrorResponse(Exception ex)
		{
			WebGLHttpResponse webGLHttpResponse = new WebGLHttpResponse();
			webGLHttpResponse.ResponseStatus = ResponseStatus.Error;
			return webGLHttpResponse;
		}

		private XmlHttpRequest PutPostInternalAsync(string method, Action<IHttpResponse> callback)
		{
			XmlHttpRequest xmlHttpRequest = null;
			try
			{
				xmlHttpRequest = ConfigureWebRequest(method, Url);
				PreparePostBody(xmlHttpRequest);
				LogHttpRequest(xmlHttpRequest);
				WriteRequestBodyAsync(xmlHttpRequest, callback);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				ExecuteCallback(CreateErrorResponse(ex), callback);
			}
			return xmlHttpRequest;
		}

		private void WriteRequestBodyAsync(XmlHttpRequest webRequest, Action<IHttpResponse> callback)
		{
			if (HasBody || HasFiles || AlwaysMultipartFormData)
			{
				if (RequestBodyBytes != null)
				{
					webRequest.Send(RequestBodyBytes);
				}
				else if (RequestBody != null)
				{
					webRequest.SendText(RequestBody);
				}
				else
				{
					webRequest.Send();
				}
				monoBehaviour.StartCoroutine(GetResponse(callback, webRequest));
			}
			else
			{
				webRequest.Send();
				monoBehaviour.StartCoroutine(GetResponse(callback, webRequest));
			}
		}

		private long CalculateContentLength()
		{
			if (RequestBodyBytes != null)
			{
				return RequestBodyBytes.Length;
			}
			if (!HasFiles && !AlwaysMultipartFormData)
			{
				return Encoding.GetByteCount(RequestBody);
			}
			long num = 0L;
			foreach (HttpFile file in Files)
			{
				num += Encoding.GetByteCount(GetMultipartFileHeader(file));
				num += file.ContentLength;
				num += Encoding.GetByteCount("\r\n");
			}
			foreach (HttpParameter parameter in Parameters)
			{
				num += Encoding.GetByteCount(GetMultipartFormData(parameter));
			}
			return num + Encoding.GetByteCount(GetMultipartFooter());
		}

		private void RequestStreamCallback(IAsyncResult result, Action<IHttpResponse> callback)
		{
		}

		private static void GetRawResponseAsync(IAsyncResult result, Action<XmlHttpRequest> callback)
		{
			XmlHttpRequest xmlHttpRequest = (XmlHttpRequest)result.AsyncState;
			XmlHttpRequest obj = xmlHttpRequest;
			callback(obj);
		}

		private void ResponseCallback(IAsyncResult result, Action<IHttpResponse> callback)
		{
			WebGLHttpResponse response = new WebGLHttpResponse
			{
				ResponseStatus = ResponseStatus.None,
				RequestId = RequestId
			};
			try
			{
				GetRawResponseAsync(result, delegate(XmlHttpRequest webResponse)
				{
					ExtractResponseData(response, webResponse);
					logger.Debug(response);
					ExecuteCallback(response, callback);
				});
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				ExecuteCallback(CreateErrorResponse(ex), callback);
			}
		}

		private static void ExecuteCallback(WebGLHttpResponse response, Action<IHttpResponse> callback)
		{
			PopulateErrorForIncompleteResponse(response);
			callback(response);
		}

		private static void PopulateErrorForIncompleteResponse(WebGLHttpResponse response)
		{
			if (response.ResponseStatus != ResponseStatus.Completed && response.ErrorException == null)
			{
				response.ErrorException = response.ResponseStatus.ToWebException();
				response.ErrorMessage = response.ErrorException.Message;
			}
		}

		private XmlHttpRequest ConfigureWebRequest(string method, Uri url)
		{
			XmlHttpRequest xmlHttpRequest = new XmlHttpRequest(url, method);
			RequestId = (uint)xmlHttpRequest.RequestId;
			AppendHeaders(xmlHttpRequest);
			AppendCookies(xmlHttpRequest);
			if (!HasFiles && !AlwaysMultipartFormData)
			{
			}
			if (!string.IsNullOrEmpty(UserAgent))
			{
				xmlHttpRequest.UserAgent = UserAgent;
			}
			if (Timeout != 0)
			{
				xmlHttpRequest.Timeout = Timeout;
			}
			if (ReadWriteTimeout != 0)
			{
			}
			if (FollowRedirects && MaxRedirects.HasValue)
			{
			}
			return xmlHttpRequest;
		}

		private void LogHttpRequest(XmlHttpRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("HttpRequest ").Append(RequestId).AppendLine();
			stringBuilder.Append("Uri: ").AppendLine(request.Url.AbsoluteUri);
			stringBuilder.Append("Method: ").AppendLine(request.Method);
			stringBuilder.AppendLine("Headers:");
			foreach (string key in request.RequestHeaders.Keys)
			{
				stringBuilder.Append(key).Append(": ").AppendLine(request.RequestHeaders[key]);
			}
			logger.DebugFormat("{0:g}", stringBuilder);
		}

		private void LogHttpRequestBody(XmlHttpRequest request, string body)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("HttpRequest ").Append(RequestId).AppendLine();
			stringBuilder.AppendLine("Body:");
			stringBuilder.AppendLine(body);
			logger.DebugFormat("{0:g}", stringBuilder);
		}
	}
}
