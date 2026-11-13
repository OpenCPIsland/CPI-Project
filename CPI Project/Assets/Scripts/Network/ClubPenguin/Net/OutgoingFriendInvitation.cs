using Disney.Mix.SDK;
using System;

namespace ClubPenguin.Net
{
	public class OutgoingFriendInvitation
	{
		public Action<string, Friend> OnAccepted;

		public Action<string> OnRejected;

		public readonly IOutgoingFriendInvitation MixOutgoingFriendInvitation;

		public string DisplayName
		{
			get
			{
				return (MixOutgoingFriendInvitation != null) ? MixOutgoingFriendInvitation.Invitee.DisplayName.Text : null;
			}
		}

		public OutgoingFriendInvitation(IOutgoingFriendInvitation outgoingFriendInvitation)
		{
			MixOutgoingFriendInvitation = outgoingFriendInvitation;
			outgoingFriendInvitation.OnAccepted += onFriendAccepted;
			outgoingFriendInvitation.OnRejected += onFriendRejected;
		}

		private void onFriendAccepted(object sender, AbstractFriendInvitationAcceptedEventArgs args)
		{
			if (OnAccepted != null)
			{
				Friend arg = new Friend(args.Friend);
				OnAccepted(DisplayName, arg);
			}
		}

		private void onFriendRejected(object sender, AbstractFriendInvitationRejectedEventArgs args)
		{
			if (OnRejected != null)
			{
				OnRejected(DisplayName);
			}
		}

		public void Destroy()
		{
			MixOutgoingFriendInvitation.OnAccepted -= onFriendAccepted;
			MixOutgoingFriendInvitation.OnRejected -= onFriendRejected;
		}
	}
}
