using Disney.Mix.SDK;
using System;

namespace ClubPenguin.Net
{
	public class IncomingFriendInvitation
	{
		public Action<string, Friend> OnAccepted;

		public Action<string> OnRejected;

		public readonly IIncomingFriendInvitation MixIncomingFriendInvitation;

		public string DisplayName
		{
			get
			{
				return (MixIncomingFriendInvitation != null) ? MixIncomingFriendInvitation.Inviter.DisplayName.Text : null;
			}
		}

		public IncomingFriendInvitation(IIncomingFriendInvitation incomingFriendInvitation)
		{
			MixIncomingFriendInvitation = incomingFriendInvitation;
			incomingFriendInvitation.OnAccepted += onFriendAccepted;
			incomingFriendInvitation.OnRejected += onFriendRejected;
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
			MixIncomingFriendInvitation.OnAccepted -= onFriendAccepted;
			MixIncomingFriendInvitation.OnRejected -= onFriendRejected;
		}
	}
}
