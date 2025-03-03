using Disney.Mix.SDK.Internal.MixDomain;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class NotificationValidator
	{
		private static bool Validate(BaseNotification notification)
		{
			return notification != null && notification.SequenceNumber.HasValue && notification.SequenceNumber != 0;
		}

		public static bool Validate(AddFriendshipInvitationNotification notification)
		{
			return Validate((BaseNotification)notification) && notification.Invitation != null && notification.Invitation.FriendDisplayName != null && notification.Invitation.IsInviter.HasValue && notification.Invitation.FriendshipInvitationId.HasValue && notification.Invitation.IsTrusted.HasValue;
		}

		public static bool Validate(AddFriendshipNotification notification)
		{
			return Validate((BaseNotification)notification) && notification.FriendshipInvitationId.HasValue && notification.Friend != null && notification.Friend.DisplayName != null && notification.Friend.UserId != null && notification.IsTrusted.HasValue;
		}

		public static bool Validate(RemoveFriendshipInvitationNotification notification)
		{
			return Validate((BaseNotification)notification) && notification.InvitationId.HasValue;
		}

		public static bool Validate(RemoveFriendshipNotification notification)
		{
			return Validate((BaseNotification)notification) && notification.FriendUserId != null;
		}

		public static bool Validate(RemoveFriendshipTrustNotification notification)
		{
			return Validate((BaseNotification)notification) && notification.FriendUserId != null;
		}

		public static bool Validate(AddAlertNotification notification)
		{
			return Validate((BaseNotification)notification) && notification.Alert != null && notification.Alert.Text != null && notification.Alert.AlertId.HasValue && notification.Alert.Level != null && notification.Alert.Timestamp.HasValue;
		}

		public static bool Validate(ClearAlertNotification notification)
		{
			return Validate((BaseNotification)notification) && notification.AlertIds != null && !notification.AlertIds.Any((long? id) => !id.HasValue);
		}
	}
}
