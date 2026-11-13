using System.Collections.Generic;

namespace SwrveUnity.Helpers
{
	public static class MiniJsonHelper
	{
		public static int GetInt(Dictionary<string, object> json, string key)
		{
			return GetInt(json, key, 0);
		}

		public static int GetInt(Dictionary<string, object> json, string key, int defaultValue)
		{
			if (json.ContainsKey(key))
			{
				object obj = json[key];
				if (obj is int)
				{
					return (int)obj;
				}
				if (obj is long)
				{
					return (int)(long)obj;
				}
				if (obj is double)
				{
					return (int)(double)obj;
				}
			}
			return defaultValue;
		}

		public static long GetLong(Dictionary<string, object> json, string key)
		{
			return GetLong(json, key, 0L);
		}

		public static long GetLong(Dictionary<string, object> json, string key, long defaultValue)
		{
			if (json.ContainsKey(key))
			{
				object obj = json[key];
				if (obj is long)
				{
					return (long)obj;
				}
				if (obj is int)
				{
					return (int)obj;
				}
				if (obj is double)
				{
					return (long)(double)obj;
				}
			}
			return defaultValue;
		}

		public static float GetFloat(Dictionary<string, object> json, string key)
		{
			return GetFloat(json, key, 0f);
		}

		public static float GetFloat(Dictionary<string, object> json, string key, float defaultValue)
		{
			if (json.ContainsKey(key))
			{
				object obj = json[key];
				if (obj is float)
				{
					return (float)obj;
				}
				if (obj is double)
				{
					return (float)(double)obj;
				}
				if (obj is long)
				{
					return (long)obj;
				}
			}
			return defaultValue;
		}

		public static bool GetBool(Dictionary<string, object> json, string key)
		{
			return GetBool(json, key, false);
		}

		public static bool GetBool(Dictionary<string, object> json, string key, bool defaultValue)
		{
			if (json.ContainsKey(key))
			{
				object obj = json[key];
				if (obj is bool)
				{
					return (bool)obj;
				}
			}
			return defaultValue;
		}

		public static string GetString(Dictionary<string, object> json, string key)
		{
			return GetString(json, key, null);
		}

		public static string GetString(Dictionary<string, object> json, string key, string defaultValue)
		{
			if (json.ContainsKey(key))
			{
				object obj = json[key];
				if (obj is string)
				{
					return (string)obj;
				}
			}
			return defaultValue;
		}
	}
}
