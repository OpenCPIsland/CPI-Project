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
			PlayerPrefs.SetString(getKey(token, typeFromHandle.Name), value2);
		}

		public T Read<T>() where T : struct, IOfflineData
		{
			return Read<T>(accessToken);
		}

		public static T Read<T>(string token) where T : struct, IOfflineData
		{
			Type typeFromHandle = typeof(T);
			T result = default(T);
			string @string = PlayerPrefs.GetString(getKey(token, typeFromHandle.Name));
			if (!string.IsNullOrEmpty(@string))
			{
				return Service.Get<JsonService>().Deserialize<T>(@string);
			}
			result.Init();
			return result;
		}

		private static string getKey(string token, string table)
		{
			return "ol." + table + "." + token;
		}
	}
}
