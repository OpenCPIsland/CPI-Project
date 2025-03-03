using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.Mix.SDK.Internal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Mix
{
	internal class OfflineLocalUser : ILocalUser
	{
		private OfflineRegistrationProfile registrationProfile;

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
				return new List<IFriend>();
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
				return new List<IIncomingFriendInvitation>();
			}
		}

		public IEnumerable<IOutgoingFriendInvitation> OutgoingFriendInvitations
		{
			get
			{
				return new List<IOutgoingFriendInvitation>();
			}
		}

		public IRegistrationProfile RegistrationProfile
		{
			get
			{
				return registrationProfile;
			}
		}

		public event EventHandler<AbstractAlertsAddedEventArgs> OnAlertsAdded;

		public event EventHandler<AbstractAlertsClearedEventArgs> OnAlertsCleared;

		public event EventHandler<AbstractLegalMarketingUpdateRequiredEventArgs> OnLegalMarketingUpdateRequired;

		public event EventHandler<AbstractReceivedIncomingFriendInvitationEventArgs> OnReceivedIncomingFriendInvitation;

		public event EventHandler<AbstractReceivedOutgoingFriendInvitationEventArgs> OnReceivedOutgoingFriendInvitation;

		public event EventHandler<AbstractUnfriendedEventArgs> OnUnfriended;

		public event EventHandler<AbstractUntrustedEventArgs> OnUntrusted;

		public OfflineLocalUser(string userName)
		{
			registrationProfile = OfflineRegistrationProfile.Load(userName);
		}

		public void AcceptFriendInvitation(IIncomingFriendInvitation invitation, bool acceptTrust, Action<IAcceptFriendInvitationResult> callback)
		{
			CoroutineRunner.StartPersistent(delayAcceptFriendInvitation(callback), this, "AcceptFriendInvitation");
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
			CoroutineRunner.StartPersistent(delayFindUser(callback), this, "FindUser");
		}

		private IEnumerator delayFindUser(Action<IFindUserResult> callback)
		{
			yield return null;
			callback(new FindUserResult(true, null));
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
			CoroutineRunner.StartPersistent(delayRejectFriendInvitation(callback), this, "RejectFriendInvitation");
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
			CoroutineRunner.StartPersistent(delaySendFriendInvitation(callback), this, "SendFriendInvitation");
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
			CoroutineRunner.StartPersistent(delayUnfriend(callback), this, "Unfriend");
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
