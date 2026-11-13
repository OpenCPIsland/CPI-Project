using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public abstract class AbstractPlayerListItem : MonoBehaviour
	{
		public Text NameText;

		public GameObject PreloadPanel;

		public GameObject FriendAvatarIconContainer;

		public RawImage FriendAvatarIcon;

		public GameObject OfflineIcon;

		public GameObject MembershipBadgeIcon;

		public SpriteSelector MembershipSpriteSelector;

		protected DataEntityHandle handle;

		protected ProfileData profileData;

		private DataEventListener profileDataListener;

		private CPDataEntityCollection dataEntityCollection;

		public bool IsRendered
		{
			get;
			set;
		}

		public bool IsOnlineSet
		{
			get;
			private set;
		}

		public bool IsOnline
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				return NameText.text;
			}
			set
			{
				NameText.text = value;
			}
		}

		private void OnDestroy()
		{
			if (profileDataListener != null)
			{
				profileDataListener.StopListening();
			}
			onDestroy();
		}

		protected virtual void onDestroy()
		{
		}

		public void SetOnlineStatus(bool isOnline)
		{
			IsOnlineSet = true;
			IsOnline = isOnline;
			OfflineIcon.SetActive(!isOnline);
		}

		public virtual void SetPlayer(DataEntityHandle handle)
		{
			this.handle = handle;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			profileDataListener = dataEntityCollection.When<ProfileData>(handle, onProfileDataAdded);
		}

		public void SetMembershipType(MembershipType membershipType)
		{
			if (MembershipBadgeIcon != null && MembershipSpriteSelector != null)
			{
				switch (membershipType)
				{
				case MembershipType.None:
					MembershipBadgeIcon.SetActive(false);
					break;
				case MembershipType.Member:
					MembershipSpriteSelector.SelectSprite(0);
					MembershipBadgeIcon.SetActive(true);
					break;
				case MembershipType.AllAccessEventMember:
					MembershipSpriteSelector.SelectSprite(1);
					MembershipBadgeIcon.SetActive(true);
					break;
				}
			}
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

		private void onProfileDataAdded(ProfileData profileData)
		{
			this.profileData = profileData;
			UpdateProfileData(profileData);
		}

		public virtual void UpdateProfileData(ProfileData profileData)
		{
		}

		public abstract void OnPressed();

		public virtual void Reset()
		{
			IsRendered = false;
			PreloadPanel.SetActive(true);
			FriendAvatarIconContainer.SetActive(false);
			FriendAvatarIcon.texture = null;
			MembershipBadgeIcon.SetActive(false);
			OfflineIcon.SetActive(false);
		}
	}
}
