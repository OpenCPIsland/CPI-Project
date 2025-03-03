using SwrveUnity.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SwrveUnity.REST
{
	public class RESTClient : IRESTClient
	{
		private const string CONTENT_ENCODING_HEADER_KEY = "CONTENT-ENCODING";

		private List<string> metrics = new List<string>();

		public virtual IEnumerator Get(string url, Action<RESTResponse> listener)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>();
			if (!Application.isEditor)
			{
				headers = AddMetricsHeader(headers);
			}
			long start = SwrveHelper.GetMilliseconds();
			using (UnityWebRequest www = CrossPlatformUtils.MakeRequest(url, "GET", null, headers))
			{
				yield return www.SendWebRequest();
				long wwwTime = SwrveHelper.GetMilliseconds() - start;
				ProcessResponse(www, wwwTime, url, listener);
			}
		}

		public virtual IEnumerator Post(string url, byte[] encodedData, Dictionary<string, string> headers, Action<RESTResponse> listener)
		{
			if (!Application.isEditor)
			{
				headers = AddMetricsHeader(headers);
			}
			long start = SwrveHelper.GetMilliseconds();
			using (UnityWebRequest www = CrossPlatformUtils.MakeRequest(url, "POST", encodedData, headers))
			{
				yield return www.SendWebRequest();
				long wwwTime = SwrveHelper.GetMilliseconds() - start;
				ProcessResponse(www, wwwTime, url, listener);
			}
		}

		protected Dictionary<string, string> AddMetricsHeader(Dictionary<string, string> headers)
		{
			if (metrics.Count > 0)
			{
				string value = string.Join(";", metrics.ToArray());
				headers.Add("Swrve-Latency-Metrics", value);
				metrics.Clear();
			}
			return headers;
		}

		private void AddMetrics(string url, long wwwTime, bool error)
		{
			Uri uri = new Uri(url);
			url = string.Format("{0}{1}{2}", uri.Scheme, "://", uri.Authority);
			string item = (!error) ? string.Format("u={0},c={1},sh={1},sb={1},rh={1},rb={1}", url, wwwTime.ToString()) : string.Format("u={0},c={1},c_error=1", url, wwwTime.ToString());
			metrics.Add(item);
		}

		protected void ProcessResponse(UnityWebRequest www, long wwwTime, string url, Action<RESTResponse> listener)
		{
			try
			{
				if (!www.isNetworkError || !www.isHttpError)
				{
					string decodedString = null;
					bool flag = ResponseBodyTester.TestUTF8(www.downloadHandler.data, out decodedString);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					string text = null;
					if (www.GetResponseHeaders() != null)
					{
						Dictionary<string, string>.Enumerator enumerator = www.GetResponseHeaders().GetEnumerator();
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, string> current = enumerator.Current;
							dictionary.Add(current.Key.ToUpper(), current.Value);
						}
						if (dictionary.ContainsKey("CONTENT-ENCODING"))
						{
							text = dictionary["CONTENT-ENCODING"];
						}
					}
					if (flag)
					{
						AddMetrics(url, wwwTime, false);
						listener(new RESTResponse(decodedString, dictionary));
					}
					else
					{
						AddMetrics(url, wwwTime, true);
						listener(new RESTResponse(decodedString, dictionary));
					}
				}
				else
				{
					AddMetrics(url, wwwTime, true);
					listener(new RESTResponse(www.responseCode));
				}
			}
			catch (Exception message)
			{
				SwrveLog.LogError(message);
			}
		}
	}
}
