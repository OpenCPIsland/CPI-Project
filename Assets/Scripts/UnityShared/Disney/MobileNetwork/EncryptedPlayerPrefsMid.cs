using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.MobileNetwork
{
	internal class EncryptedPlayerPrefsMid : EncryptedPlayerPrefsBase
	{
		private const int DEFAULT_DIRTY_THRESHOLD = 20;

		protected const string MID_MASTER_NAME = "__unvrs__";

		protected const int MAX_SECOND_LEVEL_KEYS = 5;

		protected const string SECOND_NAME = "__second__";

		private IDictionary<string, Type> m_unencryptedKeys;

		private int m_dirtyTally;

		private int m_maxDirtyCount;

		internal static bool isCurrentVersion
		{
			get
			{
				return PlayerPrefs.HasKey("__unvrs__");
			}
		}

		public int dirtyThreshold
		{
			set
			{
				m_maxDirtyCount = value;
			}
		}

		public EncryptedPlayerPrefsMid()
			: this(20)
		{
		}

		public EncryptedPlayerPrefsMid(int maxDirtyCount)
			: base("__unvrs__", "__second__", 5, false)
		{
			m_unencryptedKeys = new Dictionary<string, Type>();
			m_dirtyTally = 0;
			m_maxDirtyCount = maxDirtyCount;
		}

		public override void Cleanup()
		{
			base.Cleanup();
			m_unencryptedKeys.Clear();
		}

		public override void Save(bool clearCache)
		{
			foreach (string key in m_unencryptedKeys.Keys)
			{
				if (PlayerPrefs.HasKey(key))
				{
					object obj = EncryptedPlayerPrefsBase.TypedGet(m_unencryptedKeys[key], key);
					if (obj != null)
					{
						string unencryptedValue = obj.ToString();
						PlayerPrefs.DeleteKey(key);
						SaveValueEncrypted(key, unencryptedValue);
					}
				}
			}
			m_dirtyTally = 0;
			base.Save(clearCache);
		}

		public override bool DeleteKey(string prefsKey)
		{
			if (base.DeleteKey(prefsKey))
			{
				m_unencryptedKeys.Remove(prefsKey);
				return true;
			}
			return false;
		}

		public override float GetFloat(string key, float defaultValue)
		{
			if (HaveCached(key, EncryptedPlayerPrefsBase.s_floatType))
			{
				return PlayerPrefs.GetFloat(key, defaultValue);
			}
			float? num = null;
			string decryptedValue = GetDecryptedValue(key);
			if (decryptedValue != null)
			{
				try
				{
					num = float.Parse(decryptedValue);
				}
				catch (Exception)
				{
					num = null;
				}
			}
			if ((!num.HasValue || !num.HasValue) && PlayerPrefs.HasKey(key))
			{
				num = PlayerPrefs.GetFloat(key, defaultValue);
			}
			if (!num.HasValue || !num.HasValue)
			{
				return defaultValue;
			}
			SetFloat(key, num.Value);
			return num.Value;
		}

		public override int GetInt(string key, int defaultValue)
		{
			if (HaveCached(key, EncryptedPlayerPrefsBase.s_intType))
			{
				return PlayerPrefs.GetInt(key, defaultValue);
			}
			int? num = null;
			string decryptedValue = GetDecryptedValue(key);
			if (decryptedValue != null)
			{
				try
				{
					num = int.Parse(decryptedValue);
				}
				catch (Exception)
				{
					num = null;
				}
			}
			if ((!num.HasValue || !num.HasValue) && PlayerPrefs.HasKey(key))
			{
				num = PlayerPrefs.GetInt(key, defaultValue);
			}
			if (!num.HasValue || !num.HasValue)
			{
				return defaultValue;
			}
			SetInt(key, num.Value);
			return num.Value;
		}

		public override string GetString(string key, string defaultValue)
		{
			if (HaveCached(key, EncryptedPlayerPrefsBase.s_stringType))
			{
				return PlayerPrefs.GetString(key, defaultValue);
			}
			string text = GetDecryptedValue(key);
			if (text == null && PlayerPrefs.HasKey(key))
			{
				text = PlayerPrefs.GetString(key, defaultValue);
			}
			if (text == null)
			{
				return defaultValue;
			}
			SetString(key, text);
			return text;
		}

		public override void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
			CacheKey(key, EncryptedPlayerPrefsBase.s_floatType);
		}

		public override void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
			CacheKey(key, EncryptedPlayerPrefsBase.s_intType);
		}

		public override void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
			CacheKey(key, EncryptedPlayerPrefsBase.s_stringType);
		}

		protected bool HaveCached(string key, Type valType)
		{
			if (m_unencryptedKeys != null && m_unencryptedKeys.ContainsKey(key) && m_unencryptedKeys[key] == valType)
			{
				return true;
			}
			return false;
		}

		protected void CacheKey(string key, Type valType)
		{
			m_unencryptedKeys[key] = valType;
			m_dirtyTally++;
			if (m_dirtyTally > m_maxDirtyCount)
			{
				Save(false);
			}
		}
	}
}
