using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public class OfflineDatabase
	{
		private string accessToken;

		public string AccessToken
		{
			get
			{
				return accessToken;
			}
			set
			{
				accessToken = value;
			}
		}

		public void Write<T>(T value) where T : struct, IOfflineData
		{
			Write(value, accessToken);
		}

		public static void Write<T>(T value, string token) where T : struct, IOfflineData
		{
			Type typeFromHandle = typeof(T);
			string value2 = Service.Get<JsonService>().Serialize(value);
			string key = getKey(token, typeFromHandle.Name);
			key = GetPlatformKey(key);
			PlayerPrefs.SetString(key, value2);
			// Flushed on every write: without this the whole session lives in memory
			// until a clean OnApplicationQuit, so a crash loses all progress since launch.
			PlayerPrefs.Save();
		}

		public T Read<T>() where T : struct, IOfflineData
		{
			return Read<T>(accessToken);
		}

		public static T Read<T>(string token) where T : struct, IOfflineData
		{
			Type typeFromHandle = typeof(T);
			T result = default(T);
			string key = getKey(token, typeFromHandle.Name);
			key = GetPlatformKey(key);
			string @string = PlayerPrefs.GetString(key);
			if (!string.IsNullOrEmpty(@string))
			{
				return Service.Get<JsonService>().Deserialize<T>(@string);
			}
			result.Init();
			return result;
		}

		private static string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (UnityEngine.Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}

		private static string getKey(string token, string table)
		{
			return "ol." + table + "." + token;
		}
	}
}
