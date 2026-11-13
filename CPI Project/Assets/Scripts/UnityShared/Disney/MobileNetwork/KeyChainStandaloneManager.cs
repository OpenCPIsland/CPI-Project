using UnityEngine;

namespace Disney.MobileNetwork
{
	public class KeyChainStandaloneManager : KeyChainManager
	{
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
        protected override void Init()
		{
		}

		public override void PutString(string key, string value)
		{
			key = GetPlatformKey(key);
			PlayerPrefs.SetString(key, value);
		}

		public override string GetString(string key)
		{
			key = GetPlatformKey(key);
			return PlayerPrefs.GetString(key, "");
		}

		public override void RemoveString(string key)
		{
			key = GetPlatformKey(key);
			PlayerPrefs.DeleteKey(key);
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
#endif
    }
}
