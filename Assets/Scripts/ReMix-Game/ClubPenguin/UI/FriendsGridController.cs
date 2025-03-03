using ClubPenguin.CellPhone;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class FriendsGridController : AbstractPlayerListController
	{
		public FriendsScreenStateHandler StateHandler;

		private List<DataEntityHandle> outgoingInvitationsList;

		private List<DataEntityHandle> visibleOutgoingInvitationsList;

		private List<DataEntityHandle> tempVisibleFriendsList;

		private List<DataEntityHandle> tempVisibleOutgoingInvitationsList;

		private FriendsSearchFriendFilter friendsSearchFriendFilter;

		private string searchString;

		private bool isAddFriendButtonVisible;

		protected override void start()
		{
			isAddFriendButtonVisible = true;
			friendsSearchFriendFilter = new FriendsSearchFriendFilter();
			outgoingInvitationsList = FriendsDataModelService.OutgoingInvitationsList;
			sortPlayers(outgoingInvitationsList);
			visibleOutgoingInvitationsList = outgoingInvitationsList;
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAdded);
			List<string> outgoingInvitationDisplayNames = getOutgoingInvitationDisplayNames();
			if (outgoingInvitationDisplayNames.Count > 0)
			{
				Service.Get<INetworkServicesManager>().PlayerStateService.GetOtherPlayerDataByDisplayNames(outgoingInvitationDisplayNames);
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.HideLoadingScreen));
		}

		protected override void onEnable()
		{
			eventChannel.AddListener<FriendsServiceEvents.FriendsListUpdated>(onFriendsListUpdated);
			eventChannel.AddListener<FriendsServiceEvents.OutgoingInvitationsListUpdated>(onOutgoingInvitationsListUpdated);
			eventChannel.AddListener<FriendsScreenEvents.SearchStringUpdated>(onSearchStringUpdated);
			if (isInitialized)
			{
				updateFriendsList(FriendsDataModelService.FriendsList);
			}
		}

		protected override void initPlayersList()
		{
			allPlayersList = FriendsDataModelService.FriendsList;
			sortPlayers(allPlayersList);
			visiblePlayersList = allPlayersList;
			setFriendCount(allPlayersList.Count);
		}

		public int TotalFriendsDisplayed()
		{
			return visiblePlayersList.Count + visibleOutgoingInvitationsList.Count;
		}

		private List<string> getOutgoingInvitationDisplayNames()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < outgoingInvitationsList.Count; i++)
			{
				DisplayNameData component;
				if (dataEntityCollection.TryGetComponent(outgoingInvitationsList[i], out component) && !string.IsNullOrEmpty(component.DisplayName))
				{
					list.Add(component.DisplayName);
				}
			}
			return list;
		}

		protected override bool onProfileDataAdded(DataEntityEvents.ComponentAddedEvent<ProfileData> evt)
		{
			int indexFromList = getIndexFromList(allPlayersList, evt.Handle);
			int indexFromList2 = getIndexFromList(outgoingInvitationsList, evt.Handle);
			if (indexFromList >= 0 || indexFromList2 >= 0)
			{
				evt.Component.ProfileDataUpdated += onProfileDataUpdated;
				profileDataList.Add(evt.Component);
			}
			return false;
		}

		protected override bool onMembershipDataAdded(DataEntityEvents.ComponentAddedEvent<MembershipData> evt)
		{
			int indexFromList = getIndexFromList(allPlayersList, evt.Handle);
			int indexFromList2 = getIndexFromList(outgoingInvitationsList, evt.Handle);
			if (indexFromList >= 0 || indexFromList2 >= 0)
			{
				evt.Component.MembershipDataUpdated += onMembershipDataUpdated;
				membershipDataList.Add(evt.Component);
			}
			return false;
		}

		protected override void setUpObject(RectTransform item, int poolIndex)
		{
			FriendListItem component = item.GetComponent<FriendListItem>();
			bool flag = !isAddFriendButtonVisible || poolIndex > 0;
			if (flag)
			{
				DataEntityHandle handleFromPoolIndex = getHandleFromPoolIndex(poolIndex);
				base.setUpObject(item, poolIndex);
				component.SetMembershipType(getMembershipType(handleFromPoolIndex));
				bool isPending = getListIndex(poolIndex) >= visiblePlayersList.Count;
				component.SetIsPending(isPending);
			}
			component.SetIsFriend(flag);
		}

		protected override void onObjectRemoved(RectTransform item, int poolIndex)
		{
			AbstractPlayerListItem component = item.GetComponent<AbstractPlayerListItem>();
			component.Reset();
			if (isAddFriendButtonVisible && poolIndex <= 0)
			{
				return;
			}
			DataEntityHandle handleFromPoolIndex = getHandleFromPoolIndex(poolIndex);
			if (!handleFromPoolIndex.IsNull)
			{
				string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handleFromPoolIndex).DisplayName;
				if (playerAvatarRenderer.IsRenderInProgress(displayName))
				{
					playerAvatarRenderer.CancelRender(displayName);
				}
			}
		}

		private void setFriendCount(int friendCount)
		{
			StateHandler.SetFriendCount(friendCount);
		}

		private bool onFriendsListUpdated(FriendsServiceEvents.FriendsListUpdated evt)
		{
			if (pooledScrollRect != null && pooledScrollRect.IsInitialized)
			{
				updateFriendsList(evt.FriendsList);
			}
			return false;
		}

		private void updateFriendsList(List<DataEntityHandle> updatedFriendsList)
		{
			allPlayersList = updatedFriendsList;
			sortPlayers(allPlayersList);
			tempVisibleFriendsList = getVisiblePlayersList(allPlayersList);
			pooledScrollRect.RefreshList(getPoolCount(tempVisibleFriendsList, visibleOutgoingInvitationsList));
			setFriendCount(allPlayersList.Count);
		}

		private bool onOutgoingInvitationsListUpdated(FriendsServiceEvents.OutgoingInvitationsListUpdated evt)
		{
			updateOutgoingInvitationsList(evt.OutgoingInvitationsList);
			return false;
		}

		private void updateOutgoingInvitationsList(List<DataEntityHandle> updatedOutgoingInvitationsList)
		{
			outgoingInvitationsList = updatedOutgoingInvitationsList;
			sortPlayers(outgoingInvitationsList);
			tempVisibleOutgoingInvitationsList = getVisiblePlayersList(outgoingInvitationsList);
			pooledScrollRect.RefreshList(getPoolCount(visiblePlayersList, tempVisibleOutgoingInvitationsList));
		}

		private bool onSearchStringUpdated(FriendsScreenEvents.SearchStringUpdated evt)
		{
			int num = string.IsNullOrEmpty(evt.SearchString) ? 1 : 0;
			searchString = evt.SearchString;
			tempVisibleFriendsList = getVisiblePlayersList(allPlayersList);
			tempVisibleOutgoingInvitationsList = getVisiblePlayersList(outgoingInvitationsList);
			pooledScrollRect.RefreshList(num + tempVisibleFriendsList.Count + tempVisibleOutgoingInvitationsList.Count);
			return false;
		}

		protected override void onRefreshCompleted()
		{
			isAddFriendButtonVisible = string.IsNullOrEmpty(searchString);
			if (tempVisibleFriendsList != null)
			{
				visiblePlayersList = tempVisibleFriendsList;
			}
			if (tempVisibleOutgoingInvitationsList != null)
			{
				visibleOutgoingInvitationsList = tempVisibleOutgoingInvitationsList;
			}
			tempVisibleFriendsList = null;
			tempVisibleOutgoingInvitationsList = null;
		}

		private List<DataEntityHandle> getVisiblePlayersList(List<DataEntityHandle> players)
		{
			if (!string.IsNullOrEmpty(searchString))
			{
				List<DataEntityHandle> matchingFriends = getMatchingFriends(searchString, players);
				sortPlayers(matchingFriends);
				return matchingFriends;
			}
			return players;
		}

		private List<DataEntityHandle> getMatchingFriends(string substring, List<DataEntityHandle> friendsList)
		{
			List<string> list = new List<string>();
			Dictionary<string, DataEntityHandle> dictionary = new Dictionary<string, DataEntityHandle>();
			for (int i = 0; i < friendsList.Count; i++)
			{
				DisplayNameData component;
				if (dataEntityCollection.TryGetComponent(friendsList[i], out component) && component != null && !string.IsNullOrEmpty(component.DisplayName))
				{
					list.Add(component.DisplayName);
					dictionary.Add(component.DisplayName, friendsList[i]);
				}
			}
			List<string> matches = friendsSearchFriendFilter.GetMatches(substring, list);
			List<DataEntityHandle> list2 = new List<DataEntityHandle>();
			for (int i = 0; i < matches.Count; i++)
			{
				list2.Add(dictionary[matches[i]]);
			}
			return list2;
		}

		protected override void sortPlayers(List<DataEntityHandle> listToSort)
		{
			listToSort.Sort(delegate(DataEntityHandle a, DataEntityHandle b)
			{
				bool isOnline = getIsOnline(a);
				bool isOnline2 = getIsOnline(b);
				if (isOnline == isOnline2)
				{
					return string.Compare(dataEntityCollection.GetComponent<DisplayNameData>(a).DisplayName, dataEntityCollection.GetComponent<DisplayNameData>(b).DisplayName);
				}
				return (!isOnline || isOnline2) ? 1 : (-1);
			});
		}

		protected override DataEntityHandle getHandleFromPoolIndex(int poolIndex)
		{
			int listIndex = getListIndex(poolIndex);
			if (listIndex < visiblePlayersList.Count)
			{
				return visiblePlayersList[listIndex];
			}
			if (listIndex < visiblePlayersList.Count + visibleOutgoingInvitationsList.Count)
			{
				return visibleOutgoingInvitationsList[listIndex - visiblePlayersList.Count];
			}
			throw new IndexOutOfRangeException(string.Format("The index is beyond the combined ranges of the friends and outgoing invitations lists. poolIndex = {0}, List Sizes: friends = {1}, invitations = {2}, list index = {3}.", poolIndex, visiblePlayersList.Count, visibleOutgoingInvitationsList.Count, listIndex));
		}

		protected override int getPoolIndexFromHandle(DataEntityHandle handle)
		{
			int result = -1;
			int indexFromList = getIndexFromList(visiblePlayersList, handle);
			if (indexFromList >= 0)
			{
				result = getPoolIndex(indexFromList);
			}
			else
			{
				int indexFromList2 = getIndexFromList(visibleOutgoingInvitationsList, handle);
				if (indexFromList2 >= 0)
				{
					result = getPoolIndex(indexFromList2);
					result += visiblePlayersList.Count;
				}
			}
			return result;
		}

		private int getPoolIndex(int listIndex)
		{
			return isAddFriendButtonVisible ? (listIndex + 1) : listIndex;
		}

		private int getListIndex(int poolIndex)
		{
			return isAddFriendButtonVisible ? (poolIndex - 1) : poolIndex;
		}

		protected override int getPoolCount()
		{
			return getPoolCount(visiblePlayersList, visibleOutgoingInvitationsList);
		}

		private int getPoolCount(List<DataEntityHandle> targetFriendsList, List<DataEntityHandle> targetOutgoingInvitationsList)
		{
			int num = targetFriendsList.Count + targetOutgoingInvitationsList.Count;
			return isAddFriendButtonVisible ? (num + 1) : num;
		}

		private bool getIsOnline(DataEntityHandle handle)
		{
			ProfileData component = dataEntityCollection.GetComponent<ProfileData>(handle);
			return component != null && component.IsOnline;
		}

		protected override void onDestroy()
		{
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
