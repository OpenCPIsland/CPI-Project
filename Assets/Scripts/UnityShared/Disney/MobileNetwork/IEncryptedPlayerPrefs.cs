namespace Disney.MobileNetwork
{
	internal interface IEncryptedPlayerPrefs
	{
		bool isLoaded
		{
			get;
		}

		bool convertingTo
		{
			set;
		}

		bool convertingFrom
		{
			set;
		}

		void Load();

		void Save(bool clearCache);

		void DeleteAll();

		void DeleteAll(bool wipeInternalKeys);

		bool DeleteKey(string key);

		bool HasKey(string key);

		float GetFloat(string key, float defaultValue);

		int GetInt(string key, int defaultValue);

		string GetString(string key, string defaultValue);

		void SetFloat(string key, float value);

		void SetInt(string key, int value);

		void SetString(string key, string value);

		void EncryptedSetStringDirect(string key, string value);

		string EncryptedGetStringDirect(string key);
	}
}
