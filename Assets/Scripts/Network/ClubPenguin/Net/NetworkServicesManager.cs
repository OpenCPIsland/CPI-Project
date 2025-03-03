using ClubPenguin.Net.Client;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using System;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class NetworkServicesManager : INetworkServicesManager
	{
		private ClubPenguinClient clubPenguinClient;

		private MonoBehaviour monoBehaviour;

		private NetworkServicesConfig currentConfig;

		private IWorldService worldService;

		private IPlayerStateService playerStateService;

		private IChatService chatService;

		private IPlayerActionService playerActionService;

		private IInventoryService inventoryService;

		private IBreadcrumbService breadcrumbService;

		private ISavedOutfitService savedOutfitService;

		private IIglooService iglooService;

		private IPrototypeService prototypeService;

		private IFriendsService friendsService;

		private IQuestService questService;

		private IConsumableService consumableService;

		private IRewardService rewardService;

		private ITaskService taskService;

		private IMinigameService minigameService;

		private IIAPService iapService;

		private ITutorialService tutorialService;

		private IModerationService moderationService;

		private IDisneyStoreService disneyStoreService;

		private INewsfeedService newsfeedService;

		private ICatalogService catalogService;

		private IPartyGameService partyGameService;

		private IScheduledEventService scheduledEventService;

		private IDiagnosticsService diagnosticsService;

		private ICaptchaService captchaService;

		private bool offlineMode;

		public IWorldService WorldService
		{
			get
			{
				return worldService;
			}
		}

		public IPlayerStateService PlayerStateService
		{
			get
			{
				return playerStateService;
			}
		}

		public IChatService ChatService
		{
			get
			{
				return chatService;
			}
		}

		public IPlayerActionService PlayerActionService
		{
			get
			{
				return playerActionService;
			}
		}

		public IInventoryService InventoryService
		{
			get
			{
				return inventoryService;
			}
		}

		public IBreadcrumbService BreadcrumbService
		{
			get
			{
				return breadcrumbService;
			}
		}

		public ISavedOutfitService SavedOutfitService
		{
			get
			{
				return savedOutfitService;
			}
		}

		public IIglooService IglooService
		{
			get
			{
				return iglooService;
			}
		}

		public IPrototypeService PrototypeService
		{
			get
			{
				return prototypeService;
			}
		}

		public IFriendsService FriendsService
		{
			get
			{
				return friendsService;
			}
		}

		public IQuestService QuestService
		{
			get
			{
				return questService;
			}
		}

		public IConsumableService ConsumableService
		{
			get
			{
				return consumableService;
			}
		}

		public IRewardService RewardService
		{
			get
			{
				return rewardService;
			}
		}

		public ITaskService TaskService
		{
			get
			{
				return taskService;
			}
		}

		public IMinigameService MinigameService
		{
			get
			{
				return minigameService;
			}
		}

		public IIAPService IAPService
		{
			get
			{
				return iapService;
			}
		}

		public ITutorialService TutorialService
		{
			get
			{
				return tutorialService;
			}
		}

		public IModerationService ModerationService
		{
			get
			{
				return moderationService;
			}
		}

		public IDisneyStoreService DisneyStoreService
		{
			get
			{
				return disneyStoreService;
			}
		}

		public INewsfeedService NewsfeedService
		{
			get
			{
				return newsfeedService;
			}
		}

		public ICatalogService CatalogService
		{
			get
			{
				return catalogService;
			}
		}

		public IPartyGameService PartyGameService
		{
			get
			{
				return partyGameService;
			}
		}

		public IScheduledEventService ScheduledEventService
		{
			get
			{
				return scheduledEventService;
			}
		}

		public IDiagnosticsService DiagnosticsService
		{
			get
			{
				return diagnosticsService;
			}
		}

		public ICaptchaService CaptchaService
		{
			get
			{
				return captchaService;
			}
		}

		public long GameTimeMilliseconds
		{
			get
			{
				return clubPenguinClient.GameServer.ServerTime;
			}
		}

		public int ServerUserCount
		{
			get
			{
				return clubPenguinClient.GameServer.UserCount;
			}
		}

		public DateTime ServerDateTime
		{
			get
			{
				return GameTimeMilliseconds.MsToDateTime();
			}
		}

		public RollingStatistics GameServerLatency
		{
			get
			{
				return clubPenguinClient.GameServerLatency;
			}
		}

		public RollingStatistics WebServiceLatency
		{
			get
			{
				return clubPenguinClient.WebServiceLatency;
			}
		}

		public bool IsGameServerConnected()
		{
			if (clubPenguinClient != null && clubPenguinClient.GameServer != null)
			{
				return clubPenguinClient.GameServer.IsConnected();
			}
			return false;
		}

		public NetworkServicesManager(MonoBehaviour monoBehaviour, NetworkServicesConfig config, bool offlineMode)
		{
			this.monoBehaviour = monoBehaviour;
			this.offlineMode = offlineMode;
			Configure(config);
		}

		public void Configure(NetworkServicesConfig config)
		{
			if (clubPenguinClient != null)
			{
				clubPenguinClient.Destroy();
			}
			clubPenguinClient = new ClubPenguinClient(monoBehaviour, config.CPAPIServicehost, config.CPAPIClientToken, config.ClientApiVersion, config.CPGameServerZone, !offlineMode && config.CPGameServerEncrypted, true, config.CPLagMonitoring, config.CPGameServerLatencyWindowSize, config.CPWebServiceLatencyWindowSize, config.CPMonitoringServicehost, config.CPWebsiteAPIServicehost, offlineMode);
			currentConfig = config;
			worldService = new WorldService();
			worldService.Initialize(clubPenguinClient);
			playerStateService = new PlayerStateService();
			playerStateService.Initialize(clubPenguinClient);
			chatService = new ChatService();
			chatService.Initialize(clubPenguinClient);
			playerActionService = new PlayerActionService();
			playerActionService.Initialize(clubPenguinClient);
			iglooService = new IglooService();
			iglooService.Initialize(clubPenguinClient);
			inventoryService = new InventoryService();
			inventoryService.Initialize(clubPenguinClient);
			breadcrumbService = new BreadcrumbService();
			breadcrumbService.Initialize(clubPenguinClient);
			savedOutfitService = new SavedOutfitService();
			savedOutfitService.Initialize(clubPenguinClient);
			prototypeService = new PrototypeService();
			prototypeService.Initialize(clubPenguinClient);
			questService = new QuestService();
			questService.Initialize(clubPenguinClient);
			consumableService = new ConsumableService();
			consumableService.Initialize(clubPenguinClient);
			friendsService = new FriendsService();
			friendsService.Initialize(clubPenguinClient);
			rewardService = new RewardService();
			rewardService.Initialize(clubPenguinClient);
			taskService = new TaskNetworkService();
			taskService.Initialize(clubPenguinClient);
			minigameService = new MinigameService();
			minigameService.Initialize(clubPenguinClient);
			iapService = new IAPService();
			iapService.Initialize(clubPenguinClient);
			tutorialService = new TutorialService();
			tutorialService.Initialize(clubPenguinClient);
			moderationService = new ModerationService();
			moderationService.Initialize(clubPenguinClient);
			disneyStoreService = new DisneyStoreService();
			disneyStoreService.Initialize(clubPenguinClient);
			newsfeedService = new NewsfeedService();
			newsfeedService.Initialize(clubPenguinClient);
			catalogService = new CatalogService();
			catalogService.Initialize(clubPenguinClient);
			partyGameService = new PartyGameService();
			partyGameService.Initialize(clubPenguinClient);
			scheduledEventService = new ScheduledEventService();
			scheduledEventService.Initialize(clubPenguinClient);
			diagnosticsService = new DiagnosticsService();
			diagnosticsService.Initialize(clubPenguinClient);
			captchaService = new CaptchaService();
			captchaService.Initialize(clubPenguinClient);
		}

		public void Reset()
		{
			questService.ClearQueue();
			try
			{
				Configure(currentConfig);
			}
			catch
			{
				Log.LogError(this, "Cannot reset network services, configuration missing");
			}
		}
	}
}
