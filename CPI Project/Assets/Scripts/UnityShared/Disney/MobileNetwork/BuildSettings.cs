using LitJson;
using System.Collections.Generic;

namespace Disney.MobileNetwork
{
	public static class BuildSettings
	{
		public static byte[] SaltBytes = new byte[32]
		{
			251,
			239,
			229,
			223,
			199,
			193,
			181,
			173,
			163,
			151,
			139,
			131,
			113,
			107,
			101,
			89,
			79,
			73,
			67,
			59,
			47,
			41,
			31,
			23,
			19,
			17,
			13,
			11,
			7,
			5,
			3,
			2
		};

		public static string SETTINGS_FILE = "BuildSettings.txt";

		private static IDictionary<string, object> m_keyValueStore;

		public static void LoadSettings()
		{
			string buildSettingsJson = EnvironmentManager.GetBuildSettingsJson();
			m_keyValueStore = (string.IsNullOrEmpty(buildSettingsJson) ? new Dictionary<string, object>() : (LPFJsonMapper.ToObjectSimple(buildSettingsJson) as IDictionary<string, object>));
		}

		public static bool ContainsKey(string key)
		{
			return m_keyValueStore.ContainsKey(key);
		}

		public static T Get<T>(string key)
		{
			return (T)m_keyValueStore[key];
		}

		public static T Get<T>(string key, T defaultValue)
		{
			if (m_keyValueStore.ContainsKey(key))
			{
				return (T)m_keyValueStore[key];
			}
			return defaultValue;
		}
	}
}
