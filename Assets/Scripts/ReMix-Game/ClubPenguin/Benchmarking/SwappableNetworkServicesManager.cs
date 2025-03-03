using ClubPenguin.Net;
using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Benchmarking
{
	internal class SwappableNetworkServicesManager : INetworkServicesManager
	{
		public INetworkServicesManager NetworkServicesManager
		{
			get;
			set;
		}

		public IWorldService WorldService
		{
			get
			{
				return NetworkServicesManager.WorldService;
			}
		}

		public IPlayerStateService PlayerStateService
		{
			get
			{
				return NetworkServicesManager.PlayerStateService;
			}
		}

		public IChatService ChatService
		{
			get
			{
				return NetworkServicesManager.ChatService;
			}
		}

		public IPlayerActionService PlayerActionService
		{
			get
			{
				return NetworkServicesManager.PlayerActionService;
			}
		}

		public IInventoryService InventoryService
		{
			get
			{
				return NetworkServicesManager.InventoryService;
			}
		}

		public IBreadcrumbService BreadcrumbService
		{
			get
			{
				return NetworkServicesManager.BreadcrumbService;
			}
		}

		public ITutorialService TutorialService
		{
			get
			{
				return NetworkServicesManager.TutorialService;
			}
		}

		public ISavedOutfitService SavedOutfitService
		{
			get
			{
				return NetworkServicesManager.SavedOutfitService;
			}
		}

		public IIglooService IglooService
		{
			get
			{
				return NetworkServicesManager.IglooService;
			}
		}

		public IPrototypeService PrototypeService
		{
			get
			{
				return NetworkServicesManager.PrototypeService;
			}
		}

		public IFriendsService FriendsService
		{
			get
			{
				return NetworkServicesManager.FriendsService;
			}
		}

		public IQuestService QuestService
		{
			get
			{
				return NetworkServicesManager.QuestService;
			}
		}

		public long GameTimeMilliseconds
		{
			get
			{
				return NetworkServicesManager.GameTimeMilliseconds;
			}
		}

		public IConsumableService ConsumableService
		{
			get
			{
				return NetworkServicesManager.ConsumableService;
			}
		}

		public IRewardService RewardService
		{
			get
			{
				return NetworkServicesManager.RewardService;
			}
		}

		public ITaskService TaskService
		{
			get
			{
				return NetworkServicesManager.TaskService;
			}
		}

		public IMinigameService MinigameService
		{
			get
			{
				return NetworkServicesManager.MinigameService;
			}
		}

		public IIAPService IAPService
		{
			get
			{
				return NetworkServicesManager.IAPService;
			}
		}

		public IModerationService ModerationService
		{
			get
			{
				return NetworkServicesManager.ModerationService;
			}
		}

		public IDisneyStoreService DisneyStoreService
		{
			get
			{
				return NetworkServicesManager.DisneyStoreService;
			}
		}

		public INewsfeedService NewsfeedService
		{
			get
			{
				return NetworkServicesManager.NewsfeedService;
			}
		}

		public ICatalogService CatalogService
		{
			get
			{
				return NetworkServicesManager.CatalogService;
			}
		}

		public IPartyGameService PartyGameService
		{
			get
			{
				return NetworkServicesManager.PartyGameService;
			}
		}

		public IScheduledEventService ScheduledEventService
		{
			get
			{
				return NetworkServicesManager.ScheduledEventService;
			}
		}

		public IDiagnosticsService DiagnosticsService
		{
			get
			{
				return NetworkServicesManager.DiagnosticsService;
			}
		}

		public ICaptchaService CaptchaService
		{
			get
			{
				return NetworkServicesManager.CaptchaService;
			}
		}

		public DateTime ServerDateTime
		{
			get
			{
				return NetworkServicesManager.ServerDateTime;
			}
		}

		public RollingStatistics GameServerLatency
		{
			get
			{
				return NetworkServicesManager.GameServerLatency;
			}
		}

		public RollingStatistics WebServiceLatency
		{
			get
			{
				return NetworkServicesManager.WebServiceLatency;
			}
		}

		public int ServerUserCount
		{
			get
			{
				return NetworkServicesManager.ServerUserCount;
			}
		}

		public bool IsGameServerConnected()
		{
			return false;
		}

		public void Configure(NetworkServicesConfig networkServicesConfig)
		{
			NetworkServicesManager.Configure(networkServicesConfig);
		}

		public void Reset()
		{
		}
	}
}
