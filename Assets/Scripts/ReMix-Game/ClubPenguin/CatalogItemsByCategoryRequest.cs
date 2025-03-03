using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class CatalogItemsByCategoryRequest : CatalogItemsRequest
	{
		public CatalogItemsByCategoryRequest(string category, long scheduledThemeChallengeId, string cursor = "")
		{
			CacheId = CatalogItemsRequest.GetPrefixedCacheId(category, scheduledThemeChallengeId);
			Service.Get<EventDispatcher>().AddListener<CatalogServiceEvents.CatalogItemsByCategoryRetrievedEvent>(onItemsByCategory);
			Service.Get<INetworkServicesManager>().CatalogService.GetCatalogItemsByCategory(category, scheduledThemeChallengeId, cursor);
		}

		public void RemoveListener()
		{
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.CatalogItemsByCategoryRetrievedEvent>(onItemsByCategory);
		}

		private bool onItemsByCategory(CatalogServiceEvents.CatalogItemsByCategoryRetrievedEvent evt)
		{
			RemoveListener();
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.OnCatalogItemsByCategory(CacheId, evt));
			return false;
		}
	}
}
