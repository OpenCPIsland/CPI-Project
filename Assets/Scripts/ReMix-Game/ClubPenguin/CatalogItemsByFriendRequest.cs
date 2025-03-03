using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class CatalogItemsByFriendRequest : CatalogItemsRequest
	{
		public CatalogItemsByFriendRequest(long scheduledThemeChallengeId, string cursor = "")
		{
			CacheId = CatalogItemsRequest.GetPrefixedCacheId("Friend", scheduledThemeChallengeId);
			Service.Get<EventDispatcher>().AddListener<CatalogServiceEvents.CatalogItemsByFriendsRetrievedEvent>(onItems);
			Service.Get<INetworkServicesManager>().CatalogService.GetCatalogItemsByFriends(scheduledThemeChallengeId, cursor);
		}

		public void RemoveListener()
		{
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.CatalogItemsByFriendsRetrievedEvent>(onItems);
		}

		private bool onItems(CatalogServiceEvents.CatalogItemsByFriendsRetrievedEvent evt)
		{
			RemoveListener();
			Service.Get<EventDispatcher>().DispatchEvent(new CatalogServiceProxyEvents.OnCatalogItemsByFriend(CacheId, evt));
			return false;
		}
	}
}
