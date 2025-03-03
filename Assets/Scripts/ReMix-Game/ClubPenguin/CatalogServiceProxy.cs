using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class CatalogServiceProxy
	{
		public CatalogThemeColors themeColors;

		public int CurrentThemeIndex;

		private bool isCatalogThemeActive = false;

		private long activeThemeScheduleId;

		private EventChannel eventChannel;

		public CatalogCache cache
		{
			get;
			private set;
		}

		public CurrentThemeData currentThemeData
		{
			get;
			private set;
		}

		public CatalogServiceProxy()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<CatalogServiceProxyEvents.OnCatalogItemsByCategory>(onItemsByCategory);
			eventChannel.AddListener<CatalogServiceProxyEvents.OnCatalogItemsByFriend>(onItemsByFriend);
			eventChannel.AddListener<CatalogServiceProxyEvents.OnCatalogItemsByPopular>(onItemsByPopular);
			eventChannel.AddListener<CatalogServiceProxyEvents.OnCatalogItemsByRecent>(onItemsByRecent);
			eventChannel.AddListener<CatalogServiceEvents.CurrentThemesRetrievedEvent>(onThemes);
			eventChannel.AddListener<CatalogServiceEvents.CurrentThemesErrorEvent>(onThemesError);
			eventChannel.AddListener<CatalogServiceEvents.UserStatsRetrievedEvent>(onStats);
			eventChannel.AddListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
			eventChannel.AddListener<WorldServiceEvents.SelfRoomJoinedEvent>(onRoomJoined);
			themeColors = new CatalogThemeColors();
			cache = new CatalogCache();
		}

		public bool IsCatalogThemeActive()
		{
			return isCatalogThemeActive;
		}

		public void ActivateCatalogTheme()
		{
			isCatalogThemeActive = true;
		}

		public void DeactivateCatalogTheme()
		{
			isCatalogThemeActive = false;
		}

		public CatalogThemeDefinition GetThemeByScheduelId(long id)
		{
			CatalogThemeScheduleDefinition scheduleById = GetScheduleById(id);
			if (scheduleById == null)
			{
				return null;
			}
			return GetThemeById(scheduleById.CatalogThemeId);
		}

		public CatalogThemeDefinition GetThemeById(long themeId)
		{
			CatalogThemeDefinition result = null;
			Dictionary<int, CatalogThemeDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, CatalogThemeDefinition>>();
			if (dictionary != null)
			{
				if (dictionary.ContainsKey((int)themeId))
				{
					result = dictionary[(int)themeId];
				}
				else
				{
					Log.LogErrorFormatted(this, "Unable to locate theme definition with id {0}.", themeId);
				}
			}
			return result;
		}

		public CatalogThemeScheduleDefinition GetScheduleById(long scheduleId)
		{
			CatalogThemeScheduleDefinition result = null;
			Dictionary<int, CatalogThemeScheduleDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, CatalogThemeScheduleDefinition>>();
			if (dictionary != null)
			{
				if (dictionary.ContainsKey((int)scheduleId))
				{
					result = dictionary[(int)scheduleId];
				}
				else
				{
					Log.LogErrorFormatted(this, "Unable to locate schedule definition with id {0}.", scheduleId);
				}
			}
			return result;
		}

		public long GetActiveThemeScheduleId()
		{
			return activeThemeScheduleId;
		}

		public CatalogThemeDefinition GetCatalogTheme()
		{
			return GetThemeByScheduelId(activeThemeScheduleId);
		}

		public CurrentThemeData GetCurrentThemeData()
		{
			return currentThemeData;
		}

		public void GetCatalogItemsByCategory(string category, long scheduledThemeChallengeId, string cursor = "")
		{
			string prefixedCacheId = CatalogItemsRequest.GetPrefixedCacheId(category, scheduledThemeChallengeId);
			if (cache.isShouldUseCache(prefixedCacheId))
			{
				CatalogCacheData cacheDataById = cache.GetCacheDataById(prefixedCacheId);
				Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(cacheDataById.Data as List<CatalogItemData>));
			}
			else
			{
				new CatalogItemsByCategoryRequest(category, scheduledThemeChallengeId, cursor);
			}
		}

		public void GetCatalogItemsByFriends(long scheduledThemeChallengeId, string cursor = "")
		{
			string prefixedCacheId = CatalogItemsRequest.GetPrefixedCacheId("Friend", scheduledThemeChallengeId);
			if (cache.isShouldUseCache(prefixedCacheId))
			{
				CatalogCacheData cacheDataById = cache.GetCacheDataById(prefixedCacheId);
				Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(cacheDataById.Data as List<CatalogItemData>));
			}
			else
			{
				new CatalogItemsByFriendRequest(scheduledThemeChallengeId, cursor);
			}
		}

		public void GetCatalogItemsByPopularity(long scheduledThemeChallengeId, string cursor = "")
		{
			string prefixedCacheId = CatalogItemsRequest.GetPrefixedCacheId("Popular", scheduledThemeChallengeId);
			if (cache.isShouldUseCache(prefixedCacheId))
			{
				CatalogCacheData cacheDataById = cache.GetCacheDataById(prefixedCacheId);
				Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(cacheDataById.Data as List<CatalogItemData>));
			}
			else
			{
				new CatalogItemsByPopularRequest(scheduledThemeChallengeId, cursor);
			}
		}

		public void GetCatalogItemsByRecent(long scheduledThemeChallengeId, string cursor = "")
		{
			string prefixedCacheId = CatalogItemsRequest.GetPrefixedCacheId("Recent", scheduledThemeChallengeId);
			if (cache.isShouldUseCache(prefixedCacheId))
			{
				CatalogCacheData cacheDataById = cache.GetCacheDataById(prefixedCacheId);
				Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(cacheDataById.Data as List<CatalogItemData>));
			}
			else
			{
				new CatalogItemsByRecentRequest(scheduledThemeChallengeId, cursor);
			}
		}

		public void GetCurrentThemes()
		{
			if (cache.isShouldUseCache("CatalogThemeCacheId"))
			{
				CatalogCacheData cacheDataById = cache.GetCacheDataById("CatalogThemeCacheId");
				Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ChallengesReponse(cacheDataById.Data as List<CurrentThemeData>));
			}
			else
			{
				Service.Get<INetworkServicesManager>().CatalogService.GetCurrentThemes();
			}
		}

		public void GetPlayerCatalogStats()
		{
			if (cache.isShouldUseCache("catalogStatsCacheId"))
			{
				CatalogCacheData cacheDataById = cache.GetCacheDataById("catalogStatsCacheId");
				Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.StatsReponse((CatalogStats)cacheDataById.Data));
			}
			else
			{
				Service.Get<INetworkServicesManager>().CatalogService.GetPlayerCatalogStats();
			}
		}

		private bool onItemsByCategory(CatalogServiceProxyEvents.OnCatalogItemsByCategory evt)
		{
			List<CatalogItemData> items = evt.Request.Response.items;
			CatalogCacheData cacheDataById = cache.GetCacheDataById(evt.CacheId);
			cacheDataById.Data = items;
			cache.SetCatalogCacheData(evt.CacheId, cacheDataById);
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(items));
			return false;
		}

		private bool onItemsByFriend(CatalogServiceProxyEvents.OnCatalogItemsByFriend evt)
		{
			List<CatalogItemData> items = evt.Request.Response.items;
			CatalogCacheData cacheDataById = cache.GetCacheDataById(evt.CacheId);
			cacheDataById.Data = items;
			cache.SetCatalogCacheData(evt.CacheId, cacheDataById);
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(items));
			return false;
		}

		private bool onItemsByPopular(CatalogServiceProxyEvents.OnCatalogItemsByPopular evt)
		{
			List<CatalogItemData> items = evt.Request.Response.items;
			CatalogCacheData cacheDataById = cache.GetCacheDataById(evt.CacheId);
			cacheDataById.Data = items;
			cache.SetCatalogCacheData(evt.CacheId, cacheDataById);
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(items));
			return false;
		}

		private bool onItemsByRecent(CatalogServiceProxyEvents.OnCatalogItemsByRecent evt)
		{
			List<CatalogItemData> items = evt.Request.Response.items;
			CatalogCacheData cacheDataById = cache.GetCacheDataById(evt.CacheId);
			cacheDataById.Data = items;
			cache.SetCatalogCacheData(evt.CacheId, cacheDataById);
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ShopItemsReponse(items));
			return false;
		}

		private bool onThemes(CatalogServiceEvents.CurrentThemesRetrievedEvent evt)
		{
			themeColors.SetIndex();
			List<CurrentThemeData> themes = evt.Response.themes;
			if (themes != null && themes.Count > 0)
			{
				activeThemeScheduleId = themes[0].scheduledThemeChallengeId;
				currentThemeData = themes[0];
			}
			CatalogCacheData cacheDataById = cache.GetCacheDataById("CatalogThemeCacheId");
			cacheDataById.Data = themes;
			cache.SetCatalogCacheData("CatalogThemeCacheId", cacheDataById);
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ChallengesReponse(themes));
			return false;
		}

		private bool onThemesError(CatalogServiceEvents.CurrentThemesErrorEvent evt)
		{
			CatalogCacheData cacheDataById = cache.GetCacheDataById("CatalogThemeCacheId");
			if (cacheDataById.Data == null)
			{
				cacheDataById.Data = new List<CurrentThemeData>();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.ChallengesReponse(cacheDataById.Data as List<CurrentThemeData>));
			return false;
		}

		private bool onStats(CatalogServiceEvents.UserStatsRetrievedEvent evt)
		{
			List<CatalogItemData> currentItems = evt.Response.currentItems;
			CatalogItemData mostPopularItem = evt.Response.mostPopularItem;
			long totalItemsPurchased = evt.Response.totalItemsPurchased;
			long totalItemsSold = evt.Response.totalItemsSold;
			CatalogStats catalogStats = new CatalogStats(mostPopularItem, currentItems, totalItemsPurchased, totalItemsSold);
			CatalogCacheData cacheDataById = cache.GetCacheDataById("catalogStatsCacheId");
			cacheDataById.Data = catalogStats;
			cacheDataById.CacheTime = 0;
			cache.SetCatalogCacheData("catalogStatsCacheId", cacheDataById);
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.StatsReponse(catalogStats));
			return false;
		}

		private bool onRoomJoined(WorldServiceEvents.SelfRoomJoinedEvent evt)
		{
			GetCurrentThemes();
			return false;
		}

		private bool onThemesRetrieved(CatalogServiceProxyEvents.ChallengesReponse evt)
		{
			eventChannel.RemoveListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
			eventChannel.RemoveListener<WorldServiceEvents.SelfRoomJoinedEvent>(onRoomJoined);
			if (evt.Themes.Count > 0)
			{
				CurrentThemeData currentThemeData = evt.Themes[0];
				CatalogThemeDefinition themeByScheduelId = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(currentThemeData.scheduledThemeChallengeId);
				TaskDefinition clothingCatalogChallenge = Service.Get<TaskService>().ClothingCatalogChallenge;
				clothingCatalogChallenge.Title = themeByScheduelId.Title;
				clothingCatalogChallenge.CompletionMessage = themeByScheduelId.CompleteMessage;
				clothingCatalogChallenge.Description = themeByScheduelId.Description;
			}
			return false;
		}
	}
}
