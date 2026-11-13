using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class FriendRequestItem : MonoBehaviour
	{
		public Text NameText;

		public GameObject MembershipIcon;

		public SpriteSelector MembershipSpriteSelector;

		public GameObject PreloadPanel;

		public GameObject FriendAvatarIconContainer;

		public RawImage FriendAvatarIcon;

		private DataEntityHandle handle;

		public void SetPlayer(DataEntityHandle handle)
		{
			setButtonsActive(true);
			this.handle = handle;
		}

		public void SetName(string displayName)
		{
			NameText.text = displayName;
		}

		public void SetAvatarIcon(Texture2D icon)
		{
			FriendAvatarIcon.texture = icon;
		}

		public void SetAvatarIconActive(bool isActive)
		{
			FriendAvatarIconContainer.SetActive(isActive);
		}

		public void SetPreloaderActive(bool isActive)
		{
			PreloadPanel.SetActive(isActive);
		}

		public void SetMembershipType(MembershipType membershipType)
		{
			switch (membershipType)
			{
			case MembershipType.None:
				MembershipIcon.SetActive(false);
				break;
			case MembershipType.Member:
				MembershipSpriteSelector.SelectSprite(0);
				MembershipIcon.SetActive(true);
				break;
			case MembershipType.AllAccessEventMember:
				MembershipSpriteSelector.SelectSprite(1);
				MembershipIcon.SetActive(true);
				break;
			}
		}

		public void AcceptButtonClicked()
		{
			if (FriendsDataModelService.FriendsList.Count < FriendsDataModelService.MaxFriendsCount)
			{
				setButtonsActive(false);
				IncomingFriendInvitationData component = Service.Get<CPDataEntityCollection>().GetComponent<IncomingFriendInvitationData>(handle);
				Service.Get<INetworkServicesManager>().FriendsService.AcceptFriendInvitation(component.Invitation, Service.Get<SessionManager>().LocalUser);
			}
			else
			{
				Service.Get<PromptManager>().ShowPrompt("MaximumFriendsPrompt", null);
			}
		}

		public void DeclineButtonClicked()
		{
			setButtonsActive(false);
			IncomingFriendInvitationData component = Service.Get<CPDataEntityCollection>().GetComponent<IncomingFriendInvitationData>(handle);
			Service.Get<INetworkServicesManager>().FriendsService.RejectFriendInvitation(component.Invitation, Service.Get<SessionManager>().LocalUser);
		}

		public void PlayerCardButtonClicked()
		{
			OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(handle);
			openPlayerCardCommand.Execute();
		}

		private void setButtonsActive(bool isActive)
		{
			Button[] componentsInChildren = GetComponentsInChildren<Button>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].interactable = isActive;
			}
			OnOffSpriteSelector[] componentsInChildren2 = GetComponentsInChildren<OnOffSpriteSelector>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].IsOn = isActive;
			}
		}
	}
}
