using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class GetStateResponseParser : IGetStateResponseParser
	{
		private readonly AbstractLogger logger;

		public GetStateResponseParser(AbstractLogger logger)
		{
			this.logger = logger;
		}

		public IList<IInternalFriend> ParseFriendships(GetStateResponse response, IUserDatabase userDatabase)
		{
			List<IInternalFriend> list = new List<IInternalFriend>();
			List<User> users = response.Users;
			if (response.Friendships != null)
			{
				foreach (Friendship friendship in response.Friendships)
				{
					string userId = friendship.FriendUserId;
					bool value = friendship.IsTrusted.Value;
					string displayNameText = string.Empty;
					string firstName = string.Empty;
					if (users != null)
					{
						User user = users.FirstOrDefault((User u) => u.UserId == userId);
						if (user != null)
						{
							displayNameText = user.DisplayName;
							firstName = user.FirstName;
						}
					}
					IInternalFriend item = RemoteUserFactory.CreateFriend(userId, value, displayNameText, firstName, userDatabase);
					list.Add(item);
				}
			}
			return list;
		}

		public void ParseFriendshipInvitations(GetStateResponse response, IUserDatabase userDatabase, IInternalLocalUser localUser, out IList<IInternalIncomingFriendInvitation> incomingFriendInvitations, out IList<IInternalOutgoingFriendInvitation> outgoingFriendInvitations)
		{
			incomingFriendInvitations = new List<IInternalIncomingFriendInvitation>();
			outgoingFriendInvitations = new List<IInternalOutgoingFriendInvitation>();
			if (response.FriendshipInvitations != null)
			{
				foreach (FriendshipInvitation invitation in response.FriendshipInvitations)
				{
					List<User> users = response.Users;
					Func<User, bool> predicate = (User user) => user.DisplayName == invitation.FriendDisplayName;
					User user2 = users.FirstOrDefault(predicate);
					string firstName = (user2 == null) ? null : user2.FirstName;
					if (invitation.IsInviter.Value)
					{
						IInternalUnidentifiedUser invitee = RemoteUserFactory.CreateUnidentifiedUser(invitation.FriendDisplayName, firstName, userDatabase);
						OutgoingFriendInvitation outgoingFriendInvitation = new OutgoingFriendInvitation(localUser, invitee, invitation.IsTrusted.Value);
						outgoingFriendInvitation.SendComplete(invitation.FriendshipInvitationId.Value);
						outgoingFriendInvitations.Add(outgoingFriendInvitation);
					}
					else
					{
						IInternalUnidentifiedUser inviter = RemoteUserFactory.CreateUnidentifiedUser(invitation.FriendDisplayName, firstName, userDatabase);
						IncomingFriendInvitation incomingFriendInvitation = new IncomingFriendInvitation(inviter, localUser, invitation.IsTrusted.Value);
						incomingFriendInvitation.SendComplete(invitation.FriendshipInvitationId.Value);
						incomingFriendInvitations.Add(incomingFriendInvitation);
					}
				}
			}
		}

		public void ReconcileWithLocalUser(IMixWebCallFactory mixWebCallFactory, GetStateResponse response, IInternalLocalUser localUser, IUserDatabase userDatabase)
		{
			ReconcileFriends(response, localUser, userDatabase);
			ReconcileFriendInvitations(response, localUser, userDatabase);
		}

		private void ReconcileFriends(GetStateResponse response, IInternalLocalUser localUser, IUserDatabase userDatabase)
		{
			List<IInternalFriend> list = localUser.InternalFriends.ToList();
			List<IInternalIncomingFriendInvitation> list2 = localUser.InternalIncomingFriendInvitations.ToList();
			List<IInternalOutgoingFriendInvitation> list3 = localUser.InternalOutgoingFriendInvitations.ToList();
			if (response.Friendships != null)
			{
				foreach (Friendship friendship in response.Friendships)
				{
					string friendUserId = friendship.FriendUserId;
					IInternalFriend internalFriend = list.FirstOrDefault((IInternalFriend f) => f.Swid == friendUserId);
					User user = response.Users.First((User u) => u.UserId == friendUserId);
					if (internalFriend != null)
					{
						if (internalFriend.IsTrusted != friendship.IsTrusted)
						{
							if (friendship.IsTrusted.Value)
							{
								internalFriend.ChangeTrust(true);
							}
							else
							{
								localUser.UntrustFriend(internalFriend);
							}
						}
						list.Remove(internalFriend);
					}
					else
					{
						IInternalFriend internalFriend2 = RemoteUserFactory.CreateFriend(friendship.FriendUserId, friendship.IsTrusted.Value, user.DisplayName, user.FirstName, userDatabase);
						IInternalIncomingFriendInvitation internalIncomingFriendInvitation = list2.FirstOrDefault((IInternalIncomingFriendInvitation i) => i.InternalInviter.DisplayName.Text == user.DisplayName);
						if (internalIncomingFriendInvitation != null)
						{
							localUser.AddFriend(internalFriend2);
							internalIncomingFriendInvitation.Accepted(internalFriend2.IsTrusted, internalFriend2);
							list2.Remove(internalIncomingFriendInvitation);
							localUser.RemoveIncomingFriendInvitation(internalIncomingFriendInvitation);
						}
						else
						{
							IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = list3.FirstOrDefault((IInternalOutgoingFriendInvitation i) => i.InternalInvitee.DisplayName.Text == user.DisplayName);
							if (internalOutgoingFriendInvitation != null)
							{
								localUser.AddFriend(internalFriend2);
								internalOutgoingFriendInvitation.Accepted(internalFriend2.IsTrusted, internalFriend2);
								list3.Remove(internalOutgoingFriendInvitation);
								localUser.RemoveOutgoingFriendInvitation(internalOutgoingFriendInvitation);
							}
							else
							{
								localUser.AddFriend(internalFriend2);
							}
						}
					}
				}
			}
			foreach (IInternalFriend item in list)
			{
				localUser.RemoveFriend(item);
			}
		}

		private void ReconcileFriendInvitations(GetStateResponse response, IInternalLocalUser localUser, IUserDatabase userDatabase)
		{
			List<FriendshipInvitation> friendshipInvitations = response.FriendshipInvitations;
			List<IInternalIncomingFriendInvitation> list = localUser.InternalIncomingFriendInvitations.ToList();
			List<IInternalOutgoingFriendInvitation> list2 = localUser.InternalOutgoingFriendInvitations.ToList();
			if (friendshipInvitations != null)
			{
				foreach (FriendshipInvitation friendInvitation in friendshipInvitations)
				{
					Func<IInternalIncomingFriendInvitation, bool> predicate = (IInternalIncomingFriendInvitation i) => i.InvitationId == friendInvitation.FriendshipInvitationId;
					IInternalIncomingFriendInvitation internalIncomingFriendInvitation = list.FirstOrDefault(predicate);
					if (internalIncomingFriendInvitation != null)
					{
						if (internalIncomingFriendInvitation.RequestTrust != friendInvitation.IsTrusted)
						{
							internalIncomingFriendInvitation.RequestTrust = friendInvitation.IsTrusted.Value;
						}
						list.Remove(internalIncomingFriendInvitation);
					}
					else
					{
						IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = list2.FirstOrDefault((IInternalOutgoingFriendInvitation i) => i.InvitationId == friendInvitation.FriendshipInvitationId);
						if (internalOutgoingFriendInvitation != null)
						{
							if (internalOutgoingFriendInvitation.RequestTrust != friendInvitation.IsTrusted)
							{
								internalOutgoingFriendInvitation.RequestTrust = friendInvitation.IsTrusted.Value;
							}
							list2.Remove(internalOutgoingFriendInvitation);
						}
						else
						{
							User user2 = response.Users.FirstOrDefault((User user) => user.DisplayName == friendInvitation.FriendDisplayName);
							string firstName = (user2 == null) ? null : user2.FirstName;
							if (friendInvitation.IsInviter.Value)
							{
								IInternalUnidentifiedUser invitee = RemoteUserFactory.CreateUnidentifiedUser(friendInvitation.FriendDisplayName, firstName, userDatabase);
								OutgoingFriendInvitation outgoingFriendInvitation = new OutgoingFriendInvitation(localUser, invitee, friendInvitation.IsTrusted.Value);
								outgoingFriendInvitation.SendComplete(friendInvitation.FriendshipInvitationId.Value);
								localUser.AddOutgoingFriendInvitation(outgoingFriendInvitation);
							}
							else
							{
								IInternalUnidentifiedUser inviter = RemoteUserFactory.CreateUnidentifiedUser(friendInvitation.FriendDisplayName, firstName, userDatabase);
								IncomingFriendInvitation incomingFriendInvitation = new IncomingFriendInvitation(inviter, localUser, friendInvitation.IsTrusted.Value);
								incomingFriendInvitation.SendComplete(friendInvitation.FriendshipInvitationId.Value);
								localUser.AddIncomingFriendInvitation(incomingFriendInvitation);
							}
						}
					}
				}
			}
			foreach (IInternalIncomingFriendInvitation item in list)
			{
				localUser.RemoveIncomingFriendInvitation(item);
				item.Rejected();
			}
			foreach (IInternalOutgoingFriendInvitation item2 in list2)
			{
				localUser.RemoveOutgoingFriendInvitation(item2);
				item2.Rejected();
			}
		}

		public void ParsePollIntervals(GetStateResponse response, out int[] pollIntervals, out int[] pokeIntervals)
		{
			pollIntervals = response.PollIntervals.Select((int? i) => i.Value * 1000).ToArray();
			pokeIntervals = response.PokeIntervals.Select((int? i) => i.Value * 1000).ToArray();
		}

		public IList<IInternalAlert> ParseAlerts(GetStateResponse response)
		{
			return ((IEnumerable<Disney.Mix.SDK.Internal.MixDomain.Alert>)response.Alerts).Select((Func<Disney.Mix.SDK.Internal.MixDomain.Alert, IInternalAlert>)((Disney.Mix.SDK.Internal.MixDomain.Alert a) => new Alert(a))).ToList();
		}

		public List<BaseNotification> CollectNotifications(GetNotificationsResponse response)
		{
			return EnumerateValidNotifications(logger, response.AddFriendshipInvitation, NotificationValidator.Validate).Concat(EnumerateValidNotifications(logger, response.AddFriendship, NotificationValidator.Validate)).Concat(EnumerateValidNotifications(logger, response.RemoveFriendship, NotificationValidator.Validate)).Concat(EnumerateValidNotifications(logger, response.RemoveFriendshipInvitation, NotificationValidator.Validate))
				.Concat(EnumerateValidNotifications(logger, response.RemoveFriendshipTrust, NotificationValidator.Validate))
				.Concat(EnumerateValidNotifications(logger, response.AddAlert, NotificationValidator.Validate))
				.Concat(EnumerateValidNotifications(logger, response.ClearAlert, NotificationValidator.Validate))
				.ToList();
		}

		private static IEnumerable<BaseNotification> EnumerateValidNotifications<TNotification>(AbstractLogger logger, IList<TNotification> notifications, Func<TNotification, bool> isValid) where TNotification : BaseNotification
		{
			if (notifications != null)
			{
				foreach (TNotification notification in notifications)
				{
					if (isValid(notification))
					{
						yield return notification;
					}
					else
					{
						logger.Critical("Invalid notification: " + JsonParser.ToJson(notification));
					}
				}
			}
		}
	}
}
