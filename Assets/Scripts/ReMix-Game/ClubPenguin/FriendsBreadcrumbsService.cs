using ClubPenguin.Breadcrumbs;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class FriendsBreadcrumbsService : AbstractDataModelService
	{
		private NotificationBreadcrumbController notificationBreadcrumbController;

		private StaticBreadcrumbDefinitionKey friendAddedBreadcrumb;

		private StaticBreadcrumbDefinitionKey friendRequestBreadcrumb;

		public void Init(StaticBreadcrumbDefinitionKey friendAddedBreadcrumb, StaticBreadcrumbDefinitionKey friendRequestBreadcrumb)
		{
			this.friendAddedBreadcrumb = friendAddedBreadcrumb;
			this.friendRequestBreadcrumb = friendRequestBreadcrumb;
		}

		private void Start()
		{
			notificationBreadcrumbController = Service.Get<NotificationBreadcrumbController>();
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendsServiceInitialized>(onFriendsServiceInitialized);
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendsServiceCleared>(onFriendsServiceCleared);
		}

		private bool onFriendsServiceInitialized(FriendsServiceEvents.FriendsServiceInitialized evt)
		{
			notificationBreadcrumbController.ResetBreadcrumbs(friendAddedBreadcrumb);
			notificationBreadcrumbController.ResetBreadcrumbs(friendRequestBreadcrumb);
			int count = FriendsDataModelService.IncomingInvitationsList.Count;
			if (count > 0)
			{
				notificationBreadcrumbController.AddBreadcrumb(friendRequestBreadcrumb, count);
			}
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<FriendData>>(onFriendComponentAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<IncomingFriendInvitationData>>(onIncomingFriendInvitationComponentAdded);
			return false;
		}

		private bool onFriendsServiceCleared(FriendsServiceEvents.FriendsServiceCleared evt)
		{
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<FriendData>>(onFriendComponentAdded);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<IncomingFriendInvitationData>>(onIncomingFriendInvitationComponentAdded);
			return false;
		}

		private bool onFriendComponentAdded(DataEntityEvents.ComponentAddedEvent<FriendData> evt)
		{
			notificationBreadcrumbController.AddBreadcrumb(friendAddedBreadcrumb);
			return false;
		}

		private bool onIncomingFriendInvitationComponentAdded(DataEntityEvents.ComponentAddedEvent<IncomingFriendInvitationData> evt)
		{
			evt.Component.OnAccepted += onIncomingFriendInvitationAccepted;
			evt.Component.OnRejected += onIncomingFriendInvitationRejected;
			notificationBreadcrumbController.AddBreadcrumb(friendRequestBreadcrumb);
			return false;
		}

		private void onIncomingFriendInvitationAccepted(string displayName, IFriend friend)
		{
			notificationBreadcrumbController.RemoveBreadcrumb(friendRequestBreadcrumb);
		}

		private void onIncomingFriendInvitationRejected(string displayName)
		{
			notificationBreadcrumbController.RemoveBreadcrumb(friendRequestBreadcrumb);
		}
	}
}
