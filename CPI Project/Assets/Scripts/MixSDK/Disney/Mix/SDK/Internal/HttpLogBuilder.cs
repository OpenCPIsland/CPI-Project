using System;
using System.Collections.Generic;
using System.Text;

namespace Disney.Mix.SDK.Internal
{
	public static class HttpLogBuilder
	{
		public static string BuildRequestLog(int requestId, Uri uri, HttpMethod method, Dictionary<string, string> headers, string body)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('#');
			stringBuilder.Append(requestId);
			stringBuilder.Append(' ');
			stringBuilder.Append("HTTP Request");
			stringBuilder.Append(":\n");
			stringBuilder.Append(method);
			stringBuilder.Append(' ');
			stringBuilder.Append(uri.AbsoluteUri);
			stringBuilder.Append('\n');
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					stringBuilder.Append(header.Key);
					stringBuilder.Append('=');
					stringBuilder.Append(header.Value);
					stringBuilder.Append('\n');
				}
				stringBuilder.Append('\n');
			}
			stringBuilder.Append(body);
			return stringBuilder.ToString();
		}

		public static string BuildResponseLog(int requestId, Uri uri, HttpMethod method, Dictionary<string, string> headers, string body, uint statusCode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('#');
			stringBuilder.Append(requestId);
			stringBuilder.Append(' ');
			stringBuilder.Append("HTTP Response ");
			stringBuilder.Append(statusCode);
			stringBuilder.Append(":\n");
			stringBuilder.Append(method);
			stringBuilder.Append(' ');
			stringBuilder.Append(uri.AbsoluteUri);
			stringBuilder.Append('\n');
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					stringBuilder.Append(header.Key);
					stringBuilder.Append('=');
					stringBuilder.Append(header.Value);
					stringBuilder.Append('\n');
				}
				stringBuilder.Append('\n');
			}
			stringBuilder.Append(body);
			return stringBuilder.ToString();
		}

		public static string BuildTimeoutLog(int requestId, Uri uri, HttpMethod method, Dictionary<string, string> headers, string body, long timeToStartUpload, long timeToFinishUpload, float percentUploaded, long timeToStartDownload, long timeToFinishDownload, float percentDownloaded, string timeoutReason, long timeoutTime)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('#');
			stringBuilder.Append(requestId);
			stringBuilder.Append(' ');
			stringBuilder.Append("HTTP Timeout:");
			stringBuilder.Append('\n');
			stringBuilder.Append(method);
			stringBuilder.Append(' ');
			stringBuilder.Append(uri.AbsoluteUri);
			stringBuilder.Append('\n');
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> header in headers)
				{
					stringBuilder.Append(header.Key);
					stringBuilder.Append('=');
					stringBuilder.Append(header.Value);
					stringBuilder.Append('\n');
				}
				stringBuilder.Append('\n');
			}
			stringBuilder.Append(body);
			stringBuilder.Append('\n');
			stringBuilder.Append("\nTime to start upload: ");
			stringBuilder.Append(timeToStartUpload);
			stringBuilder.Append("\nTime to finish upload: ");
			stringBuilder.Append(timeToFinishUpload);
			stringBuilder.Append("\nPercent uploaded: ");
			stringBuilder.Append(percentUploaded);
			stringBuilder.Append("\nTime to start download: ");
			stringBuilder.Append(timeToStartDownload);
			stringBuilder.Append("\nTime to finish download: ");
			stringBuilder.Append(timeToFinishDownload);
			stringBuilder.Append("\nPercent downloaded: ");
			stringBuilder.Append(percentDownloaded);
			stringBuilder.Append("\nTimeout time: ");
			stringBuilder.Append(timeoutTime);
			stringBuilder.Append("\nTimeout reason: ");
			stringBuilder.Append(timeoutReason);
			return stringBuilder.ToString();
		}
	}
}
