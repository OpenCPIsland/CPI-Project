using ClubPenguin.Net.Client.Converters;
using ClubPenguin.Net.Utils;
using DeviceDB;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Async.Unity;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.providers;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public class ClubPenguinClient
	{
		private string accessToken;

		public ICPKeyValueDatabase CPKeyValueDatabase;

		public bool OfflineMode;

		private RSAParameters? rsaParameters;

		public SynchronizationContext SyncContext
		{
			get;
			private set;
		}

		public IGameServerClient GameServer
		{
			get;
			private set;
		}

		public BreadcrumbApi BreadcrumbApi
		{
			get;
			private set;
		}

		public ChatApi ChatApi
		{
			get;
			private set;
		}

		public ConsumableApi ConsumableApi
		{
			get;
			private set;
		}

		public DurableApi DurableApi
		{
			get;
			private set;
		}

		public EncryptionApi EncryptionApi
		{
			get;
			private set;
		}

		public GameApi GameApi
		{
			get;
			private set;
		}

		public IglooApi IglooApi
		{
			get;
			private set;
		}

		public InventoryApi InventoryApi
		{
			get;
			private set;
		}

		public MinigameApi MinigameApi
		{
			get;
			private set;
		}

		public PlayerApi PlayerApi
		{
			get;
			private set;
		}

		public QuestApi QuestApi
		{
			get;
			private set;
		}

		public SavedOutfitApi SavedOutfitApi
		{
			get;
			private set;
		}

		public RewardApi RewardApi
		{
			get;
			private set;
		}

		public TaskApi TaskApi
		{
			get;
			private set;
		}

		public IAPApi IAPApi
		{
			get;
			private set;
		}

		public TutorialApi TutorialApi
		{
			get;
			private set;
		}

		public ModerationApi ModerationApi
		{
			get;
			private set;
		}

		public TubeApi TubeApi
		{
			get;
			private set;
		}

		public DisneyStoreApi DisneyStoreApi
		{
			get;
			private set;
		}

		public NewsfeedApi NewsfeedApi
		{
			get;
			private set;
		}

		public CatalogApi CatalogApi
		{
			get;
			private set;
		}

		public ScheduledEventApi ScheduledEventApi
		{
			get;
			private set;
		}

		public DiagnosticsApi DiagnosticsApi
		{
			get;
			private set;
		}

		public CaptchaApi CaptchaApi
		{
			get;
			private set;
		}

		public RollingStatistics GameServerLatency
		{
			get;
			private set;
		}

		public RollingStatistics WebServiceLatency
		{
			get;
			private set;
		}

		public string AccessToken
		{
			get
			{
				return accessToken;
			}
			set
			{
				accessToken = value;
				Configuration.SetSetting("cp-api-username", AccessToken);
				if (OfflineMode)
				{
					Service.Get<OfflineDatabase>().AccessToken = accessToken;
				}
				if (RecoverableOperationService != null && accessToken != null)
				{
					RecoverableOperationService.AccessTokenRefreshed();
				}
			}
		}

		public string ContentVersion
		{
			set
			{
				Configuration.SetSetting("cp-content-version", value);
			}
		}

		public DateTime ContentVersionDate
		{
			get
			{
				return (DateTime)Configuration.GetSetting("cp-content-version-date");
			}
			set
			{
				Configuration.SetSetting("cp-content-version-date", value);
			}
		}

		public string SWID
		{
			get;
			set;
		}

		public long PlayerSessionId
		{
			get;
			set;
		}

		public string PlayerName
		{
			get;
			set;
		}

		public RecoverableOperationService RecoverableOperationService
		{
			get;
			private set;
		}

		public ClubPenguinClient(MonoBehaviour monoBehaviour, string apiURL, string clientToken, string clientApiVersion, string gameZone, bool gameEncryption, bool gameDebugging, bool lagMonitoring, int gameServerLatencyWindowSize, int webServiceLatencyWindowSize, string cpMonitoringURL, string websiteApiURL, bool offlineMode)
		{
			OfflineMode = offlineMode;
			SyncContext = new UnitySynchronizationContext(monoBehaviour);
			ICommonGameSettings commonGameSettings = Service.Get<ICommonGameSettings>();
			if (offlineMode && string.IsNullOrEmpty(commonGameSettings.GameServerHost) && string.IsNullOrEmpty(commonGameSettings.CPAPIServicehost))
			{
				GameServer = new OfflineGameServerClient(this);
			}
			else
			{
				GameServer = new SmartFoxGameServerClient(this, gameZone, gameEncryption, gameDebugging, lagMonitoring);
			}
			BreadcrumbApi = new BreadcrumbApi(this);
			ChatApi = new ChatApi(this);
			ConsumableApi = new ConsumableApi(this);
			DurableApi = new DurableApi(this);
			EncryptionApi = new EncryptionApi(this);
			GameApi = new GameApi(this);
			IglooApi = new IglooApi(this);
			InventoryApi = new InventoryApi(this);
			MinigameApi = new MinigameApi(this);
			PlayerApi = new PlayerApi(this);
			QuestApi = new QuestApi(this);
			RewardApi = new RewardApi(this);
			SavedOutfitApi = new SavedOutfitApi(this);
			TaskApi = new TaskApi(this);
			IAPApi = new IAPApi(this);
			TutorialApi = new TutorialApi(this);
			ModerationApi = new ModerationApi(this);
			TubeApi = new TubeApi(this);
			DisneyStoreApi = new DisneyStoreApi(this);
			NewsfeedApi = new NewsfeedApi(this);
			CatalogApi = new CatalogApi(this);
			ScheduledEventApi = new ScheduledEventApi(this);
			DiagnosticsApi = new DiagnosticsApi(this);
			CaptchaApi = new CaptchaApi(this);
			initWAK(apiURL, clientToken, clientApiVersion);
			Configuration.SetBaseUri("cp-monitoring-base-uri", cpMonitoringURL);
			Configuration.SetBaseUri("cp-website-api-base-uri", websiteApiURL);
			initCPKeyValueDatabase(Service.Get<IKeychain>());
			initEncryption();
			RecoverableOperationService = new QueuableRecoverableOperationService(this);
			if (lagMonitoring)
			{
				GameServerLatency = new RollingStatistics(gameServerLatencyWindowSize);
				WebServiceLatency = new RollingStatistics(webServiceLatencyWindowSize);
			}
		}

		public void Destroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<Content.ContentManifestUpdated>(onContentManifestUpdated);
		}

		private void initWAK(string apiUrl, string clientToken, string clientApiVersion)
		{
			Configuration.SetSetting("log-internal", false);
			Configuration.SetSetting("log-VERBOSE", false);
			Configuration.SetSetting("log-DEBUG", true);
			Configuration.SetSetting("log-INFO", true);
			Configuration.SetSetting("log-WARNING", true);
			Configuration.SetSetting("log-ERROR", true);
			Configuration.SetSetting("log-callback", (Action<string, LogSeverity>)delegate(string message, LogSeverity severity)
			{
				switch (severity)
				{
				case LogSeverity.ERROR:
				{
					Log.PriorityFlags priorityFlags = Log.PriorityFlags.NETWORK_ERROR;
					break;
				}
				case LogSeverity.WARNING:
				{
					Log.PriorityFlags priorityFlags = Log.PriorityFlags.WARNING;
					break;
				}
				case LogSeverity.INFO:
				{
					Log.PriorityFlags priorityFlags = Log.PriorityFlags.INFO;
					break;
				}
				case LogSeverity.DEBUG:
				{
					Log.PriorityFlags priorityFlags = Log.PriorityFlags.DEBUG;
					break;
				}
				case LogSeverity.VERBOSE:
				{
					Log.PriorityFlags priorityFlags = Log.PriorityFlags.TRACE;
					break;
				}
				default:
				{
					Log.PriorityFlags priorityFlags = Log.PriorityFlags.NONE;
					break;
				}
				}
			});
			Configuration.SetSetting("destroy-operation-on-completion", false);
			Configuration.SetSetting("persistent-game-object-name", "com.clubpenguin/WebApiKit");
			Configuration.SetSetting("default-http-client", typeof(HttpWWWClient));
			Configuration.SetSetting("request-timeout", 35f);
			Configuration.SetSetting("yield-time", 0f);
			Configuration.SetSetting("default-json-serializer", typeof(SerializeLitJson));
			Configuration.SetSetting("default-json-deserializer", typeof(DeserializeLitJson));
			Configuration.SetBaseUri("cp-api-base-uri", apiUrl);
			Configuration.SetSetting("cp-api-client-token", clientToken);
			Configuration.SetSetting("cp-api-client-version", clientApiVersion);
			if (AccessToken != null)
			{
				Configuration.SetSetting("cp-api-username", AccessToken);
			}
			setWakContentVersionFromContentSystem();
			Service.Get<EventDispatcher>().AddListener<Content.ContentManifestUpdated>(onContentManifestUpdated);
			Configuration.Bootstrap();
		}

		private bool onContentManifestUpdated(Content.ContentManifestUpdated evt)
		{
			setWakContentVersionFromContentSystem();
			return false;
		}

		private void setWakContentVersionFromContentSystem()
		{
			try
			{
				Content content = Service.Get<Content>();
				ContentVersion = content.ContentVersion;
			}
			catch (Exception)
			{
				ContentVersion = "UNKNOWN";
			}
		}

		private void initCPKeyValueDatabase(IKeychain keychainData)
		{
			string localStorageDirPath = Path.Combine(Application.persistentDataPath, "KeyValueDatabase");
			byte[] localStorageKey = keychainData.LocalStorageKey;
			DocumentCollectionFactory documentCollectionFactory = new DocumentCollectionFactory();
			CPKeyValueDatabase = new CPKeyValueDatabase(localStorageKey, localStorageDirPath, documentCollectionFactory);
		}

		private void initEncryption()
		{
			if (!haveKeyPair())
			{
				initKeyPair();
			}
		}

		private bool haveKeyPair()
		{
			return CPKeyValueDatabase.GetRsaParameters().HasValue;
		}

		private void initKeyPair()
		{
			GenerateKeyPair(delegate(RSAParameters rsaParams)
			{
				CPKeyValueDatabase.SetRsaParameters(rsaParams);
			}, delegate
			{
				Log.LogError(this, "Failed to generate key pair for ClubPenguinClient. Will not be able to encrypt requests.");
			});
		}

		public void GenerateKeyPair(Action<RSAParameters> successCallback, System.Action failureCallback)
		{
			CoroutineRunner.StartPersistent(generateKeyPairCoroutine(successCallback, failureCallback), this, "GenerateKeyPair");
		}

		private IEnumerator generateKeyPairCoroutine(Action<RSAParameters> successCallback, System.Action failureCallback)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				rsaParameters = RsaEncryptor.GenerateKeypair();
			});
			thread.Start();
			while (!rsaParameters.HasValue)
			{
				yield return null;
			}
			successCallback(rsaParameters.Value);
		}

		internal void logGameServerPing(int milliseconds)
		{
			if (GameServerLatency != null)
			{
				GameServerLatency.AddSample(milliseconds);
			}
		}

		internal void logWebServicePing(int milliseconds)
		{
			if (WebServiceLatency != null)
			{
				WebServiceLatency.AddSample(milliseconds);
			}
		}
	}
}
