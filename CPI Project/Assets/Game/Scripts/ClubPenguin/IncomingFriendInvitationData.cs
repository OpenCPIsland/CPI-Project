using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class IncomingFriendInvitationData : ScopedData
	{
		private IIncomingFriendInvitation invitation;

		public IIncomingFriendInvitation Invitation
		{
			get
			{
				return invitation;
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(IncomingFriendInvitationDataMonoBehaviour);
			}
		}

		public event Action<IncomingFriendInvitationData> OnInitialized;

		public event Action<string, IFriend> OnAccepted;

		public event Action<string> OnRejected;

		public void Init(IIncomingFriendInvitation invitation)
		{
			this.invitation = invitation;
			invitation.OnAccepted += onFriendAccepted;
			invitation.OnRejected += onFriendRejected;
			if (this.OnInitialized != null)
			{
				this.OnInitialized(this);
			}
		}

		private void removeListeners()
		{
			invitation.OnAccepted -= onFriendAccepted;
			invitation.OnRejected -= onFriendRejected;
			this.OnInitialized = null;
			this.OnAccepted = null;
			this.OnRejected = null;
		}

		private void onFriendAccepted(object sender, AbstractFriendInvitationAcceptedEventArgs eventArgs)
		{
			if (this.OnAccepted != null)
			{
				this.OnAccepted(invitation.Inviter.DisplayName.Text, eventArgs.Friend);
			}
		}

		private void onFriendRejected(object sender, AbstractFriendInvitationRejectedEventArgs eventArgs)
		{
			if (this.OnRejected != null)
			{
				this.OnRejected(invitation.Inviter.DisplayName.Text);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
			removeListeners();
		}
	}
}
