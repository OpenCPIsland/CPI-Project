using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Net.Domain.Diagnostics
{
	public class StructuredLogObject
	{
		private static string valueToken = "Value";

		private static string deviceModelToken = "DeviceModel";

		private static string operatingSystemToken = "OperatingSystem";

		private static string systemMemorySizeToken = "SystemMemorySize";

		private static string logMessageTypeToken = "Type";

		private static string metricToken = "Metric";

		private static string playerIdToken = "PlayerId";

		private static string playerNameToken = "PlayerName";

		private static string zoneToken = "Zone";

		private static string clientVersionToken = "ClientVersion";

		private static string stackTraceToken = "StackTrace";

		[SerializeField]
		private Dictionary<string, string> keyValueData;

		public string Value
		{
			set
			{
				AddKeyValueData(valueToken, value);
			}
		}

		public string LogMessageType
		{
			set
			{
				AddKeyValueData(logMessageTypeToken, value);
			}
		}

		public string MetricType
		{
			set
			{
				AddKeyValueData(metricToken, value);
			}
		}

		public string ClientVersion
		{
			set
			{
				AddKeyValueData(clientVersionToken, value);
			}
		}

		public string DeviceModel
		{
			set
			{
				AddKeyValueData(deviceModelToken, value);
			}
		}

		public string OperatingSystem
		{
			set
			{
				AddKeyValueData(operatingSystemToken, value);
			}
		}

		public string SystemMemorySize
		{
			set
			{
				AddKeyValueData(systemMemorySizeToken, value);
			}
		}

		public string PlayerId
		{
			set
			{
				AddKeyValueData(playerIdToken, value);
			}
		}

		public string PlayerName
		{
			set
			{
				AddKeyValueData(playerNameToken, value);
			}
		}

		public string Zone
		{
			set
			{
				AddKeyValueData(zoneToken, value);
			}
		}

		public string StackTrace
		{
			set
			{
				AddKeyValueData(stackTraceToken, value);
			}
		}

		public Dictionary<string, string> KeyValueData
		{
			get
			{
				return keyValueData;
			}
			set
			{
				keyValueData = value;
			}
		}

		public void AddKeyValueData<T>(string key, T data)
		{
			if (KeyValueData == null)
			{
				KeyValueData = new Dictionary<string, string>();
			}
			KeyValueData[key] = data.ToString();
		}
	}
}
