using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class PlayerPrefsList
	{
		private class ListWrapper<T>
		{
			public List<T> List;

			public ListWrapper(List<T> list)
			{
				List = list;
			}
		}

		public static void SetValue<T>(string key, List<T> value)
		{
			ListWrapper<T> obj = new ListWrapper<T>(value);
			string value2 = JsonUtility.ToJson(obj);
			PlayerPrefs.SetString(key, value2);
		}

		public static List<T> GetValue<T>(string key)
		{
			string @string = PlayerPrefs.GetString(key);
			ListWrapper<T> listWrapper = JsonUtility.FromJson<ListWrapper<T>>(@string);
			return listWrapper.List;
		}

		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}
	}
}
