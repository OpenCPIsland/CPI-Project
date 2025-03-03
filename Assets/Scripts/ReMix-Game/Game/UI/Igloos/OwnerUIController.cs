using ClubPenguin;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace Game.UI.Igloos
{
	public class OwnerUIController : MonoBehaviour
	{
		private const string IDLE_CACHE_CONTEXT = "FriendsListIdle";

		private const string SLEEPING_CACHE_CONTEXT = "FriendsListSleeping";

		private const float IDLE_TIME = 0.5f;

		private const float SLEEPING_TIME = 0.75f;

		public FriendListItem ownerDisplay;

		public string IdlePenguinState;

		public string SleepingPenguinState;

		private DataEntityHandle ownerHandle;

		private DataEntityCollection dataEntityCollection;

		private DataEventListener ownerDataLoader;

		private AvatarImageComponent friendAvatarRenderer;

		private ProfileData myProfileData;

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
			friendAvatarRenderer = GetComponent<AvatarImageComponent>();
			AvatarImageComponent avatarImageComponent = friendAvatarRenderer;
			avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Combine(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			ownerDataLoader = dataEntityCollection.When<SceneOwnerData>("ActiveSceneData", showOwner);
		}

		private void OnEnable()
		{
			ItemImageBuilder.acquire();
		}

		private void OnDisable()
		{
			ItemImageBuilder.release();
		}

		private void OnDestroy()
		{
			if (dataEntityCollection != null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
			}
			if (friendAvatarRenderer != null)
			{
				AvatarImageComponent avatarImageComponent = friendAvatarRenderer;
				avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Remove(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			}
			if (ownerDataLoader != null)
			{
				ownerDataLoader.StopListening();
			}
			if (myProfileData != null)
			{
				myProfileData.ProfileDataUpdated -= onProfileDataUpdated;
			}
		}

		private void showOwner(SceneOwnerData ownerData)
		{
			ownerHandle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, ownerData.Name);
			ownerDisplay.SetPlayer(ownerHandle);
			ProfileData component;
			bool flag = dataEntityCollection.TryGetComponent(ownerHandle, out component);
			MembershipData component2;
			bool flag2 = dataEntityCollection.TryGetComponent(ownerHandle, out component2);
			if (!flag || !flag2)
			{
				Service.Get<OtherPlayerDetailsRequestBatcher>().RequestOtherPlayerDetails(ownerHandle);
			}
			if (flag)
			{
				bool isOnline = getIsOnline(ownerHandle);
				ownerDisplay.SetOnlineStatus(isOnline);
				renderOwner(ownerDisplay, ownerHandle, ownerData.Name, isOnline);
			}
			ownerDisplay.SetMembershipType(getMembershipType(ownerHandle));
		}

		private void renderOwner(FriendListItem friendListItem, DataEntityHandle handle, string displayName, bool isOnline)
		{
			if (!friendListItem.IsRendered || (friendListItem.IsOnlineSet && isOnline != friendListItem.IsOnline))
			{
				friendListItem.IsRendered = true;
				friendListItem.SetPreloaderActive(true);
				friendListItem.SetAvatarIconActive(false);
				if (friendAvatarRenderer.IsRenderInProgress(displayName))
				{
					friendAvatarRenderer.CancelRender(displayName);
				}
				if (isOnline)
				{
					AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame(IdlePenguinState, 0.5f);
					friendAvatarRenderer.RequestImage(handle, avatarAnimationFrame, "FriendsListIdle");
				}
				else
				{
					AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame(SleepingPenguinState, 0.75f);
					friendAvatarRenderer.RequestImage(handle, avatarAnimationFrame, "FriendsListSleeping");
				}
			}
		}

		private bool getIsOnline(DataEntityHandle handle)
		{
			ProfileData component = dataEntityCollection.GetComponent<ProfileData>(handle);
			return component != null && component.IsOnline;
		}

		private MembershipType getMembershipType(DataEntityHandle handle)
		{
			MembershipData component = dataEntityCollection.GetComponent<MembershipData>(handle);
			return (component != null) ? component.MembershipType : MembershipType.None;
		}

		private bool onProfileDataAdded(DataEntityEvents.ComponentAddedEvent<ProfileData> evt)
		{
			if (evt.Handle == ownerHandle)
			{
				evt.Component.ProfileDataUpdated += onProfileDataUpdated;
				myProfileData = evt.Component;
			}
			return false;
		}

		private void onProfileDataUpdated(ProfileData profileData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(profileData);
			if (entityByComponent == ownerHandle)
			{
				string displayName = dataEntityCollection.GetComponent<DisplayNameData>(entityByComponent).DisplayName;
				bool isOnline = getIsOnline(entityByComponent);
				ownerDisplay.SetOnlineStatus(isOnline);
				renderOwner(ownerDisplay, entityByComponent, displayName, isOnline);
			}
		}

		private bool onMembershipDataAdded(DataEntityEvents.ComponentAddedEvent<MembershipData> evt)
		{
			if (evt.Handle == ownerHandle)
			{
				evt.Component.MembershipDataUpdated += onMembershipDataUpdated;
			}
			return false;
		}

		private void onMembershipDataUpdated(MembershipData membershipData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(membershipData);
			if (entityByComponent == ownerHandle)
			{
				ownerDisplay.SetMembershipType(getMembershipType(entityByComponent));
			}
		}

		private void onImageReady(DataEntityHandle handle, Texture2D icon)
		{
			ownerDisplay.SetPreloaderActive(false);
			ownerDisplay.SetAvatarIcon(icon);
			ownerDisplay.SetAvatarIconActive(true);
		}
	}
}
