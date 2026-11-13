using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(AvatarImageComponent))]
	public class FriendRequestsSubsetController : MonoBehaviour
	{
		private const string REQUESTS_HEADER_TOKEN = "Friends.FriendRequestContent.FriendRequestText";

		private const float IDLE_TIME = 0.5f;

		private const int SUBSET_SIZE = 4;

		public GameObject FriendsRequestsHeader;

		public Text TitleText;

		public Transform FriendRequestsContainer;

		public PrefabContentKey FriendRequestItemContentKey;

		public RuntimeAnimatorController PenguinAnimatorController;

		public string IdlePenguinState;

		private AvatarImageComponent friendAvatarRenderer;

		private DataEntityCollection dataEntityCollection;

		private List<DataEntityHandle> incomingFriendInvitationsList;

		private List<FriendRequestItem> friendRequestItems;

		private string screenTitle;

		private bool isInitialized;

		private List<MembershipData> membershipDataList;

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			membershipDataList = new List<MembershipData>();
			friendAvatarRenderer = GetComponent<AvatarImageComponent>();
			AvatarImageComponent avatarImageComponent = friendAvatarRenderer;
			avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Combine(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
			incomingFriendInvitationsList = FriendsDataModelService.IncomingInvitationsList;
			createIncomingFriendInvitations(incomingFriendInvitationsList);
			bool active = incomingFriendInvitationsList.Count > 0;
			FriendsRequestsHeader.SetActive(active);
			screenTitle = Service.Get<Localizer>().GetTokenTranslation("Friends.FriendRequestContent.FriendRequestText");
			TitleText.text = string.Format(screenTitle, incomingFriendInvitationsList.Count);
			isInitialized = true;
		}

		private void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.IncomingInvitationsListUpdated>(onIncomingInvitationsListUpdated);
			if (isInitialized)
			{
				updateIncomingInvitationList(FriendsDataModelService.IncomingInvitationsList);
			}
		}

		private void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.IncomingInvitationsListUpdated>(onIncomingInvitationsListUpdated);
		}

		public void Show()
		{
			bool active = incomingFriendInvitationsList.Count > 0;
			FriendsRequestsHeader.SetActive(active);
			FriendRequestsContainer.gameObject.SetActive(true);
		}

		public void Hide()
		{
			FriendsRequestsHeader.SetActive(false);
			FriendRequestsContainer.gameObject.SetActive(false);
		}

		private void createIncomingFriendInvitations(List<DataEntityHandle> invitationsList)
		{
			sortInvitations(invitationsList);
			Content.LoadAsync(onFriendRequestItemPrefabLoaded, FriendRequestItemContentKey);
		}

		private void onFriendRequestItemPrefabLoaded(string path, GameObject friendRequestItemPrefab)
		{
			showFriendRequests(true, friendRequestItemPrefab);
		}

		private void showFriendRequests(bool isSettingUp, GameObject friendRequestItemPrefab = null)
		{
			bool active = incomingFriendInvitationsList.Count > 0;
			FriendsRequestsHeader.SetActive(active);
			if (isSettingUp)
			{
				friendRequestItems = new List<FriendRequestItem>();
			}
			for (int i = 0; i < 4; i++)
			{
				FriendRequestItem friendRequestItem;
				if (isSettingUp)
				{
					friendRequestItem = UnityEngine.Object.Instantiate(friendRequestItemPrefab).GetComponent<FriendRequestItem>();
					friendRequestItem.transform.SetParent(FriendRequestsContainer, false);
					friendRequestItems.Add(friendRequestItem);
				}
				else
				{
					friendRequestItem = friendRequestItems[i];
				}
				if (incomingFriendInvitationsList.Count > i)
				{
					friendRequestItem.gameObject.SetActive(true);
					DataEntityHandle handle = incomingFriendInvitationsList[i];
					populateRequestPrefab(friendRequestItem, handle);
					DynamicMulticoloredListElement component = friendRequestItem.GetComponent<DynamicMulticoloredListElement>();
					component.SetIndex(i);
				}
				else
				{
					friendRequestItem.gameObject.SetActive(false);
				}
			}
		}

		private void populateRequestPrefab(FriendRequestItem friendRequestItem, DataEntityHandle handle)
		{
			string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName;
			friendRequestItem.SetPlayer(handle);
			friendRequestItem.SetName(displayName);
			friendRequestItem.SetPreloaderActive(true);
			friendRequestItem.SetAvatarIconActive(false);
			friendRequestItem.SetMembershipType(getMembershipType(handle));
			AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame(IdlePenguinState, 0.5f);
			friendAvatarRenderer.RequestImage(handle, avatarAnimationFrame);
		}

		private bool onMembershipDataAdded(DataEntityEvents.ComponentAddedEvent<MembershipData> evt)
		{
			int incomingInvitationIndex = getIncomingInvitationIndex(evt.Handle);
			if (incomingInvitationIndex >= 0)
			{
				evt.Component.MembershipDataUpdated += onMembershipDataUpdated;
				membershipDataList.Add(evt.Component);
			}
			return false;
		}

		private void onMembershipDataUpdated(MembershipData membershipData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(membershipData);
			int incomingInvitationIndex = getIncomingInvitationIndex(entityByComponent);
			if (incomingInvitationIndex >= 0 && incomingInvitationIndex < 4 && friendRequestItems != null && friendRequestItems[incomingInvitationIndex] != null)
			{
				friendRequestItems[incomingInvitationIndex].SetMembershipType(getMembershipType(entityByComponent));
			}
		}

		private void onImageReady(DataEntityHandle handle, Texture2D icon)
		{
			int incomingInvitationIndex = getIncomingInvitationIndex(handle);
			if (incomingInvitationIndex >= 0 && incomingInvitationIndex < 4)
			{
				FriendRequestItem friendRequestItem = friendRequestItems[incomingInvitationIndex];
				friendRequestItem.SetPreloaderActive(false);
				friendRequestItem.FriendAvatarIcon.texture = icon;
				friendRequestItem.SetAvatarIconActive(true);
			}
		}

		private int getIncomingInvitationIndex(DataEntityHandle handle)
		{
			return incomingFriendInvitationsList.FindIndex((DataEntityHandle x) => !x.IsNull && !handle.IsNull && dataEntityCollection.GetComponent<DisplayNameData>(x).DisplayName == dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName);
		}

		private bool onIncomingInvitationsListUpdated(FriendsServiceEvents.IncomingInvitationsListUpdated evt)
		{
			updateIncomingInvitationList(evt.IncomingInvitationsList);
			return false;
		}

		private void updateIncomingInvitationList(List<DataEntityHandle> updatedFriendInvitations)
		{
			int count = updatedFriendInvitations.Count;
			incomingFriendInvitationsList = updatedFriendInvitations;
			sortInvitations(incomingFriendInvitationsList);
			showFriendRequests(false);
			TitleText.text = string.Format(screenTitle, count);
		}

		private MembershipType getMembershipType(DataEntityHandle handle)
		{
			MembershipData component = dataEntityCollection.GetComponent<MembershipData>(handle);
			return (component != null) ? component.MembershipType : MembershipType.None;
		}

		private void sortInvitations(List<DataEntityHandle> listToSort)
		{
			listToSort.Sort((DataEntityHandle a, DataEntityHandle b) => string.Compare(dataEntityCollection.GetComponent<DisplayNameData>(a).DisplayName, dataEntityCollection.GetComponent<DisplayNameData>(b).DisplayName));
		}

		private void OnDestroy()
		{
			if (dataEntityCollection != null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
			}
			if (friendAvatarRenderer != null)
			{
				AvatarImageComponent avatarImageComponent = friendAvatarRenderer;
				avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Remove(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			}
			if (membershipDataList != null)
			{
				for (int i = 0; i < membershipDataList.Count; i++)
				{
					membershipDataList[i].MembershipDataUpdated -= onMembershipDataUpdated;
				}
			}
		}
	}
}
