using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace Disney.MobileNetwork
{
	public class AccessibilityManagerAndroid : AccessibilityManager
	{
#if UNITY_ANDROID
		private AndroidJavaObject androidPlugin = null;

		public override void Init()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.mobilenetwork.plugins.AccessibilityPlugin"))
			{
				if (androidJavaClass != null)
				{
					androidPlugin = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
				}
				else
				{
					Log.LogFatal(this, "Could not find reference to com.disney.mobilenetwork.plugins.AccessibilityPlugin");
				}
			}
		}

		public override float GetAdjustedFontSize(float aFontSize)
		{
			return androidPlugin.Call<float>("GetAdjustedFontSize", new object[1]
			{
			aFontSize
			});
		}

		public override bool IsOtherAudioPlaying()
		{
			return androidPlugin.Call<bool>("IsOtherAudioPlaying", new object[0]);
		}

		public override void Speak(string aTextToSpeak, float rate)
		{
			androidPlugin.Call("Speak", aTextToSpeak, rate);
		}

		public override Vector2 GetScreenSize()
		{
			return new Vector2(androidPlugin.Call<int>("GetScreenWidth", new object[0]), androidPlugin.Call<int>("GetScreenHeight", new object[0]));
		}

		public override Vector2 GetScreenSizeWithSoftKeys()
		{
			return new Vector2(androidPlugin.Call<int>("GetFullScreenWidth", new object[0]), androidPlugin.Call<int>("GetFullScreenHeight", new object[0]));
		}

		public override int GetStatusBarHeight()
		{
			return androidPlugin.Call<int>("GetStatusBarHeight", new object[0]);
		}

		public override string GetProfileId()
		{
			return "Android";
		}
#endif
	}
}
