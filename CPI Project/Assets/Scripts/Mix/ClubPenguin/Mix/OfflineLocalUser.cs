using ClubPenguin.Net.Client;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.Mix.SDK.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin.Mix
{
	internal class OfflineLocalUser : ILocalUser
	{
		private OfflineRegistrationProfile registrationProfile;

		private readonly string lanToken;

		private readonly List<OfflineLanFriend> lanFriends = new List<OfflineLanFriend>();

		private readonly List<OfflineLanIncomingInvitation> lanIncoming = new List<OfflineLanIncomingInvitation>();

		private readonly List<OfflineLanOutgoingInvitation> lanOutgoing = new List<OfflineLanOutgoingInvitation>();

		public AgeBandType AgeBandType
		{
			get
			{
				return AgeBandType.Adult;
			}
		}

		public IEnumerable<IAlert> Alerts
		{
			get
			{
				return new List<IAlert>();
			}
		}

		public IDisplayName DisplayName
		{
			get
			{
				return new DisplayName(registrationProfile.DisplayName);
			}
		}

		public string FirstName
		{
			get
			{
				return registrationProfile.FirstName;
			}
		}

		public IEnumerable<IFriend> Friends
		{
			get
			{
				return lanFriends.Cast<IFriend>().ToList();
			}
		}

		public string HashedId
		{
			get
			{
				return registrationProfile.Id;
			}
		}

		public string Id
		{
			get
			{
				return registrationProfile.Id;
			}
		}

		public IEnumerable<IIncomingFriendInvitation> IncomingFriendInvitations
		{
			get
			{
				return lanIncoming.Cast<IIncomingFriendInvitation>().ToList();
			}
		}

		public IEnumerable<IOutgoingFriendInvitation> OutgoingFriendInvitations
		{
			get
			{
				return lanOutgoing.Cast<IOutgoingFriendInvitation>().ToList();
			}
		}

		public IRegistrationProfile RegistrationProfile
		{
			get
			{
				return registrationProfile;
			}
		}

#pragma warning disable 0067
		public event EventHandler<AbstractAlertsAddedEventArgs> OnAlertsAdded;

		public event EventHandler<AbstractAlertsClearedEventArgs> OnAlertsCleared;

		public event EventHandler<AbstractLegalMarketingUpdateRequiredEventArgs> OnLegalMarketingUpdateRequired;

		public event EventHandler<AbstractReceivedIncomingFriendInvitationEventArgs> OnReceivedIncomingFriendInvitation;

		public event EventHandler<AbstractReceivedOutgoingFriendInvitationEventArgs> OnReceivedOutgoingFriendInvitation;

		public event EventHandler<AbstractUnfriendedEventArgs> OnUnfriended;

		public event EventHandler<AbstractUntrustedEventArgs> OnUntrusted;
#pragma warning restore 0067

		public OfflineLocalUser(string userName)
		{
			registrationProfile = OfflineRegistrationProfile.Load(userName);
			lanToken = registrationProfile.Id;
			LanFriendsData data = LanFriends.Load(lanToken);
			foreach (LanFriendsData.Entry entry in data.Friends)
			{
				lanFriends.Add(new OfflineLanFriend(entry.Swid, entry.Name));
			}
			foreach (LanFriendsData.Entry entry in data.Incoming)
			{
				lanIncoming.Add(new OfflineLanIncomingInvitation(entry.Id, entry.Swid, entry.Name, this));
			}
			foreach (LanFriendsData.Entry entry in data.Outgoing)
			{
				lanOutgoing.Add(new OfflineLanOutgoingInvitation(entry.Id, entry.Swid, entry.Name, this));
			}
			LanFriends.Listen(lanToken, onLanFriends);
		}

		private void saveLanFriends()
		{
			LanFriendsData data = default(LanFriendsData);
			data.Init();
			foreach (OfflineLanFriend friend in lanFriends)
			{
				data.Friends.Add(new LanFriendsData.Entry
				{
					Swid = friend.Id,
					Name = friend.DisplayName.Text
				});
			}
			foreach (OfflineLanIncomingInvitation invitation in lanIncoming)
			{
				data.Incoming.Add(new LanFriendsData.Entry
				{
					Swid = invitation.Swid,
					Name = invitation.Inviter.DisplayName.Text,
					Id = invitation.InvitationId
				});
			}
			foreach (OfflineLanOutgoingInvitation invitation in lanOutgoing)
			{
				data.Outgoing.Add(new LanFriendsData.Entry
				{
					Swid = invitation.Swid,
					Name = invitation.Invitee.DisplayName.Text,
					Id = invitation.InvitationId
				});
			}
			LanFriends.Save(data, lanToken);
		}

		private OfflineLanFriend addLanFriend(string swid, string name)
		{
			OfflineLanFriend friend = lanFriends.FirstOrDefault((OfflineLanFriend f) => f.Id == swid);
			if (friend == null)
			{
				friend = new OfflineLanFriend(swid, name);
				lanFriends.Add(friend);
			}
			return friend;
		}

		private void onLanFriends(string kind, LanFriendsData.Entry entry)
		{
			switch (kind)
			{
			case "invitation":
				if (!lanIncoming.Any((OfflineLanIncomingInvitation i) => i.InvitationId == entry.Id) && !lanFriends.Any((OfflineLanFriend f) => f.Id == entry.Swid))
				{
					OfflineLanIncomingInvitation incoming = new OfflineLanIncomingInvitation(entry.Id, entry.Swid, entry.Name, this);
					lanIncoming.Add(incoming);
					saveLanFriends();
					if (this.OnReceivedIncomingFriendInvitation != null)
					{
						this.OnReceivedIncomingFriendInvitation(this, new OfflineLanReceivedIncomingEventArgs(incoming));
					}
				}
				break;
			case "outgoing":
				if (!lanOutgoing.Any((OfflineLanOutgoingInvitation i) => i.InvitationId == entry.Id) && !lanFriends.Any((OfflineLanFriend f) => f.Id == entry.Swid))
				{
					OfflineLanOutgoingInvitation outgoing = new OfflineLanOutgoingInvitation(entry.Id, entry.Swid, entry.Name, this);
					lanOutgoing.Add(outgoing);
					saveLanFriends();
					if (this.OnReceivedOutgoingFriendInvitation != null)
					{
						this.OnReceivedOutgoingFriendInvitation(this, new OfflineLanReceivedOutgoingEventArgs(outgoing));
					}
				}
				break;
			case "friend":
			{
				if (lanFriends.Any((OfflineLanFriend f) => f.Id == entry.Swid))
				{
					break;
				}
				OfflineLanOutgoingInvitation accepted = lanOutgoing.FirstOrDefault((OfflineLanOutgoingInvitation i) => i.InvitationId == entry.Id || i.Swid == entry.Swid);
				if (accepted == null)
				{
					accepted = new OfflineLanOutgoingInvitation(entry.Id, entry.Swid, entry.Name, this);
					if (this.OnReceivedOutgoingFriendInvitation != null)
					{
						this.OnReceivedOutgoingFriendInvitation(this, new OfflineLanReceivedOutgoingEventArgs(accepted));
					}
				}
				lanOutgoing.Remove(accepted);
				OfflineLanIncomingInvitation stale = lanIncoming.FirstOrDefault((OfflineLanIncomingInvitation i) => i.Swid == entry.Swid);
				if (stale != null)
				{
					lanIncoming.Remove(stale);
					stale.Rejected();
				}
				OfflineLanFriend friend = addLanFriend(entry.Swid, entry.Name);
				saveLanFriends();
				accepted.Accepted(friend);
				break;
			}
			case "uninvitation":
			{
				OfflineLanIncomingInvitation incoming = lanIncoming.FirstOrDefault((OfflineLanIncomingInvitation i) => i.InvitationId == entry.Id);
				if (incoming != null)
				{
					lanIncoming.Remove(incoming);
					saveLanFriends();
					incoming.Rejected();
				}
				OfflineLanOutgoingInvitation outgoing = lanOutgoing.FirstOrDefault((OfflineLanOutgoingInvitation i) => i.InvitationId == entry.Id);
				if (outgoing != null)
				{
					lanOutgoing.Remove(outgoing);
					saveLanFriends();
					outgoing.Rejected();
				}
				break;
			}
			case "unfriend":
			{
				OfflineLanFriend friend = lanFriends.FirstOrDefault((OfflineLanFriend f) => f.Id == entry.Swid);
				if (friend != null)
				{
					lanFriends.Remove(friend);
					saveLanFriends();
					if (this.OnUnfriended != null)
					{
						this.OnUnfriended(this, new OfflineLanUnfriendedEventArgs(friend));
					}
				}
				break;
			}
			}
		}

		public void AcceptFriendInvitation(IIncomingFriendInvitation invitation, bool acceptTrust, Action<IAcceptFriendInvitationResult> callback)
		{
			OfflineLanIncomingInvitation incoming = invitation as OfflineLanIncomingInvitation;
			ILanFriendsTransport transport = LanFriends.Transport;
			if (incoming == null || transport == null)
			{
				CoroutineRunner.StartPersistent(delayAcceptFriendInvitation(callback), this, "AcceptFriendInvitation");
				return;
			}
			transport.Accept(incoming.InvitationId, delegate(bool success)
			{
				if (!success)
				{
					callback(new AcceptFriendInvitationResult(false, null));
					return;
				}
				lanIncoming.Remove(incoming);
				OfflineLanFriend friend = addLanFriend(incoming.Swid, incoming.Inviter.DisplayName.Text);
				saveLanFriends();
				incoming.Accepted(friend);
				callback(new AcceptFriendInvitationResult(true, friend));
			});
		}

		private IEnumerator delayAcceptFriendInvitation(Action<IAcceptFriendInvitationResult> callback)
		{
			yield return null;
			callback(new AcceptFriendInvitationResult(false, null));
		}

		public void AnswerVerifyAdultQuiz(IVerifyAdultQuizAnswers answers, Action<IVerifyAdultResult> callback)
		{
			throw new NotImplementedException();
		}

		public void ApproveChildTrustPermission(ILinkedUser child, ActivityApprovalStatus status, Action<IPermissionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void ApproveChildTrustPermission(ISession child, ActivityApprovalStatus status, Action<IPermissionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void ClearAlerts(IEnumerable<IAlert> alerts, Action<IClearAlertsResult> callback)
		{
			CoroutineRunner.StartPersistent(delayClearAlerts(callback), this, "ClearAlerts");
		}

		private IEnumerator delayClearAlerts(Action<IClearAlertsResult> callback)
		{
			yield return null;
			callback(new ClearAlertsResult(true));
		}

		public void DisableAllPushNotifications(Action<IDisableAllPushNotificationsResult> callback)
		{
		}

		public void DisableVisiblePushNotifications(Action<IDisableVisiblePushNotificationsResult> callback)
		{
			throw new NotImplementedException();
		}

		public void EnableAllPushNotifications(string token, string serviceType, string provisionId, Action<IEnableAllPushNotificationsResult> callback)
		{
			throw new NotImplementedException();
		}

		public void EnableInvisiblePushNotifications(string token, string serviceType, string provisionId, Action<IEnableInvisiblePushNotificationsResult> callback)
		{
		}

		public void FindUser(string displayName, Action<IFindUserResult> callback)
		{
			ILanFriendsTransport transport = LanFriends.Transport;
			if (transport == null)
			{
				CoroutineRunner.StartPersistent(delayFindUser(callback), this, "FindUser");
				return;
			}
			transport.Find(displayName, delegate(string name)
			{
				callback((name != null) ? new FindUserResult(true, new UnidentifiedUser(new DisplayName(name), "", null)) : new FindUserResult(false, null));
			});
		}

		private IEnumerator delayFindUser(Action<IFindUserResult> callback)
		{
			yield return null;
			callback(new FindUserResult(false, null));
		}

		public void GetAdultVerificationRequirements(Action<IGetAdultVerificationRequirementsResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetAdultVerificationStatus(Action<IGetAdultVerificationStatusResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetChildTrustPermission(ILinkedUser child, Action<IPermissionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetChildTrustPermission(ISession child, Action<IPermissionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetClaimableChildren(Action<IGetLinkedUsersResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetLinkedChildren(Action<IGetLinkedUsersResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetLinkedGuardians(Action<IGetLinkedUsersResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetRecommendedFriends(Action<IGetRecommendedFriendsResult> callback)
		{
			throw new NotImplementedException();
		}

		public void GetVerifyAdultForm(Action<IGetVerifyAdultFormResult> callback)
		{
			throw new NotImplementedException();
		}

		public void LinkChildAccount(ISession child, Action<ILinkChildResult> callback)
		{
			throw new NotImplementedException();
		}

		public void LinkClaimableChildAccounts(IEnumerable<ILinkedUser> children, Action<ILinkChildResult> callback)
		{
			throw new NotImplementedException();
		}

		public void ModerateText(string text, bool isTrusted, Action<ITextModerationResult> callback)
		{
			throw new NotImplementedException();
		}

		public IPushNotification ReceivePushNotification(IDictionary notification)
		{
			return null;
		}

		public void RefreshProfile(Action<IRefreshProfileResult> callback)
		{
			CoroutineRunner.StartPersistent(delayRefreshProfile(callback), this, "RefreshProfile");
		}

		private IEnumerator delayRefreshProfile(Action<IRefreshProfileResult> callback)
		{
			yield return null;
			callback(new RefreshProfileResult(true));
		}

		public void RejectFriendInvitation(IIncomingFriendInvitation invitation, Action<IRejectFriendInvitationResult> callback)
		{
			OfflineLanIncomingInvitation incoming = invitation as OfflineLanIncomingInvitation;
			ILanFriendsTransport transport = LanFriends.Transport;
			if (incoming == null || transport == null)
			{
				CoroutineRunner.StartPersistent(delayRejectFriendInvitation(callback), this, "RejectFriendInvitation");
				return;
			}
			transport.Reject(incoming.InvitationId, delegate(bool success)
			{
				if (success)
				{
					lanIncoming.Remove(incoming);
					saveLanFriends();
					incoming.Rejected();
				}
				callback(new RejectFriendInvitationResult(success));
			});
		}

		private IEnumerator delayRejectFriendInvitation(Action<IRejectFriendInvitationResult> callback)
		{
			yield return null;
			callback(new RejectFriendInvitationResult(true));
		}

		public void ReportUser(IFriend user, ReportUserReason reason, Action<IReportUserResult> callback)
		{
			throw new NotImplementedException();
		}

		public void RequestTrustPermission(Action<IPermissionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void RequestTrustPermissionForChild(ILinkedUser child, Action<IPermissionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void SendAlert(int level, AlertType type, Action<ISendAlertResult> callback)
		{
			throw new NotImplementedException();
		}

		public IOutgoingFriendInvitation SendFriendInvitation(IFriend user, bool requestTrust, Action<ISendFriendInvitationResult> callback)
		{
			throw new NotImplementedException();
		}

		public IOutgoingFriendInvitation SendFriendInvitation(IUnidentifiedUser user, bool requestTrust, Action<ISendFriendInvitationResult> callback)
		{
			ILanFriendsTransport transport = LanFriends.Transport;
			if (user == null || user.DisplayName == null || transport == null)
			{
				CoroutineRunner.StartPersistent(delaySendFriendInvitation(callback), this, "SendFriendInvitation");
				return null;
			}
			transport.Invite(user.DisplayName.Text, delegate(LanFriendsData.Entry? entry, string status)
			{
				if (!entry.HasValue)
				{
					if (status == "ALREADY_FRIENDS")
					{
						callback(new SendFriendInvitationAlreadyFriendsResult(false, null));
					}
					else if (status == "INVITATION_ALREADY_EXISTS")
					{
						callback(new SendFriendInvitationAlreadyExistsResult(false, null));
					}
					else
					{
						callback(new SendFriendInvitationResult(false, null));
					}
					return;
				}
				OfflineLanOutgoingInvitation outgoing = new OfflineLanOutgoingInvitation(entry.Value.Id, entry.Value.Swid, entry.Value.Name, this);
				lanOutgoing.Add(outgoing);
				saveLanFriends();
				if (this.OnReceivedOutgoingFriendInvitation != null)
				{
					this.OnReceivedOutgoingFriendInvitation(this, new OfflineLanReceivedOutgoingEventArgs(outgoing));
				}
				callback(new SendFriendInvitationResult(true, outgoing));
			});
			return null;
		}

		private IEnumerator delaySendFriendInvitation(Action<ISendFriendInvitationResult> callback)
		{
			yield return null;
			callback(new SendFriendInvitationResult(false, null));
		}

		public void SendMassPushNotification(Action<ISendMassPushNotificationResult> callback)
		{
			throw new NotImplementedException();
		}

		public void SendParentalApprovalEmail(string languageCode, Action<ISendParentalApprovalEmailResult> callback)
		{
			throw new NotImplementedException();
		}

		public void SendVerificationEmail(string languageCode, Action<ISendVerificationEmailResult> callback)
		{
			throw new NotImplementedException();
		}

		public void SetLanguagePreference(string languageCode, Action<ISetLangaugePreferenceResult> callback)
		{
		}

		public void TemporarilyBanAccount(Action<ITemporarilyBanAccountResult> callback)
		{
			throw new NotImplementedException();
		}

		public void Unfriend(IFriend friend, Action<IUnfriendResult> callback)
		{
			OfflineLanFriend lanFriend = friend as OfflineLanFriend;
			ILanFriendsTransport transport = LanFriends.Transport;
			if (lanFriend == null || transport == null)
			{
				CoroutineRunner.StartPersistent(delayUnfriend(callback), this, "Unfriend");
				return;
			}
			transport.Unfriend(lanFriend.Id, delegate(bool success)
			{
				if (success)
				{
					lanFriends.Remove(lanFriend);
					saveLanFriends();
					if (this.OnUnfriended != null)
					{
						this.OnUnfriended(this, new OfflineLanUnfriendedEventArgs(lanFriend));
					}
				}
				callback(new UnfriendResult(success));
			});
		}

		private IEnumerator delayUnfriend(Action<IUnfriendResult> callback)
		{
			yield return null;
			callback(new UnfriendResult(false));
		}

		public void Untrust(IFriend trustedUser, Action<IUntrustResult> callback)
		{
			throw new NotImplementedException();
		}

		public void UpdateDisplayName(string displayName, Action<IUpdateDisplayNameResult> callback)
		{
			registrationProfile.DisplayName = displayName;
			registrationProfile.Save();
			CoroutineRunner.StartPersistent(delayUpdateDisplayName(callback), this, "UpdateDisplayName");
		}

		private IEnumerator delayUpdateDisplayName(Action<IUpdateDisplayNameResult> callback)
		{
			yield return null;
			callback(new UpdateDisplayNameResult(true));
		}

		public void UpdateProfile(string firstName, string lastName, string displayName, string email, string parentEmail, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IUpdateProfileResult> callback)
		{
			throw new NotImplementedException();
		}

		public void ValidateDisplayName(string displayName, Action<IValidateDisplayNameResult> callback)
		{
			CoroutineRunner.StartPersistent(delayValidateDisplayName(callback), this, "ValidateDisplayName");
		}

		private IEnumerator delayValidateDisplayName(Action<IValidateDisplayNameResult> callback)
		{
			yield return null;
			callback(new ValidateDisplayNameResult(true));
		}

		public void ValidateDisplayNames(IEnumerable<string> displayNames, Action<IValidateDisplayNamesResult> callback)
		{
			throw new NotImplementedException();
		}

		public void ValidateDisplayNameV2(string displayName, Action<IValidateDisplayNameResult> callback)
		{
			throw new NotImplementedException();
		}

		public void VerifyAdult(IVerifyAdultFormUnitedStates form, Action<IVerifyAdultResult> callback)
		{
			throw new NotImplementedException();
		}
	}
}
