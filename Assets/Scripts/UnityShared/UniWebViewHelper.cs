using UnityEngine;

public class UniWebViewHelper
{
	public static int screenHeight
	{
		get
		{
			return Screen.height;
		}
	}

	public static int screenWidth
	{
		get
		{
			return Screen.width;
		}
	}

	public static int screenScale
	{
		get
		{
			return 1;
		}
	}

	public static string streamingAssetURLForPath(string path)
	{
#if UNITY_ANDROID
        return "file:///android_asset/" + path;
#elif UNITY_IOS || UNITY_IPHONE
        return "file:///ios_asset/" + path;
#else
        return string.Empty;
#endif
    }
}
