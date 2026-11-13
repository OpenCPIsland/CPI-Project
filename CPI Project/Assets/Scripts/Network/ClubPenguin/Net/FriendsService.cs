using ClubPenguin.Net.Client;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class FriendsService : BaseNetworkService, IFriendsService, INetworkService
	{
		private List<IFriendEvents> mixFriendEventsListeners;

		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PLAYER_LOCATION_RECEIVED, onPlayerLocationReceived);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PLAYER_NOT_FOUND, onPlayerNotFound);
			Service.Get<EventDispatcher>().DispatchEvent(default(FriendsServiceEvents.FriendsServiceReady));
		}

		public void SetLocalUser(ILocalUser localUser)
		{
			localUser.OnReceivedIncomingFriendInvitation -= onReceivedIncomingFriendInvitation;
			localUser.OnReceivedOutgoingFriendInvitation -= onReceivedOutgoingFriendInvitation;
			localUser.OnReceivedIncomingFriendInvitation += onReceivedIncomingFriendInvitation;
			localUser.OnReceivedOutgoingFriendInvitation += onReceivedOutgoingFriendInvitation;
			localUser.OnUnfriended += onUnfriended;
			List<IFriend> list = new List<IFriend>();
			List<IIncomingFriendInvitation> list2 = new List<IIncomingFriendInvitation>();
			List<IOutgoingFriendInvitation> list3 = new List<IOutgoingFriendInvitation>();
			list.AddRange(localUser.Friends);
			list2.AddRange(localUser.IncomingFriendInvitations);
			list3.AddRange(localUser.OutgoingFriendInvitations);
			for (int i = 0; i < mixFriendEventsListeners.Count; i++)
			{
				mixFriendEventsListeners[i].OnFriendsListReady(list);
				mixFriendEventsListeners[i].OnIncomingInvitationsListReady(list2);
				mixFriendEventsListeners[i].OnOutgoingInvitationsListReady(list3);
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(FriendsServiceEvents.FriendsServiceInitialized));
		}

		public void ClearLocalUser(ILocalUser localUser)
		{
			if (localUser != null)
			{
				localUser.OnReceivedIncomingFriendInvitation -= onReceivedIncomingFriendInvitation;
				localUser.OnReceivedOutgoingFriendInvitation -= onReceivedOutgoingFriendInvitation;
				localUser.OnUnfriended -= onUnfriended;
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(FriendsServiceEvents.FriendsServiceCleared));
		}

		public void AddMixFriendEventsListener(IFriendEvents mixFriendEvents)
		{
			if (mixFriendEventsListeners == null)
			{
				mixFriendEventsListeners = new List<IFriendEvents>();
			}
			mixFriendEventsListeners.Add(mixFriendEvents);
		}

		private void onReceivedIncomingFriendInvitation(object sender, AbstractReceivedIncomingFriendInvitationEventArgs args)
		{
			for (int i = 0; i < mixFriendEventsListeners.Count; i++)
			{
				mixFriendEventsListeners[i].OnReceivedIncomingFriendInvitation(args.Invitation);
			}
		}

		private void onReceivedOutgoingFriendInvitation(object sender, AbstractReceivedOutgoingFriendInvitationEventArgs args)
		{
			for (int i = 0; i < mixFriendEventsListeners.Count; i++)
			{
				mixFriendEventsListeners[i].OnReceivedOutgoingFriendInvitation(args.Invitation);
			}
		}

		private void onUnfriended(object sender, AbstractUnfriendedEventArgs args)
		{
			for (int i = 0; i < mixFriendEventsListeners.Count; i++)
			{
				mixFriendEventsListeners[i].OnUnfriended(args.ExFriend);
			}
		}

		private void onPlayerLocationReceived(GameServerEvent gameServerEvent, object data)
		{
			Vector3 location = (Vector3)data;
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.FriendLocationInRoomReceived(location));
		}

		private void onPlayerNotFound(GameServerEvent gameServerEvent, object data)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(FriendsServiceEvents.FriendNotInRoom));
		}

		public void GetFriendLocationInRoom(string swid)
		{
			clubPenguinClient.GameServer.GetPlayerLocation(swid);
		}

		public void FindUser(string displayName, ILocalUser localUser)
		{
			if (string.IsNullOrEmpty(displayName))
			{
				throw new ArgumentNullException("displayName");
			}
			localUser.FindUser(displayName, onFindUserSent);
		}

		public void SendFriendInvitation(IUnidentifiedUser searchedUser, ILocalUser localUser)
		{
			if (searchedUser == null)
			{
				throw new ArgumentNullException("searchedUser");
			}
			localUser.SendFriendInvitation(searchedUser, false, delegate(ISendFriendInvitationResult result)
			{
				onSendFriendInvitationSent(result, searchedUser.DisplayName.Text);
			});
		}

		public void AcceptFriendInvitation(IIncomingFriendInvitation incomingFriendInvitation, ILocalUser localUser)
		{
			if (incomingFriendInvitation == null)
			{
				throw new ArgumentNullException("incomingFriendInvitation");
			}
			localUser.AcceptFriendInvitation(incomingFriendInvitation, false, delegate(IAcceptFriendInvitationResult result)
			{
				onAcceptFriendInvitationSent(result, incomingFriendInvitation.Inviter.DisplayName.Text);
			});
		}

		public void RejectFriendInvitation(IIncomingFriendInvitation incomingFriendInvitation, ILocalUser localUser)
		{
			if (incomingFriendInvitation == null)
			{
				throw new ArgumentNullException("incomingFriendInvitation");
			}
			localUser.RejectFriendInvitation(incomingFriendInvitation, delegate(IRejectFriendInvitationResult result)
			{
				onRejectFriendInvitationSent(result, incomingFriendInvitation.Inviter.DisplayName.Text);
			});
		}

		public void Unfriend(IFriend friend, ILocalUser localUser)
		{
			if (friend == null)
			{
				throw new ArgumentNullException("friend");
			}
			localUser.Unfriend(friend, delegate(IUnfriendResult result)
			{
				onUnfriendSent(result, friend.DisplayName.Text);
			});
		}

		private void onFindUserSent(IFindUserResult result)
		{
			for (int i = 0; i < mixFriendEventsListeners.Count; i++)
			{
				mixFriendEventsListeners[i].OnFindUserSent(result.Success, result.User);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.FindUserSent(result.Success));
		}

		private void onSendFriendInvitationSent(ISendFriendInvitationResult result, string friendName)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.SendFriendInvitationSent(result.Success, friendName));
		}

		private void onAcceptFriendInvitationSent(IAcceptFriendInvitationResult result, string friendName)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.AcceptFriendInvitationSent(result.Success, friendName));
		}

		private void onRejectFriendInvitationSent(IRejectFriendInvitationResult result, string friendName)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.RejectFriendInvitationSent(result.Success, friendName));
		}

		private void onUnfriendSent(IUnfriendResult result, string friendName)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsServiceEvents.UnfriendSent(result.Success, friendName));
		}
	}
}
