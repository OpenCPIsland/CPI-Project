using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public static class DisplayNamePlayerPrefs
	{
		public static float GetFloat(string key)
		{
			string displayNameKey = getDisplayNameKey(key);
			return PlayerPrefs.GetFloat(displayNameKey);
		}

		public static int GetInt(string key)
		{
			string displayNameKey = getDisplayNameKey(key);
			return PlayerPrefs.GetInt(displayNameKey);
		}

		public static string GetString(string key)
		{
			string displayNameKey = getDisplayNameKey(key);
			return PlayerPrefs.GetString(displayNameKey);
		}

		public static void SetFloat(string key, float value)
		{
			string displayNameKey = getDisplayNameKey(key);
			PlayerPrefs.SetFloat(displayNameKey, value);
		}

		public static void SetInt(string key, int value)
		{
			string displayNameKey = getDisplayNameKey(key);
			PlayerPrefs.SetInt(displayNameKey, value);
		}

		public static void SetString(string key, string value)
		{
			string displayNameKey = getDisplayNameKey(key);
			PlayerPrefs.SetString(displayNameKey, value);
		}

		public static List<T> GetList<T>(string key)
		{
			string displayNameKey = getDisplayNameKey(key);
			return PlayerPrefsList.GetValue<T>(displayNameKey);
		}

		public static void SetList<T>(string key, List<T> value)
		{
			string displayNameKey = getDisplayNameKey(key);
			PlayerPrefsList.SetValue(displayNameKey, value);
		}

		public static bool HasKey(string key)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			if (!string.IsNullOrEmpty(displayNameKey))
			{
				return PlayerPrefs.HasKey(displayNameKey);
			}
			return false;
		}

		public static void DeleteKey(string key)
		{
			string displayNameKey = getDisplayNameKey(key);
			PlayerPrefs.DeleteKey(displayNameKey);
		}

		private static string getDisplayNameKey(string key, bool throwException = true)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				return key + "." + component.DisplayName;
			}
			if (throwException)
			{
				throw new InvalidOperationException("Could not find DisplayNameData on local player");
			}
			return null;
		}
	}
}
