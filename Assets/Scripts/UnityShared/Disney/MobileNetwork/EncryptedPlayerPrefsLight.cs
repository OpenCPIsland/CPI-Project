using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.MobileNetwork
{
	internal class EncryptedPlayerPrefsLight : EncryptedPlayerPrefsBase
	{
		protected const string v2_MASTER_NAME = "__universes__";

		protected const string v2_SECOND_NAME = "__seconds__";

		protected const string v2_CACHE_NAME = "__huge__";

		private IDictionary<string, Type> m_unencryptedKeys;

		private bool m_isLoaded = false;

		public override bool isLoaded
		{
			get
			{
				return m_isLoaded;
			}
		}

		public EncryptedPlayerPrefsLight()
			: base("__universes__", "__seconds__", 1, false)
		{
			Load();
			m_internalKeys[GetEncryptedKey("__huge__")] = EncryptedPlayerPrefsBase.s_stringType;
		}

		public override void Cleanup()
		{
			foreach (string key in m_unencryptedKeys.Keys)
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		public override void Load()
		{
			LoadCache("__huge__");
		}

		public override void Save(bool clearCache)
		{
			SaveCache("__huge__", clearCache);
			PlayerPrefs.Save();
		}

		public override void DeleteAll(bool wipeInternalKeys)
		{
			DeleteAll(wipeInternalKeys, true);
		}

		protected void DeleteAll(bool wipeInternalKeys, bool wipeCache)
		{
			base.DeleteAll(wipeInternalKeys);
			if (wipeCache)
			{
				SaveValueEncrypted("__huge__", "");
			}
		}

		public override bool DeleteKey(string prefsKey)
		{
			IDictionary<string, Type> internalKeys = GetInternalKeys();
			if (internalKeys != null && internalKeys.ContainsKey(prefsKey))
			{
				return false;
			}
			PlayerPrefs.DeleteKey(prefsKey);
			m_unencryptedKeys.Remove(prefsKey);
			return true;
		}

		public override bool HasKey(string prefsKey)
		{
			return PlayerPrefs.HasKey(prefsKey);
		}

		public override float GetFloat(string key, float defaultValue)
		{
			float result = defaultValue;
			if (m_unencryptedKeys.ContainsKey(key))
			{
				if (m_unencryptedKeys[key] == EncryptedPlayerPrefsBase.s_floatType)
				{
					result = PlayerPrefs.GetFloat(key);
				}
			}
			else if (PlayerPrefs.HasKey(key))
			{
				try
				{
					result = PlayerPrefs.GetFloat(key);
					m_unencryptedKeys[key] = EncryptedPlayerPrefsBase.s_floatType;
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		public override int GetInt(string key, int defaultValue)
		{
			int result = defaultValue;
			if (m_unencryptedKeys.ContainsKey(key))
			{
				if (m_unencryptedKeys[key] == EncryptedPlayerPrefsBase.s_intType)
				{
					result = PlayerPrefs.GetInt(key);
				}
			}
			else if (PlayerPrefs.HasKey(key))
			{
				try
				{
					result = PlayerPrefs.GetInt(key);
					m_unencryptedKeys[key] = EncryptedPlayerPrefsBase.s_intType;
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		public override string GetString(string key, string defaultValue)
		{
			string result = defaultValue;
			if (m_unencryptedKeys.ContainsKey(key))
			{
				if (m_unencryptedKeys[key] == EncryptedPlayerPrefsBase.s_stringType)
				{
					result = PlayerPrefs.GetString(key);
				}
			}
			else if (PlayerPrefs.HasKey(key))
			{
				try
				{
					result = PlayerPrefs.GetString(key);
					m_unencryptedKeys[key] = EncryptedPlayerPrefsBase.s_stringType;
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		public override void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
			m_unencryptedKeys[key] = EncryptedPlayerPrefsBase.s_floatType;
		}

		public override void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
			m_unencryptedKeys[key] = EncryptedPlayerPrefsBase.s_intType;
		}

		public override void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
			m_unencryptedKeys[key] = EncryptedPlayerPrefsBase.s_stringType;
		}

		public override void EncryptedSetStringDirect(string key, string value)
		{
			SetString(key, value);
		}

		public override string EncryptedGetStringDirect(string key)
		{
			return GetString(key, "");
		}

		protected void LoadCache(string cacheKey)
		{
			if (!m_isLoaded)
			{
				m_unencryptedKeys = new Dictionary<string, Type>();
				string decryptedValue = GetDecryptedValue(cacheKey);
				if (decryptedValue != null && decryptedValue.Length > 0)
				{
					try
					{
						IDictionary<string, object> dictionary = LPFJson.parse(decryptedValue) as IDictionary<string, object>;
						if (dictionary != null)
						{
							if (dictionary.ContainsKey("float"))
							{
								IDictionary<string, object> dictionary2 = dictionary["float"] as IDictionary<string, object>;
								foreach (string key in dictionary2.Keys)
								{
									try
									{
										float value = Convert.ToSingle(dictionary2[key]);
										PlayerPrefs.SetFloat(key, value);
										m_unencryptedKeys[key] = EncryptedPlayerPrefsBase.s_floatType;
									}
									catch (Exception)
									{
									}
								}
							}
							if (dictionary.ContainsKey("int"))
							{
								IDictionary<string, object> dictionary3 = dictionary["int"] as IDictionary<string, object>;
								foreach (string key2 in dictionary3.Keys)
								{
									try
									{
										int value2 = Convert.ToInt32(dictionary3[key2]);
										PlayerPrefs.SetInt(key2, value2);
										m_unencryptedKeys[key2] = EncryptedPlayerPrefsBase.s_intType;
									}
									catch (Exception)
									{
									}
								}
							}
							if (dictionary.ContainsKey("string"))
							{
								IDictionary<string, object> dictionary4 = dictionary["string"] as IDictionary<string, object>;
								foreach (string key3 in dictionary4.Keys)
								{
									try
									{
										string value3 = dictionary4[key3].ToString();
										PlayerPrefs.SetString(key3, value3);
										m_unencryptedKeys[key3] = EncryptedPlayerPrefsBase.s_stringType;
									}
									catch (Exception)
									{
									}
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
				m_isLoaded = true;
			}
		}

		protected void SaveCache(string cacheKey, bool clearCache)
		{
			if (m_unencryptedKeys != null)
			{
				IDictionary<string, object> dictionary = new Dictionary<string, object>();
				IDictionary<string, object> dictionary2 = new Dictionary<string, object>();
				IDictionary<string, object> dictionary3 = new Dictionary<string, object>();
				IDictionary<string, object> dictionary4 = new Dictionary<string, object>();
				foreach (string key in m_unencryptedKeys.Keys)
				{
					try
					{
						if (m_unencryptedKeys[key] == EncryptedPlayerPrefsBase.s_floatType)
						{
							dictionary2[key] = PlayerPrefs.GetFloat(key);
						}
						else if (m_unencryptedKeys[key] == EncryptedPlayerPrefsBase.s_intType)
						{
							dictionary3[key] = PlayerPrefs.GetInt(key);
						}
						else if (m_unencryptedKeys[key] == EncryptedPlayerPrefsBase.s_stringType)
						{
							dictionary4[key] = PlayerPrefs.GetString(key);
						}
					}
					catch (Exception)
					{
					}
				}
				if (dictionary2.Count > 0)
				{
					dictionary["float"] = dictionary2;
				}
				if (dictionary3.Count > 0)
				{
					dictionary["int"] = dictionary3;
				}
				if (dictionary4.Count > 0)
				{
					dictionary["string"] = dictionary4;
				}
				try
				{
					string text = LPFJson.serialize(dictionary);
					SaveValueEncrypted(cacheKey, (text == null) ? "" : text);
					if (clearCache)
					{
						DeleteAll(false, false);
						m_isLoaded = false;
					}
				}
				catch (Exception)
				{
				}
			}
		}

		protected override string GetMasterKeyName()
		{
			return GetEncryptedKey(m_masterName);
		}
	}
}
