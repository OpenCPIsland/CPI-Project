using System;
using UnityEngine;

namespace SwrveUnity.Storage
{
	public class SwrvePlayerPrefsStorage : ISwrveStorage
	{
		public virtual void Save(string tag, string data, string userId = null)
		{
			bool flag = false;
			try
			{
				string text = tag + ((userId == null) ? string.Empty : userId);
				text = GetPlatformKey(text);
				SwrveLog.Log("Setting " + text + " in PlayerPrefs", "storage");
				PlayerPrefs.SetString(text, data);
				flag = true;
			}
			catch (PlayerPrefsException ex)
			{
				SwrveLog.LogError(ex.ToString(), "storage");
			}
			if (!flag)
			{
				SwrveLog.LogWarning(tag + " not saved!", "storage");
			}
		}

		public virtual string Load(string tag, string userId = null)
		{
			string result = null;
			try
			{
				string key = tag + ((userId == null) ? string.Empty : userId);
				key = GetPlatformKey(key);
				if (PlayerPrefs.HasKey(key))
				{
					SwrveLog.Log("Got " + tag + " from PlayerPrefs", "storage");
					result = PlayerPrefs.GetString(key);
				}
			}
			catch (PlayerPrefsException ex)
			{
				SwrveLog.LogError(ex.ToString(), "storage");
			}
			return result;
		}

		public virtual void Remove(string tag, string userId = null)
		{
			try
			{
				string text = tag + ((userId == null) ? string.Empty : userId);
				text = GetPlatformKey(text);
				SwrveLog.Log("Setting " + text + " to null", "storage");
				PlayerPrefs.SetString(text, null);
			}
			catch (PlayerPrefsException ex)
			{
				SwrveLog.LogError(ex.ToString());
			}
		}

		private string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (UnityEngine.Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}

		public void SetSecureFailedListener(Action callback)
		{
		}

		public virtual void SaveSecure(string tag, string data, string userId = null)
		{
			Save(tag, data, userId);
		}

		public virtual string LoadSecure(string tag, string userId = null)
		{
			return Load(tag, userId);
		}
	}
}
