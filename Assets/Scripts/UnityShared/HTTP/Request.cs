using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HTTP
{
	public class Request
	{
		public static bool LogAllRequests = false;

		public static bool VerboseLogging = false;

		public CookieJar cookieJar = CookieJar.Instance;

		public string method = "GET";

		public string protocol = "HTTP/1.1";

		public byte[] bytes;

		public Uri uri;

		public static byte[] EOL = new byte[2]
		{
			13,
			10
		};

		public Response response = null;

		public bool isDone = false;

		public int maximumRetryCount = 8;

		public bool acceptGzip = true;

		public bool useCache = false;

		public Exception exception = null;

		public RequestState state = RequestState.Waiting;

		public long responseTime = 0L;

		public bool synchronous = false;

		public Action<Request> completedCallback = null;

		private Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

		private static Dictionary<string, string> etags = new Dictionary<string, string>();

		private static string[] sizes = new string[4]
		{
			"B",
			"KB",
			"MB",
			"GB"
		};

		public string Text
		{
			set
			{
				bytes = Encoding.UTF8.GetBytes(value);
			}
		}

		public Request(string method, string uri)
		{
			this.method = method;
			this.uri = new Uri(uri);
		}

		public Request(string method, string uri, bool useCache)
		{
			this.method = method;
			this.uri = new Uri(uri);
			this.useCache = useCache;
		}

		public Request(string method, string uri, byte[] bytes)
		{
			this.method = method;
			this.uri = new Uri(uri);
			this.bytes = bytes;
		}

		public Request(string method, string uri, WWWForm form)
		{
			this.method = method;
			this.uri = new Uri(uri);
			bytes = form.data;
			foreach (KeyValuePair<string, string> header in form.headers)
			{
				AddHeader(header.Key, header.Value);
			}
		}

		public Request(string method, string uri, Hashtable data)
		{
			this.method = method;
			this.uri = new Uri(uri);
			bytes = Encoding.UTF8.GetBytes(JSON.JsonEncode(data));
			AddHeader("Content-Type", "application/json");
		}

		public void AddHeader(string name, string value)
		{
			name = name.ToLower().Trim();
			value = value.Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			headers[name].Add(value);
		}

		public string GetHeader(string name)
		{
			name = name.ToLower().Trim();
			if (!headers.ContainsKey(name))
			{
				return "";
			}
			return headers[name][0];
		}

		public List<string> GetHeaders()
		{
			List<string> list = new List<string>();
			foreach (string key in headers.Keys)
			{
				foreach (string item in headers[key])
				{
					list.Add(key + ": " + item);
				}
			}
			return list;
		}

		public List<string> GetHeaders(string name)
		{
			name = name.ToLower().Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			return headers[name];
		}

		public void SetHeader(string name, string value)
		{
			name = name.ToLower().Trim();
			value = value.Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			headers[name].Clear();
			headers[name].TrimExcess();
			headers[name].Add(value);
		}

		private void GetResponse()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				int num = 0;
				while (++num < maximumRetryCount)
				{
					if (useCache)
					{
						string value = "";
						if (etags.TryGetValue(uri.AbsoluteUri, out value))
						{
							SetHeader("If-None-Match", value);
						}
					}
					SetHeader("Host", uri.Host);
					TcpClient tcpClient = new TcpClient();
					tcpClient.Connect(uri.Host, uri.Port);
					using (NetworkStream networkStream = tcpClient.GetStream())
					{
						Stream stream = networkStream;
						if (uri.Scheme.ToLower() == "https")
						{
							stream = new SslStream(networkStream, false, ValidateServerCertificate);
							try
							{
								SslStream sslStream = stream as SslStream;
								sslStream.AuthenticateAsClient(uri.Host);
							}
							catch (Exception ex)
							{
								UnityEngine.Debug.LogError("Exception: " + ex.Message);
								return;
							}
						}
						WriteToStream(stream);
						response = new Response();
						response.request = this;
						state = RequestState.Reading;
						response.ReadFromStream(stream);
					}
					tcpClient.Close();
					switch (response.status)
					{
					case 301:
					case 302:
					case 307:
						uri = new Uri(response.GetHeader("Location"));
						break;
					default:
						num = maximumRetryCount;
						break;
					}
				}
				if (useCache)
				{
					string value = response.GetHeader("etag");
					if (value.Length > 0)
					{
						etags[uri.AbsoluteUri] = value;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unhandled Exception, aborting request.");
				Console.WriteLine(ex);
				exception = ex;
				response = null;
			}
			state = RequestState.Done;
			isDone = true;
			responseTime = stopwatch.ElapsedMilliseconds;
			if (completedCallback != null)
			{
				if (synchronous)
				{
					completedCallback(this);
				}
				else
				{
					ResponseCallbackDispatcher.Singleton.requests.Enqueue(this);
				}
			}
			if (LogAllRequests)
			{
				Console.WriteLine("NET: " + InfoString(VerboseLogging));
			}
		}

		public virtual void Send(Action<Request> callback = null)
		{
			if (!synchronous && callback != null && ResponseCallbackDispatcher.Singleton == null)
			{
				ResponseCallbackDispatcher.Init();
			}
			completedCallback = callback;
			isDone = false;
			state = RequestState.Waiting;
			if (acceptGzip)
			{
				SetHeader("Accept-Encoding", "gzip");
			}
			if (cookieJar != null)
			{
				List<Cookie> cookies = cookieJar.GetCookies(new CookieAccessInfo(uri.Host, uri.AbsolutePath));
				string text = GetHeader("cookie");
				for (int i = 0; i < cookies.Count; i++)
				{
					if (text.Length > 0 && text[text.Length - 1] != ';')
					{
						text += ';';
					}
					object obj = text;
					text = string.Concat(obj, cookies[i].name, '=', cookies[i].value, ';');
				}
				SetHeader("cookie", text);
			}
			if (bytes != null && bytes.Length > 0 && GetHeader("Content-Length") == "")
			{
				SetHeader("Content-Length", bytes.Length.ToString());
			}
			if (GetHeader("User-Agent") == "")
			{
				SetHeader("User-Agent", "UnityWeb 1.0 ( Unity 4.6 ) ( Mac OS X 10.9 )");
			}
			if (GetHeader("Connection") == "")
			{
				SetHeader("Connection", "close");
			}
			if (!string.IsNullOrEmpty(uri.UserInfo))
			{
				SetHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(uri.UserInfo)));
			}
			if (synchronous)
			{
				GetResponse();
			}
			else
			{
				ThreadPool.QueueUserWorkItem(delegate
				{
					GetResponse();
				});
			}
		}

		public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			Console.WriteLine("NET: SSL Cert: " + sslPolicyErrors);
			return true;
		}

		private void WriteToStream(Stream outputStream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(outputStream);
			binaryWriter.Write(Encoding.ASCII.GetBytes(method.ToUpper() + " " + uri.PathAndQuery + " " + protocol));
			binaryWriter.Write(EOL);
			foreach (string key in headers.Keys)
			{
				foreach (string item in headers[key])
				{
					binaryWriter.Write(Encoding.ASCII.GetBytes(key));
					binaryWriter.Write(':');
					binaryWriter.Write(Encoding.ASCII.GetBytes(item));
					binaryWriter.Write(EOL);
				}
			}
			binaryWriter.Write(EOL);
			if (bytes != null && bytes.Length > 0)
			{
				binaryWriter.Write(bytes);
			}
		}

		public string InfoString(bool verbose)
		{
			string text = (isDone && response != null) ? response.status.ToString() : "---";
			string text2 = (isDone && response != null) ? response.message : "Unknown";
			double num = (isDone && response != null && response.bytes != null) ? ((float)response.bytes.Length) : 0f;
			int num2 = 0;
			while (num >= 1024.0 && num2 + 1 < sizes.Length)
			{
				num2++;
				num /= 1024.0;
			}
			string text3 = string.Format("{0:0.##}{1}", num, sizes[num2]);
			string text4 = uri.ToString() + " [ " + method.ToUpper() + " ] [ " + text + " " + text2 + " ] [ " + text3 + " ] [ " + responseTime + "ms ]";
			if (verbose && response != null)
			{
				text4 = text4 + "\n\nRequest Headers:\n\n" + string.Join("\n", GetHeaders().ToArray());
				text4 = text4 + "\n\nResponse Headers:\n\n" + string.Join("\n", response.GetHeaders().ToArray());
				if (response.Text != null)
				{
					text4 = text4 + "\n\nResponse Body:\n" + response.Text;
				}
			}
			return text4;
		}
	}
}
