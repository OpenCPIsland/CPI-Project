using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections.Generic;
using Tweaker.Core;

namespace ClubPenguin
{
	public class FriendsDataModelService : AbstractDataModelService, IFriendEvents
	{
		[Tweakable("UI.Friends.MaxFriendsCount")]
		public static int MaxFriendsCount = 200;

		public static List<DataEntityHandle> FriendsList
		{
			get
			{
				return new List<DataEntityHandle>(Service.Get<CPDataEntityCollection>().GetEntitiesByType<FriendData>());
			}
		}

		public static List<DataEntityHandle> IncomingInvitationsList
		{
			get
			{
				return new List<DataEntityHandle>(Service.Get<CPDataEntityCollection>().GetEntitiesByType<IncomingFriendInvitationData>());
			}
		}

		public static List<DataEntityHandle> OutgoingInvitationsList
		{
			get
			{
				return new List<DataEntityHandle>(Service.Get<CPDataEntityCollection>().GetEntitiesByType<OutgoingFriendInvitationData>());
			}
		}

		private void Awake()
		{
			if (Service.IsSet<INetworkServicesManager>())
			{
				Service.Get<INetworkServicesManager>().FriendsService.AddMixFriendEventsListener(this);
			}
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendsServiceReady>(onFriendsServiceReady);
		}

		private bool onFriendsServiceReady(FriendsServiceEvents.FriendsServiceReady evt)
		{
			Service.Get<INetworkServicesManager>().FriendsService.AddMixFriendEventsListener(this);
			return false;
		}

		public void OnFriendsListReady(List<IFriend> friends)
		{
			for (int i = 0; i < friends.Count; i++)
			{
				string text = friends[i].DisplayName.Text;
				if (text == null || text.Length <= 0)
				{
					Log.LogError(this, "DisplayName is empty for a Mix friend, will not add friend to list.");
					continue;
				}
				DataEntityHandle handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, friends[i].DisplayName.Text);
				addFriendStatusAndSwid(handle, friends[i]);
			}
		}

		public void OnIncomingInvitationsListReady(List<IIncomingFriendInvitation> incomingFriendInvitations)
		{
			for (int i = 0; i < incomingFriendInvitations.Count; i++)
			{
				string text = incomingFriendInvitations[i].Inviter.DisplayName.Text;
				if (text == null || text.Length <= 0)
				{
					Log.LogError(this, "DisplayName is empty for a Mix friend, will not add friend to list.");
					continue;
				}
				DataEntityHandle handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, incomingFriendInvitations[i].Inviter.DisplayName.Text);
				addIncomingInvitation(handle, incomingFriendInvitations[i]);
			}
		}

		public void OnOutgoingInvitationsListReady(List<IOutgoingFriendInvitation> outgoingFriendInvitations)
		{
			for (int i = 0; i < outgoingFriendInvitations.Count; i++)
			{
				string text = outgoingFriendInvitations[i].Invitee.DisplayName.Text;
				if (text == null || text.Length <= 0)
				{
					Log.LogError(this, "DisplayName is empty for a Mix friend, will not add friend to list.");
					continue;
				}
				DataEntityHandle handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, outgoingFriendInvitations[i].Invitee.DisplayName.Text);
				addOutgoingInvitation(handle, outgoingFriendInvitations[i]);
			}
		}

		public void OnFindUserSent(bool success, IUnidentifiedUser searchedUser)
		{
			if (!success)
			{
				return;
			}
			DataEntityHandle dataEntityHandle;
			if (!dataEntityCollection.TryFindEntity<DisplayNameData, string>(searchedUser.DisplayName.Text, out dataEntityHandle))
			{
				dataEntityHandle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, searchedUser.DisplayName.Text);
			}
			SearchedUserData component;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				DataEntityHandle entityByType = dataEntityCollection.GetEntityByType<SearchedUserData>();
				if (!entityByType.IsNull)
				{
					dataEntityCollection.RemoveComponent<SearchedUserData>(entityByType);
				}
				component = dataEntityCollection.AddComponent<SearchedUserData>(dataEntityHandle);
			}
			component.SearchedUser = searchedUser;
		}

		public void OnReceivedIncomingFriendInvitation(IIncomingFriendInvitation incomingFriendInvitation)
		{
			string text = incomingFriendInvitation.Inviter.DisplayName.Text;
			DataEntityHandle dataEntityHandle;
			if (!dataEntityCollection.TryFindEntity<DisplayNameData, string>(text, out dataEntityHandle))
			{
				dataEntityHandle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, text);
			}
			addIncomingInvitation(dataEntityHandle, incomingFriendInvitation);
			onIncomingInvitationsListUpdated();
		}

		public void OnReceivedOutgoingFriendInvitation(IOutgoingFriendInvitation outgoingFriendInvitation)
		{
			string text = outgoingFriendInvitation.Invitee.DisplayName.Text;
			DataEntityHandle dataEntityHandle;
			if (!dataEntityCollection.TryFindEntity<DisplayNameData, string>(text, out dataEntityHandle))
			{
				dataEntityHandle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, text);
			}
			addOutgoingInvitation(dataEntityHandle, outgoingFriendInvitation);
			onOutgoingInvitationsListUpdated();
		}

		public void OnUnfriended(IFriend exFriend)
		{
			removeFriendStatusAndSwid(exFriend);
			onFriendsListUpdated();
		}

		private DataEntityHandle addFriendStatusAndSwid(DataEntityHandle handle, IFriend friend)
		{
			if (!handle.IsNull)
			{
				FriendData friendData = dataEntityCollection.AddComponent<FriendData>(handle);
				friendData.Friend = friend;
				SwidData swidData = dataEntityCollection.AddComponent<SwidData>(handle);
				swidData.Swid = friend.Id;
				DisplayNameData component = dataEntityCollection.GetComponent<DisplayNameData>(handle);
				component.DisplayName = friend.DisplayName.Text;
				Service.Get<OtherPlayerDetailsRequestBatcher>().RequestOtherPlayerDetails(handle);
			}
			return handle;
		}

		private DataEntityHandle addIncomingInvitation(DataEntityHandle handle, IIncomingFriendInvitation invitation)
		{
			if (!handle.IsNull)
			{
				IncomingFriendInvitationData incomingFriendInvitationData = dataEntityCollection.AddComponent<IncomingFriendInvitationData>(handle);
				incomingFriendInvitationData.Init(invitation);
				incomingFriendInvitationData.OnAccepted += onIncomingFriendInvitationAccepted;
				incomingFriendInvitationData.OnRejected += onIncomingFriendInvitationRejected;
			}
			return handle;
		}

		private DataEntityHandle addOutgoingInvitation(DataEntityHandle handle, IOutgoingFriendInvitation invitation)
		{
			if (!handle.IsNull)
			{
				OutgoingFriendInvitationData outgoingFriendInvitationData = dataEntityCollection.AddComponent<OutgoingFriendInvitationData>(handle);
				outgoingFriendInvitationData.Init(invitation);
				outgoingFriendInvitationData.OnAccepted += onOutgoingFriendInvitationAccepted;
				outgoingFriendInvitationData.OnRejected += onOutgoingFriendInvitationRejected;
			}
			return handle;
		}

		private DataEntityHandle removeFriendStatusAndSwid(IFriend exFriend)
		{
			DataEntityHandle dataEntityHandle;
			if (dataEntityCollection.TryFindEntity<DisplayNameData, string>(exFriend.DisplayName.Text, out dataEntityHandle))
			{
				dataEntityCollection.RemoveComponent<FriendData>(dataEntityHandle);
				dataEntityCollection.RemoveComponent<SwidData>(dataEntityHandle);
			}
			return dataEntityHandle;
		}

		private DataEntityHandle removeIncomingInvitationComponent(string displayName)
		{
			DataEntityHandle dataEntityHandle;
			IncomingFriendInvitationData component;
			if (dataEntityCollection.TryFindEntity<DisplayNameData, string>(displayName, out dataEntityHandle) && dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				dataEntityCollection.RemoveComponent<IncomingFriendInvitationData>(dataEntityHandle);
			}
			return dataEntityHandle;
		}

		private DataEntityHandle removeOutgoingInvitationComponent(string displayName)
		{
			DataEntityHandle dataEntityHandle;
			OutgoingFriendInvitationData component;
			if (dataEntityCollection.TryFindEntity<DisplayNameData, string>(displayName, out dataEntityHandle) && dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				dataEntityCollection.RemoveComponent<OutgoingFriendInvitationData>(dataEntityHandle);
			}
			return dataEntityHandle;
		}

		private void onIncomingFriendInvitationAccepted(string displayName, IFriend friend)
		{
			DataEntityHandle handle = dataEntityCollection.FindEntity<DisplayNameData, string>(displayName);
			addFriendStatusAndSwid(handle, friend);
			removeIncomingInvitationComponent(displayName);
			onIncomingInvitationsListUpdated();
			onFriendsListUpdated();
		}

		private void onIncomingFriendInvitationRejected(string displayName)
		{
			removeIncomingInvitationComponent(displayName);
			onIncomingInvitationsListUpdated();
		}

		private void onOutgoingFriendInvitationAccepted(string displayName, IFriend friend)
		{
			DataEntityHandle handle = dataEntityCollection.FindEntity<DisplayNameData, string>(displayName);
			addFriendStatusAndSwid(handle, friend);
			removeOutgoingInvitationComponent(displayName);
			onFriendsListUpdated();
			onOutgoingInvitationsListUpdated();
		}

		private void onOutgoingFriendInvitationRejected(string displayName)
		{
			removeOutgoingInvitationComponent(displayName);
			onOutgoingInvitationsListUpdated();
		}

		public static FriendStatus GetFriendStatus(DataEntityHandle handle)
		{
			if (handle.IsNull)
			{
				return FriendStatus.None;
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (isLocalUser(handle, cPDataEntityCollection))
			{
				return FriendStatus.Self;
			}
			if (cPDataEntityCollection.HasComponent<FriendData>(handle))
			{
				return FriendStatus.Friend;
			}
			if (cPDataEntityCollection.HasComponent<IncomingFriendInvitationData>(handle))
			{
				return FriendStatus.IncomingInvite;
			}
			if (cPDataEntityCollection.HasComponent<OutgoingFriendInvitationData>(handle))
			{
				return FriendStatus.OutgoingInvite;
			}
			return FriendStatus.None;
		}

		public static void GetFriendIgloos(out List<DataEntityHandle> handles, out IList<ZoneId> zoneIds)
		{
			handles = new List<DataEntityHandle>();
			zoneIds = new List<ZoneId>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			List<DataEntityHandle> friendsList = FriendsList;
			for (int i = 0; i < friendsList.Count; i++)
			{
				DataEntityHandle dataEntityHandle = friendsList[i];
				ProfileData component;
				if (cPDataEntityCollection.TryGetComponent(friendsList[i], out component) && component.ZoneId != null && !component.ZoneId.isEmpty())
				{
					handles.Add(friendsList[i]);
					zoneIds.Add(component.ZoneId);
				}
			}
		}

		private static bool isLocalUser(DataEntityHandle handle, CPDataEntityCollection dataEntityCollection)
		{
			DisplayNameData component;
			DisplayNameData component2;
			if (dataEntityCollection.TryGetComponent(handle, out component) && dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2) && component.DisplayName == component2.DisplayName)
			{
				return true;
			}
			return false;
		}

		private void onFriendsListUpdated()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.FriendsListUpdated(FriendsList));
		}

		private void onIncomingInvitationsListUpdated()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.IncomingInvitationsListUpdated(IncomingInvitationsList));
		}

		private void onOutgoingInvitationsListUpdated()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.OutgoingInvitationsListUpdated(OutgoingInvitationsList));
		}
	}
}
