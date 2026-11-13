using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin
{
	public class FriendsNotificationService : AbstractDataModelService
	{
		private Localizer localizer;

		private TrayNotificationManager trayNotificationManager;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendsServiceInitialized>(onFriendsServiceInitialized);
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendsServiceCleared>(onFriendsServiceCleared);
			localizer = Service.Get<Localizer>();
			trayNotificationManager = Service.Get<TrayNotificationManager>();
		}

		private bool onFriendsServiceInitialized(FriendsServiceEvents.FriendsServiceInitialized evt)
		{
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.SendFriendInvitationSent>(onFriendInvitationSent);
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.AcceptFriendInvitationSent>(onAcceptFriendInvitationSent);
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.RejectFriendInvitationSent>(onRejectFriendInvitationSent);
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.UnfriendSent>(onUnfriendSent);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<IncomingFriendInvitationData>>(onIncomingFriendInvitationComponentAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<OutgoingFriendInvitationData>>(onOutgoingFriendInvitationComponentAdded);
			return false;
		}

		private bool onFriendsServiceCleared(FriendsServiceEvents.FriendsServiceCleared evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.SendFriendInvitationSent>(onFriendInvitationSent);
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.AcceptFriendInvitationSent>(onAcceptFriendInvitationSent);
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.RejectFriendInvitationSent>(onRejectFriendInvitationSent);
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.UnfriendSent>(onUnfriendSent);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<IncomingFriendInvitationData>>(onIncomingFriendInvitationComponentAdded);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<OutgoingFriendInvitationData>>(onOutgoingFriendInvitationComponentAdded);
			return false;
		}

		private bool onIncomingFriendInvitationComponentAdded(DataEntityEvents.ComponentAddedEvent<IncomingFriendInvitationData> evt)
		{
			evt.Component.OnInitialized += onIncomingFriendInvitationComponentInitialized;
			return false;
		}

		private void onIncomingFriendInvitationComponentInitialized(IncomingFriendInvitationData incomingFriendInvitationData)
		{
			incomingFriendInvitationData.OnInitialized -= onIncomingFriendInvitationComponentInitialized;
			DNotification dNotification = createNotification(true, "Friends.FriendsService.wantsfriend", incomingFriendInvitationData.Invitation.Inviter.DisplayName.Text);
			dNotification.DataPayload = incomingFriendInvitationData;
			TrayNotificationManager obj = trayNotificationManager;
			obj.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Combine(obj.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(onFriendInvitationNotificationDismissed));
			trayNotificationManager.ShowNotification(dNotification);
		}

		private bool onOutgoingFriendInvitationComponentAdded(DataEntityEvents.ComponentAddedEvent<OutgoingFriendInvitationData> evt)
		{
			evt.Component.OnAccepted += onOutgoingFriendInvitationAccepted;
			return false;
		}

		private void onOutgoingFriendInvitationAccepted(string displayName, IFriend friend)
		{
			DNotification notification = createNotification(false, "Friends.FriendsService.acceptedinvitation", displayName);
			trayNotificationManager.ShowNotification(notification);
		}

		private bool onFriendInvitationSent(FriendsServiceEvents.SendFriendInvitationSent evt)
		{
			string messageToken = evt.Success ? "Friends.FriendsService.invitationsent" : "Friends.FriendsService.invitationfailed";
			DNotification notification = createNotification(false, messageToken);
			Service.Get<TrayNotificationManager>().ShowNotification(notification);
			return false;
		}

		private bool onAcceptFriendInvitationSent(FriendsServiceEvents.AcceptFriendInvitationSent evt)
		{
			string messageToken = evt.Success ? "Friends.FriendsService.requestaccepted" : "Friends.FriendsService.actionfailed";
			DNotification notification = createNotification(false, messageToken);
			Service.Get<TrayNotificationManager>().ShowNotification(notification);
			return false;
		}

		private bool onRejectFriendInvitationSent(FriendsServiceEvents.RejectFriendInvitationSent evt)
		{
			string messageToken = evt.Success ? "Friends.FriendsService.requestrejected" : "Friends.FriendsService.actionfailed";
			DNotification notification = createNotification(false, messageToken);
			Service.Get<TrayNotificationManager>().ShowNotification(notification);
			return false;
		}

		private bool onUnfriendSent(FriendsServiceEvents.UnfriendSent evt)
		{
			string messageToken = evt.Success ? "Friends.FriendsService.removedsuccessful" : "Friends.FriendsService.removednotsuccessful";
			DNotification notification = createNotification(false, messageToken);
			Service.Get<TrayNotificationManager>().ShowNotification(notification);
			return false;
		}

		private void onFriendInvitationNotificationDismissed(NotificationCompleteEnum dismissAction, DNotification data)
		{
			TrayNotificationManager obj = trayNotificationManager;
			obj.NotificationDismissed = (Action<NotificationCompleteEnum, DNotification>)Delegate.Remove(obj.NotificationDismissed, new Action<NotificationCompleteEnum, DNotification>(onFriendInvitationNotificationDismissed));
			switch (dismissAction)
			{
			case NotificationCompleteEnum.acceptButton:
				if (FriendsDataModelService.FriendsList.Count < FriendsDataModelService.MaxFriendsCount)
				{
					IncomingFriendInvitationData incomingFriendInvitationData2 = (IncomingFriendInvitationData)data.DataPayload;
					Service.Get<INetworkServicesManager>().FriendsService.AcceptFriendInvitation(incomingFriendInvitationData2.Invitation, Service.Get<SessionManager>().LocalUser);
				}
				else
				{
					Service.Get<PromptManager>().ShowPrompt("MaximumFriendsPrompt", null);
				}
				break;
			case NotificationCompleteEnum.declineButton:
			{
				IncomingFriendInvitationData incomingFriendInvitationData = (IncomingFriendInvitationData)data.DataPayload;
				Service.Get<INetworkServicesManager>().FriendsService.RejectFriendInvitation(incomingFriendInvitationData.Invitation, Service.Get<SessionManager>().LocalUser);
				break;
			}
			}
		}

		private DNotification createNotification(bool containsButtons, string messageToken, params string[] args)
		{
			DNotification dNotification = new DNotification();
			dNotification.ContainsButtons = containsButtons;
			dNotification.PopUpDelayTime = 3f;
			dNotification.Message = string.Format(localizer.GetTokenTranslation(messageToken), args);
			return dNotification;
		}
	}
}
