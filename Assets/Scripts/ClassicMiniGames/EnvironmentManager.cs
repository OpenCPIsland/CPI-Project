using DisneyMobile.CoreUnitySystems;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour, IConfigurable
{
	public delegate void GetServerTimeDelegate(string serverTime, int errorCode);

	private const string kUnsetConfigValue = "TODO";

	protected string mWebPlayerBundleIdentifier = "TODO";

	protected string mWebPlayerBundleVersion = "TODO";

	protected string mAndroidActivityName = "TODO";

	private string[] kOperatingSystemTypes = new string[3]
	{
		"iPhone OS",
		"Android OS",
		"Mac OS X"
	};

	private static EnvironmentManager mInstance = null;

	private static GetServerTimeDelegate mGetServerTimeDelegate = null;

	protected static string mDeviceModel = null;

	protected static DeviceType mDeviceType;

	protected static string mOperatingSystemType = null;

	protected static Version mOperatingSystemVersion = null;

	protected static string mOperatingSystemDescription = null;

	protected static int mMemoryTotalMegabytes = 0;

	protected static string mBundleIdentifier = null;

	protected static Version mBundleVersion = null;

	private static string mUDID = "";

	public static EnvironmentManager Instance
	{
		get
		{
			return mInstance;
		}
	}

	public static string DeviceModel
	{
		get
		{
			return mDeviceModel;
		}
	}

	public static DeviceType DeviceType
	{
		get
		{
			return mDeviceType;
		}
	}

	public static string UDID
	{
		get
		{
			if (string.IsNullOrEmpty(mUDID))
			{
				mUDID = SystemInfo.deviceUniqueIdentifier;
			}
			return mUDID;
		}
	}

	public static string OperatingSystemType
	{
		get
		{
			return mOperatingSystemType;
		}
	}

	public static Version OperatingSystemVersion
	{
		get
		{
			return mOperatingSystemVersion;
		}
	}

	public static string OperatingSystemDescription
	{
		get
		{
			return mOperatingSystemDescription;
		}
	}

	public static int MemoryTotalMegabytes
	{
		get
		{
			return mMemoryTotalMegabytes;
		}
	}

	public static int DiskSpaceFreeMegabytes
	{
		get
		{
			return mInstance._GetDiskSpaceFreeMegabytes();
		}
	}

	public static string Locale
	{
		get
		{
			return mInstance._GetLocale();
		}
	}

	public static string BundleIdentifier
	{
		get
		{
			return mBundleIdentifier;
		}
	}

	public static Version BundleVersion
	{
		get
		{
			return mBundleVersion;
		}
	}

	public static bool HasInternetConnection
	{
		get
		{
			return _GetHasInternetConnection();
		}
	}

	public static bool HasWiFiInternetConnection
	{
		get
		{
			return _GetHasWiFiInternetConnection();
		}
	}

	public static Version UnityVersion
	{
		get
		{
			Match match = Regex.Match(Application.unityVersion, "(?<Major>\\d*)\\.(?<Minor>\\d*)(\\.(?<Build>\\d*)(\\.(?<Revision>\\d*))?)?");
			return new Version(match.ToString());
		}
	}

	public static bool IsMusicPlaying
	{
		get
		{
			return mInstance._GetIsMusicPlaying();
		}
	}

	public static bool AreHeadphonesConnected
	{
		get
		{
			return mInstance._GetAreHeadphonesConnected();
		}
	}

	public static bool IsAmazon
	{
		get
		{
			if (mInstance == null)
			{
				throw new Exception("Attempting to call EnvironmentManager.IfAmazon, but it has not yet been instantiated.");
			}
			return mInstance._GetIsAmazon();
		}
	}

	public static bool IsGoogle
	{
		get
		{
			if (mInstance == null)
			{
				throw new Exception("Attempting to call EnvironmentManager.IsGoogle, but it has not yet been instantiated.");
			}
			return mInstance._GetIsGoogle();
		}
	}

	public static string LocalTime
	{
		get
		{
			return _GetLocalTime();
		}
	}

	public static event Action<bool> FocusChangedEvent;

	public static event Action<bool> HeadphonesConnectedEvent;

	public static string GetPlatformTag()
	{
		string result = "";
		if (mInstance != null)
		{
			if (IsAmazon)
			{
				result = "Amazon";
			}
			if (IsGoogle)
			{
				result = "Google";
			}
		}
		return result;
	}

	public static void GetServerTime(GetServerTimeDelegate getServerTimeDelegate)
	{
		mGetServerTimeDelegate = getServerTimeDelegate;
	}

	public void Configure(IDictionary<string, object> dictionary)
	{
		AutoConfigurable.AutoConfigureObject(this, dictionary);
	}

	public void Reconfigure(IDictionary<string, object> dictionary)
	{
		Configure(dictionary);
	}

	private void Awake()
	{
		mInstance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		base.name = "EnvironmentManager";
	}

	public virtual void Initialize()
	{
		mInstance = this;
		_PopulateDeviceInfo();
		_PopulateOperatingSystemInfo();
		_PopulateMemoryInfo();
		mBundleVersion = new Version(0, 0, 0);
		mBundleIdentifier = "EditorBundleID";
		if (mWebPlayerBundleIdentifier == null)
		{
		}
		if (mWebPlayerBundleVersion == null)
		{
		}
		if (mAndroidActivityName == null)
		{
		}
		_Init();
	}

	protected virtual void _Init()
	{
		mBundleVersion = new Version(0, 0, 0);
	}

	protected virtual int _GetDiskSpaceFreeMegabytes()
	{
		return -1;
	}

	protected virtual string _GetLocale()
	{
		return null;
	}

	private void _PopulateDeviceInfo()
	{
		mDeviceType = SystemInfo.deviceType;
		mDeviceModel = SystemInfo.deviceModel;
	}

	private void _PopulateOperatingSystemInfo()
	{
		mOperatingSystemType = "Unknown";
		string text = mOperatingSystemDescription = SystemInfo.operatingSystem;
		mOperatingSystemVersion = new Version();
		string text2 = "0.0.0";
		string[] array = kOperatingSystemTypes;
		int num = 0;
		string text3;
		while (true)
		{
			if (num < array.Length)
			{
				text3 = array[num];
				if (mOperatingSystemDescription.StartsWith(text3))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		mOperatingSystemType = text3;
		int num2 = text.IndexOf(" /");
		if (num2 > -1)
		{
			text = text.Substring(0, num2);
		}
		text2 = text.Substring(text3.Length + 1, text.Length - (text3.Length + 1));
		mOperatingSystemVersion = new Version(text2);
	}

	private void _PopulateMemoryInfo()
	{
		mMemoryTotalMegabytes = SystemInfo.systemMemorySize;
	}

	private static bool _GetHasInternetConnection()
	{
		bool result = false;
		NetworkReachability internetReachability = Application.internetReachability;
		if (internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			result = true;
		}
		return result;
	}

	private static bool _GetHasWiFiInternetConnection()
	{
		bool result = false;
		NetworkReachability internetReachability = Application.internetReachability;
		if (internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			result = true;
		}
		return result;
	}

	protected virtual bool _GetIsAmazon()
	{
		return false;
	}

	protected virtual bool _GetIsGoogle()
	{
		return false;
	}

	private static string _GetLocalTime()
	{
		string text = "";
		return DateTime.UtcNow.ToString("s") + "Z";
	}

	protected virtual bool _GetIsMusicPlaying()
	{
		return false;
	}

	protected virtual bool _GetAreHeadphonesConnected()
	{
		return false;
	}

	public void OnSetDisneyMobileId(string disneyMobileId)
	{
	}

	public void OnSetFacebookId(int errorCode)
	{
	}

	public void OnConsumeGift(int giftId, int errorCode)
	{
	}

	public void OnGetRewardsBalance(int amount, int errorCode)
	{
	}

	public void OnConsumeRewards(int amount, int errorCode)
	{
	}

	public void OnGetServerTime(string serverTime, int errorCode)
	{
		if (mGetServerTimeDelegate != null)
		{
			mGetServerTimeDelegate(serverTime, errorCode);
		}
	}

	public string SelectProfile(string localProfileJSON, string serverProfileJSON)
	{
		return null;
	}

	public string EnsureProfileContainsFactorySettings(string localProfileJSON, string factoryProfileJSON)
	{
		return null;
	}

	public void OnFocusChanged(string focused)
	{
		DisneyMobile.CoreUnitySystems.Logger.LogDebug(this, "OnFocusChanged called with " + focused);
		if (EnvironmentManager.FocusChangedEvent != null)
		{
			EnvironmentManager.FocusChangedEvent(focused != "0");
		}
	}

	public void OnHeadphonesConnected(string connected)
	{
		DisneyMobile.CoreUnitySystems.Logger.LogDebug(this, "OnHeadphonesConnected called with " + connected);
		bool obj = false;
		if (connected.Equals("true"))
		{
			obj = true;
		}
		if (EnvironmentManager.HeadphonesConnectedEvent != null)
		{
			EnvironmentManager.HeadphonesConnectedEvent(obj);
		}
	}

	public static bool LaunchApp(string appURLSchema)
	{
		return false;
	}

	public static bool CanLaunchApp(string appURLSchema)
	{
		return false;
	}
}
