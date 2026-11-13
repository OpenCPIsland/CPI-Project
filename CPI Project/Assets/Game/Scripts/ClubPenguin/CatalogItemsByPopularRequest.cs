using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class CatalogItemsByPopularRequest : CatalogItemsRequest
	{
		public CatalogItemsByPopularRequest(long scheduledThemeChallengeId, string cursor = "")
		{
			CacheId = CatalogItemsRequest.GetPrefixedCacheId("Popular", scheduledThemeChallengeId);
			Service.Get<EventDispatcher>().AddListener<CatalogServiceEvents.CatalogItemsByPopularityRetrievedEvent>(onItems);
			Service.Get<INetworkServicesManager>().CatalogService.GetCatalogItemsByPopularity(scheduledThemeChallengeId, cursor);
		}

		public void RemoveListener()
		{
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.CatalogItemsByPopularityRetrievedEvent>(onItems);
		}

		private bool onItems(CatalogServiceEvents.CatalogItemsByPopularityRetrievedEvent evt)
		{
			RemoveListener();
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.OnCatalogItemsByPopular(CacheId, evt));
			return false;
		}
	}
}
