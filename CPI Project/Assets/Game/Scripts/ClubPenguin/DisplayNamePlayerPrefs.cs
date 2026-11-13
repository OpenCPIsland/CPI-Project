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
			string displayNameKey = getDisplayNameKey(key, false);
			return !string.IsNullOrEmpty(displayNameKey) ? PlayerPrefs.GetFloat(displayNameKey) : 0f;
		}

		public static int GetInt(string key)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			return !string.IsNullOrEmpty(displayNameKey) ? PlayerPrefs.GetInt(displayNameKey) : 0;
		}

		public static string GetString(string key)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			return !string.IsNullOrEmpty(displayNameKey) ? PlayerPrefs.GetString(displayNameKey) : string.Empty;
		}

		public static void SetFloat(string key, float value)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			if (!string.IsNullOrEmpty(displayNameKey))
			{
				PlayerPrefs.SetFloat(displayNameKey, value);
			}
		}

		public static void SetInt(string key, int value)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			if (!string.IsNullOrEmpty(displayNameKey))
			{
				PlayerPrefs.SetInt(displayNameKey, value);
			}
		}

		public static void SetString(string key, string value)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			if (!string.IsNullOrEmpty(displayNameKey))
			{
				PlayerPrefs.SetString(displayNameKey, value);
			}
		}

		public static List<T> GetList<T>(string key)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			return !string.IsNullOrEmpty(displayNameKey) ? PlayerPrefsList.GetValue<T>(displayNameKey) : new List<T>();
		}

		public static void SetList<T>(string key, List<T> value)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			if (!string.IsNullOrEmpty(displayNameKey))
			{
				PlayerPrefsList.SetValue(displayNameKey, value);
			}
		}

		public static bool HasKey(string key)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			return !string.IsNullOrEmpty(displayNameKey) && PlayerPrefs.HasKey(displayNameKey);
		}

		public static void DeleteKey(string key)
		{
			string displayNameKey = getDisplayNameKey(key, false);
			if (!string.IsNullOrEmpty(displayNameKey))
			{
				PlayerPrefs.DeleteKey(displayNameKey);
			}
		}

		private static string getDisplayNameKey(string key, bool throwException = true)
		{
			if (!Service.IsSet<CPDataEntityCollection>())
			{
				if (throwException)
				{
					throw new InvalidOperationException("CPDataEntityCollection service is not set.");
				}
				return null;
			}

			CPDataEntityCollection dataEntity = Service.Get<CPDataEntityCollection>();

			if (dataEntity.TryGetComponent<DisplayNameData>(dataEntity.LocalPlayerHandle, out DisplayNameData component))
			{
				return key + "." + component.DisplayName;
			}

			if (throwException)
			{
				throw new InvalidOperationException("Could not find DisplayNameData on local player.");
			}

			return null;
		}
	}
}
