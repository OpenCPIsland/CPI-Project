using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public interface INotificationDispatcher
	{
		event EventHandler<AbstractAddFriendshipNotificationEventArgs> OnFriendshipAdded;

		event EventHandler<AbstractAddFriendshipInvitationNotificationEventArgs> OnFriendshipInvitationAdded;

		event EventHandler<AbstractRemoveFriendshipInvitationNotificationEventArgs> OnFriendshipInvitationRemoved;

		event EventHandler<AbstractRemoveFriendshipNotificationEventArgs> OnFriendshipRemoved;

		event EventHandler<AbstractRemoveFriendshipTrustNotificationEventArgs> OnFriendshipTrustRemoved;

		event EventHandler<AbstractAddAlertNotificationEventArgs> OnAlertAdded;

		event EventHandler<AbstractClearAlertNotificationEventArgs> OnAlertCleared;

		void Dispatch(BaseNotification notification);
	}
}
