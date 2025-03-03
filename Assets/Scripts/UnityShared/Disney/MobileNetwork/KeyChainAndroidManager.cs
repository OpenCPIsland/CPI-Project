using Disney.MobileNetwork;
using UnityEngine;

namespace Disney.MobileNetwork
{
	public class KeyChainAndroidManager : KeyChainManager
	{
#if UNITY_ANDROID
		private AndroidJavaObject androidPlugin;

		protected override void Init()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.mobilenetwork.plugins.KeyChainPlugin"))
			{
				if (androidJavaClass != null)
				{
					androidPlugin = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
				}
			}
			androidPlugin.Call("GenerateAndStoreKey");
		}

		public override void PutString(string key, string value)
		{
			androidPlugin.Call("PutString", key, value);
		}

		public override string GetString(string key)
		{
			return androidPlugin.Call<string>("GetString", new object[1]
			{
			key
			});
		}

		public override void RemoveString(string key)
		{
			androidPlugin.Call("RemoveString", key);
		}
#endif
	}
}
