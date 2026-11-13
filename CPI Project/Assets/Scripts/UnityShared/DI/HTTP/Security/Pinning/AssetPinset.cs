using DI.JSON;
using DI.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace DI.HTTP.Security.Pinning
{
	public class AssetPinset : IPinset
	{
		private TextAsset content;

		private IDictionary<string, IList<IPinningInfo>> pinningInfo;

		private IDictionary<string, IHostInfo> hostInfo;

		public AssetPinset(string name, IJSONParser parser)
		{
			if (parser == null)
			{
				throw new HTTPException("JSON parser is required for AssetPinset.");
			}
			UnityThreadHelper.Dispatcher.Dispatch(delegate
			{
				loadPinsetConfiguration(name, parser);
			});
		}

		private void loadPinsetConfiguration(string name, IJSONParser parser)
		{
			content = (Resources.Load(name) as TextAsset);
			pinningInfo = new Dictionary<string, IList<IPinningInfo>>();
			hostInfo = new Dictionary<string, IHostInfo>();
			try
			{
				if (parser.Parse(content.text))
				{
					IDictionary<string, object> dictionary = parser.AsDictionary();
					IDictionary<string, object> dictionary2 = (IDictionary<string, object>)dictionary["pinset"];
					if (dictionary2 != null)
					{
						foreach (KeyValuePair<string, object> item in dictionary2)
						{
							string key = item.Key;
							IList<IPinningInfo> list = new List<IPinningInfo>();
							pinningInfo[key] = list;
							IEnumerable enumerable = (IEnumerable)item.Value;
							foreach (object item2 in enumerable)
							{
								list.Add(new PinningInfo((IDictionary<string, object>)item2));
								Debug.Log("Added " + key + " : " + item2.ToString() + " to pinset");
							}
						}
					}
					IDictionary<string, object> dictionary3 = (IDictionary<string, object>)dictionary["hosts"];
					if (dictionary3 != null)
					{
						foreach (KeyValuePair<string, object> item3 in dictionary3)
						{
							string key = item3.Key;
							hostInfo[key] = new HostInfo((IDictionary<string, object>)item3.Value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to parse pinset." + ex.Message);
				Debug.LogException(ex);
			}
		}

		public IList<string> getSubjects()
		{
			List<string> list = new List<string>();
			list.AddRange(pinningInfo.Keys);
			return list;
		}

		public IList<string> getHosts()
		{
			List<string> list = new List<string>();
			list.AddRange(hostInfo.Keys);
			return list;
		}

		public IHostInfo getHostInfo(string host)
		{
			return hostInfo.ContainsKey(host) ? hostInfo[host] : null;
		}

		public IList<IPinningInfo> getPinningInfo(string url)
		{
			URLParser uRLParser = new URLParser(url);
			string host = uRLParser.getHost();
			if (host != null && this.hostInfo.ContainsKey(host))
			{
				IHostInfo hostInfo = this.hostInfo[host];
				if (hostInfo != null)
				{
					string commonName = hostInfo.getCommonName();
					if (commonName != null && pinningInfo.ContainsKey(commonName))
					{
						return pinningInfo[commonName];
					}
				}
			}
			return null;
		}

		public IDictionary<string, IList<IPinningInfo>> getPinningInfo()
		{
			return pinningInfo;
		}

		public IDictionary<string, IHostInfo> getHostInfo()
		{
			return hostInfo;
		}

		public bool validateCertificate(X509Certificate certificate)
		{
			return false;
		}
	}
}
