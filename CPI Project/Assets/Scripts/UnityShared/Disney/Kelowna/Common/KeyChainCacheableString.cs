using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;

namespace Disney.Kelowna.Common
{
	public class KeyChainCacheableString : ICachableType
	{
		private string key;

		private string defaultValue;

		public string Value
		{
			get
			{
				return GetValue();
			}
			set
			{
				SetValue(value);
			}
		}

		public KeyChainCacheableString(string playerPrefsKey, string defaultValue)
		{
			key = playerPrefsKey;
			this.defaultValue = defaultValue;
		}

		public void SetValue(string value)
		{
			try
			{
				Service.Get<KeyChainManager>().PutString(key, value);
			}
			catch (Exception ex)
			{
				Log.LogErrorFormatted(this, "Unable to save key chain key value pair: {0}:{1}", key, value);
				Log.LogException(this, ex);
			}
		}

		public string GetValue()
		{
			return Service.Get<KeyChainManager>().GetString(key);
		}

		public void Reset()
		{
			SetValue(defaultValue);
		}
	}
}
