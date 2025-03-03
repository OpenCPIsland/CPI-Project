using UnityEngine;

namespace DCPI.Platforms.SwrveManager.Utils
{
	public static class SwrveManagerUtilsAndroid
	{
		private static AndroidJavaObject _plugin;

		private static AndroidJavaObject playerActivityContext;

		static SwrveManagerUtilsAndroid()
		{
			if (Application.platform == RuntimePlatform.Android && (playerActivityContext == null || _plugin == null))
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					playerActivityContext = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					if (playerActivityContext != null)
					{
						Debug.Log("SwrveManagerUtilsAndroid:SwrveManagerUtilsAndroid() created playerActivityContext");
					}
					else
					{
						Debug.LogError("SwrveManagerUtilsAndroid:SwrveManagerUtilsAndroid() failed to create playerActivityContext!");
					}
				}
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.disney.dcpi.swrveutils.SwrveUtilsPlugin"))
				{
					if (androidJavaClass2 != null)
					{
						androidJavaClass2.CallStatic("setContext", playerActivityContext);
						_plugin = androidJavaClass2.CallStatic<AndroidJavaObject>("instance", new object[0]);
						if (_plugin != null)
						{
							Debug.Log("### Successfully set up the SwrveManagerUtilsAndroid instance");
						}
						else
						{
							Debug.LogError("#### still not able to get SwrveUtilsPlugin instance for some reason.");
						}
					}
				}
			}
		}

		public static string GetIsJailBroken()
		{
			if (_plugin != null)
			{
				return _plugin.Call<int>("isJailBroken", new object[0]).ToString();
			}
			Debug.LogError("SwrveManagerUtilsAndroid::GetIsJailBroken - no plugin!!");
			return "-1";
		}

		public static int GetIsLat()
		{
			if (_plugin != null)
			{
				return _plugin.Call<int>("isLat", new object[0]);
			}
			Debug.LogError("SwrveManagerUtilsAndroid::GetIsLat - no plugin!!");
			return 2;
		}

		public static string GetGIDA()
		{
			if (_plugin != null)
			{
				return _plugin.Call<string>("gida", new object[0]);
			}
			Debug.LogError("SwrveManagerUtilsAndroid::GetGIDA - no plugin!!");
			return string.Empty;
		}
	}
}
