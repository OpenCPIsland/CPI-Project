#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ClubPenguin.Net.Lan
{
	public static class LanStore
	{
		public const string CatalogKey = "LAN.Data.Catalog";

		public const string BotsKey = "LAN.Data.Bots";

		private static readonly object gate = new object();

		private static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

		private static readonly Dictionary<string, string> pending = new Dictionary<string, string>();

		private static readonly HashSet<string> suppressed = new HashSet<string>();

		public static void Prime(string key, string legacyFile)
		{
			string value = PlayerPrefs.GetString(key, null);
			if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(legacyFile))
			{
				string legacy = Path.Combine(Application.persistentDataPath, legacyFile);
				try
				{
					if (File.Exists(legacy))
					{
						value = File.ReadAllText(legacy, Encoding.UTF8);
						PlayerPrefs.SetString(key, value);
						PlayerPrefs.Save();
						File.Delete(legacy);
					}
					if (File.Exists(legacy + ".tmp"))
					{
						File.Delete(legacy + ".tmp");
					}
				}
				catch (Exception)
				{
				}
			}
			lock (gate)
			{
				suppressed.Remove(key);
				pending.Remove(key);
				if (string.IsNullOrEmpty(value))
				{
					cache.Remove(key);
				}
				else
				{
					cache[key] = value;
				}
			}
		}

		public static string Read(string key)
		{
			lock (gate)
			{
				string value;
				return cache.TryGetValue(key, out value) ? value : null;
			}
		}

		public static void Write(string key, string json)
		{
			lock (gate)
			{
				if (suppressed.Contains(key))
				{
					return;
				}
				cache[key] = json;
				pending[key] = json;
			}
		}

		public static void Flush()
		{
			KeyValuePair<string, string>[] dirty;
			lock (gate)
			{
				if (pending.Count == 0)
				{
					return;
				}
				dirty = new KeyValuePair<string, string>[pending.Count];
				((ICollection<KeyValuePair<string, string>>)pending).CopyTo(dirty, 0);
				pending.Clear();
			}
			foreach (KeyValuePair<string, string> kv in dirty)
			{
				PlayerPrefs.SetString(kv.Key, kv.Value);
			}
			PlayerPrefs.Save();
		}

		public static void Clear(string key)
		{
			lock (gate)
			{
				suppressed.Add(key);
				cache.Remove(key);
				pending.Remove(key);
			}
			PlayerPrefs.DeleteKey(key);
			PlayerPrefs.Save();
		}
	}
}
#endif
