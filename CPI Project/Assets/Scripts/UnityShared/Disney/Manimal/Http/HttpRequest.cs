using Disney.Manimal.Common.Diagnostics;
using Disney.Manimal.Common.Util;
using Disney.Manimal.Http.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Disney.Manimal.Http
{
	public class HttpRequest : IHttpRequest
	{
		private class TimeOutState
		{
			public bool TimedOut
			{
				get;
				set;
			}

			public HttpWebRequest Request
			{
				get;
				set;
			}
		}

		private const string LineBreak = "\r\n";

		private const string FormBoundary = "-----------------------------28947758029299";

		protected static readonly ILogger logger = LogManager.GetLogger<HttpRequest>();

		private readonly IDictionary<string, Action<HttpWebRequest, string>> _restrictedHeaderActions;

		private TimeOutState _timeoutState;

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
			get;
			set;
		}

		public ICredentials Credentials
		{
			get;
			set;
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
			get;
			set;
		}

		public DecompressionMethods AutomaticDecompression
		{
			get;
			set;
		}

		public uint RequestId
		{
			get;
			private set;
		}

		public HttpRequest()
		{
			Encoding = Encoding.UTF8;
			Headers = new List<HttpHeader>();
			Files = new List<HttpFile>();
			Parameters = new List<HttpParameter>();
			Cookies = new List<HttpCookie>();
			RequestId = IdGenerator<HttpRequest>.Get();
			_restrictedHeaderActions = new Dictionary<string, Action<HttpWebRequest, string>>(StringComparer.OrdinalIgnoreCase);
			AddRestrictedHeaders();
		}

		public IHttpRequestAsyncHandle AsPostAsync(Action<IHttpResponse> action, string httpMethod)
		{
			HttpWebRequest webRequest = PutPostInternalAsync(httpMethod.ToUpperInvariant(), action);
			return new HttpRequestAsyncHandle(webRequest);
		}

		public IHttpRequestAsyncHandle AsGetAsync(Action<IHttpResponse> action, string httpMethod)
		{
			HttpWebRequest styleMethodInternalAsync = GetStyleMethodInternalAsync(httpMethod.ToUpperInvariant(), action);
			return new HttpRequestAsyncHandle(styleMethodInternalAsync);
		}

		private void AddRestrictedHeaders()
		{
			_restrictedHeaderActions.Add("Accept", delegate(HttpWebRequest r, string v)
			{
				r.Accept = v;
			});
			_restrictedHeaderActions.Add("Connection", delegate(HttpWebRequest r, string v)
			{
				r.Connection = v;
			});
			_restrictedHeaderActions.Add("Content-Length", delegate(HttpWebRequest r, string v)
			{
				r.ContentLength = Convert.ToInt64(v);
			});
			_restrictedHeaderActions.Add("Content-Type", delegate(HttpWebRequest r, string v)
			{
				r.ContentType = v;
			});
			_restrictedHeaderActions.Add("Expect", delegate(HttpWebRequest r, string v)
			{
				r.Expect = v;
			});
			_restrictedHeaderActions.Add("Date", delegate
			{
			});
			_restrictedHeaderActions.Add("Host", delegate
			{
			});
			_restrictedHeaderActions.Add("If-Modified-Since", delegate(HttpWebRequest r, string v)
			{
				r.IfModifiedSince = Convert.ToDateTime(v);
			});
			_restrictedHeaderActions.Add("Range", AddRange);
			_restrictedHeaderActions.Add("Referer", delegate(HttpWebRequest r, string v)
			{
				r.Referer = v;
			});
			_restrictedHeaderActions.Add("Transfer-Encoding", delegate(HttpWebRequest r, string v)
			{
				r.TransferEncoding = v;
				r.SendChunked = true;
			});
			_restrictedHeaderActions.Add("User-Agent", delegate(HttpWebRequest r, string v)
			{
				r.UserAgent = v;
			});
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

		private void AppendHeaders(HttpWebRequest webRequest)
		{
			foreach (HttpHeader header in Headers)
			{
				if (_restrictedHeaderActions.ContainsKey(header.Name))
				{
					_restrictedHeaderActions[header.Name](webRequest, header.Value);
				}
				else
				{
					webRequest.Headers.Add(header.Name, header.Value);
				}
			}
		}

		private void AppendCookies(HttpWebRequest webRequest)
		{
			webRequest.CookieContainer = (CookieContainer ?? new CookieContainer());
			foreach (HttpCookie cooky in Cookies)
			{
				Cookie cookie = new Cookie();
				cookie.Name = cooky.Name;
				cookie.Value = cooky.Value;
				cookie.Domain = webRequest.RequestUri.Host;
				Cookie cookie2 = cookie;
				webRequest.CookieContainer.Add(cookie2);
			}
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

		private void PreparePostBody(HttpWebRequest webRequest)
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

		private void ExtractResponseData(HttpResponse response, HttpWebResponse webResponse)
		{
			using (webResponse)
			{
				response.ContentEncoding = webResponse.ContentEncoding;
				response.Server = webResponse.Server;
				response.ContentType = webResponse.ContentType;
				response.ContentLength = webResponse.ContentLength;
				Stream responseStream = webResponse.GetResponseStream();
				ProcessResponseStream(responseStream, response);
				response.StatusCode = webResponse.StatusCode;
				response.StatusDescription = webResponse.StatusDescription;
				response.ResponseUri = webResponse.ResponseUri;
				response.ResponseStatus = ResponseStatus.Completed;
				if (webResponse.Cookies != null)
				{
					foreach (Cookie cooky in webResponse.Cookies)
					{
						response.Cookies.Add(new HttpCookie
						{
							Comment = cooky.Comment,
							CommentUri = cooky.CommentUri,
							Discard = cooky.Discard,
							Domain = cooky.Domain,
							Expired = cooky.Expired,
							Expires = cooky.Expires,
							HttpOnly = cooky.HttpOnly,
							Name = cooky.Name,
							Path = cooky.Path,
							Port = cooky.Port,
							Secure = cooky.Secure,
							TimeStamp = cooky.TimeStamp,
							Value = cooky.Value,
							Version = cooky.Version
						});
					}
				}
				string[] allKeys = webResponse.Headers.AllKeys;
				foreach (string name in allKeys)
				{
					string value = webResponse.Headers[name];
					response.Headers.Add(new HttpHeader
					{
						Name = name,
						Value = value
					});
				}
			}
		}

		private void ProcessResponseStream(Stream webResponseStream, HttpResponse response)
		{
			if (ResponseWriter == null)
			{
				response.RawBytes = webResponseStream.ReadAsBytes();
			}
			else
			{
				ResponseWriter(webResponseStream);
			}
		}

		private void AddRange(HttpWebRequest r, string range)
		{
			Match match = Regex.Match(range, "=(\\d+)-(\\d+)$");
			if (match.Success)
			{
				int from = Convert.ToInt32(match.Groups[1].Value);
				int to = Convert.ToInt32(match.Groups[2].Value);
				r.AddRange(from, to);
			}
		}

		private HttpWebRequest GetStyleMethodInternalAsync(string method, Action<IHttpResponse> callback)
		{
			HttpWebRequest httpWebRequest = null;
			try
			{
				httpWebRequest = ConfigureWebRequest(method, Url);
				if (HasBody && (method == "DELETE" || method == "OPTIONS"))
				{
					httpWebRequest.ContentType = RequestContentType;
					WriteRequestBodyAsync(httpWebRequest, callback);
				}
				else
				{
					_timeoutState = new TimeOutState
					{
						Request = httpWebRequest
					};
					LogHttpRequest(httpWebRequest);
					IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(delegate(IAsyncResult result)
					{
						ResponseCallback(result, callback);
					}, httpWebRequest);
					SetTimeout(asyncResult, _timeoutState);
				}
			}
			catch (Exception ex)
			{
				ExecuteCallback(CreateErrorResponse(ex), callback);
			}
			return httpWebRequest;
		}

		private HttpResponse CreateErrorResponse(Exception ex)
		{
			HttpResponse httpResponse = new HttpResponse();
			WebException ex2 = ex as WebException;
			if (ex2 != null && ex2.Status == WebExceptionStatus.RequestCanceled)
			{
				httpResponse.ResponseStatus = (_timeoutState.TimedOut ? ResponseStatus.TimedOut : ResponseStatus.Aborted);
				return httpResponse;
			}
			httpResponse.ErrorMessage = ex.Message;
			httpResponse.ErrorException = ex;
			httpResponse.ResponseStatus = ResponseStatus.Error;
			return httpResponse;
		}

		private HttpWebRequest PutPostInternalAsync(string method, Action<IHttpResponse> callback)
		{
			HttpWebRequest httpWebRequest = null;
			try
			{
				httpWebRequest = ConfigureWebRequest(method, Url);
				PreparePostBody(httpWebRequest);
				LogHttpRequest(httpWebRequest);
				WriteRequestBodyAsync(httpWebRequest, callback);
			}
			catch (Exception ex)
			{
				ExecuteCallback(CreateErrorResponse(ex), callback);
			}
			return httpWebRequest;
		}

		private void WriteRequestBodyAsync(HttpWebRequest webRequest, Action<IHttpResponse> callback)
		{
			_timeoutState = new TimeOutState
			{
				Request = webRequest
			};
			IAsyncResult asyncResult;
			if (HasBody || HasFiles || AlwaysMultipartFormData)
			{
				webRequest.ContentLength = CalculateContentLength();
				asyncResult = webRequest.BeginGetRequestStream(delegate(IAsyncResult result)
				{
					RequestStreamCallback(result, callback);
				}, webRequest);
			}
			else
			{
				asyncResult = webRequest.BeginGetResponse(delegate(IAsyncResult r)
				{
					ResponseCallback(r, callback);
				}, webRequest);
			}
			SetTimeout(asyncResult, _timeoutState);
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
			HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
			if (_timeoutState.TimedOut)
			{
				HttpResponse httpResponse = new HttpResponse();
				httpResponse.ResponseStatus = ResponseStatus.TimedOut;
				HttpResponse response = httpResponse;
				ExecuteCallback(response, callback);
			}
			else
			{
				try
				{
					using (Stream stream = httpWebRequest.EndGetRequestStream(result))
					{
						if (HasFiles || AlwaysMultipartFormData)
						{
							WriteMultipartFormData(stream);
						}
						else if (RequestBodyBytes != null)
						{
							stream.Write(RequestBodyBytes, 0, RequestBodyBytes.Length);
						}
						else
						{
							LogHttpRequestBody(httpWebRequest, RequestBody);
							WriteStringTo(stream, RequestBody);
						}
					}
				}
				catch (Exception ex)
				{
					ExecuteCallback(CreateErrorResponse(ex), callback);
					return;
				}
				IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(delegate(IAsyncResult r)
				{
					ResponseCallback(r, callback);
				}, httpWebRequest);
				SetTimeout(asyncResult, _timeoutState);
			}
		}

		private void SetTimeout(IAsyncResult asyncResult, TimeOutState timeOutState)
		{
			if (Timeout != 0)
			{
				ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, TimeoutCallback, timeOutState, Timeout, true);
			}
		}

		private static void TimeoutCallback(object state, bool timedOut)
		{
			if (!timedOut)
			{
				return;
			}
			TimeOutState timeOutState = state as TimeOutState;
			if (timeOutState != null)
			{
				lock (timeOutState)
				{
					timeOutState.TimedOut = true;
				}
				if (timeOutState.Request != null)
				{
					timeOutState.Request.Abort();
				}
			}
		}

		private static void GetRawResponseAsync(IAsyncResult result, Action<HttpWebResponse> callback)
		{
			HttpWebResponse httpWebResponse;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
				httpWebResponse = (httpWebRequest.EndGetResponse(result) as HttpWebResponse);
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.RequestCanceled)
				{
					throw;
				}
				HttpWebResponse httpWebResponse2 = ex.Response as HttpWebResponse;
				if (httpWebResponse2 == null)
				{
					throw;
				}
				httpWebResponse = httpWebResponse2;
			}
			callback(httpWebResponse);
			if (httpWebResponse != null)
			{
				httpWebResponse.Close();
			}
		}

		private void ResponseCallback(IAsyncResult result, Action<IHttpResponse> callback)
		{
			HttpResponse response = new HttpResponse
			{
				ResponseStatus = ResponseStatus.None,
				RequestId = RequestId
			};
			try
			{
				if (_timeoutState.TimedOut)
				{
					response.ResponseStatus = ResponseStatus.TimedOut;
					ExecuteCallback(response, callback);
				}
				else
				{
					GetRawResponseAsync(result, delegate(HttpWebResponse webResponse)
					{
						ExtractResponseData(response, webResponse);
						logger.Debug(response);
						ExecuteCallback(response, callback);
					});
				}
			}
			catch (Exception ex)
			{
				ExecuteCallback(CreateErrorResponse(ex), callback);
			}
		}

		private static void ExecuteCallback(HttpResponse response, Action<IHttpResponse> callback)
		{
			PopulateErrorForIncompleteResponse(response);
			callback(response);
		}

		private static void PopulateErrorForIncompleteResponse(HttpResponse response)
		{
			if (response.ResponseStatus != ResponseStatus.Completed && response.ErrorException == null)
			{
				response.ErrorException = response.ResponseStatus.ToWebException();
				response.ErrorMessage = response.ErrorException.Message;
			}
		}

		private HttpWebRequest ConfigureWebRequest(string method, Uri url)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.UseDefaultCredentials = UseDefaultCredentials;
			httpWebRequest.PreAuthenticate = PreAuthenticate;
			AppendHeaders(httpWebRequest);
			AppendCookies(httpWebRequest);
			httpWebRequest.Method = method;
			if (!HasFiles && !AlwaysMultipartFormData)
			{
				httpWebRequest.ContentLength = 0L;
			}
			if (Credentials != null)
			{
				httpWebRequest.Credentials = Credentials;
			}
			if (!string.IsNullOrEmpty(UserAgent))
			{
				httpWebRequest.UserAgent = UserAgent;
			}
			if (ClientCertificates != null)
			{
				httpWebRequest.ClientCertificates.AddRange(ClientCertificates);
			}
			httpWebRequest.AutomaticDecompression = AutomaticDecompression;
			ServicePointManager.Expect100Continue = false;
			if (Timeout != 0)
			{
				httpWebRequest.Timeout = Timeout;
			}
			if (ReadWriteTimeout != 0)
			{
				httpWebRequest.ReadWriteTimeout = ReadWriteTimeout;
			}
			if (Proxy != null)
			{
				httpWebRequest.Proxy = Proxy;
			}
			if (FollowRedirects && MaxRedirects.HasValue)
			{
				httpWebRequest.MaximumAutomaticRedirections = MaxRedirects.Value;
			}
			httpWebRequest.AllowAutoRedirect = FollowRedirects;
			return httpWebRequest;
		}

		private void LogHttpRequest(HttpWebRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("HttpRequest ").Append(RequestId).AppendLine();
			stringBuilder.Append("Uri: ").AppendLine(request.RequestUri.AbsoluteUri);
			stringBuilder.Append("Method: ").AppendLine(request.Method);
			stringBuilder.AppendLine("Headers:");
			string[] allKeys = request.Headers.AllKeys;
			foreach (string text in allKeys)
			{
				stringBuilder.Append(text).Append(": ").AppendLine(request.Headers[text]);
			}
			logger.DebugFormat("{0:g}", stringBuilder);
		}

		private void LogHttpRequestBody(HttpWebRequest request, string body)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("HttpRequest ").Append(RequestId).AppendLine();
			stringBuilder.AppendLine("Body:");
			stringBuilder.AppendLine(body);
			logger.DebugFormat("{0:g}", stringBuilder);
		}
	}
}
