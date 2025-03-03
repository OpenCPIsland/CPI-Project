using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Disney.Mix.SDK.Internal
{
	public class SystemWww : IWww, IDisposable
	{
		private UnityWebRequest www;

		private DownloadHandler downloadHandler;

		private readonly string url;

		private readonly HttpMethod method;

		private readonly byte[] requestBody;

		private readonly Dictionary<string, string> requestHeaders;

		public bool IsDone
		{
			get
			{
				return www.isDone;
			}
		}

		public float UploadProgress
		{
			get
			{
				return www.uploadProgress;
			}
		}

		public float DownloadProgress
		{
			get
			{
				return www.downloadProgress;
			}
		}

		public string Error
		{
			get
			{
				return www.error;
			}
		}

		public byte[] Bytes
		{
			get
			{
				return www.downloadHandler.data;
			}
		}

		public Dictionary<string, string> ResponseHeaders
		{
			get
			{
				return www.GetResponseHeaders() ?? new Dictionary<string, string>();
			}
		}

		public uint StatusCode
		{
			get
			{
				return (uint)www.responseCode;
			}
		}

		public SystemWww(string url, HttpMethod method, byte[] requestBody, Dictionary<string, string> requestHeaders)
		{
			this.url = url;
			this.method = method;
			this.requestBody = requestBody;
			this.requestHeaders = requestHeaders;
		}

		public void Execute()
		{
			string httpMethodString = GetHttpMethodString(method);
			www = new UnityWebRequest(url, httpMethodString);
			if (method != 0)
			{
				UploadHandlerRaw uploadHandler = new UploadHandlerRaw(requestBody);
				www.uploadHandler = uploadHandler;
			}
			foreach (KeyValuePair<string, string> requestHeader in requestHeaders)
			{
				if (!string.IsNullOrEmpty(requestHeader.Value))
				{
					www.SetRequestHeader(requestHeader.Key, requestHeader.Value);
				}
			}
			downloadHandler = new DownloadHandlerBuffer();
			www.downloadHandler = downloadHandler;
			www.SendWebRequest();
		}

		public void Dispose()
		{
			www.Dispose();
		}

		private static string GetHttpMethodString(HttpMethod method)
		{
			switch (method)
			{
			case HttpMethod.GET:
				return "GET";
			case HttpMethod.PUT:
				return "PUT";
			case HttpMethod.POST:
				return "POST";
			default:
				return null;
			}
		}
	}
}
