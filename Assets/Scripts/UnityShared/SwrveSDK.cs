using SwrveUnity;
using SwrveUnity.Device;
using SwrveUnity.Helpers;
using SwrveUnity.Input;
using SwrveUnity.Messaging;
using SwrveUnity.ResourceManager;
using SwrveUnity.REST;
using SwrveUnity.Storage;
using SwrveUnityMiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class SwrveSDK
{
	public const string SdkVersion = "5.1.1";

	private const string Platform = "Unity ";

	private const float DefaultDPI = 160f;

	protected const string EventsSave = "Swrve_Events";

	protected const string InstallTimeEpochSave = "Swrve_JoinedDate";

	protected const string iOSdeviceTokenSave = "Swrve_iOSDeviceToken";

	protected const string GcmDeviceTokenSave = "Swrve_gcmDeviceToken";

	protected const string AdmDeviceTokenSave = "Swrve_admDeviceToken";

	protected const string WindowsDeviceTokenSave = "Swrve_windowsDeviceToken";

	protected const string GoogleAdvertisingIdSave = "Swrve_googleAdvertisingId";

	protected const string AbTestUserResourcesSave = "srcngt2";

	protected const string AbTestUserResourcesDiffSave = "rsdfngt2";

	protected const string DeviceIdSave = "Swrve_DeviceId";

	protected const string SeqNumSave = "Swrve_SeqNum";

	protected const string ResourcesCampaignTagSave = "cmpg_etag";

	protected const string ResourcesCampaignFlushFrequencySave = "swrve_cr_flush_frequency";

	protected const string ResourcesCampaignFlushDelaySave = "swrve_cr_flush_delay";

	private const string DeviceIdKey = "Swrve.deviceUniqueIdentifier";

	private const string EmptyJSONObject = "{}";

	private const float DefaultCampaignResourcesFlushFrenquency = 60f;

	private const float DefaultCampaignResourcesFlushRefreshDelay = 5f;

	public const string DefaultAutoShowMessagesTrigger = "Swrve.Messages.showAtSessionStart";

	private const string PushTrackingKey = "_p";

	private const string SilentPushTrackingKey = "_sp";

	private const string PushDeeplinkKey = "_sd";

	private const string PushNestedJsonKey = "_s.JsonPayload";

	private const int DefaultDelayFirstMessage = 150;

	private const long DefaultMaxShows = 99999L;

	private const int DefaultMinDelay = 55;

	protected int appId;

	protected string apiKey;

	protected string userId;

	protected SwrveConfig config;

	public string Language;

	public SwrveResourceManager ResourceManager;

	protected ISwrveAssetsManager SwrveAssetsManager;

	public MonoBehaviour Container;

	public ISwrveInstallButtonListener GlobalInstallButtonListener = null;

	public ISwrveCustomButtonListener GlobalCustomButtonListener = null;

	public ISwrveMessageListener GlobalMessageListener = null;

	public ISwrveConversationListener GlobalConversationListener = null;

	public ISwrvePushNotificationListener PushNotificationListener = null;

	public ISwrveTriggeredMessageListener TriggeredMessageListener = null;

	public Action ResourcesUpdatedCallback;

	public bool Initialised = false;

	public bool Destroyed = false;

	private string escapedUserId;

	private long installTimeEpoch;

	private string installTimeFormatted;

	private string lastPushEngagedId;

	private int deviceWidth;

	private int deviceHeight;

	private long lastSessionTick;

	private ICarrierInfo deviceCarrierInfo;

	private System.Random rnd = new System.Random();

	protected StringBuilder eventBufferStringBuilder;

	protected string eventsPostString;

	protected string swrvePath;

	protected ISwrveStorage storage;

	protected IRESTClient restClient;

	private string eventsUrl;

	private string abTestResourcesDiffUrl;

	protected bool eventsConnecting;

	protected bool abTestUserResourcesDiffConnecting;

	protected string userResourcesRaw;

	protected Dictionary<string, Dictionary<string, string>> userResources;

	protected float campaignsAndResourcesFlushFrequency;

	protected float campaignsAndResourcesFlushRefreshDelay;

	protected string lastETag;

	protected long campaignsAndResourcesLastRefreshed;

	protected bool campaignsAndResourcesInitialized;

	private static readonly int CampaignEndpointVersion = 6;

	private static readonly int CampaignResponseVersion = 2;

	protected static readonly string CampaignsSave = "cmcc2";

	protected static readonly string CampaignsSettingsSave = "Swrve_CampaignsData";

	protected static readonly string LocationSave = "loccc2";

	protected static readonly string QaUserSave = "swrve.q1";

	protected static readonly string InstallTimeFormat = "yyyyMMdd";

	private string resourcesAndCampaignsUrl;

	protected string swrveTemporaryPath;

	protected bool campaignsConnecting;

	protected bool autoShowMessagesEnabled;

	protected Dictionary<int, SwrveCampaignState> campaignsState = new Dictionary<int, SwrveCampaignState>();

	protected List<SwrveBaseCampaign> campaigns = new List<SwrveBaseCampaign>();

	protected Dictionary<string, object> campaignSettings = new Dictionary<string, object>();

	protected Dictionary<string, string> appStoreLinks = new Dictionary<string, string>();

	protected SwrveMessageFormat currentMessage = null;

	protected SwrveMessageFormat currentDisplayingMessage = null;

	protected SwrveOrientation currentOrientation;

	protected IInputManager inputManager = NativeInputManager.Instance;

	protected string prefabName;

	private DateTime initialisedTime;

	private DateTime showMessagesAfterLaunch;

	private DateTime showMessagesAfterDelay;

	private long messagesLeftToShow;

	private int minDelayBetweenMessage;

	protected SwrveQAUser qaUser;

	private bool campaignAndResourcesCoroutineEnabled = true;

	private IEnumerator campaignAndResourcesCoroutineInstance;

	private int locationSegmentVersion;

	private int conversationVersion;

	public int AppId
	{
		get
		{
			return appId;
		}
	}

	public string ApiKey
	{
		get
		{
			return apiKey;
		}
	}

	public string UserId
	{
		get
		{
			return userId;
		}
	}

	private void setNativeInfo(Dictionary<string, string> deviceInfo)
	{
	}

	private string getNativeLanguage()
	{
		return null;
	}

	private void setNativeAppVersion()
	{
	}

	private void showNativeConversation(string conversation)
	{
	}

	private void initNative()
	{
	}

	private void startNativeLocation()
	{
	}

	private void startNativeLocationAfterPermission()
	{
	}

	public void LocationUserUpdate(Dictionary<string, string> map)
	{
	}

	public string GetPlotNotifications()
	{
		return "[]";
	}

	private void setNativeConversationVersion()
	{
	}

	private bool NativeIsBackPressed()
	{
		return false;
	}

	public void updateQAUser(Dictionary<string, object> map)
	{
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey)
	{
		Init(container, appId, apiKey, new SwrveConfig());
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey, string userId)
	{
		SwrveConfig swrveConfig = new SwrveConfig();
		swrveConfig.UserId = userId;
		Init(container, appId, apiKey, swrveConfig);
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey, string userId, SwrveConfig config)
	{
		config.UserId = userId;
		Init(container, appId, apiKey, config);
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey, SwrveConfig config)
	{
		Container = container;
		ResourceManager = new SwrveResourceManager();
		this.config = config;
		prefabName = container.name;
		this.appId = appId;
		this.apiKey = apiKey;
		userId = config.UserId;
		Language = config.Language;
		lastSessionTick = GetSessionTime();
		initialisedTime = SwrveHelper.GetNow();
		campaignsAndResourcesInitialized = false;
		autoShowMessagesEnabled = true;
		swrveTemporaryPath = GetSwrveTemporaryCachePath();
		InitAssetsManager(container, swrveTemporaryPath);
		if (string.IsNullOrEmpty(apiKey))
		{
			throw new Exception("The api key has not been specified.");
		}
		if (string.IsNullOrEmpty(userId))
		{
			userId = GetDeviceUniqueId();
		}
		if (!string.IsNullOrEmpty(userId))
		{
			PlayerPrefs.SetString("Swrve.deviceUniqueIdentifier", userId);
			PlayerPrefs.Save();
		}
		SwrveLog.Log("Your user id is: " + userId);
		escapedUserId = WWW.EscapeURL(userId);
		if (string.IsNullOrEmpty(Language))
		{
			Language = GetDeviceLanguage();
			if (string.IsNullOrEmpty(Language))
			{
				Language = config.DefaultLanguage;
			}
		}
		config.CalculateEndpoints(appId);
		string contentServer = config.ContentServer;
		eventsUrl = config.EventsServer + "/1/batch";
		abTestResourcesDiffUrl = contentServer + "/api/1/user_resources_diff";
		resourcesAndCampaignsUrl = contentServer + "/api/1/user_resources_and_campaigns";
		swrvePath = GetSwrvePath();
		if (storage == null)
		{
			storage = CreateStorage();
		}
		storage.SetSecureFailedListener(delegate
		{
			NamedEventInternal("Swrve.signature_invalid", null, false);
		});
		restClient = CreateRestClient();
		eventBufferStringBuilder = new StringBuilder(config.MaxBufferChars);
		string savedInstallTimeEpoch = GetSavedInstallTimeEpoch();
		LoadData();
		if (config.ABTestDetailsEnabled)
		{
			try
			{
				LoadABTestDetails();
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while initializing " + arg);
			}
		}
		InitUserResources();
		deviceCarrierInfo = new DeviceCarrierInfo();
		GetDeviceScreenInfo();
		Initialised = true;
		if (config.AutomaticSessionManagement)
		{
			QueueSessionStart();
			GenerateNewSessionInterval();
		}
		if (string.IsNullOrEmpty(savedInstallTimeEpoch))
		{
			NamedEventInternal("Swrve.first_session", null, false);
		}
		if (SwrveHelper.IsOnDevice())
		{
			InitNative();
		}
		ProcessInfluenceData();
		QueueDeviceInfo();
		SendQueuedEvents();
		if (config.MessagingEnabled)
		{
			if (string.IsNullOrEmpty(Language))
			{
				throw new Exception("Language needed to use messaging");
			}
			if (string.IsNullOrEmpty(config.AppStore))
			{
				throw new Exception("App store must be apple, google, amazon or a custom app store");
			}
			try
			{
				LoadTalkData();
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while initializing " + arg);
			}
		}
		DisableAutoShowAfterDelay();
		StartCampaignsAndResourcesTimer();
	}

	protected virtual void InitAssetsManager(MonoBehaviour container, string swrveTemporaryPath)
	{
		SwrveAssetsManager = new SwrveAssetsManager(container, swrveTemporaryPath);
	}

	public virtual void SessionStart()
	{
		QueueSessionStart();
		SendQueuedEvents();
	}

	public virtual void SessionEnd()
	{
		Dictionary<string, object> eventParameters = new Dictionary<string, object>();
		AppendEventToBuffer("session_end", eventParameters);
	}

	public virtual void NamedEvent(string name, Dictionary<string, string> payload = null)
	{
		if (name != null && !name.ToLower().StartsWith("swrve."))
		{
			NamedEventInternal(name, payload);
		}
		else
		{
			SwrveLog.LogError("Event cannot begin with \"Swrve.\". The event " + name + " will not be sent");
		}
	}

	public virtual void UserUpdate(Dictionary<string, string> attributes)
	{
		if (attributes != null && attributes.Count > 0)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("attributes", attributes);
			AppendEventToBuffer("user", dictionary);
		}
		else
		{
			SwrveLog.LogError("Invoked user update with no update attributes");
		}
	}

	public virtual void UserUpdate(string name, DateTime date)
	{
		if (name != null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = date.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			dictionary.Add(name, value);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("attributes", dictionary);
			AppendEventToBuffer("user", dictionary2);
		}
		else
		{
			SwrveLog.LogError("Invoked user update with date with no name specified");
		}
	}

	public virtual void Purchase(string item, string currency, int cost, int quantity)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("item", item);
		dictionary.Add("currency", currency);
		dictionary.Add("cost", cost);
		dictionary.Add("quantity", quantity);
		AppendEventToBuffer("purchase", dictionary);
	}

	public virtual void Iap(int quantity, string productId, double productPrice, string currency)
	{
		IapRewards rewards = new IapRewards();
		Iap(quantity, productId, productPrice, currency, rewards);
	}

	public virtual void Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards)
	{
		_Iap(quantity, productId, productPrice, currency, rewards, string.Empty, string.Empty, string.Empty, "unknown_store");
	}

	public virtual void CurrencyGiven(string givenCurrency, double amount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("given_currency", givenCurrency);
		dictionary.Add("given_amount", amount);
		AppendEventToBuffer("currency_given", dictionary);
	}

	public virtual bool SendQueuedEvents()
	{
		bool result = false;
		if (Initialised)
		{
			if (!eventsConnecting)
			{
				byte[] array = null;
				if (eventsPostString == null || eventsPostString.Length == 0)
				{
					eventsPostString = eventBufferStringBuilder.ToString();
					eventBufferStringBuilder.Length = 0;
				}
				if (eventsPostString.Length > 0)
				{
					long seconds = SwrveHelper.GetSeconds();
					array = PostBodyBuilder.Build(apiKey, appId, userId, GetDeviceId(), GetAppVersion(), seconds, eventsPostString);
				}
				if (array != null)
				{
					eventsConnecting = true;
					SwrveLog.Log("Sending events to Swrve");
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("Content-Type", "application/json; charset=utf-8");
					Dictionary<string, string> requestHeaders = dictionary;
					result = true;
					StartTask("PostEvents_Coroutine", PostEvents_Coroutine(requestHeaders, array));
				}
				else
				{
					eventsPostString = null;
				}
			}
			else
			{
				SwrveLog.LogWarning("Sending events already in progress");
			}
		}
		return result;
	}

	public virtual void GetUserResources(Action<Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
		if (Initialised)
		{
			if (userResources != null)
			{
				onResult(userResources, userResourcesRaw);
			}
			else
			{
				onResult(new Dictionary<string, Dictionary<string, string>>(), "[]");
			}
		}
	}

	public virtual void GetUserResourcesDiff(Action<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
		if (Initialised && !abTestUserResourcesDiffConnecting)
		{
			abTestUserResourcesDiffConnecting = true;
			StringBuilder stringBuilder = new StringBuilder(abTestResourcesDiffUrl);
			stringBuilder.AppendFormat("?user={0}&api_key={1}&app_version={2}&joined={3}", escapedUserId, apiKey, WWW.EscapeURL(GetAppVersion()), installTimeEpoch);
			SwrveLog.Log("AB Test User Resources Diff request: " + stringBuilder.ToString());
			StartTask("GetUserResourcesDiff_Coroutine", GetUserResourcesDiff_Coroutine(stringBuilder.ToString(), onResult, onError, "rsdfngt2"));
		}
		else
		{
			string message = "Failed to initiate A/B test Diff GET request";
			SwrveLog.LogError(message);
			if (onError != null)
			{
				onError(new Exception(message));
			}
		}
	}

	public virtual void LoadFromDisk()
	{
		LoadEventsFromDisk();
	}

	public virtual void FlushToDisk(bool saveEventsBeingSent = false)
	{
		if (!Initialised || eventBufferStringBuilder == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string text = eventBufferStringBuilder.ToString();
		eventBufferStringBuilder.Length = 0;
		if (saveEventsBeingSent)
		{
			stringBuilder.Append(eventsPostString);
			eventsPostString = null;
			if (text.Length > 0)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(text);
			}
		}
		else
		{
			stringBuilder.Append(text);
		}
		try
		{
			string value = storage.Load("Swrve_Events", userId);
			if (!string.IsNullOrEmpty(value))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(value);
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Could not read events from cache (" + ex.ToString() + ")");
		}
		string data = stringBuilder.ToString();
		storage.Save("Swrve_Events", data, userId);
	}

	public string BasePath()
	{
		return swrvePath;
	}

	public virtual Dictionary<string, string> GetDeviceInfo()
	{
		string deviceModel = GetDeviceModel();
		string operatingSystem = SystemInfo.operatingSystem;
		string value = "PC";
		float num = (Screen.dpi == 0f) ? 160f : Screen.dpi;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("swrve.device_name", deviceModel);
		dictionary.Add("swrve.os", value);
		dictionary.Add("swrve.device_width", deviceWidth.ToString());
		dictionary.Add("swrve.device_height", deviceHeight.ToString());
		dictionary.Add("swrve.device_dpi", num.ToString());
		dictionary.Add("swrve.language", Language);
		dictionary.Add("swrve.os_version", operatingSystem);
		dictionary.Add("swrve.app_store", config.AppStore);
		dictionary.Add("swrve.sdk_version", "Unity 5.1.1");
		dictionary.Add("swrve.unity_version", Application.unityVersion);
		dictionary.Add("swrve.install_date", installTimeFormatted);
		Dictionary<string, string> dictionary2 = dictionary;
		string text2 = dictionary2["swrve.utc_offset_seconds"] = DateTimeOffset.Now.Offset.TotalSeconds.ToString();
		setNativeInfo(dictionary2);
		ICarrierInfo carrierInfoProvider = GetCarrierInfoProvider();
		if (carrierInfoProvider != null)
		{
			string name = carrierInfoProvider.GetName();
			if (!string.IsNullOrEmpty(name))
			{
				dictionary2["swrve.sim_operator.name"] = name;
			}
			string isoCountryCode = carrierInfoProvider.GetIsoCountryCode();
			if (!string.IsNullOrEmpty(isoCountryCode))
			{
				dictionary2["swrve.sim_operator.iso_country_code"] = isoCountryCode;
			}
			string carrierCode = carrierInfoProvider.GetCarrierCode();
			if (!string.IsNullOrEmpty(carrierCode))
			{
				dictionary2["swrve.sim_operator.code"] = carrierCode;
			}
		}
		return dictionary2;
	}

	public virtual void OnSwrvePause()
	{
		if (Initialised)
		{
			FlushToDisk();
			GenerateNewSessionInterval();
			if (config != null && config.AutoDownloadCampaignsAndResources)
			{
				StopCheckForCampaignAndResources();
			}
		}
	}

	public virtual void OnSwrveResume()
	{
		if (Initialised)
		{
			LoadFromDisk();
			QueueDeviceInfo();
			long sessionTime = GetSessionTime();
			if (sessionTime >= lastSessionTick)
			{
				SessionStart();
			}
			else
			{
				SendQueuedEvents();
			}
			GenerateNewSessionInterval();
			StartCampaignsAndResourcesTimer();
			DisableAutoShowAfterDelay();
			ProcessInfluenceData();
		}
	}

	public virtual void OnSwrveDestroy()
	{
		if (!Destroyed)
		{
			Destroyed = true;
			if (Initialised)
			{
				FlushToDisk(true);
			}
			if (config != null && config.AutoDownloadCampaignsAndResources)
			{
				StopCheckForCampaignAndResources();
			}
		}
	}

	public virtual List<SwrveBaseCampaign> GetCampaigns()
	{
		return campaigns;
	}

	public virtual void ButtonWasPressedByUser(SwrveButton button)
	{
		if (button != null)
		{
			try
			{
				SwrveLog.Log(string.Concat("Button ", button.ActionType, ": ", button.Action, " app id: ", button.AppId));
				if (button.ActionType != SwrveActionType.Dismiss)
				{
					string text = "Swrve.Messages.Message-" + button.Message.Id + ".click";
					SwrveLog.Log("Sending click event: " + text);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("name", button.Name);
					NamedEventInternal(text, dictionary, false);
				}
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while processing button click " + arg);
			}
		}
	}

	public virtual void MessageWasShownToUser(SwrveMessageFormat messageFormat)
	{
		try
		{
			SetMessageMinDelayThrottle();
			messagesLeftToShow--;
			SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)messageFormat.Message.Campaign;
			if (swrveMessagesCampaign != null)
			{
				swrveMessagesCampaign.MessageWasShownToUser(messageFormat);
				SaveCampaignData(swrveMessagesCampaign);
			}
			string text = "Swrve.Messages.Message-" + messageFormat.Message.Id + ".impression";
			SwrveLog.Log("Sending view event: " + text);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("format", messageFormat.Name);
			dictionary.Add("orientation", messageFormat.Orientation.ToString());
			dictionary.Add("size", messageFormat.Size.X + "x" + messageFormat.Size.Y);
			NamedEventInternal(text, dictionary, false);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while processing message impression " + arg);
		}
	}

	public virtual bool IsMessageDisplaying()
	{
		return currentMessage != null;
	}

	public void SetLocationSegmentVersion(int locationSegmentVersion)
	{
		this.locationSegmentVersion = locationSegmentVersion;
	}

	public void SetConversationVersion(int conversationVersion)
	{
		this.conversationVersion = conversationVersion;
	}

	public string GetAppStoreLink(int appId)
	{
		string value = null;
		if (appStoreLinks != null)
		{
			appStoreLinks.TryGetValue(appId.ToString(), out value);
		}
		return value;
	}

	public virtual SwrveMessage GetMessageForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		if (!checkCampaignRules(eventName, SwrveHelper.GetNow()))
		{
			return null;
		}
		try
		{
			return _getMessageForEvent(eventName, payload);
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "message");
		}
		return null;
	}

	private SwrveMessage _getMessageForEvent(string eventName, IDictionary<string, string> payload)
	{
		SwrveMessage swrveMessage = null;
		SwrveBaseCampaign swrveBaseCampaign = null;
		SwrveLog.Log("Trying to get message for: " + eventName);
		IEnumerator<SwrveBaseCampaign> enumerator = campaigns.GetEnumerator();
		List<SwrveMessage> list = new List<SwrveMessage>();
		int num = int.MaxValue;
		List<SwrveMessage> list2 = new List<SwrveMessage>();
		SwrveOrientation deviceOrientation = GetDeviceOrientation();
		while (enumerator.MoveNext() && swrveMessage == null)
		{
			if (!enumerator.Current.IsA<SwrveMessagesCampaign>())
			{
				continue;
			}
			SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)enumerator.Current;
			SwrveMessage messageForEvent = swrveMessagesCampaign.GetMessageForEvent(eventName, payload, qaUser);
			if (messageForEvent == null)
			{
				continue;
			}
			if (messageForEvent.SupportsOrientation(deviceOrientation))
			{
				list.Add(messageForEvent);
				if (messageForEvent.Priority <= num)
				{
					if (messageForEvent.Priority < num)
					{
						list2.Clear();
					}
					num = messageForEvent.Priority;
					list2.Add(messageForEvent);
				}
			}
			else if (qaUser != null)
			{
				qaUser.campaignMessages[swrveMessagesCampaign.Id] = messageForEvent;
				qaUser.campaignReasons[swrveMessagesCampaign.Id] = "Message didn't support the current device orientation: " + deviceOrientation;
			}
		}
		if (list2.Count > 0)
		{
			list2.Shuffle();
			swrveMessage = list2[0];
			swrveBaseCampaign = swrveMessage.Campaign;
		}
		if (qaUser != null && swrveBaseCampaign != null && swrveMessage != null)
		{
			IEnumerator<SwrveMessage> enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				SwrveMessage current = enumerator2.Current;
				if (current != swrveMessage)
				{
					int id = current.Campaign.Id;
					if (qaUser != null && !qaUser.campaignMessages.ContainsKey(id))
					{
						qaUser.campaignMessages.Add(id, current);
						qaUser.campaignReasons.Add(id, "Campaign " + swrveBaseCampaign.Id + " was selected for display ahead of this campaign");
					}
				}
			}
		}
		return swrveMessage;
	}

	public virtual SwrveConversation GetConversationForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		if (!checkCampaignRules(eventName, SwrveHelper.GetNow()))
		{
			return null;
		}
		try
		{
			return _getConversationForEvent(eventName, payload);
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "conversation");
		}
		return null;
	}

	private SwrveConversation _getConversationForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		SwrveConversation swrveConversation = null;
		SwrveBaseCampaign swrveBaseCampaign = null;
		SwrveLog.Log("Trying to get conversation for: " + eventName);
		IEnumerator<SwrveBaseCampaign> enumerator = campaigns.GetEnumerator();
		List<SwrveConversation> list = new List<SwrveConversation>();
		int num = int.MaxValue;
		List<SwrveConversation> list2 = new List<SwrveConversation>();
		while (enumerator.MoveNext() && swrveConversation == null)
		{
			if (!enumerator.Current.IsA<SwrveConversationCampaign>())
			{
				continue;
			}
			SwrveConversationCampaign swrveConversationCampaign = (SwrveConversationCampaign)enumerator.Current;
			SwrveConversation conversationForEvent = swrveConversationCampaign.GetConversationForEvent(eventName, payload, qaUser);
			if (conversationForEvent == null)
			{
				continue;
			}
			list.Add(conversationForEvent);
			if (conversationForEvent.Priority <= num)
			{
				if (conversationForEvent.Priority < num)
				{
					list2.Clear();
				}
				num = conversationForEvent.Priority;
				list2.Add(conversationForEvent);
			}
		}
		if (list2.Count > 0)
		{
			list2.Shuffle();
			swrveConversation = list2[0];
		}
		if (qaUser != null && swrveBaseCampaign != null && swrveConversation != null)
		{
			IEnumerator<SwrveConversation> enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				SwrveConversation current = enumerator2.Current;
				if (current != swrveConversation)
				{
					int id = current.Campaign.Id;
					if (qaUser != null && !qaUser.campaignMessages.ContainsKey(id))
					{
						qaUser.campaignMessages[id] = current;
						qaUser.campaignReasons[id] = "Campaign " + swrveBaseCampaign.Id + " was selected for display ahead of this campaign";
					}
				}
			}
		}
		return swrveConversation;
	}

	private bool checkCampaignRules(string eventName, DateTime now)
	{
		return false;
	}

	public virtual void ShowMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
		ShowMessageCenterCampaign(campaign, GetDeviceOrientation());
	}

	public virtual void ShowMessageCenterCampaign(SwrveBaseCampaign campaign, SwrveOrientation orientation)
	{
		if (campaign.IsA<SwrveMessagesCampaign>())
		{
			Container.StartCoroutine(LaunchMessage(((SwrveMessagesCampaign)campaign).Messages.Where((SwrveMessage a) => a.SupportsOrientation(orientation)).First(), GlobalInstallButtonListener, GlobalCustomButtonListener, GlobalMessageListener));
		}
		else if (campaign.IsA<SwrveConversationCampaign>())
		{
			Container.StartCoroutine(LaunchConversation(((SwrveConversationCampaign)campaign).Conversation));
		}
		campaign.Status = SwrveCampaignState.Status.Seen;
		SaveCampaignData(campaign);
	}

	public virtual List<SwrveBaseCampaign> GetMessageCenterCampaigns()
	{
		return GetMessageCenterCampaigns(GetDeviceOrientation());
	}

	public virtual List<SwrveBaseCampaign> GetMessageCenterCampaigns(SwrveOrientation orientation)
	{
		List<SwrveBaseCampaign> list = new List<SwrveBaseCampaign>();
		IEnumerator<SwrveBaseCampaign> enumerator = campaigns.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SwrveBaseCampaign current = enumerator.Current;
			if (isValidMessageCenter(current, orientation))
			{
				list.Add(current);
			}
		}
		return list;
	}

	public virtual void RemoveMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
		campaign.Status = SwrveCampaignState.Status.Deleted;
		SaveCampaignData(campaign);
	}

	public virtual SwrveMessage GetMessageForId(int messageId)
	{
		SwrveMessage swrveMessage = null;
		IEnumerator<SwrveBaseCampaign> enumerator = campaigns.GetEnumerator();
		while (enumerator.MoveNext() && swrveMessage == null)
		{
			if (enumerator.Current.IsA<SwrveMessagesCampaign>())
			{
				SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)enumerator.Current;
				swrveMessage = swrveMessagesCampaign.GetMessageForId(messageId);
				if (swrveMessage != null)
				{
					return swrveMessage;
				}
			}
		}
		SwrveLog.LogWarning("Message with id " + messageId + " not found");
		return null;
	}

	public virtual IEnumerator ShowMessageForEvent(string eventName, SwrveMessage message, ISwrveInstallButtonListener installButtonListener = null, ISwrveCustomButtonListener customButtonListener = null, ISwrveMessageListener messageListener = null)
	{
		if (TriggeredMessageListener != null)
		{
			if (message != null)
			{
				TriggeredMessageListener.OnMessageTriggered(message);
			}
		}
		else if (currentMessage == null)
		{
			yield return Container.StartCoroutine(LaunchMessage(message, installButtonListener, customButtonListener, messageListener));
		}
		TaskFinished("ShowMessageForEvent");
	}

	public virtual IEnumerator ShowConversationForEvent(string eventName, SwrveConversation conversation)
	{
		yield return Container.StartCoroutine(LaunchConversation(conversation));
		TaskFinished("ShowConversationForEvent");
	}

	public virtual void DismissMessage()
	{
		if (TriggeredMessageListener != null)
		{
			TriggeredMessageListener.DismissCurrentMessage();
		}
		else
		{
			try
			{
				if (currentMessage != null)
				{
					SetMessageMinDelayThrottle();
					currentMessage.Dismiss();
				}
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while dismissing a message " + arg);
			}
		}
	}

	public virtual void RefreshUserResourcesAndCampaigns()
	{
		LoadResourcesAndCampaigns();
	}

	private void QueueSessionStart()
	{
		Dictionary<string, object> eventParameters = new Dictionary<string, object>();
		AppendEventToBuffer("session_start", eventParameters);
	}

	protected void NamedEventInternal(string name, Dictionary<string, string> payload = null, bool allowShowMessage = true)
	{
		if (payload == null)
		{
			payload = new Dictionary<string, string>();
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("name", name);
		dictionary.Add("payload", payload);
		AppendEventToBuffer("event", dictionary, allowShowMessage);
	}

	protected static string GetSwrvePath()
	{
		string text = Application.persistentDataPath;
		if (string.IsNullOrEmpty(text))
		{
			text = Application.temporaryCachePath;
			SwrveLog.Log("Swrve path (tried again): " + text);
		}
		return text;
	}

	protected static string GetSwrveTemporaryCachePath()
	{
		string text = Application.temporaryCachePath;
		if (text == null || text.Length == 0)
		{
			text = Application.persistentDataPath;
		}
		if (!File.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	private void _Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards, string receipt, string receiptSignature, string transactionId, string appStore)
	{
		if (!_Iap_check_arguments(quantity, productId, productPrice, currency, appStore))
		{
			SwrveLog.LogError("ERROR: IAP event not sent because it received an illegal argument");
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("app_store", appStore);
		dictionary.Add("local_currency", currency);
		dictionary.Add("cost", productPrice);
		dictionary.Add("product_id", productId);
		dictionary.Add("quantity", quantity);
		dictionary.Add("rewards", rewards.getRewards());
		if (!string.IsNullOrEmpty(GetAppVersion()))
		{
			dictionary.Add("app_version", GetAppVersion());
		}
		if (appStore == "apple")
		{
			dictionary.Add("receipt", receipt);
			if (!string.IsNullOrEmpty(transactionId))
			{
				dictionary.Add("transaction_id", transactionId);
			}
		}
		else if (appStore == "google")
		{
			dictionary.Add("receipt", receipt);
			dictionary.Add("receipt_signature", receiptSignature);
		}
		else
		{
			dictionary.Add("receipt", receipt);
		}
		AppendEventToBuffer("iap", dictionary);
		if (config.AutoDownloadCampaignsAndResources)
		{
			CheckForCampaignsAndResourcesUpdates(false);
		}
	}

	protected virtual SwrveOrientation GetDeviceOrientation()
	{
		switch (Screen.orientation)
		{
		case ScreenOrientation.LandscapeLeft:
		case ScreenOrientation.LandscapeRight:
			return SwrveOrientation.Landscape;
		case ScreenOrientation.Portrait:
		case ScreenOrientation.PortraitUpsideDown:
			return SwrveOrientation.Portrait;
		default:
			if (Screen.height >= Screen.width)
			{
				return SwrveOrientation.Portrait;
			}
			return SwrveOrientation.Landscape;
		}
	}

	private bool _Iap_check_arguments(int quantity, string productId, double productPrice, string currency, string appStore)
	{
		if (string.IsNullOrEmpty(productId))
		{
			SwrveLog.LogError("IAP event illegal argument: productId cannot be empty");
			return false;
		}
		if (string.IsNullOrEmpty(currency))
		{
			SwrveLog.LogError("IAP event illegal argument: currency cannot be empty");
			return false;
		}
		if (string.IsNullOrEmpty(appStore))
		{
			SwrveLog.LogError("IAP event illegal argument: appStore cannot be empty");
			return false;
		}
		if (quantity <= 0)
		{
			SwrveLog.LogError("IAP event illegal argument: quantity must be greater than zero");
			return false;
		}
		if (productPrice < 0.0)
		{
			SwrveLog.LogError("IAP event illegal argument: productPrice must be greater than or equal to zero");
			return false;
		}
		return true;
	}

	private Dictionary<string, Dictionary<string, string>> ProcessUserResources(IList<object> userResources)
	{
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
		if (userResources != null)
		{
			IEnumerator<object> enumerator = userResources.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)enumerator.Current;
				string key = (string)dictionary2["uid"];
				dictionary.Add(key, NormalizeJson(dictionary2));
			}
		}
		return dictionary;
	}

	private Dictionary<string, string> NormalizeJson(Dictionary<string, object> json)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, object>.Enumerator enumerator = json.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, object> current = enumerator.Current;
			if (current.Value != null)
			{
				dictionary.Add(current.Key, current.Value.ToString());
			}
		}
		return dictionary;
	}

	private IEnumerator GetUserResourcesDiff_Coroutine(string getRequest, Action<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError, string saveCategory)
	{
		Exception wwwException = null;
		string abTestCandidate = null;
		yield return Container.StartCoroutine(restClient.Get(getRequest, delegate(RESTResponse response)
		{
			if (response.Error == WwwDeducedError.NoError)
			{
				abTestCandidate = response.Body;
				SwrveLog.Log("AB Test result: " + abTestCandidate);
				storage.SaveSecure(saveCategory, abTestCandidate, userId);
				TaskFinished("GetUserResourcesDiff_Coroutine");
			}
			else
			{
				wwwException = new Exception(response.Error.ToString());
				SwrveLog.LogError("AB Test request failed: " + response.Error);
				TaskFinished("GetUserResourcesDiff_Coroutine");
			}
		}));
		abTestUserResourcesDiffConnecting = false;
		if (wwwException != null || string.IsNullOrEmpty(abTestCandidate))
		{
			try
			{
				string text = storage.LoadSecure(saveCategory, userId);
				if (string.IsNullOrEmpty(text))
				{
					onError(wwwException);
				}
				else if (ResponseBodyTester.TestUTF8(text, out abTestCandidate))
				{
					Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, Dictionary<string, string>> dictionary2 = new Dictionary<string, Dictionary<string, string>>();
					ProcessUserResourcesDiff(abTestCandidate, dictionary, dictionary2);
					onResult(dictionary, dictionary2, abTestCandidate);
				}
				else
				{
					onError(wwwException);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Could not read user resources diff from cache (" + ex.ToString() + ")");
				onError(wwwException);
			}
		}
		else if (!string.IsNullOrEmpty(abTestCandidate))
		{
			Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
			Dictionary<string, Dictionary<string, string>> dictionary2 = new Dictionary<string, Dictionary<string, string>>();
			ProcessUserResourcesDiff(abTestCandidate, dictionary, dictionary2);
			onResult(dictionary, dictionary2, abTestCandidate);
		}
	}

	private void ProcessUserResourcesDiff(string abTestJson, Dictionary<string, Dictionary<string, string>> newResources, Dictionary<string, Dictionary<string, string>> oldResources)
	{
		IList<object> list = (List<object>)Json.Deserialize(abTestJson);
		if (list == null)
		{
			return;
		}
		IEnumerator<object> enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)enumerator.Current;
			string key = (string)dictionary["uid"];
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["diff"];
			IEnumerator<string> enumerator2 = dictionary2.Keys.GetEnumerator();
			Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
			Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
			while (enumerator2.MoveNext())
			{
				Dictionary<string, string> dictionary5 = NormalizeJson((Dictionary<string, object>)dictionary2[enumerator2.Current]);
				dictionary3.Add(enumerator2.Current, dictionary5["new"]);
				dictionary4.Add(enumerator2.Current, dictionary5["old"]);
			}
			newResources.Add(key, dictionary3);
			oldResources.Add(key, dictionary4);
		}
	}

	private long GetInstallTimeEpoch()
	{
		string savedInstallTimeEpoch = GetSavedInstallTimeEpoch();
		if (!string.IsNullOrEmpty(savedInstallTimeEpoch))
		{
			long result = 0L;
			if (long.TryParse(savedInstallTimeEpoch, out result))
			{
				return result;
			}
		}
		long sessionTime = GetSessionTime();
		storage.Save("Swrve_JoinedDate", sessionTime.ToString(), userId);
		return sessionTime;
	}

	private string GetDeviceId()
	{
		string text = storage.Load("Swrve_DeviceId", userId);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		short num = (short)new System.Random().Next(32767);
		storage.Save("Swrve_DeviceId", num.ToString(), userId);
		return num.ToString();
	}

	private string getNextSeqNum()
	{
		string s = storage.Load("Swrve_SeqNum", userId);
		object obj;
		int result;
		if (!int.TryParse(s, out result))
		{
			obj = "1";
		}
		else
		{
			int num = ++result;
			obj = num.ToString();
		}
		s = (string)obj;
		storage.Save("Swrve_SeqNum", s, userId);
		return s;
	}

	protected string GetDeviceLanguage()
	{
		string text = getNativeLanguage();
		if (string.IsNullOrEmpty(text))
		{
			CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
			string text2 = currentUICulture.TwoLetterISOLanguageName.ToLower();
			if (text2 != "iv")
			{
				text = text2;
			}
		}
		return text;
	}

	protected string GetSavedInstallTimeEpoch()
	{
		try
		{
			string text = storage.Load("Swrve_JoinedDate", userId);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogError("Couldn't obtain saved install time: " + ex.Message);
		}
		return null;
	}

	protected void InvalidateETag()
	{
		lastETag = string.Empty;
		storage.Remove("cmpg_etag", userId);
	}

	private void InitUserResources()
	{
		userResourcesRaw = storage.LoadSecure("srcngt2", userId);
		if (!string.IsNullOrEmpty(userResourcesRaw))
		{
			IList<object> list = (IList<object>)Json.Deserialize(userResourcesRaw);
			userResources = ProcessUserResources(list);
			NotifyUpdateUserResources();
		}
		else
		{
			InvalidateETag();
		}
	}

	private void NotifyUpdateUserResources()
	{
		if (userResources != null)
		{
			ResourceManager.SetResourcesFromJSON(userResources);
			if (ResourcesUpdatedCallback != null)
			{
				ResourcesUpdatedCallback();
			}
		}
	}

	private void LoadEventsFromDisk()
	{
		try
		{
			string value = storage.Load("Swrve_Events", userId);
			storage.Remove("Swrve_Events", userId);
			if (!string.IsNullOrEmpty(value))
			{
				if (eventBufferStringBuilder.Length != 0)
				{
					eventBufferStringBuilder.Insert(0, ",");
				}
				eventBufferStringBuilder.Insert(0, value);
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Could not read events from cache (" + ex.ToString() + ")");
		}
	}

	private void LoadData()
	{
		LoadEventsFromDisk();
		installTimeEpoch = GetInstallTimeEpoch();
		installTimeFormatted = SwrveHelper.EpochToFormat(installTimeEpoch, InstallTimeFormat);
		lastETag = storage.Load("cmpg_etag", userId);
		string text = storage.Load("swrve_cr_flush_frequency", userId);
		if (!string.IsNullOrEmpty(text) && float.TryParse(text, out campaignsAndResourcesFlushFrequency))
		{
			campaignsAndResourcesFlushFrequency /= 1000f;
		}
		if (campaignsAndResourcesFlushFrequency == 0f)
		{
			campaignsAndResourcesFlushFrequency = 60f;
		}
		string text2 = storage.Load("swrve_cr_flush_delay", userId);
		if (!string.IsNullOrEmpty(text2) && float.TryParse(text2, out campaignsAndResourcesFlushRefreshDelay))
		{
			campaignsAndResourcesFlushRefreshDelay /= 1000f;
		}
		if (campaignsAndResourcesFlushRefreshDelay == 0f)
		{
			campaignsAndResourcesFlushRefreshDelay = 5f;
		}
	}

	protected string GetUniqueKey()
	{
		return apiKey + userId;
	}

	private string GetDeviceUniqueId()
	{
		string text = PlayerPrefs.GetString("Swrve.deviceUniqueIdentifier", null);
		if (string.IsNullOrEmpty(text))
		{
			text = GetRandomUUID();
		}
		return text;
	}

	private string GetRandomUUID()
	{
		try
		{
			Type type = Type.GetType("System.Guid");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("NewGuid");
				if (method != null)
				{
					object obj = method.Invoke(null, null);
					if (obj != null)
					{
						string text = obj.ToString();
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get random UUID: " + ex.ToString());
		}
		string text2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		string text3 = string.Empty;
		for (int i = 0; i < 128; i++)
		{
			int index = rnd.Next(text2.Length);
			text3 += text2[index];
		}
		return text3;
	}

	protected virtual IRESTClient CreateRestClient()
	{
		return new RESTClient();
	}

	protected virtual ISwrveStorage CreateStorage()
	{
		if (config.StoreDataInPlayerPrefs)
		{
			return new SwrvePlayerPrefsStorage();
		}
		return new SwrveFileStorage(swrvePath, GetUniqueKey());
	}

	private IEnumerator PostEvents_Coroutine(Dictionary<string, string> requestHeaders, byte[] eventsPostEncodedData)
	{
		yield return Container.StartCoroutine(restClient.Post(eventsUrl, eventsPostEncodedData, requestHeaders, delegate(RESTResponse response)
		{
			if (response.Error != WwwDeducedError.NetworkError)
			{
				ClearEventBuffer();
				eventsPostEncodedData = null;
			}
			eventsConnecting = false;
			TaskFinished("PostEvents_Coroutine");
		}));
	}

	protected virtual void ClearEventBuffer()
	{
		eventsPostString = null;
	}

	private void AppendEventToBuffer(string eventType, Dictionary<string, object> eventParameters, bool allowShowMessage = true)
	{
		eventParameters.Add("type", eventType);
		eventParameters.Add("seqnum", getNextSeqNum());
		eventParameters.Add("time", GetSessionTime());
		string text = Json.Serialize(eventParameters);
		string eventName = SwrveHelper.GetEventName(eventParameters);
		bool flag = eventBufferStringBuilder.Length + text.Length <= config.MaxBufferChars;
		if (flag || config.SendEventsIfBufferTooLarge)
		{
			if (!flag && config.SendEventsIfBufferTooLarge)
			{
				SendQueuedEvents();
			}
			if (eventBufferStringBuilder.Length > 0)
			{
				eventBufferStringBuilder.Append(',');
			}
			AppendEventToBuffer(text);
		}
		else
		{
			SwrveLog.LogError("Could not append the event to the buffer. Please consider enabling SendEventsIfBufferTooLarge");
		}
		if (allowShowMessage)
		{
			object value;
			eventParameters.TryGetValue("payload", out value);
			ShowBaseMessage(eventName, (IDictionary<string, string>)value);
		}
	}

	protected virtual void AppendEventToBuffer(string eventJson)
	{
		eventBufferStringBuilder.Append(eventJson);
	}

	protected virtual Coroutine StartTask(string tag, IEnumerator task)
	{
		return Container.StartCoroutine(task);
	}

	protected virtual void TaskFinished(string tag)
	{
	}

	protected void ShowBaseMessage(string eventName, IDictionary<string, string> payload)
	{
		SwrveBaseMessage baseMessage = GetBaseMessage(eventName, payload);
		if (null != baseMessage)
		{
			if (baseMessage.Campaign.IsA<SwrveConversationCampaign>())
			{
				StartTask("ShowConversationForEvent", ShowConversationForEvent(eventName, (SwrveConversation)baseMessage));
			}
			else
			{
				StartTask("ShowMessageForEvent", ShowMessageForEvent(eventName, (SwrveMessage)baseMessage, GlobalInstallButtonListener, GlobalCustomButtonListener, GlobalMessageListener));
			}
		}
		if (qaUser != null)
		{
			qaUser.Trigger(eventName, baseMessage);
		}
		if (baseMessage != null)
		{
			NamedEventInternal(baseMessage.GetEventPrefix() + "returned", new Dictionary<string, string>
			{
				{
					"id",
					baseMessage.Id.ToString()
				}
			}, false);
		}
	}

	public SwrveBaseMessage GetBaseMessage(string eventName, IDictionary<string, string> payload = null)
	{
		if (!checkCampaignRules(eventName, SwrveHelper.GetNow()))
		{
			return null;
		}
		SwrveBaseMessage swrveBaseMessage = null;
		if (config.ConversationsEnabled)
		{
			swrveBaseMessage = GetConversationForEvent(eventName, payload);
		}
		if (swrveBaseMessage == null && config.MessagingEnabled)
		{
			swrveBaseMessage = GetMessageForEvent(eventName, payload);
		}
		if (swrveBaseMessage == null)
		{
			SwrveLog.Log("Not showing message: no candidate for " + eventName);
		}
		else
		{
			SwrveLog.Log(string.Format("[{0}] {1} has been chosen for {2}\nstate: {3}", swrveBaseMessage, swrveBaseMessage.Campaign.Id, eventName, swrveBaseMessage.Campaign.State));
		}
		return swrveBaseMessage;
	}

	private bool IsAlive()
	{
		return Container != null && !Destroyed;
	}

	protected virtual void GetDeviceScreenInfo()
	{
		deviceWidth = Screen.width;
		deviceHeight = Screen.height;
		if (deviceWidth > deviceHeight)
		{
			int num = deviceWidth;
			deviceWidth = deviceHeight;
			deviceHeight = num;
		}
	}

	private void QueueDeviceInfo()
	{
		Dictionary<string, string> deviceInfo = GetDeviceInfo();
		UserUpdate(deviceInfo);
	}

	private void SendDeviceInfo()
	{
		QueueDeviceInfo();
		SendQueuedEvents();
	}

	private IEnumerator WaitAndRefreshResourcesAndCampaigns_Coroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		RefreshUserResourcesAndCampaigns();
	}

	private void CheckForCampaignsAndResourcesUpdates(bool invokedByTimer)
	{
		if (IsAlive())
		{
			if (SendQueuedEvents())
			{
				Container.StartCoroutine(WaitAndRefreshResourcesAndCampaigns_Coroutine(campaignsAndResourcesFlushRefreshDelay));
			}
			if (!invokedByTimer)
			{
				StopCheckForCampaignAndResources();
				StartCheckForCampaignsAndResources();
			}
		}
	}

	private void StartCheckForCampaignsAndResources()
	{
		if (campaignAndResourcesCoroutineInstance == null)
		{
			campaignAndResourcesCoroutineInstance = CheckForCampaignsAndResourcesUpdates_Coroutine();
			Container.StartCoroutine(campaignAndResourcesCoroutineInstance);
		}
		campaignAndResourcesCoroutineEnabled = true;
	}

	private void StopCheckForCampaignAndResources()
	{
		if (campaignAndResourcesCoroutineInstance != null)
		{
			Container.StopCoroutine("campaignAndResourcesCoroutineInstance");
			campaignAndResourcesCoroutineInstance = null;
		}
		campaignAndResourcesCoroutineEnabled = false;
	}

	private IEnumerator CheckForCampaignsAndResourcesUpdates_Coroutine()
	{
		yield return new WaitForSeconds(campaignsAndResourcesFlushFrequency);
		CheckForCampaignsAndResourcesUpdates(true);
		if (campaignAndResourcesCoroutineEnabled)
		{
			campaignAndResourcesCoroutineInstance = null;
			StartCheckForCampaignsAndResources();
		}
	}

	protected virtual long GetSessionTime()
	{
		return SwrveHelper.GetMilliseconds();
	}

	private void GenerateNewSessionInterval()
	{
		lastSessionTick = GetSessionTime() + config.NewSessionInterval * 1000;
	}

	public void Update()
	{
		if (currentDisplayingMessage == null)
		{
			return;
		}
		if (!currentMessage.Closing)
		{
			if (inputManager.GetMouseButtonDown(0))
			{
				ProcessButtonDown();
			}
			else if (inputManager.GetMouseButtonUp(0))
			{
				ProcessButtonUp();
			}
		}
		if (!currentMessage.Closing && NativeIsBackPressed())
		{
			currentMessage.Dismiss();
		}
	}

	public void OnGUI()
	{
		if (currentDisplayingMessage == null)
		{
			return;
		}
		SwrveOrientation deviceOrientation = GetDeviceOrientation();
		if (deviceOrientation != currentOrientation)
		{
			if (currentDisplayingMessage.Orientation != deviceOrientation)
			{
				if (currentDisplayingMessage.Message.SupportsOrientation(deviceOrientation))
				{
					StartTask("SwitchMessageOrienation", SwitchMessageOrienation(deviceOrientation));
				}
				else
				{
					currentDisplayingMessage.Rotate = true;
				}
			}
			else
			{
				currentDisplayingMessage.Rotate = false;
			}
		}
		int depth = GUI.depth;
		Matrix4x4 matrix = GUI.matrix;
		GUI.depth = 0;
		SwrveMessageRenderer.DrawMessage(currentMessage, Screen.width / 2 + currentMessage.Message.Position.X, Screen.height / 2 + currentMessage.Message.Position.Y);
		GUI.matrix = matrix;
		GUI.depth = depth;
		if (currentDisplayingMessage.MessageListener != null)
		{
			currentDisplayingMessage.MessageListener.OnShowing(currentDisplayingMessage);
		}
		if (currentMessage.Dismissed)
		{
			currentMessage = null;
			currentDisplayingMessage = null;
		}
		currentOrientation = deviceOrientation;
	}

	private IEnumerator SwitchMessageOrienation(SwrveOrientation newOrientation)
	{
		SwrveMessageFormat newFormat = currentMessage.Message.GetFormat(newOrientation);
		if (newFormat != null && newFormat != currentMessage)
		{
			SwrveMessageFormat oldFormat = currentMessage;
			CoroutineReference<bool> wereAllLoaded = new CoroutineReference<bool>(false);
			yield return StartTask("PreloadFormatAssets", PreloadFormatAssets(newFormat, wereAllLoaded));
			if (wereAllLoaded.Value())
			{
				currentOrientation = GetDeviceOrientation();
				newFormat.Init(currentOrientation);
				newFormat.MessageListener = oldFormat.MessageListener;
				newFormat.CustomButtonListener = oldFormat.CustomButtonListener;
				newFormat.InstallButtonListener = oldFormat.InstallButtonListener;
				currentMessage = (currentDisplayingMessage = newFormat);
				oldFormat.UnloadAssets();
			}
			else
			{
				SwrveLog.LogError("Could not switch orientation. Not all assets could be preloaded");
			}
			TaskFinished("SwitchMessageOrienation");
		}
	}

	private void ProcessButtonDown()
	{
		Vector3 mousePosition = inputManager.GetMousePosition();
		for (int i = 0; i < currentMessage.Buttons.Count; i++)
		{
			SwrveButton swrveButton = currentMessage.Buttons[i];
			if (swrveButton.PointerRect.Contains(mousePosition))
			{
				swrveButton.Pressed = true;
			}
		}
	}

	private void ProcessButtonUp()
	{
		SwrveButton swrveButton = null;
		int num = currentMessage.Buttons.Count - 1;
		while (num >= 0 && swrveButton == null)
		{
			SwrveButton swrveButton2 = currentMessage.Buttons[num];
			Vector3 mousePosition = inputManager.GetMousePosition();
			if (swrveButton2.PointerRect.Contains(mousePosition) && swrveButton2.Pressed)
			{
				swrveButton = swrveButton2;
			}
			else
			{
				swrveButton2.Pressed = false;
			}
			num--;
		}
		if (swrveButton != null)
		{
			SwrveLog.Log("Clicked button " + swrveButton.ActionType);
			ButtonWasPressedByUser(swrveButton);
			try
			{
				if (swrveButton.ActionType == SwrveActionType.Install)
				{
					string text = swrveButton.AppId.ToString();
					if (appStoreLinks.ContainsKey(text))
					{
						string text2 = appStoreLinks[text];
						if (!string.IsNullOrEmpty(text2))
						{
							bool flag = true;
							if (currentMessage.InstallButtonListener != null)
							{
								flag = currentMessage.InstallButtonListener.OnAction(text2);
							}
							if (flag)
							{
								OpenURL(text2);
							}
						}
						else
						{
							SwrveLog.LogError("No app store url for app " + text);
						}
					}
					else
					{
						SwrveLog.LogError("Install button app store url empty!");
					}
				}
				else if (swrveButton.ActionType == SwrveActionType.Custom)
				{
					string action = swrveButton.Action;
					if (currentMessage.CustomButtonListener != null)
					{
						currentMessage.CustomButtonListener.OnAction(action);
					}
					else
					{
						SwrveLog.Log("No custom button listener, treating action as URL");
						if (!string.IsNullOrEmpty(action))
						{
							OpenURL(action);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("Error processing the clicked button: " + ex.Message);
			}
			swrveButton.Pressed = false;
			DismissMessage();
		}
	}

	protected virtual void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	protected void SetMessageMinDelayThrottle()
	{
		showMessagesAfterDelay = SwrveHelper.GetNow() + TimeSpan.FromSeconds(minDelayBetweenMessage);
	}

	private void AutoShowMessages()
	{
		if (!autoShowMessagesEnabled || !campaignsAndResourcesInitialized || campaigns == null || campaigns.Count == 0)
		{
			return;
		}
		SwrveBaseMessage swrveBaseMessage = null;
		for (int i = 0; i < campaigns.Count; i++)
		{
			if (!campaigns[i].IsA<SwrveConversationCampaign>())
			{
				continue;
			}
			SwrveConversationCampaign swrveConversationCampaign = (SwrveConversationCampaign)campaigns[i];
			if (swrveConversationCampaign.CanTrigger("Swrve.Messages.showAtSessionStart") && swrveConversationCampaign.CheckImpressions(qaUser))
			{
				if (swrveConversationCampaign.AreAssetsReady())
				{
					Container.StartCoroutine(LaunchConversation(swrveConversationCampaign.Conversation));
					swrveBaseMessage = swrveConversationCampaign.Conversation;
					break;
				}
				if (qaUser != null)
				{
					int id = swrveConversationCampaign.Id;
					qaUser.campaignMessages[id] = swrveConversationCampaign.Conversation;
					qaUser.campaignReasons[id] = "Campaign " + id + " was selected to autoshow, but assets aren't fully downloaded";
				}
			}
		}
		if (swrveBaseMessage == null)
		{
			for (int i = 0; i < campaigns.Count; i++)
			{
				if (!campaigns[i].IsA<SwrveMessagesCampaign>())
				{
					continue;
				}
				SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)campaigns[i];
				if (!swrveMessagesCampaign.CanTrigger("Swrve.Messages.showAtSessionStart") || !swrveMessagesCampaign.CheckImpressions(qaUser))
				{
					continue;
				}
				if (TriggeredMessageListener != null)
				{
					SwrveMessage messageForEvent = GetMessageForEvent("Swrve.Messages.showAtSessionStart");
					if (messageForEvent != null)
					{
						autoShowMessagesEnabled = false;
						TriggeredMessageListener.OnMessageTriggered(messageForEvent);
						swrveBaseMessage = messageForEvent;
					}
				}
				else if (currentMessage == null)
				{
					SwrveMessage messageForEvent = GetMessageForEvent("Swrve.Messages.showAtSessionStart");
					if (messageForEvent != null)
					{
						autoShowMessagesEnabled = false;
						Container.StartCoroutine(LaunchMessage(messageForEvent, GlobalInstallButtonListener, GlobalCustomButtonListener, GlobalMessageListener));
						swrveBaseMessage = messageForEvent;
					}
				}
				break;
			}
		}
		if (qaUser != null)
		{
			qaUser.Trigger("Swrve.Messages.showAtSessionStart", swrveBaseMessage);
		}
	}

	private IEnumerator LaunchMessage(SwrveMessage message, ISwrveInstallButtonListener installButtonListener, ISwrveCustomButtonListener customButtonListener, ISwrveMessageListener messageListener)
	{
		if (message == null)
		{
			yield break;
		}
		SwrveOrientation currentOrientation = GetDeviceOrientation();
		SwrveMessageFormat selectedFormat = message.GetFormat(currentOrientation);
		if (selectedFormat != null)
		{
			currentMessage = selectedFormat;
			CoroutineReference<bool> wereAllLoaded = new CoroutineReference<bool>(false);
			yield return StartTask("PreloadFormatAssets", PreloadFormatAssets(selectedFormat, wereAllLoaded));
			if (wereAllLoaded.Value())
			{
				ShowMessageFormat(selectedFormat, installButtonListener, customButtonListener, messageListener);
				yield break;
			}
			SwrveLog.LogError("Could not preload all the assets for message " + message.Id);
			currentMessage = null;
		}
		else
		{
			SwrveLog.LogError("Could not get a format for the current orientation: " + currentOrientation);
		}
	}

	private bool isValidMessageCenter(SwrveBaseCampaign campaign, SwrveOrientation orientation)
	{
		return campaign.MessageCenter && campaign.Status != SwrveCampaignState.Status.Deleted && campaign.IsActive(qaUser) && campaign.SupportsOrientation(orientation) && campaign.AreAssetsReady();
	}

	private IEnumerator LaunchConversation(SwrveConversation conversation)
	{
		if (null != conversation)
		{
			yield return null;
			ShowConversation(conversation.Conversation);
			ConversationWasShownToUser(conversation);
		}
	}

	public void ConversationWasShownToUser(SwrveConversation conversation)
	{
		SetMessageMinDelayThrottle();
		if (null != conversation.Campaign)
		{
			conversation.Campaign.WasShownToUser();
			SaveCampaignData(conversation.Campaign);
		}
	}

	private void NoMessagesWereShown(string eventName, string reason)
	{
		SwrveLog.Log("Not showing message for " + eventName + ": " + reason);
		if (qaUser != null)
		{
			qaUser.TriggerFailure(eventName, reason);
		}
	}

	private IEnumerator PreloadFormatAssets(SwrveMessageFormat format, CoroutineReference<bool> wereAllLoaded)
	{
		SwrveLog.Log("Preloading format");
		bool allLoaded = true;
		for (int ii = 0; ii < format.Images.Count; ii++)
		{
			SwrveImage image = format.Images[ii];
			if (image.Texture == null && !string.IsNullOrEmpty(image.File))
			{
				SwrveLog.Log("Preloading image file " + image.File);
				CoroutineReference<Texture2D> result = new CoroutineReference<Texture2D>();
				yield return StartTask("LoadAsset", LoadAsset(image.File, result));
				if (result.Value() != null)
				{
					image.Texture = result.Value();
				}
				else
				{
					allLoaded = false;
				}
			}
		}
		for (int bi = 0; bi < format.Buttons.Count; bi++)
		{
			SwrveButton button = format.Buttons[bi];
			if (button.Texture == null && !string.IsNullOrEmpty(button.Image))
			{
				SwrveLog.Log("Preloading button image " + button.Image);
				CoroutineReference<Texture2D> result2 = new CoroutineReference<Texture2D>();
				yield return StartTask("LoadAsset", LoadAsset(button.Image, result2));
				if (result2.Value() != null)
				{
					button.Texture = result2.Value();
				}
				else
				{
					allLoaded = false;
				}
			}
		}
		wereAllLoaded.Value(allLoaded);
		TaskFinished("PreloadFormatAssets");
	}

	private bool HasShowTooManyMessagesAlready()
	{
		return messagesLeftToShow <= 0;
	}

	private bool IsTooSoonToShowMessageAfterLaunch(DateTime now)
	{
		return now < showMessagesAfterLaunch;
	}

	private bool IsTooSoonToShowMessageAfterDelay(DateTime now)
	{
		return now < showMessagesAfterDelay;
	}

	private SwrveMessageFormat ShowMessageFormat(SwrveMessageFormat format, ISwrveInstallButtonListener installButtonListener, ISwrveCustomButtonListener customButtonListener, ISwrveMessageListener messageListener)
	{
		currentMessage = format;
		format.MessageListener = messageListener;
		format.CustomButtonListener = customButtonListener;
		format.InstallButtonListener = installButtonListener;
		currentDisplayingMessage = currentMessage;
		currentOrientation = GetDeviceOrientation();
		SwrveMessageRenderer.InitMessage(currentDisplayingMessage, currentOrientation);
		if (messageListener != null)
		{
			messageListener.OnShow(format);
		}
		MessageWasShownToUser(currentDisplayingMessage);
		return format;
	}

	private string GetTemporaryPathFileName(string fileName)
	{
		return Path.Combine(swrveTemporaryPath, fileName);
	}

	private IEnumerator LoadAsset(string fileName, CoroutineReference<Texture2D> texture)
	{
		string filePath = GetTemporaryPathFileName(fileName);
		WWW www = new WWW("file://" + filePath);
		yield return www;
		if (www != null && www.error == null)
		{
			Texture2D texture2 = www.texture;
			texture.Value(texture2);
		}
		else
		{
			SwrveLog.LogError("Could not load asset with WWW " + filePath + ": " + www.error);
			if (CrossPlatformFile.Exists(filePath))
			{
				byte[] data = CrossPlatformFile.ReadAllBytes(filePath);
				Texture2D texture2 = new Texture2D(4, 4);
				if (texture2.LoadImage(data))
				{
					texture.Value(texture2);
				}
				else
				{
					SwrveLog.LogWarning("Could not load asset from I/O" + filePath);
				}
			}
			else
			{
				SwrveLog.LogError("The file " + filePath + " does not exist.");
			}
		}
		TaskFinished("LoadAsset");
	}

	protected virtual void ProcessCampaigns(Dictionary<string, object> root)
	{
		List<SwrveBaseCampaign> list = new List<SwrveBaseCampaign>();
		HashSet<SwrveAssetsQueueItem> hashSet = new HashSet<SwrveAssetsQueueItem>();
		try
		{
			if (root != null && root.ContainsKey("version"))
			{
				int @int = MiniJsonHelper.GetInt(root, "version");
				if (@int == CampaignResponseVersion)
				{
					UpdateCdnPaths(root);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)root["game_data"];
					Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string key = enumerator.Current.Key;
						if (appStoreLinks.ContainsKey(key))
						{
							appStoreLinks.Remove(key);
						}
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary[key];
						if (dictionary2 != null && dictionary2.ContainsKey("app_store_url"))
						{
							object obj = dictionary2["app_store_url"];
							if (obj != null && obj is string)
							{
								appStoreLinks.Add(key, (string)obj);
							}
						}
					}
					Dictionary<string, object> dictionary3 = (Dictionary<string, object>)root["rules"];
					int num = dictionary3.ContainsKey("delay_first_message") ? MiniJsonHelper.GetInt(dictionary3, "delay_first_message") : 150;
					long num2 = dictionary3.ContainsKey("max_messages_per_session") ? MiniJsonHelper.GetLong(dictionary3, "max_messages_per_session") : 99999;
					int num3 = dictionary3.ContainsKey("min_delay_between_messages") ? MiniJsonHelper.GetInt(dictionary3, "min_delay_between_messages") : 55;
					DateTime now = SwrveHelper.GetNow();
					minDelayBetweenMessage = num3;
					messagesLeftToShow = num2;
					showMessagesAfterLaunch = initialisedTime + TimeSpan.FromSeconds(num);
					SwrveLog.Log("App rules OK: Delay Seconds: " + num + " Max shows: " + num2);
					SwrveLog.Log("Time is " + now.ToString() + " show messages after " + showMessagesAfterLaunch.ToString());
					Dictionary<int, string> dictionary4 = null;
					bool flag = qaUser != null;
					int i;
					if (root.ContainsKey("qa"))
					{
						Dictionary<string, object> dictionary5 = (Dictionary<string, object>)root["qa"];
						SwrveLog.Log("You are a QA user!");
						dictionary4 = new Dictionary<int, string>();
						qaUser = new SwrveQAUser(this, dictionary5);
						if (dictionary5.ContainsKey("campaigns"))
						{
							IList<object> list2 = (List<object>)dictionary5["campaigns"];
							for (i = 0; i < list2.Count; i++)
							{
								Dictionary<string, object> dictionary6 = (Dictionary<string, object>)list2[i];
								int int2 = MiniJsonHelper.GetInt(dictionary6, "id");
								string text = (string)dictionary6["reason"];
								SwrveLog.Log("Campaign " + int2 + " not downloaded because: " + text);
								dictionary4.Add(int2, text);
							}
						}
					}
					else
					{
						qaUser = null;
					}
					IList<object> list3 = (List<object>)root["campaigns"];
					i = 0;
					for (int count = list3.Count; i < count; i++)
					{
						Dictionary<string, object> campaignData = (Dictionary<string, object>)list3[i];
						SwrveBaseCampaign swrveBaseCampaign = SwrveBaseCampaign.LoadFromJSON(SwrveAssetsManager, campaignData, initialisedTime, qaUser, config.DefaultBackgroundColor);
						if (swrveBaseCampaign != null)
						{
							if (swrveBaseCampaign.GetType() == typeof(SwrveConversationCampaign))
							{
								SwrveConversationCampaign swrveConversationCampaign = (SwrveConversationCampaign)swrveBaseCampaign;
								hashSet.UnionWith(swrveConversationCampaign.Conversation.ConversationAssets);
							}
							else if (swrveBaseCampaign.GetType() == typeof(SwrveMessagesCampaign))
							{
								SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)swrveBaseCampaign;
								hashSet.UnionWith(swrveMessagesCampaign.GetImageAssets());
							}
							if (campaignSettings != null && (flag || qaUser == null || !qaUser.ResetDevice))
							{
								SwrveCampaignState value = null;
								campaignsState.TryGetValue(swrveBaseCampaign.Id, out value);
								if (value != null)
								{
									swrveBaseCampaign.State = value;
								}
								else
								{
									swrveBaseCampaign.State = new SwrveCampaignState(swrveBaseCampaign.Id, campaignSettings);
								}
							}
							campaignsState[swrveBaseCampaign.Id] = swrveBaseCampaign.State;
							list.Add(swrveBaseCampaign);
							if (qaUser != null)
							{
								dictionary4.Add(swrveBaseCampaign.Id, null);
							}
						}
					}
					if (qaUser != null)
					{
						qaUser.TalkSession(dictionary4);
					}
				}
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogError("Could not process campaigns: " + ex.ToString());
		}
		StartTask("SwrveAssetsManager.DownloadAssets", SwrveAssetsManager.DownloadAssets(hashSet, AutoShowMessages));
		campaigns = new List<SwrveBaseCampaign>(list);
	}

	private void UpdateCdnPaths(Dictionary<string, object> root)
	{
		if (root.ContainsKey("cdn_root"))
		{
			string text = (string)root["cdn_root"];
			SwrveAssetsManager.CdnImages = text;
			SwrveLog.Log("CDN URL " + text);
		}
		else if (root.ContainsKey("cdn_paths"))
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)root["cdn_paths"];
			string text2 = (string)dictionary["message_images"];
			string text3 = (string)dictionary["message_fonts"];
			SwrveAssetsManager.CdnImages = text2;
			SwrveAssetsManager.CdnFonts = text3;
			SwrveLog.Log("CDN URL images:" + text2 + " fonts:" + text3);
		}
	}

	private void LoadResourcesAndCampaigns()
	{
		if (IsAlive())
		{
			try
			{
				if (!campaignsConnecting)
				{
					if (config.AutoDownloadCampaignsAndResources)
					{
						goto IL_008d;
					}
					if (campaignsAndResourcesLastRefreshed == 0)
					{
						goto IL_0072;
					}
					long sessionTime = GetSessionTime();
					if (sessionTime >= campaignsAndResourcesLastRefreshed)
					{
						goto IL_0072;
					}
					SwrveLog.Log("Request to retrieve campaign and user resource data was rate-limited.");
				}
				goto end_IL_0013;
				IL_0072:
				campaignsAndResourcesLastRefreshed = GetSessionTime() + (long)(campaignsAndResourcesFlushFrequency * 1000f);
				goto IL_008d;
				IL_008d:
				campaignsConnecting = true;
				float num = (Screen.dpi == 0f) ? 160f : Screen.dpi;
				string deviceModel = GetDeviceModel();
				string operatingSystem = SystemInfo.operatingSystem;
				StringBuilder stringBuilder = new StringBuilder(resourcesAndCampaignsUrl).AppendFormat("?user={0}&api_key={1}&app_version={2}&joined={3}", escapedUserId, ApiKey, WWW.EscapeURL(GetAppVersion()), installTimeEpoch);
				if (config.MessagingEnabled)
				{
					stringBuilder.AppendFormat("&version={0}&orientation={1}&language={2}&app_store={3}&device_width={4}&device_height={5}&device_dpi={6}&os_version={7}&device_name={8}", CampaignEndpointVersion, config.Orientation.ToString().ToLower(), Language, config.AppStore, deviceWidth, deviceHeight, num, WWW.EscapeURL(operatingSystem), WWW.EscapeURL(deviceModel));
				}
				if (config.ConversationsEnabled)
				{
					stringBuilder.AppendFormat("&conversation_version={0}", conversationVersion);
				}
				if (config.LocationEnabled)
				{
					stringBuilder.AppendFormat("&location_version={0}", locationSegmentVersion);
				}
				if (config.ABTestDetailsEnabled)
				{
					stringBuilder.AppendFormat("&ab_test_details=1");
				}
				if (!string.IsNullOrEmpty(lastETag))
				{
					stringBuilder.AppendFormat("&etag={0}", lastETag);
				}
				StartTask("GetCampaignsAndResources_Coroutine", GetCampaignsAndResources_Coroutine(stringBuilder.ToString()));
				end_IL_0013:;
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while trying to get user resources and campaign data: " + arg);
			}
		}
	}

	private string GetDeviceModel()
	{
		string text = SystemInfo.deviceModel;
		if (string.IsNullOrEmpty(text))
		{
			text = "ModelUnknown";
		}
		return text;
	}

	private IEnumerator GetCampaignsAndResources_Coroutine(string getRequest)
	{
		SwrveLog.Log("Campaigns and resources request: " + getRequest);
		yield return Container.StartCoroutine(restClient.Get(getRequest, delegate(RESTResponse response)
		{
			if (response.Error == WwwDeducedError.NoError)
			{
				string value = null;
				if (response.Headers != null)
				{
					response.Headers.TryGetValue("ETAG", out value);
					if (!string.IsNullOrEmpty(value))
					{
						lastETag = value;
						storage.Save("cmpg_etag", value, userId);
					}
				}
				if (!string.IsNullOrEmpty(response.Body))
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(response.Body);
					if (dictionary != null)
					{
						if (dictionary.ContainsKey("flush_frequency"))
						{
							string @string = MiniJsonHelper.GetString(dictionary, "flush_frequency");
							if (!string.IsNullOrEmpty(@string) && float.TryParse(@string, out campaignsAndResourcesFlushFrequency))
							{
								campaignsAndResourcesFlushFrequency /= 1000f;
								storage.Save("swrve_cr_flush_frequency", @string, userId);
							}
						}
						if (dictionary.ContainsKey("flush_refresh_delay"))
						{
							string string2 = MiniJsonHelper.GetString(dictionary, "flush_refresh_delay");
							if (!string.IsNullOrEmpty(string2) && float.TryParse(string2, out campaignsAndResourcesFlushRefreshDelay))
							{
								campaignsAndResourcesFlushRefreshDelay /= 1000f;
								storage.Save("swrve_cr_flush_delay", string2, userId);
							}
						}
						if (dictionary.ContainsKey("campaigns"))
						{
							Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["campaigns"];
							if (config.MessagingEnabled)
							{
								string cacheContent = Json.Serialize(dictionary2);
								SaveCampaignsCache(cacheContent);
								AutoShowMessages();
								ProcessCampaigns(dictionary2);
								StringBuilder stringBuilder = new StringBuilder();
								int i = 0;
								for (int count = campaigns.Count; i < count; i++)
								{
									SwrveBaseCampaign swrveBaseCampaign = campaigns[i];
									if (i != 0)
									{
										stringBuilder.Append(',');
									}
									stringBuilder.Append(swrveBaseCampaign.Id);
								}
								NamedEventInternal("Swrve.Messages.campaigns_downloaded", new Dictionary<string, string>
								{
									{
										"ids",
										stringBuilder.ToString()
									},
									{
										"count",
										(campaigns == null) ? "0" : campaigns.Count.ToString()
									}
								}, false);
							}
							if (config.ABTestDetailsEnabled && dictionary2.ContainsKey("ab_test_details"))
							{
								Dictionary<string, object> aBTestDetailsFromJSON = (Dictionary<string, object>)dictionary2["ab_test_details"];
								ResourceManager.SetABTestDetailsFromJSON(aBTestDetailsFromJSON);
							}
						}
						if (dictionary.ContainsKey("user_resources"))
						{
							IList<object> obj = (IList<object>)dictionary["user_resources"];
							string data = Json.Serialize(obj);
							storage.SaveSecure("srcngt2", data, userId);
							userResources = ProcessUserResources(obj);
							userResourcesRaw = data;
							if (campaignsAndResourcesInitialized)
							{
								NotifyUpdateUserResources();
							}
						}
						if (config.LocationEnabled && dictionary.ContainsKey("location_campaigns"))
						{
							Dictionary<string, object> obj2 = (Dictionary<string, object>)dictionary["location_campaigns"];
							string cacheContent2 = Json.Serialize(obj2);
							SaveLocationCache(cacheContent2);
						}
					}
				}
			}
			else
			{
				SwrveLog.LogError("Resources and campaigns request error: " + response.Error.ToString() + ":" + response.Body);
			}
			if (!campaignsAndResourcesInitialized)
			{
				campaignsAndResourcesInitialized = true;
				AutoShowMessages();
				NotifyUpdateUserResources();
			}
			campaignsConnecting = false;
			TaskFinished("GetCampaignsAndResources_Coroutine");
		}));
	}

	private void SaveCampaignsCache(string cacheContent)
	{
		try
		{
			if (cacheContent == null)
			{
				cacheContent = string.Empty;
			}
			storage.SaveSecure(CampaignsSave, cacheContent, userId);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while saving campaigns to the cache " + arg);
		}
	}

	private void SaveLocationCache(string cacheContent)
	{
		try
		{
			if (cacheContent == null)
			{
				cacheContent = string.Empty;
			}
			storage.SaveSecure(LocationSave, cacheContent, userId);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while saving campaigns to the cache " + arg);
		}
	}

	private void SaveCampaignData(SwrveBaseCampaign campaign)
	{
		try
		{
			campaignSettings["Next" + campaign.Id] = campaign.Next;
			campaignSettings["Impressions" + campaign.Id] = campaign.Impressions;
			campaignSettings["Status" + campaign.Id] = campaign.Status.ToString();
			string data = Json.Serialize(campaignSettings);
			storage.Save(CampaignsSettingsSave, data, userId);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while trying to save campaign settings " + arg);
		}
	}

	private void LoadTalkData()
	{
		try
		{
			string text = storage.Load(CampaignsSettingsSave, userId);
			string decodedString;
			if (text != null && text.Length != 0 && ResponseBodyTester.TestUTF8(text, out decodedString))
			{
				campaignSettings = (Dictionary<string, object>)Json.Deserialize(decodedString);
			}
		}
		catch (Exception)
		{
		}
		try
		{
			string text = storage.LoadSecure(CampaignsSave, userId);
			if (!string.IsNullOrEmpty(text))
			{
				string decodedString2 = null;
				if (ResponseBodyTester.TestUTF8(text, out decodedString2))
				{
					Dictionary<string, object> root = (Dictionary<string, object>)Json.Deserialize(decodedString2);
					ProcessCampaigns(root);
				}
				else
				{
					SwrveLog.Log("Failed to parse campaigns cache");
					InvalidateETag();
				}
			}
			else
			{
				InvalidateETag();
			}
		}
		catch (Exception ex2)
		{
			SwrveLog.LogWarning("Could not read campaigns from cache, using default (" + ex2.ToString() + ")");
			InvalidateETag();
		}
	}

	private void LoadABTestDetails()
	{
		try
		{
			string text = storage.LoadSecure(CampaignsSave, userId);
			if (!string.IsNullOrEmpty(text))
			{
				string decodedString = null;
				if (ResponseBodyTester.TestUTF8(text, out decodedString))
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(decodedString);
					if (dictionary.ContainsKey("ab_test_details"))
					{
						Dictionary<string, object> aBTestDetailsFromJSON = (Dictionary<string, object>)dictionary["ab_test_details"];
						ResourceManager.SetABTestDetailsFromJSON(aBTestDetailsFromJSON);
					}
				}
				else
				{
					SwrveLog.Log("Failed to parse AB test details cache");
				}
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Could not read ABTest details from cache, using default (" + ex.ToString() + ")");
		}
	}

	public void SendPushEngagedEvent(string pushId)
	{
		if ("0" == pushId || pushId != lastPushEngagedId)
		{
			lastPushEngagedId = pushId;
			NamedEventInternal("Swrve.Messages.Push-" + pushId + ".engaged", null, false);
			SwrveLog.Log("Got Swrve notification with ID " + pushId);
			Container.StartCoroutine(WaitASecondAndSendEvents_Coroutine());
		}
	}

	private IEnumerator WaitASecondAndSendEvents_Coroutine()
	{
		yield return new WaitForSeconds(1f);
		SendQueuedEvents();
	}

	protected int ConvertInt64ToInt32Hack(long val)
	{
		return (int)(val & uint.MaxValue);
	}

	protected virtual ICarrierInfo GetCarrierInfoProvider()
	{
		return deviceCarrierInfo;
	}

	public string GetAppVersion()
	{
		if (string.IsNullOrEmpty(config.AppVersion))
		{
			setNativeAppVersion();
		}
		return config.AppVersion;
	}

	private void ShowConversation(string conversation)
	{
		showNativeConversation(conversation);
	}

	private void SetInputManager(IInputManager inputManager)
	{
		this.inputManager = inputManager;
	}

	protected void StartCampaignsAndResourcesTimer()
	{
		if (config.AutoDownloadCampaignsAndResources)
		{
			RefreshUserResourcesAndCampaigns();
			StartCheckForCampaignsAndResources();
			Container.StartCoroutine(WaitAndRefreshResourcesAndCampaigns_Coroutine(campaignsAndResourcesFlushRefreshDelay));
		}
	}

	protected void DisableAutoShowAfterDelay()
	{
		Container.StartCoroutine(DisableAutoShowAfterDelay_Coroutine());
	}

	private IEnumerator DisableAutoShowAfterDelay_Coroutine()
	{
		yield return new WaitForSeconds(config.AutoShowMessagesMaxDelay);
		autoShowMessagesEnabled = false;
	}

	private string GetNativeDetails()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("sdkVersion", "5.1.1");
		dictionary.Add("apiKey", apiKey);
		dictionary.Add("appId", appId);
		dictionary.Add("userId", userId);
		dictionary.Add("deviceId", GetDeviceId());
		dictionary.Add("appVersion", GetAppVersion());
		dictionary.Add("uniqueKey", GetUniqueKey());
		dictionary.Add("deviceInfo", GetDeviceInfo());
		dictionary.Add("batchUrl", "/1/batch");
		dictionary.Add("eventsServer", config.EventsServer);
		dictionary.Add("contentServer", config.ContentServer);
		dictionary.Add("locationCampaignCategory", "LocationCampaign");
		dictionary.Add("httpTimeout", 60000);
		dictionary.Add("maxEventsPerFlush", 50);
		dictionary.Add("locTag", LocationSave);
		dictionary.Add("swrvePath", swrvePath);
		dictionary.Add("prefabName", prefabName);
		dictionary.Add("swrveTemporaryPath", swrveTemporaryPath);
		dictionary.Add("sigSuffix", "_SGT");
		Dictionary<string, object> obj = dictionary;
		return Json.Serialize(obj);
	}

	private void InitNative()
	{
		initNative();
		setNativeConversationVersion();
		if (config.LocationAutostart)
		{
			startLocation();
		}
	}

	protected void startLocation()
	{
		if (config.LocationEnabled)
		{
			startNativeLocation();
		}
	}

	private void ProcessInfluenceData()
	{
		string influencedDataJsonPerPlatform = GetInfluencedDataJsonPerPlatform();
		if (influencedDataJsonPerPlatform == null)
		{
			return;
		}
		List<object> list = (List<object>)Json.Deserialize(influencedDataJsonPerPlatform);
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				CheckInfluenceData((Dictionary<string, object>)list[i]);
			}
		}
		else
		{
			SwrveLog.LogError("Could not parse influence data");
		}
	}

	protected virtual string GetInfluencedDataJsonPerPlatform()
	{
		return null;
	}

	public void CheckInfluenceData(Dictionary<string, object> influenceData)
	{
		if (influenceData == null)
		{
			return;
		}
		object obj = influenceData["trackingId"];
		object obj2 = influenceData["maxInfluencedMillis"];
		long num = 0L;
		if (obj2 != null && (obj2 is long || obj2 is int || obj2 is long))
		{
			num = (long)obj2;
		}
		if (obj != null && obj is string && num > 0)
		{
			string text = (string)obj;
			long milliseconds = SwrveHelper.GetMilliseconds();
			if (milliseconds <= num)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("id", text);
				dictionary.Add("campaignType", "push");
				dictionary.Add("actionType", "influenced");
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("delta", ((num - milliseconds) / 6000).ToString());
				dictionary.Add("payload", dictionary2);
				AppendEventToBuffer("generic_campaign_event", dictionary, false);
				SwrveLog.Log("User was influenced by push " + text);
				Container.StartCoroutine(WaitASecondAndSendEvents_Coroutine());
			}
		}
	}
}
