using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Net
{
	public interface INetworkServicesManager
	{
		IWorldService WorldService
		{
			get;
		}

		IPlayerStateService PlayerStateService
		{
			get;
		}

		IChatService ChatService
		{
			get;
		}

		IPlayerActionService PlayerActionService
		{
			get;
		}

		IInventoryService InventoryService
		{
			get;
		}

		IBreadcrumbService BreadcrumbService
		{
			get;
		}

		ISavedOutfitService SavedOutfitService
		{
			get;
		}

		IIglooService IglooService
		{
			get;
		}

		IPrototypeService PrototypeService
		{
			get;
		}

		IFriendsService FriendsService
		{
			get;
		}

		IQuestService QuestService
		{
			get;
		}

		ITaskService TaskService
		{
			get;
		}

		IConsumableService ConsumableService
		{
			get;
		}

		IRewardService RewardService
		{
			get;
		}

		IMinigameService MinigameService
		{
			get;
		}

		IIAPService IAPService
		{
			get;
		}

		ITutorialService TutorialService
		{
			get;
		}

		IModerationService ModerationService
		{
			get;
		}

		IDisneyStoreService DisneyStoreService
		{
			get;
		}

		INewsfeedService NewsfeedService
		{
			get;
		}

		ICatalogService CatalogService
		{
			get;
		}

		IPartyGameService PartyGameService
		{
			get;
		}

		IScheduledEventService ScheduledEventService
		{
			get;
		}

		IDiagnosticsService DiagnosticsService
		{
			get;
		}

		ICaptchaService CaptchaService
		{
			get;
		}

		long GameTimeMilliseconds
		{
			get;
		}

		int ServerUserCount
		{
			get;
		}

		DateTime ServerDateTime
		{
			get;
		}

		RollingStatistics GameServerLatency
		{
			get;
		}

		RollingStatistics WebServiceLatency
		{
			get;
		}

		bool IsGameServerConnected();

		void Configure(NetworkServicesConfig networkServicesConfig);

		void Reset();
	}
}
