using UnityEngine;

namespace Disney.MobileNetwork
{
	internal class EncryptedPlayerPrefsNone : IEncryptedPlayerPrefs
	{
		public bool isLoaded
		{
			get
			{
				return true;
			}
		}

		public bool convertingTo
		{
			set
			{
			}
		}

		public bool convertingFrom
		{
			set
			{
			}
		}

		public void Load()
		{
		}

		public void Save(bool clearCache)
		{
			PlayerPrefs.Save();
		}

		public void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public void DeleteAll(bool wipeInternalKeys)
		{
			PlayerPrefs.DeleteAll();
		}

		public bool DeleteKey(string key)
		{
			bool result = PlayerPrefs.HasKey(key);
			PlayerPrefs.DeleteKey(key);
			return result;
		}

		public bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public float GetFloat(string key, float defaultValue)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}

		public int GetInt(string key, int defaultValue)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}

		public string GetString(string key, string defaultValue)
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}

		public void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public void EncryptedSetStringDirect(string key, string value)
		{
			SetString(key, value);
		}

		public string EncryptedGetStringDirect(string key)
		{
			return GetString(key, "");
		}
	}
}
