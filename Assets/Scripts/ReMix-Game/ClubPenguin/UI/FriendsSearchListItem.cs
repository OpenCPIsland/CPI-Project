using Disney.Kelowna.Common.DataModel;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FriendsSearchListItem : MonoBehaviour
	{
		public Text NameText;

		public GameObject MembershipIcon;

		public GameObject PreloadPanel;

		public GameObject AvatarIconContainer;

		public RawImage FriendAvatarIcon;

		private DataEntityHandle handle;

		public void SetPlayer(DataEntityHandle handle)
		{
			this.handle = handle;
		}

		public void SetName(string displayName)
		{
			NameText.text = displayName;
		}

		public void SetAvatarIcon(Texture2D icon)
		{
			FriendAvatarIcon.texture = icon;
			AvatarIconContainer.SetActive(true);
			PreloadPanel.SetActive(false);
		}

		public void ActivatePreloader()
		{
			AvatarIconContainer.SetActive(false);
			PreloadPanel.SetActive(true);
		}

		public void SetMembershipStatus(bool isMember)
		{
			MembershipIcon.SetActive(isMember);
		}

		public void PlayerCardButtonClicked()
		{
			OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(handle);
			openPlayerCardCommand.Execute();
		}
	}
}
