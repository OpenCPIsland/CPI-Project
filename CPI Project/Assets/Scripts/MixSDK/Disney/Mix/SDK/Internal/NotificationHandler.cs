using Disney.Mix.SDK.Internal.MixDomain;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class NotificationHandler
	{
		public static void Handle(INotificationDispatcher dispatcher, IUserDatabase userDatabase, IInternalLocalUser localUser, IEpochTime epochTime)
		{
			dispatcher.OnAlertAdded += delegate(object sender, AbstractAddAlertNotificationEventArgs e)
			{
				HandleAlertAdded(localUser, userDatabase, e);
			};
			dispatcher.OnAlertCleared += delegate(object sender, AbstractClearAlertNotificationEventArgs e)
			{
				HandleAlertCleared(localUser, userDatabase, e);
			};
			dispatcher.OnFriendshipRemoved += delegate(object sender, AbstractRemoveFriendshipNotificationEventArgs e)
			{
				HandleFriendshipRemoved(userDatabase, localUser, e);
			};
			dispatcher.OnFriendshipTrustRemoved += delegate(object sender, AbstractRemoveFriendshipTrustNotificationEventArgs e)
			{
				HandleFriendshipTrustRemoved(userDatabase, localUser, e);
			};
			dispatcher.OnFriendshipInvitationAdded += delegate(object sender, AbstractAddFriendshipInvitationNotificationEventArgs e)
			{
				HandleFriendshipInvitationAdded(userDatabase, localUser, e);
			};
			dispatcher.OnFriendshipInvitationRemoved += delegate(object sender, AbstractRemoveFriendshipInvitationNotificationEventArgs e)
			{
				HandleFriendshipInvitationRemoved(userDatabase, localUser, e);
			};
			dispatcher.OnFriendshipAdded += delegate(object sender, AbstractAddFriendshipNotificationEventArgs e)
			{
				HandleFriendshipAdded(userDatabase, localUser, e);
			};
		}

		private static void HandleAlertAdded(IInternalLocalUser localUser, IUserDatabase userDatabase, AbstractAddAlertNotificationEventArgs e)
		{
			Alert alert = new Alert(e.Notification.Alert);
			userDatabase.AddAlert(alert);
			localUser.DispatchOnAlertsAdded(alert);
		}

		private static void HandleAlertCleared(IInternalLocalUser localUser, IUserDatabase userDatabase, AbstractClearAlertNotificationEventArgs e)
		{
			IInternalAlert[] array = (from a in userDatabase.GetAlerts()
				where e.Notification.AlertIds.Contains(a.AlertId)
				select a).ToArray();
			IInternalAlert[] array2 = array;
			foreach (IInternalAlert internalAlert in array2)
			{
				userDatabase.RemoveAlert(internalAlert.AlertId);
			}
			localUser.DispatchOnAlertsCleared(array);
		}

		private static void HandleFriendshipAdded(IUserDatabase userDatabase, IInternalLocalUser localUser, AbstractAddFriendshipNotificationEventArgs e)
		{
			AddFriendshipNotification notification = e.Notification;
			User friend = notification.Friend;
			userDatabase.PersistUser(friend.UserId, friend.HashedUserId, friend.DisplayName, friend.FirstName, friend.Status);
			FriendDocument friendDocument = new FriendDocument();
			friendDocument.Swid = friend.UserId;
			friendDocument.IsTrusted = notification.IsTrusted.Value;
			friendDocument.Nickname = null;
			FriendDocument doc = friendDocument;
			userDatabase.InsertFriend(doc);
			long value = e.Notification.FriendshipInvitationId.Value;
			userDatabase.DeleteFriendInvitation(value);
			localUser.AddFriend(friend, notification.IsTrusted.Value, value);
		}

		private static void HandleFriendshipRemoved(IUserDatabase userDatabase, IInternalLocalUser localUser, AbstractRemoveFriendshipNotificationEventArgs e)
		{
			string swid = e.Notification.FriendUserId;
			string displayName = null;
			IInternalFriend internalFriend = localUser.InternalFriends.FirstOrDefault((IInternalFriend f) => f.Swid == swid);
			if (internalFriend != null)
			{
				displayName = internalFriend.DisplayName.Text;
			}
			else
			{
				UserDocument userBySwid = userDatabase.GetUserBySwid(swid);
				if (userBySwid != null)
				{
					displayName = userBySwid.DisplayName;
				}
			}
			userDatabase.DeleteFriend(swid);
			localUser.RemoveFriend(swid, true);
			if (displayName != null)
			{
				FriendInvitationDocument incomingInvitation = userDatabase.GetFriendInvitationDocuments(false).FirstOrDefault((FriendInvitationDocument doc) => doc.DisplayName == displayName);
				if (incomingInvitation != null)
				{
					foreach (IInternalIncomingFriendInvitation item in localUser.InternalIncomingFriendInvitations.Where((IInternalIncomingFriendInvitation i) => i.InvitationId == incomingInvitation.FriendInvitationId))
					{
						localUser.RemoveIncomingFriendInvitation(item);
					}
					userDatabase.DeleteFriendInvitation(incomingInvitation.FriendInvitationId);
				}
				FriendInvitationDocument outgoingInvitation = userDatabase.GetFriendInvitationDocuments(true).FirstOrDefault((FriendInvitationDocument doc) => doc.DisplayName == displayName);
				if (outgoingInvitation != null)
				{
					foreach (IInternalOutgoingFriendInvitation item2 in localUser.InternalOutgoingFriendInvitations.Where((IInternalOutgoingFriendInvitation i) => i.InvitationId == outgoingInvitation.FriendInvitationId))
					{
						localUser.RemoveOutgoingFriendInvitation(item2);
					}
					userDatabase.DeleteFriendInvitation(outgoingInvitation.FriendInvitationId);
				}
			}
		}

		private static void HandleFriendshipTrustRemoved(IUserDatabase userDatabase, IInternalLocalUser localUser, AbstractRemoveFriendshipTrustNotificationEventArgs e)
		{
			string friendUserId = e.Notification.FriendUserId;
			userDatabase.SetFriendIsTrusted(friendUserId, false);
			localUser.UntrustFriend(e.Notification.FriendUserId);
		}

		private static void HandleFriendshipInvitationAdded(IUserDatabase userDatabase, IInternalLocalUser localUser, AbstractAddFriendshipInvitationNotificationEventArgs e)
		{
			AddFriendshipInvitationNotification notification = e.Notification;
			FriendshipInvitation invitation = notification.Invitation;
			User friend = notification.Friend;
			userDatabase.PersistUser(null, null, invitation.FriendDisplayName, friend.FirstName, friend.Status);
			FriendInvitationDocument friendInvitationDocument = new FriendInvitationDocument();
			friendInvitationDocument.FriendInvitationId = invitation.FriendshipInvitationId.Value;
			friendInvitationDocument.IsInviter = invitation.IsInviter.Value;
			friendInvitationDocument.IsTrusted = invitation.IsTrusted.Value;
			friendInvitationDocument.DisplayName = invitation.FriendDisplayName;
			FriendInvitationDocument doc = friendInvitationDocument;
			userDatabase.InsertOrUpdateFriendInvitation(doc);
			localUser.AddFriendshipInvitation(invitation, friend);
		}

		private static void HandleFriendshipInvitationRemoved(IUserDatabase userDatabase, IInternalLocalUser localUser, AbstractRemoveFriendshipInvitationNotificationEventArgs e)
		{
			long value = e.Notification.InvitationId.Value;
			userDatabase.DeleteFriendInvitation(value);
			localUser.RemoveFriendshipInvitation(value);
		}
	}
}
