using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Disney.MobileNetwork
{
	public class EnvironmentManager : MonoBehaviour
	{
		public enum ALERT_OPTION
		{
			NONE,
			CANCEL,
			OK,
			SIZE
		}

		public delegate void ShowAlertDelegate(ALERT_OPTION alertOption);

		public const string EDITOR_PREFS_SKU = "mobilenetwork.sku";

		public const string EDITOR_PREFS_PLUGINS_MANIFEST_JSON = "mobilenetwork.pluginManifestJson";

		public const string EDITOR_PREFS_BUNDLE_VERSION = "mobilenetwork.bundleVersion";

		public const string PLAYER_PREFS_UDID = "mobilenetwork.udid";

		private LoggerHelper m_logger = new LoggerHelper();

		public static ShowAlertDelegate mShowAlertDelegate = null;

		private string[] kOperatingSystemTypes = new string[3]
		{
			"iPhone OS",
			"Android OS",
			"Mac OS X"
		};

		public static string mUDID = null;

		protected static string mDeviceModel = null;

		protected static DeviceType mDeviceType;

		protected static string mOperatingSystemType = null;

		protected static Version mOperatingSystemVersion = null;

		protected static string mOperatingSystemDescription = null;

		protected static int mMemoryTotalMegabytes = 0;

		protected static string mBundleIdentifier = null;

		protected static Version mBundleVersion = null;

		protected static string mBundleVersionCode = null;

		protected static Version mClientVersion = null;

		protected static string mSKU = "";

		private static EnvironmentManager m_instance;

		public LoggerHelper Logger
		{
			get
			{
				return m_logger;
			}
		}

		public static EnvironmentManager Instance
		{
			get
			{
				return m_instance;
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
				return UniversalDeviceIdentifier;
			}
		}

		public static string UniversalDeviceIdentifier
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
				return Service.Get<EnvironmentManager>()._GetDiskSpaceFreeMegabytes();
			}
		}

		public static string Locale
		{
			get
			{
				return Service.Get<EnvironmentManager>()._GetLocale();
			}
		}

		public static string DeviceLanguage
		{
			get
			{
				return Service.Get<EnvironmentManager>()._GetDeviceLanguage();
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

		public static string BundleVersionCode
		{
			get
			{
				return mBundleVersionCode;
			}
		}

		public static Version ClientVersion
		{
			get
			{
				if (mClientVersion == null)
				{
					if (mBundleVersion == null)
					{
						return null;
					}
					string version = VersionUtils.ParseClientVersion(mBundleVersion.ToString());
					mClientVersion = new Version(version);
				}
				return mClientVersion;
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
				return Service.Get<EnvironmentManager>()._GetIsMusicPlaying();
			}
		}

		public static bool AreHeadphonesConnected
		{
			get
			{
				return Service.Get<EnvironmentManager>()._GetAreHeadphonesConnected();
			}
		}

		public static bool IsExternalLinksRestricted
		{
			get
			{
				return Service.Get<EnvironmentManager>()._GetIsExternalLinksRestricted();
			}
		}

		public static string SKU
		{
			get
			{
				return Service.Get<EnvironmentManager>()._SKU;
			}
		}

		public static bool IsAmazon
		{
			get
			{
				return IsSKU("Amazon");
			}
		}

		public static string SKUPluginManifestJson
		{
			get
			{
				return Service.Get<EnvironmentManager>()._SKUPluginManifestJson;
			}
		}

		public static string LocalTime
		{
			get
			{
				return _GetLocalTime();
			}
		}

		protected virtual string _SKU
		{
			get
			{
				if (string.IsNullOrEmpty(mSKU))
				{
					mSKU = BuildSettings.Get("mobilenetwork.sku", "").ToUpper();
				}
				return mSKU;
			}
		}

		protected virtual string _SKUPluginManifestJson
		{
			get
			{
				return BuildSettings.Get("mobilenetwork.pluginManifestJson", "");
			}
		}

		public static event Action<bool> FocusChangedEvent;

		public static event Action<bool> HeadphonesConnectedEvent;

		public static event Action LowMemoryEvent;

		public void SetLogger(LoggerHelper.LoggerDelegate loggerMessageHandler)
		{
			m_logger.LogMessageHandler += loggerMessageHandler;
		}

		public static bool IsSKU(string SKUToCompare)
		{
			return SKU.Equals(SKUToCompare.ToUpper());
		}

		public static string GetBuildSettingsJson()
		{
			return Service.Get<EnvironmentManager>()._GetBuildSettingsJson();
		}

		public static void ShowStatusBar(bool show, bool useLightColor = true)
		{
			Service.Get<EnvironmentManager>()._ShowStatusBar(show, useLightColor);
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			base.name = "EnvironmentManager";
			m_instance = this;
		}

		public virtual void Initialize()
		{
			_PopulateDeviceInfo();
			_PopulateOperatingSystemInfo();
			_PopulateMemoryInfo();
			mBundleIdentifier = "EditorBundleID";
			_Init();
		}

		protected virtual void _Init()
		{
			BuildSettings.LoadSettings();
		}

		protected virtual int _GetDiskSpaceFreeMegabytes()
		{
			return -1;
		}

		protected virtual string _GetLocale()
		{
			return null;
		}

		protected virtual string _GetDeviceLanguage()
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
			int result;
			if (int.TryParse(text2, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out result))
			{
				mOperatingSystemVersion = new Version(result, 0);
			}
			else
			{
				mOperatingSystemVersion = new Version(text2);
			}
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

		protected virtual bool _GetIsExternalLinksRestricted()
		{
			return false;
		}

		protected virtual string _GetBuildSettingsJson()
		{
			return "";
		}

		protected virtual void _ShowStatusBar(bool show, bool useLightColor)
		{
		}

		public void OnFocusChanged(string focused)
		{
			Logger.LogDebug(this, "OnFocusChanged called with " + focused);
			if (EnvironmentManager.FocusChangedEvent != null)
			{
				EnvironmentManager.FocusChangedEvent(focused != "0");
			}
		}

		public void OnLowMemory(string emptyString)
		{
			Logger.LogWarning(this, "Native platform notified Unity of Low Memory!");
			if (EnvironmentManager.LowMemoryEvent != null)
			{
				EnvironmentManager.LowMemoryEvent();
			}
		}

		public void OnHeadphonesConnected(string connected)
		{
			Logger.LogDebug(this, "OnHeadphonesConnected called with " + connected);
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

		public virtual void ShowAlert(ShowAlertDelegate showAlertDelegate, string title, string message, string viewButtonText, string cancelButtonText)
		{
			if (mShowAlertDelegate == null)
			{
				mShowAlertDelegate = showAlertDelegate;
			}
		}

		public virtual void ShowAlert(ShowAlertDelegate showAlertDelegate, string message, string viewButtonText, string cancelButtonText)
		{
			ShowAlert(showAlertDelegate, "", message, viewButtonText, cancelButtonText);
		}

		protected void OnAlertViewDismissed(string message)
		{
			if (mShowAlertDelegate != null)
			{
				int num = Convert.ToInt32(message);
				ALERT_OPTION aLERT_OPTION = ALERT_OPTION.NONE;
				switch (num)
				{
				case 0:
					aLERT_OPTION = ALERT_OPTION.CANCEL;
					break;
				case 1:
					aLERT_OPTION = ALERT_OPTION.OK;
					break;
				default:
					aLERT_OPTION = ALERT_OPTION.CANCEL;
					break;
				}
				mShowAlertDelegate(aLERT_OPTION);
			}
			mShowAlertDelegate = null;
		}

		protected static string NormalizeVersionString(string versionStr)
		{
			char[] array = versionStr.ToCharArray();
			char[] array2 = new char[array.Length];
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i] >= '0' && array[i] <= '9') || array[i] == '.')
				{
					array2[num++] = array[i];
				}
				else if (array[i] == '-')
				{
					array2[num++] = '.';
				}
			}
			versionStr = new string(array2);
			return versionStr;
		}
	}
}
