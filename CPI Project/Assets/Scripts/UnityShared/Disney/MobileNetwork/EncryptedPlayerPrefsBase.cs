using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Disney.MobileNetwork
{
	internal abstract class EncryptedPlayerPrefsBase : IEncryptedPlayerPrefs
	{
		protected string[] m_secondLevel;

		protected MD5 m_hasher;

		protected string m_masterName;

		protected string m_secondaryPrefix;

		protected bool m_fallbackUnencrypted;

		protected IDictionary<string, Type> m_internalKeys;

		protected bool m_convertingTo;

		protected bool m_convertingFrom;

		protected static Type s_floatType = typeof(float);

		protected static Type s_intType = typeof(int);

		protected static Type s_stringType = typeof(string);

		public bool convertingTo
		{
			set
			{
				m_convertingTo = value;
			}
		}

		public bool convertingFrom
		{
			set
			{
				m_convertingFrom = value;
			}
		}

		public virtual bool isLoaded
		{
			get
			{
				return true;
			}
		}

		public EncryptedPlayerPrefsBase(string masterName, string secondName, int maxSecondKeys, bool fallbackUnenc)
		{
			m_fallbackUnencrypted = fallbackUnenc;
			m_masterName = masterName;
			m_secondaryPrefix = secondName;
			m_secondLevel = null;
			m_hasher = MD5.Create();
			LoadOrGenerateSecondLevel(m_secondaryPrefix, maxSecondKeys);
		}

		public virtual IDictionary<string, Type> GetInternalKeys()
		{
			return m_internalKeys;
		}

		public virtual void Load()
		{
		}

		public virtual void Save(bool clearCache)
		{
			PlayerPrefs.Save();
		}

		public virtual void Cleanup()
		{
			PlayerPrefs.DeleteAll();
		}

		public virtual void DeleteAll()
		{
			DeleteAll(false);
		}

		public virtual void DeleteAll(bool wipeInternalKeys)
		{
			IDictionary<string, Type> dictionary = null;
			IDictionary<string, object> dictionary2 = null;
			if (!wipeInternalKeys)
			{
				dictionary = GetInternalKeys();
				if (dictionary != null)
				{
					dictionary2 = new Dictionary<string, object>();
					foreach (string key in dictionary.Keys)
					{
						dictionary2[key] = TypedGet(dictionary[key], key);
					}
				}
			}
			Cleanup();
			if (!wipeInternalKeys && dictionary != null)
			{
				foreach (string key2 in dictionary2.Keys)
				{
					TypedSet(dictionary[key2], key2, dictionary2[key2]);
				}
			}
		}

		public virtual bool DeleteKey(string prefsKey)
		{
			IDictionary<string, Type> internalKeys = GetInternalKeys();
			if (internalKeys != null && internalKeys.ContainsKey(prefsKey))
			{
				return false;
			}
			string encryptedKey = GetEncryptedKey(prefsKey);
			PlayerPrefs.DeleteKey(encryptedKey);
			if (m_fallbackUnencrypted)
			{
				try
				{
					PlayerPrefs.DeleteKey(prefsKey);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
			return true;
		}

		public virtual bool HasKey(string prefsKey)
		{
			string encryptedKey = GetEncryptedKey(prefsKey);
			return PlayerPrefs.HasKey(encryptedKey) || (m_fallbackUnencrypted && PlayerPrefs.HasKey(prefsKey));
		}

		public virtual float GetFloat(string key, float defaultValue)
		{
			string decryptedValue = GetDecryptedValue(key);
			if (decryptedValue == null)
			{
				if (m_fallbackUnencrypted && PlayerPrefs.HasKey(key))
				{
					float @float = PlayerPrefs.GetFloat(key, defaultValue);
					SetFloat(key, @float);
					return @float;
				}
				return defaultValue;
			}
			try
			{
				return float.Parse(decryptedValue);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public virtual int GetInt(string key, int defaultValue)
		{
			string decryptedValue = GetDecryptedValue(key);
			if (decryptedValue == null)
			{
				if (m_fallbackUnencrypted && PlayerPrefs.HasKey(key))
				{
					int @int = PlayerPrefs.GetInt(key, defaultValue);
					SetInt(key, @int);
					return @int;
				}
				return defaultValue;
			}
			try
			{
				return int.Parse(decryptedValue);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public virtual string GetString(string key, string defaultValue)
		{
			string decryptedValue = GetDecryptedValue(key);
			if (decryptedValue == null)
			{
				if (m_fallbackUnencrypted && PlayerPrefs.HasKey(key))
				{
					decryptedValue = PlayerPrefs.GetString(key, defaultValue);
					SetString(key, decryptedValue);
					return decryptedValue;
				}
				return defaultValue;
			}
			return decryptedValue;
		}

		public virtual void SetFloat(string key, float value)
		{
			string unencryptedValue = value.ToString();
			SaveValueEncrypted(key, unencryptedValue);
		}

		public virtual void SetInt(string key, int value)
		{
			string unencryptedValue = value.ToString();
			SaveValueEncrypted(key, unencryptedValue);
		}

		public virtual void SetString(string key, string value)
		{
			SaveValueEncrypted(key, value);
		}

		public virtual void EncryptedSetStringDirect(string key, string value)
		{
			SaveValueEncrypted(key, value);
		}

		public virtual string EncryptedGetStringDirect(string key)
		{
			string decryptedValue = GetDecryptedValue(key);
			if (decryptedValue == null)
			{
				return "";
			}
			return decryptedValue;
		}

		protected string GetDecryptedValue(string prefsKey)
		{
			byte[] keyHash = GetKeyHash(prefsKey);
			string encryptionKey = GetEncryptionKey(keyHash);
			string encryptedKey = GetEncryptedKey(keyHash);
			if (!PlayerPrefs.HasKey(encryptedKey))
			{
				return null;
			}
			string @string = PlayerPrefs.GetString(encryptedKey);
			if (@string == null || @string.Length == 0)
			{
				return @string;
			}
			return AesCipher.Decrypt(@string, encryptionKey);
		}

		protected void SaveValueEncrypted(string prefsKey, string unencryptedValue)
		{
			if (prefsKey == null || prefsKey.Length == 0)
			{
				throw new PlayerPrefsException("Not a valid key");
			}
			byte[] keyHash = GetKeyHash(prefsKey);
			string encryptedKey = GetEncryptedKey(keyHash);
			string encryptionKey = GetEncryptionKey(keyHash);
			string value = (unencryptedValue == null || unencryptedValue.Length == 0) ? "" : AesCipher.Encrypt(unencryptedValue, encryptionKey);
			if (m_fallbackUnencrypted)
			{
				PlayerPrefs.DeleteKey(prefsKey);
			}
			PlayerPrefs.SetString(encryptedKey, value);
		}

		protected byte[] GetKeyHash(string prefsKey)
		{
			return m_hasher.ComputeHash(Encoding.UTF8.GetBytes(prefsKey));
		}

		protected string GetEncryptedKey(string prefsKey)
		{
			byte[] keyHash = GetKeyHash(prefsKey);
			return GetEncryptedKey(keyHash);
		}

		protected string GetEncryptedKey(byte[] prefsKeyHashed)
		{
			return StringHelper.ToHexString(prefsKeyHashed);
		}

		protected string GetEncryptionKey(string prefsKey)
		{
			byte[] hashedBytes = m_hasher.ComputeHash(Encoding.UTF8.GetBytes(prefsKey));
			return GetEncryptionKey(hashedBytes);
		}

		protected string GetEncryptionKey(byte[] hashedBytes)
		{
			if (hashedBytes != null && hashedBytes.Length >= 4)
			{
				int num = hashedBytes[hashedBytes.Length - 1] + (hashedBytes[hashedBytes.Length - 2] << 8);
				int num2 = num % m_secondLevel.Length;
				return m_secondLevel[num2];
			}
			return null;
		}

		protected virtual void LoadOrGenerateSecondLevel(string secondaryPrefix, int maxSecondLevelKeys)
		{
			string masterKeyName = GetMasterKeyName();
			m_internalKeys = new Dictionary<string, Type>();
			m_internalKeys[masterKeyName] = s_intType;
			if (PlayerPrefs.HasKey(masterKeyName))
			{
				int @int = PlayerPrefs.GetInt(masterKeyName);
				string masterKey = GetMasterKey(@int);
				IList<string> list = new List<string>();
				try
				{
					for (int i = 0; i < maxSecondLevelKeys; i++)
					{
						string encryptedKey = GetEncryptedKey(secondaryPrefix + (i + 1));
						m_internalKeys[encryptedKey] = s_stringType;
						string @string = PlayerPrefs.GetString(encryptedKey);
						string item = AesCipher.Decrypt(@string, masterKey);
						list.Add(item);
					}
					m_secondLevel = new string[list.Count];
					list.CopyTo(m_secondLevel, 0);
				}
				catch (ArgumentNullException)
				{
					m_secondLevel = null;
				}
			}
			if (m_secondLevel == null)
			{
				System.Random random = new System.Random();
				int @int = random.Next();
				PlayerPrefs.SetInt(masterKeyName, @int);
				string masterKey = GetMasterKey(@int);
				m_secondLevel = new string[maxSecondLevelKeys];
				for (int i = 0; i < m_secondLevel.Length; i++)
				{
					string encryptedKey = GetEncryptedKey(secondaryPrefix + (i + 1));
					m_internalKeys[encryptedKey] = s_stringType;
					m_secondLevel[i] = Guid.NewGuid().ToString();
					string @string = AesCipher.Encrypt(m_secondLevel[i], masterKey);
					PlayerPrefs.SetString(encryptedKey, @string);
				}
				PlayerPrefs.Save();
			}
		}

		protected virtual string GetMasterKeyName()
		{
			return m_masterName;
		}

		protected string GetMasterKey(int seed)
		{
			string text = seed.ToString("X");
			string text2 = "";
			for (int i = 0; i < text.Length; i++)
			{
				text2 = text2 + text[i] + "-";
			}
			return text2;
		}

		protected static object TypedGet(Type expectedType, string key)
		{
			if (expectedType == s_floatType)
			{
				return PlayerPrefs.GetFloat(key);
			}
			if (expectedType == s_intType)
			{
				return PlayerPrefs.GetInt(key);
			}
			if (expectedType == s_stringType)
			{
				return PlayerPrefs.GetString(key);
			}
			return null;
		}

		protected static void TypedSet(Type expectedType, string key, object value)
		{
			if (expectedType == s_floatType)
			{
				PlayerPrefs.SetFloat(key, (float)value);
			}
			else if (expectedType == s_intType)
			{
				PlayerPrefs.SetInt(key, (int)value);
			}
			else if (expectedType == s_stringType)
			{
				PlayerPrefs.SetString(key, (string)value);
			}
		}
	}
}
