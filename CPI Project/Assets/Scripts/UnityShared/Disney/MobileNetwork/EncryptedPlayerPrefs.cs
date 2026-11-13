namespace Disney.MobileNetwork
{
	public class EncryptedPlayerPrefs
	{
		private static IEncryptedPlayerPrefs s_impl = new EncryptedPlayerPrefsLight();

		private static IEncryptedPlayerPrefs s_priorImpl;

		private static bool s_converted = false;

		public static bool isLoaded
		{
			get
			{
				return s_impl.isLoaded;
			}
		}

		public static void Load()
		{
			if (s_priorImpl != null)
			{
				s_priorImpl.Load();
			}
			s_impl.Load();
		}

		public static void Save(bool clearCache)
		{
			if (s_priorImpl != null)
			{
				s_priorImpl.Save(clearCache);
			}
			if (s_impl.isLoaded)
			{
				s_impl.Save(clearCache);
			}
		}

		public static void Save()
		{
			Save(true);
		}

		public static void DeleteAll()
		{
			CheckLoad();
			s_impl.DeleteAll();
		}

		public static void DeleteAll(bool wipeInternalKeys)
		{
			CheckLoad();
			s_impl.DeleteAll(wipeInternalKeys);
		}

		public static void DeleteKey(string prefsKey)
		{
			CheckLoad();
			s_impl.DeleteKey(prefsKey);
		}

		public static bool HasKey(string prefsKey)
		{
			CheckLoad();
			if (s_priorImpl != null && s_priorImpl.HasKey(prefsKey))
			{
				return true;
			}
			return s_impl.HasKey(prefsKey);
		}

		public static float GetFloat(string key)
		{
			return GetFloat(key, 0f);
		}

		public static float GetFloat(string key, float defaultValue)
		{
			CheckLoad();
			if (s_priorImpl != null && s_priorImpl.HasKey(key))
			{
				float @float = s_priorImpl.GetFloat(key, defaultValue);
				s_priorImpl.DeleteKey(key);
				s_impl.SetFloat(key, @float);
				return @float;
			}
			return s_impl.GetFloat(key, defaultValue);
		}

		public static int GetInt(string key)
		{
			return GetInt(key, 0);
		}

		public static int GetInt(string key, int defaultValue)
		{
			CheckLoad();
			if (s_priorImpl != null && s_priorImpl.HasKey(key))
			{
				int @int = s_priorImpl.GetInt(key, defaultValue);
				s_priorImpl.DeleteKey(key);
				s_impl.SetInt(key, @int);
				return @int;
			}
			return s_impl.GetInt(key, defaultValue);
		}

		public static string GetString(string key)
		{
			return GetString(key, "");
		}

		public static string GetString(string key, string defaultValue)
		{
			CheckLoad();
			if (s_priorImpl != null && s_priorImpl.HasKey(key))
			{
				string @string = s_priorImpl.GetString(key, defaultValue);
				s_priorImpl.DeleteKey(key);
				s_impl.SetString(key, @string);
				return @string;
			}
			return s_impl.GetString(key, defaultValue);
		}

		public static void SetFloat(string key, float value)
		{
			CheckLoad();
			s_impl.SetFloat(key, value);
		}

		public static void SetInt(string key, int value)
		{
			CheckLoad();
			s_impl.SetInt(key, value);
		}

		public static void SetString(string key, string value)
		{
			CheckLoad();
			s_impl.SetString(key, value);
		}

		internal static string GetEncryptedString(string key)
		{
			CheckLoad();
			return s_impl.EncryptedGetStringDirect(key);
		}

		internal static void SetEncryptedString(string key, string value)
		{
			CheckLoad();
			s_impl.EncryptedSetStringDirect(key, value);
		}

		protected static void CheckLoad()
		{
			if (!s_converted)
			{
				if (s_priorImpl == null)
				{
					if (!(s_impl is EncryptedPlayerPrefsHeavy) && EncryptedPlayerPrefsHeavy.isCurrentVersion)
					{
						s_priorImpl = new EncryptedPlayerPrefsHeavy(false);
					}
					else if (!(s_impl is EncryptedPlayerPrefsMid) && EncryptedPlayerPrefsMid.isCurrentVersion)
					{
						s_priorImpl = new EncryptedPlayerPrefsMid();
					}
					else if (!(s_impl is EncryptedPlayerPrefsLight) && !EncryptedPlayerPrefsHeavy.isCurrentVersion && !EncryptedPlayerPrefsMid.isCurrentVersion)
					{
						s_priorImpl = new EncryptedPlayerPrefsLight();
					}
				}
				if (s_priorImpl != null)
				{
					s_impl.convertingTo = true;
					s_priorImpl.convertingFrom = true;
				}
				s_converted = true;
			}
			if (!s_impl.isLoaded)
			{
				Load();
			}
		}
	}
}
