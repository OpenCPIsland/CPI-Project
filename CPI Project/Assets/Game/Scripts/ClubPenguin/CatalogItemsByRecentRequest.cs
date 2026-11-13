using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class CatalogItemsByRecentRequest : CatalogItemsRequest
	{
		public CatalogItemsByRecentRequest(long scheduledThemeChallengeId, string cursor = "")
		{
			CacheId = CatalogItemsRequest.GetPrefixedCacheId("Recent", scheduledThemeChallengeId);
			Service.Get<EventDispatcher>().AddListener<CatalogServiceEvents.CatalogItemsByRecentRetrievedEvent>(onItems);
			Service.Get<INetworkServicesManager>().CatalogService.GetCatalogItemsByRecent(scheduledThemeChallengeId, cursor);
		}

		public void RemoveListener()
		{
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.CatalogItemsByRecentRetrievedEvent>(onItems);
		}

		private bool onItems(CatalogServiceEvents.CatalogItemsByRecentRetrievedEvent evt)
		{
			RemoveListener();
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.OnCatalogItemsByRecent(CacheId, evt));
			return false;
		}
	}
}
