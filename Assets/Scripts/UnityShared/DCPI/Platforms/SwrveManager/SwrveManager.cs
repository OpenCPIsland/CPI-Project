using DCPI.Platforms.SwrveManager.Analytics;
using DCPI.Platforms.SwrveManager.Utils;
using SwrveUnity;
using SwrveUnity.ResourceManager;
using SwrveUnityMiniJSON;
using System.Collections.Generic;
using UnityEngine;

namespace DCPI.Platforms.SwrveManager
{
	public class SwrveManager : MonoBehaviour, ISwrveManager
	{
		private const int SWRVE_MAX_PAYLOAD = 500;

		private const int SWRVE_MAX_EVENT_NAMES = 1000;

		private const string SWRVE_MANAGER_LIBVERSION = "1.3.0";

		private HashSet<string> EventsSent = new HashSet<string>();

		public static SwrveManager instance;

		private SwrveResourceManager resourceManager;

		private SwrveConfig customSwrveConfig;

		private Dictionary<string, string> customUserData = null;

		private string userId;

		private static bool _isDebugEnvLog = false;

		public SwrveResourceManager SwrveResourceManager
		{
			get
			{
				return resourceManager;
			}
		}

		public string AnalyticsId
		{
			get;
			private set;
		}

		public string AnalyticsKey
		{
			get;
			private set;
		}

		public static bool DebugLogging
		{
			get
			{
				return _isDebugEnvLog;
			}
			set
			{
				_isDebugEnvLog = true;
			}
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
				SwrveComponent swrveComponent = base.gameObject.AddComponent<SwrveComponent>();
				customSwrveConfig = new SwrveConfig();
			}
			else if (instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public SwrveSDK GetSDK()
		{
			return SwrveComponent.Instance.SDK;
		}

		public SwrveConfig GetSwrveConfig()
		{
			return customSwrveConfig;
		}

		public static void Log(string msg)
		{
			if (_isDebugEnvLog)
			{
				Debug.Log("SwrveManager: " + msg);
			}
		}

		public void InitWithAnalyticsKeySecretConfigAndCustomData(int appId, string apiKey, SwrveConfig customConfig = null, Dictionary<string, string> customData = null)
		{
			if (customData != null)
			{
				customUserData = customData;
			}
			InitWithAnalyticsKeySecretAndConfig(appId, apiKey, customConfig);
		}

		public void InitWithAnalyticsKeySecretAndConfig(int appId, string apiKey, SwrveConfig customConfig)
		{
			if (customConfig != null)
			{
				customSwrveConfig = customConfig;
			}
			InitWithAnalyticsKeySecret(appId, apiKey);
		}

		public void InitWithAnalyticsKeySecret(int appId, string apiKey)
		{
			AnalyticsId = appId.ToString();
			AnalyticsKey = apiKey;
			Log("Init with appId " + AnalyticsId + " and apiKey: " + AnalyticsKey);
			customSwrveConfig.UseHttpsForEventsServer = true;
			customSwrveConfig.UseHttpsForContentServer = true;
			customSwrveConfig.SendEventsIfBufferTooLarge = true;
			customSwrveConfig.AutomaticSessionManagement = true;
			if (string.IsNullOrEmpty(customSwrveConfig.AppVersion))
			{
				customSwrveConfig.AppVersion = Application.version;
			}
			FinishSwrveSDKInit();
		}

		private void FinishSwrveSDKInit()
		{
			SwrveConfig swrveConfig = customSwrveConfig;
			SwrveComponent.Instance.Init(int.Parse(AnalyticsId), AnalyticsKey, customSwrveConfig);
			Dictionary<string, string> deviceInfo = SwrveComponent.Instance.SDK.GetDeviceInfo();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (deviceInfo != null && deviceInfo.Count > 0)
			{
				string text = deviceInfo["swrve.device_name"];
				string value = deviceInfo["swrve.os"];
				string value2 = deviceInfo["swrve.device_dpi"];
				string value3 = deviceInfo["swrve.device_width"];
				string value4 = deviceInfo["swrve.device_height"];
				dictionary.Add("device_name", text);
				dictionary.Add("os", value);
				dictionary.Add("device_dpi", value2);
				dictionary.Add("device_width", value3);
				dictionary.Add("device_height", value4);
				if (!string.IsNullOrEmpty(swrveConfig.UserId))
				{
					dictionary.Add("swrve_user_id", swrveConfig.UserId);
				}
				else
				{
					Log("### !!! unable to add userId to the userProps");
				}
				dictionary.Add("jailbroken.is_jailbroken", SwrveManagerUtils.GetIsJailBroken());
				dictionary.Add("lat.is_lat", SwrveManagerUtils.GetIsLat().ToString());
				string text2 = string.Empty;
				if (Application.platform == RuntimePlatform.Android)
				{
					text2 = "gida";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					text2 = "idfa";
				}
				if (SwrveManagerUtils.IsAndiAvailable() && SwrveManagerUtils.IsAndiInitialized())
				{
					string text3 = (string)SwrveManagerUtils.ANDIType.GetMethod("GetAndiu").Invoke(null, null);
					Log("### Let's send Swrve the andiu: " + text3);
					dictionary.Add("andiu", text3);
				}
				string text4 = SwrveManagerUtils.AESEncrypt(text, SwrveManagerUtils.GetAdvertiserID());
				Log("### encryptedAdvertiserId: " + text4);
				if (!string.IsNullOrEmpty(text2))
				{
					dictionary.Add(text2, text4);
				}
				string rSAEncryptedKey = SwrveManagerUtils.GetRSAEncryptedKey();
				Log("### eskKey: " + rSAEncryptedKey);
				if (!string.IsNullOrEmpty(rSAEncryptedKey))
				{
					dictionary.Add("esk", rSAEncryptedKey);
				}
			}
			else
			{
				Log("### !! Unable to get deviceInfo and therefore unable to get and set the userProperties information.");
			}
			if (customUserData != null)
			{
				foreach (KeyValuePair<string, string> customUserDatum in customUserData)
				{
					if (!dictionary.ContainsKey(customUserDatum.Key))
					{
						dictionary.Add(customUserDatum.Key, customUserDatum.Value);
					}
					else
					{
						Log("###Duplicate KEY!! unable to add " + customUserDatum.Key + " - " + customUserDatum.Value);
					}
				}
			}
			SwrveComponent.Instance.SDK.UserUpdate(dictionary);
			resourceManager = SwrveComponent.Instance.SDK.ResourceManager;
		}

		public void InitWithAnalyticsKeySecretAndUserId(int appId, string apiKey, string userId)
		{
			RegisterPlayer(userId);
			InitWithAnalyticsKeySecret(appId, apiKey);
		}

		public void InitWithAnalyticsKeySecretAndUserIdAndAppVersion(int appId, string apiKey, string userId, string appVersion)
		{
			customSwrveConfig.AppVersion = appVersion;
			InitWithAnalyticsKeySecretAndUserId(appId, apiKey, userId);
		}

		private void RegisterPlayerCore(string playerId)
		{
			customSwrveConfig.UserId = WWW.EscapeURL(playerId);
		}

		public void RegisterPlayer(string playerId)
		{
			userId = playerId;
			RegisterPlayerCore(userId);
		}

		public string GetLibVersion()
		{
			return "SwrveManager: 1.3.0, Swrve SDK: 5.1.1";
		}

		public void RegisterPlayer(int appId, string apiKey, string playerId)
		{
			userId = playerId;
			customSwrveConfig.UserId = WWW.EscapeURL(userId);
			AnalyticsId = appId.ToString();
			AnalyticsKey = apiKey;
			Log("Init with appId " + appId + " and apiKey: " + apiKey);
			SwrveComponent.Instance.Init(appId, apiKey, customSwrveConfig);
			resourceManager = SwrveComponent.Instance.SDK.ResourceManager;
		}

		public void LogAdAction(AdActionAnalytics analytics)
		{
			Log("Logging AdAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogFunnelAction(FunnelStepsAnalytics analytics)
		{
			Log("Logging FunnelAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogNavigationAction(NavigationActionAnalytics analytics)
		{
			Log("Logging NavigationAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogTimingAction(TimingAnalytics analytics)
		{
			Log("Logging TimingAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogSwrvePurchase(string itemId, int cost, int quantity, string currency)
		{
			SwrveComponent.Instance.SDK.Purchase(itemId, currency, cost, quantity);
		}

		public void LogSwrvePurchase(string itemId, int cost, string currency)
		{
			SwrveComponent.Instance.SDK.Purchase(itemId, currency, cost, 1);
		}

		public void LogSwrveCurrencyGiven(string givenCurrency, double givenAmount)
		{
			SwrveComponent.Instance.SDK.CurrencyGiven(givenCurrency, givenAmount);
		}

		public void LogIAPAction(IAPAnalytics analytics)
		{
			Log("Logging IAP_custom: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogPurchaseAction(PurchaseAnalytics analytics)
		{
			Log("Logging purchase_custom: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogCurrencyGivenAction(CurrencyGivenAnalytics analytics)
		{
			Log("Logging currency_given_custom: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogAction(ActionAnalytics analytics)
		{
			Log("Logging Action: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogTestImpressionAction(TestImpressionAnalytics analytics)
		{
			Log("Logging TestImpressionAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogErrorAction(ErrorAnalytics analytics)
		{
			Log("Logging ErrorAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogFailedReceiptAction(FailedReceiptAnalytics analytics)
		{
			Log("Logging FailedReceiptAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogAnalyticsAction(IAnalytics analytics)
		{
			Log("Logging GenericAction: " + analytics.ToString());
			GenericSendLogAction(analytics);
		}

		public void LogGenericAction(string action)
		{
			GenericStringAnalytics genericStringAnalytics = new GenericStringAnalytics(action);
			Log("Logging GenericAction: " + genericStringAnalytics.ToString());
			GenericSendLogAction(genericStringAnalytics);
		}

		public void LogGenericAction(string action, Dictionary<string, object> messageDetails)
		{
			GenericWrapperAnalytics genericWrapperAnalytics = new GenericWrapperAnalytics(action, messageDetails);
			Log("Logging GenericAction: " + genericWrapperAnalytics.ToString());
			GenericSendLogAction(genericWrapperAnalytics);
		}

		private void GenericSendLogAction(IAnalytics analytics)
		{
			if (instance != null)
			{
				instance.LogAction(analytics.GetSwrveEvent(), analytics.Serialize());
			}
		}

		private Dictionary<string, string> CreateSwrvePayload(Dictionary<string, object> dataDetails)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> dataDetail in dataDetails)
			{
				dictionary.Add(dataDetail.Key, Json.Serialize(dataDetail.Value).Replace("\"", ""));
			}
			return dictionary;
		}

		private void LogAction(string eventName, int value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(eventName, value);
			LogAction(eventName, dictionary);
		}

		private void LogAction(string eventName, string value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(eventName, value);
			LogAction(eventName, dictionary);
		}

		private void LogAction(string eventName)
		{
			Dictionary<string, object> payload = null;
			LogAction(eventName, payload);
		}

		private void LogAction(string eventName, Dictionary<string, object> payload)
		{
			if (eventName == null)
			{
				return;
			}
			if (eventName.ToLower().StartsWith("swrve."))
			{
				RaiseAlert(string.Format("Event name {0} cannot be used because 'swrve.' is a reserved prefix.", eventName));
			}
			else if (!EventsSent.Contains(eventName) && EventsSent.Count >= 1000)
			{
				RaiseAlert(string.Format("There is a limit of {0} unique event namse that can be sent to Swrve. Not tracking {1}", 1000, eventName));
			}
			else if (SwrveComponent.Instance.SDK != null && SwrveComponent.Instance.SDK.Initialised)
			{
				if (payload != null)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					FlattenPayload(dictionary, payload, null);
					SwrveComponent.Instance.SDK.NamedEvent(eventName, dictionary);
				}
				else
				{
					SwrveComponent.Instance.SDK.NamedEvent(eventName);
				}
				EventsSent.Add(eventName);
			}
		}

		private void RaiseAlert(string message)
		{
			Log(message);
		}

		private void FlattenPayload(Dictionary<string, string> targetPayload, Dictionary<string, object> srcPayload, string prefix)
		{
			if (srcPayload != null && targetPayload != null)
			{
				foreach (string key in srcPayload.Keys)
				{
					if (targetPayload.Count > 500)
					{
						RaiseAlert(string.Format("Cannot have more than {0} keys in an event payload. Ignoring rest.", 500));
						break;
					}
					object obj = srcPayload[key];
					if (obj is Dictionary<string, object>)
					{
						string prefix2 = (prefix == null) ? key : string.Format("{0}|{1}", prefix, key);
						FlattenPayload(targetPayload, obj as Dictionary<string, object>, prefix2);
					}
					else
					{
						targetPayload.Add(key, obj.ToString());
					}
				}
			}
		}
	}
}
