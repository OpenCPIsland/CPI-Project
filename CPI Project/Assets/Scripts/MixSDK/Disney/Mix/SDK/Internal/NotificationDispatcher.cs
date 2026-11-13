using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class NotificationDispatcher : INotificationDispatcher
	{
		public event EventHandler<AbstractAddFriendshipNotificationEventArgs> OnFriendshipAdded = delegate
		{
		};

		public event EventHandler<AbstractAddFriendshipInvitationNotificationEventArgs> OnFriendshipInvitationAdded = delegate
		{
		};

		public event EventHandler<AbstractRemoveFriendshipInvitationNotificationEventArgs> OnFriendshipInvitationRemoved = delegate
		{
		};

		public event EventHandler<AbstractRemoveFriendshipNotificationEventArgs> OnFriendshipRemoved = delegate
		{
		};

		public event EventHandler<AbstractRemoveFriendshipTrustNotificationEventArgs> OnFriendshipTrustRemoved = delegate
		{
		};

		public event EventHandler<AbstractAddAlertNotificationEventArgs> OnAlertAdded = delegate
		{
		};

		public event EventHandler<AbstractClearAlertNotificationEventArgs> OnAlertCleared = delegate
		{
		};

		public void Dispatch(BaseNotification notification)
		{
			Action action = GetDispatchFunc<AddFriendshipInvitationNotification>(notification, Dispatch) ?? GetDispatchFunc<AddFriendshipNotification>(notification, Dispatch) ?? GetDispatchFunc<RemoveFriendshipInvitationNotification>(notification, Dispatch) ?? GetDispatchFunc<RemoveFriendshipNotification>(notification, Dispatch) ?? GetDispatchFunc<RemoveFriendshipTrustNotification>(notification, Dispatch) ?? GetDispatchFunc<AddAlertNotification>(notification, Dispatch) ?? GetDispatchFunc<ClearAlertNotification>(notification, Dispatch);
			action();
		}

		private static Action GetDispatchFunc<TNotification>(BaseNotification notification, Action<TNotification> dispatch) where TNotification : BaseNotification
		{
			TNotification castedNotification = notification as TNotification;
			return (castedNotification != null) ? ((Action)delegate
			{
				dispatch(castedNotification);
			}) : null;
		}

		private void Dispatch(AddFriendshipNotification notification)
		{
			AddFriendshipNotificationEventArgs e = new AddFriendshipNotificationEventArgs(notification);
			this.OnFriendshipAdded(this, e);
		}

		private void Dispatch(AddFriendshipInvitationNotification notification)
		{
			AddFriendshipInvitationNotificationEventArgs e = new AddFriendshipInvitationNotificationEventArgs(notification);
			this.OnFriendshipInvitationAdded(this, e);
		}

		private void Dispatch(RemoveFriendshipInvitationNotification notification)
		{
			RemoveFriendshipInvitationNotificationEventArgs e = new RemoveFriendshipInvitationNotificationEventArgs(notification);
			this.OnFriendshipInvitationRemoved(this, e);
		}

		private void Dispatch(RemoveFriendshipNotification notification)
		{
			RemoveFriendshipNotificationEventArgs e = new RemoveFriendshipNotificationEventArgs(notification);
			this.OnFriendshipRemoved(this, e);
		}

		private void Dispatch(RemoveFriendshipTrustNotification notification)
		{
			RemoveFriendshipTrustNotificationEventArgs e = new RemoveFriendshipTrustNotificationEventArgs(notification);
			this.OnFriendshipTrustRemoved(this, e);
		}

		private void Dispatch(AddAlertNotification notification)
		{
			AddAlertNotificationEventArgs e = new AddAlertNotificationEventArgs(notification);
			this.OnAlertAdded(this, e);
		}

		private void Dispatch(ClearAlertNotification notification)
		{
			ClearAlertNotificationEventArgs e = new ClearAlertNotificationEventArgs(notification);
			this.OnAlertCleared(this, e);
		}
	}
}
