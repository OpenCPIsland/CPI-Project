using Disney.Mix.SDK.Internal.GuestControllerDomain;
using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class LocalUser : IInternalLocalUser, ILocalUser
	{
		private const string TrustPermissionCode = "MIX_TRUSTEDFRIENDSCOMMUNICATIONS";

		private readonly IList<IInternalIncomingFriendInvitation> incomingFriendInvitations;

		private readonly IList<IInternalOutgoingFriendInvitation> outgoingFriendInvitations;

		private readonly IList<long> oldInvitationIds;

		private readonly IList<IInternalFriend> friends;

		private readonly IList<IInternalUnidentifiedUser> unidentifiedUsers;

		private readonly AgeBandType ageBandType;

		private readonly IDatabase database;

		private readonly IUserDatabase userDatabase;

		private readonly AbstractLogger logger;

		private readonly IInternalRegistrationProfile registrationProfile;

		private readonly IMixWebCallFactory mixWebCallFactory;

		private readonly IGuestControllerClient guestControllerClient;

		private readonly INotificationQueue notificationQueue;

		private readonly IEncryptor encryptor;

		private readonly IEpochTime epochTime;

		public string Swid
		{
			get;
			private set;
		}

		public string Id
		{
			get;
			private set;
		}

		public string HashedId
		{
			get
			{
				return userDatabase.GetUserBySwid(Swid).HashedSwid;
			}
		}

		public IDisplayName DisplayName
		{
			get;
			private set;
		}

		public string FirstName
		{
			get;
			private set;
		}

		public IEnumerable<IAlert> Alerts
		{
			get
			{
				return userDatabase.GetAlerts().Cast<IAlert>().ToArray();
			}
		}

		public IEnumerable<IFriend> Friends
		{
			get
			{
				return friends.ToArray();
			}
		}

		public IEnumerable<IIncomingFriendInvitation> IncomingFriendInvitations
		{
			get
			{
				return incomingFriendInvitations.ToArray();
			}
		}

		public IEnumerable<IOutgoingFriendInvitation> OutgoingFriendInvitations
		{
			get
			{
				return outgoingFriendInvitations.ToArray();
			}
		}

		public AgeBandType AgeBandType
		{
			get
			{
				return ageBandType;
			}
		}

		public IRegistrationProfile RegistrationProfile
		{
			get
			{
				return registrationProfile;
			}
		}

		public IEnumerable<IInternalFriend> InternalFriends
		{
			get
			{
				return friends.ToArray();
			}
		}

		public IEnumerable<IInternalIncomingFriendInvitation> InternalIncomingFriendInvitations
		{
			get
			{
				return incomingFriendInvitations.ToArray();
			}
		}

		public IEnumerable<IInternalOutgoingFriendInvitation> InternalOutgoingFriendInvitations
		{
			get
			{
				return outgoingFriendInvitations.ToArray();
			}
		}

		public IInternalRegistrationProfile InternalRegistrationProfile
		{
			get
			{
				return registrationProfile;
			}
		}

		public event EventHandler<AbstractReceivedOutgoingFriendInvitationEventArgs> OnReceivedOutgoingFriendInvitation = delegate
		{
		};

		public event EventHandler<AbstractReceivedIncomingFriendInvitationEventArgs> OnReceivedIncomingFriendInvitation = delegate
		{
		};

		public event EventHandler<AbstractUnfriendedEventArgs> OnUnfriended = delegate
		{
		};

		public event EventHandler<AbstractUntrustedEventArgs> OnUntrusted = delegate
		{
		};

		public event Action<bool> OnPushNotificationsToggled = delegate
		{
		};

		public event Action<bool> OnPushNotificationReceived = delegate
		{
		};

		public event Action<string> OnDisplayNameUpdated = delegate
		{
		};

		public event EventHandler<AbstractAlertsAddedEventArgs> OnAlertsAdded = delegate
		{
		};

		public event EventHandler<AbstractAlertsClearedEventArgs> OnAlertsCleared = delegate
		{
		};

		public event EventHandler<AbstractLegalMarketingUpdateRequiredEventArgs> OnLegalMarketingUpdateRequired = delegate
		{
		};

		public LocalUser(AbstractLogger logger, IDisplayName displayName, string swid, IList<IInternalFriend> friends, AgeBandType ageBandType, IDatabase database, IUserDatabase userDatabase, IInternalRegistrationProfile registrationProfile, IMixWebCallFactory mixWebCallFactory, IGuestControllerClient guestControllerClient, INotificationQueue notificationQueue, IEncryptor encryptor, IEpochTime epochTime)
		{
			DisplayName = displayName;
			FirstName = registrationProfile.FirstName;
			Swid = swid;
			Id = swid;
			this.logger = logger;
			this.registrationProfile = registrationProfile;
			this.mixWebCallFactory = mixWebCallFactory;
			this.friends = friends;
			this.ageBandType = ageBandType;
			this.database = database;
			this.userDatabase = userDatabase;
			incomingFriendInvitations = new List<IInternalIncomingFriendInvitation>();
			outgoingFriendInvitations = new List<IInternalOutgoingFriendInvitation>();
			oldInvitationIds = new List<long>();
			unidentifiedUsers = new List<IInternalUnidentifiedUser>();
			this.guestControllerClient = guestControllerClient;
			this.notificationQueue = notificationQueue;
			this.encryptor = encryptor;
			this.epochTime = epochTime;
			guestControllerClient.OnLegalMarketingUpdateRequired += delegate(object sender, AbstractLegalMarketingUpdateRequiredEventArgs e)
			{
				this.OnLegalMarketingUpdateRequired(this, e);
			};
		}

		public void FindUser(string displayNameText, Action<IFindUserResult> callback)
		{
			DisplayNameSearcher.Search(logger, mixWebCallFactory, displayNameText, userDatabase, delegate(IInternalUnidentifiedUser user)
			{
				unidentifiedUsers.Add(user);
				callback(new FindUserResult(true, user));
			}, delegate
			{
				callback(new FindUserResult(false, null));
			});
		}

		public IOutgoingFriendInvitation SendFriendInvitation(IUnidentifiedUser user, bool requestTrust, Action<ISendFriendInvitationResult> callback)
		{
			IInternalUnidentifiedUser internalUnidentifiedUser = unidentifiedUsers.FirstOrDefault((IInternalUnidentifiedUser u) => u.DisplayName.Text == user.DisplayName.Text);
			if (internalUnidentifiedUser == null)
			{
				logger.Critical("User to send friend invitation to not found");
				callback(new SendFriendInvitationResult(false, null));
				return null;
			}
			ISendFriendInvitationResult invitationValidationResult = GetInvitationValidationResult(user.DisplayName.Text, requestTrust);
			if (invitationValidationResult != null)
			{
				callback(invitationValidationResult);
				return null;
			}
			return InternalSendFriendInvitation(internalUnidentifiedUser, requestTrust, callback);
		}

		public IOutgoingFriendInvitation SendFriendInvitation(IFriend user, bool requestTrust, Action<ISendFriendInvitationResult> callback)
		{
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.Id == user.Id);
			if (internalFriend == null)
			{
				logger.Critical("User to send friend invitation to not found");
				callback(new SendFriendInvitationResult(false, null));
				return null;
			}
			IInternalUnidentifiedUser internalUser = RemoteUserFactory.CreateUnidentifiedUser(internalFriend.DisplayName.Text, internalFriend.FirstName, userDatabase);
			return InternalSendFriendInvitation(internalUser, requestTrust, callback);
		}

		private IOutgoingFriendInvitation InternalSendFriendInvitation(IInternalUnidentifiedUser internalUser, bool requestTrust, Action<ISendFriendInvitationResult> callback)
		{
			ISendFriendInvitationResult invitationValidationResult = GetInvitationValidationResult(internalUser.DisplayName.Text, requestTrust);
			if (invitationValidationResult != null)
			{
				callback(invitationValidationResult);
				return null;
			}
			OutgoingFriendInvitation invitation = new OutgoingFriendInvitation(this, internalUser, requestTrust);
			AddOutgoingFriendInvitation(invitation);
			FriendInvitationSender.Send(logger, notificationQueue, mixWebCallFactory, internalUser.DisplayName.Text, requestTrust, delegate
			{
				callback(new SendFriendInvitationResult(true, invitation));
			}, delegate(ISendFriendInvitationResult r)
			{
				RemoveOutgoingFriendInvitation(invitation);
				callback(r);
			});
			return invitation;
		}

		private ISendFriendInvitationResult GetInvitationValidationResult(string displayName, bool requestTrust)
		{
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.DisplayName.Text == displayName);
			if (internalFriend != null)
			{
				if (internalFriend.IsTrusted)
				{
					logger.Critical("This user is already a trusted friend");
					return new SendFriendInvitationAlreadyFriendsResult(false, null);
				}
				if (!requestTrust)
				{
					logger.Critical("This user is already an untrusted friend");
					return new SendFriendInvitationAlreadyFriendsResult(false, null);
				}
			}
			IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = outgoingFriendInvitations.FirstOrDefault((IInternalOutgoingFriendInvitation i) => i.Invitee.DisplayName.Text == displayName);
			if (internalOutgoingFriendInvitation != null)
			{
				if (internalOutgoingFriendInvitation.RequestTrust)
				{
					logger.Critical("We already have a trusted invitation out for this user");
					return new SendFriendInvitationAlreadyExistsResult(false, null);
				}
				if (!requestTrust)
				{
					logger.Critical("We already have an untrusted invitation out for this user");
					return new SendFriendInvitationAlreadyExistsResult(false, null);
				}
			}
			return null;
		}

		public void AcceptFriendInvitation(IIncomingFriendInvitation invitation, bool acceptTrust, Action<IAcceptFriendInvitationResult> callback)
		{
			IInternalIncomingFriendInvitation internalIncomingFriendInvitation = incomingFriendInvitations.FirstOrDefault((IInternalIncomingFriendInvitation i) => i.Id == invitation.Id);
			if (internalIncomingFriendInvitation == null)
			{
				logger.Critical("Friend invitation to accept not found");
				callback(new AcceptFriendInvitationResult(false, null));
			}
			else
			{
				FriendInvitationAccepter.Accept(logger, notificationQueue, mixWebCallFactory, internalIncomingFriendInvitation.InvitationId, acceptTrust, delegate(AddFriendshipNotification notification)
				{
					IInternalFriend friend = friends.FirstOrDefault((IInternalFriend f) => f.Swid == notification.Friend.UserId);
					callback(new AcceptFriendInvitationResult(true, friend));
				}, delegate
				{
					callback(new AcceptFriendInvitationResult(false, null));
				});
			}
		}

		public void RejectFriendInvitation(IIncomingFriendInvitation invitation, Action<IRejectFriendInvitationResult> callback)
		{
			IInternalIncomingFriendInvitation internalInvitation = incomingFriendInvitations.FirstOrDefault((IInternalIncomingFriendInvitation i) => i.Id == invitation.Id);
			if (internalInvitation == null)
			{
				logger.Critical("Friend invitation to reject not found");
				callback(new RejectFriendInvitationResult(false));
			}
			else
			{
				FriendInvitationRejecter.Reject(logger, notificationQueue, mixWebCallFactory, internalInvitation.InvitationId, delegate
				{
					RemoveIncomingFriendInvitation(internalInvitation.InvitationId);
					internalInvitation.Rejected();
					callback(new RejectFriendInvitationResult(true));
				}, delegate
				{
					callback(new RejectFriendInvitationResult(false));
				});
			}
		}

		public void Unfriend(IFriend friend, Action<IUnfriendResult> callback)
		{
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.Id == friend.Id);
			if (internalFriend == null)
			{
				logger.Critical("Friend to unfriend not found");
				callback(new UnfriendResult(false));
			}
			else
			{
				Unfriender.Unfriend(logger, notificationQueue, mixWebCallFactory, internalFriend.Swid, delegate
				{
					callback(new UnfriendResult(true));
				}, delegate
				{
					callback(new UnfriendResult(false));
				});
			}
		}

		public void Untrust(IFriend trustedUser, Action<IUntrustResult> callback)
		{
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.Id == trustedUser.Id);
			if (internalFriend == null)
			{
				logger.Critical("Friend to untrust not found");
				callback(new UntrustResult(false));
			}
			else if (!internalFriend.IsTrusted)
			{
				logger.Critical("Friend is already untrusted");
				callback(new UntrustResult(false));
			}
			else
			{
				Untruster.Untrust(logger, notificationQueue, mixWebCallFactory, internalFriend.Swid, delegate
				{
					UntrustFriend(internalFriend);
					callback(new UntrustResult(true));
				}, delegate
				{
					callback(new UntrustResult(false));
				});
			}
		}

		public void GetRecommendedFriends(Action<IGetRecommendedFriendsResult> callback)
		{
			FriendRecommender.Recommend(logger, mixWebCallFactory, userDatabase, delegate(IEnumerable<IInternalUnidentifiedUser> internalUsers)
			{
				IUnidentifiedUser[] users = internalUsers.Select((Func<IInternalUnidentifiedUser, IUnidentifiedUser>)delegate(IInternalUnidentifiedUser user)
				{
					unidentifiedUsers.Add(user);
					return user;
				}).ToArray();
				callback(new GetRecommendedFriendResult(true, users));
			}, delegate
			{
				callback(new GetRecommendedFriendResult(false, Enumerable.Empty<IUnidentifiedUser>()));
			});
		}

		public void EnableAllPushNotifications(string token, string serviceType, string provisionId, Action<IEnableAllPushNotificationsResult> callback)
		{
			PushNotificationSettings.EnableAllPushNotifications(logger, database, mixWebCallFactory, token, serviceType, provisionId, Swid, delegate(IEnableAllPushNotificationsResult result)
			{
				if (result.Success)
				{
					this.OnPushNotificationsToggled(true);
				}
				callback(result);
			});
		}

		public void EnableInvisiblePushNotifications(string token, string serviceType, string provisionId, Action<IEnableInvisiblePushNotificationsResult> callback)
		{
			PushNotificationSettings.EnableInvisiblePushNotifications(logger, database, mixWebCallFactory, token, serviceType, provisionId, Swid, delegate(IEnableInvisiblePushNotificationsResult result)
			{
				if (result.Success)
				{
					this.OnPushNotificationsToggled(true);
				}
				callback(result);
			});
		}

		public void DisableAllPushNotifications(Action<IDisableAllPushNotificationsResult> callback)
		{
			PushNotificationSettings.DisableAllPushNotifications(logger, database, mixWebCallFactory, Swid, delegate(IDisableAllPushNotificationsResult result)
			{
				if (result.Success)
				{
					this.OnPushNotificationsToggled(false);
				}
				callback(result);
			});
		}

		public void DisableVisiblePushNotifications(Action<IDisableVisiblePushNotificationsResult> callback)
		{
			PushNotificationSettings.DisableVisiblePushNotifications(logger, database, mixWebCallFactory, Swid, delegate(IDisableVisiblePushNotificationsResult result)
			{
				if (result.Success)
				{
					this.OnPushNotificationsToggled(true);
				}
				callback(result);
			});
		}

		public IPushNotification ReceivePushNotification(IDictionary notification)
		{
			IInternalPushNotification internalPushNotification = PushNotificationReceiver.Receive(logger, encryptor, database, Swid, notification);
			this.OnPushNotificationReceived(internalPushNotification.NotificationsAvailable);
			return internalPushNotification;
		}

		public void TemporarilyBanAccount(Action<ITemporarilyBanAccountResult> callback)
		{
			AccountBanner.TemporarilyBan(logger, mixWebCallFactory, delegate(bool success)
			{
				callback(new TemporarilyBanAccountResult(success));
			});
		}

		public void SendMassPushNotification(Action<ISendMassPushNotificationResult> callback)
		{
			MassPushNotificationSender.SendMassPushNotification(logger, mixWebCallFactory, delegate(bool success)
			{
				callback(new SendMassPushNotificationResult(success));
			});
		}

		public void SendAlert(int level, AlertType type, Action<ISendAlertResult> callback)
		{
			try
			{
				IWebCall<TriggerAlertRequest, BaseResponse> webCall = mixWebCallFactory.IntegrationTestSupportUserAlertPost(new TriggerAlertRequest
				{
					Level = level.ToString(),
					Text = AlertTypeUtils.ToString(type)
				});
				webCall.OnResponse += delegate
				{
					callback(new SendAlertResult(true));
				};
				webCall.OnError += delegate
				{
					callback(new SendAlertResult(false));
				};
				webCall.Execute();
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new SendAlertResult(false));
			}
		}

		public void DispatchOnAlertsCleared(IEnumerable<IAlert> alerts)
		{
			this.OnAlertsCleared(this, new AlertsClearedEventArgs(alerts));
		}

		public void DispatchOnAlertsAdded(IAlert alert)
		{
			this.OnAlertsAdded(this, new AlertsAddedEventArgs(new IAlert[1]
			{
				alert
			}));
		}

		public void UpdateProfile(string firstName, string lastName, string displayName, string email, string parentEmail, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IUpdateProfileResult> callback)
		{
			ProfileUpdater.UpdateProfile(logger, guestControllerClient, database, Swid, epochTime, registrationProfile, firstName, lastName, displayName, email, parentEmail, dateOfBirth, marketingAgreements, acceptedLegalDocuments, callback);
		}

		public void RefreshProfile(Action<IRefreshProfileResult> callback)
		{
			ProfileGetter.GetProfile(logger, guestControllerClient, delegate(ProfileData profileData)
			{
				HandleRefreshProfile(profileData, callback);
			});
		}

		private void HandleRefreshProfile(ProfileData profileData, Action<IRefreshProfileResult> callback)
		{
			bool flag = profileData != null;
			if (flag)
			{
				flag = ProfileStorer.StoreProfile(logger, database, epochTime, Swid, profileData);
				registrationProfile.Update(profileData.profile, profileData.displayName, profileData.marketing);
			}
			callback(new RefreshProfileResult(flag));
		}

		public void SendParentalApprovalEmail(string langPref, Action<ISendParentalApprovalEmailResult> callback)
		{
			ParentalApprovalEmailSender.SendParentalApprovalEmail(logger, langPref, guestControllerClient, callback);
		}

		public void SendVerificationEmail(string languageCode, Action<ISendVerificationEmailResult> callback)
		{
			if (RegistrationProfile.EmailVerified)
			{
				callback(new SendVerificationEmailResult(true));
			}
			else
			{
				VerificationEmailSender.SendVerificationEmail(logger, languageCode, guestControllerClient, callback);
			}
		}

		public void ValidateDisplayName(string displayName, Action<IValidateDisplayNameResult> callback)
		{
			DisplayNameValidator.ValidateDisplayName(logger, guestControllerClient, mixWebCallFactory, displayName, callback);
		}

		public void ValidateDisplayNameV2(string displayName, Action<IValidateDisplayNameResult> callback)
		{
			DisplayNameValidator.ValidateDisplayNameV2(logger, mixWebCallFactory, displayName, callback);
		}

		public void ValidateDisplayNames(IEnumerable<string> displayNames, Action<IValidateDisplayNamesResult> callback)
		{
			string[] array = displayNames.ToArray();
			if (array.Any((string d) => d == null))
			{
				logger.Critical("Can't validate empty name");
				callback(new ValidateDisplayNamesResult(false, Enumerable.Empty<string>()));
			}
			else
			{
				DisplayNameValidator.ValidateDisplayNames(logger, mixWebCallFactory, array, callback);
			}
		}

		public void UpdateDisplayName(string displayName, Action<IUpdateDisplayNameResult> callback)
		{
			DisplayNameUpdater.UpdateDisplayName(logger, mixWebCallFactory, displayName, delegate(IUpdateDisplayNameResult r)
			{
				try
				{
					if (r.Success)
					{
						DisplayName = new DisplayName(displayName);
						this.OnDisplayNameUpdated(displayName);
						UserDocument userBySwid = userDatabase.GetUserBySwid(Swid);
						userBySwid.DisplayName = displayName;
						userDatabase.UpdateUserDocument(userBySwid);
						registrationProfile.UpdateDisplayName(displayName);
					}
					callback(r);
				}
				catch (Exception arg)
				{
					logger.Critical("Unhandled exception: " + arg);
					callback(new UpdateDisplayNameResult(false));
				}
			});
		}

		public void SetLanguagePreference(string languageCode, Action<ISetLangaugePreferenceResult> callback)
		{
			try
			{
				IWebCall<SetLanguageRequest, BaseResponse> webCall = mixWebCallFactory.LanguagePreferencePost(new SetLanguageRequest
				{
					LanguageCode = languageCode
				});
				webCall.OnResponse += delegate
				{
					callback(new SetLangaugePreferenceResult(true));
				};
				webCall.OnError += delegate
				{
					logger.Critical("Error setting language preference");
					callback(new SetLangaugePreferenceResult(false));
				};
				webCall.Execute();
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new SetLangaugePreferenceResult(false));
			}
		}

		public void ModerateText(string text, bool isTrusted, Action<ITextModerationResult> callback)
		{
			TextModerator.ModerateText(logger, mixWebCallFactory, text, isTrusted, delegate(ModerateTextResponse response)
			{
				callback(new TextModerationResult(true, response.Moderated.Value, response.Text));
			}, delegate
			{
				callback(new TextModerationResult(false, false, null));
			});
		}

		public void ReportUser(IFriend user, ReportUserReason reportUserReason, Action<IReportUserResult> callback)
		{
			string userId = null;
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.Id == user.Id);
			if (internalFriend != null)
			{
				userId = internalFriend.Swid;
			}
			ReportUser(userId, reportUserReason, callback);
		}

		private void ReportUser(string userId, ReportUserReason reportUserReason, Action<IReportUserResult> callback)
		{
			if (userId == null)
			{
				logger.Critical("Can not report this user as we do not know their ID!");
				callback(new ReportUserResult(false));
			}
			else
			{
				UserReporter.Report(logger, mixWebCallFactory, userId, reportUserReason, delegate
				{
					callback(new ReportUserResult(true));
				}, delegate
				{
					callback(new ReportUserResult(false));
				});
			}
		}

		public void AddIncomingFriendInvitation(IInternalIncomingFriendInvitation invitation)
		{
			if (!incomingFriendInvitations.Any((IInternalIncomingFriendInvitation i) => i.Id == invitation.Id))
			{
				incomingFriendInvitations.Add(invitation);
				ReceivedIncomingFriendInvitationEventArgs e = new ReceivedIncomingFriendInvitationEventArgs(invitation);
				this.OnReceivedIncomingFriendInvitation(this, e);
			}
		}

		public void AddOutgoingFriendInvitation(IInternalOutgoingFriendInvitation invitation)
		{
			IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = outgoingFriendInvitations.FirstOrDefault((IInternalOutgoingFriendInvitation i) => i.InternalInvitee.DisplayName.Text == invitation.InternalInvitee.DisplayName.Text);
			IOutgoingFriendInvitation outgoingFriendInvitation = null;
			if (internalOutgoingFriendInvitation == null)
			{
				outgoingFriendInvitations.Add(invitation);
				outgoingFriendInvitation = invitation;
			}
			else if (!internalOutgoingFriendInvitation.Sent && invitation.Sent)
			{
				internalOutgoingFriendInvitation.SendComplete(invitation.InvitationId);
				outgoingFriendInvitation = internalOutgoingFriendInvitation;
			}
			if (invitation.Sent && outgoingFriendInvitation != null)
			{
				ReceivedOutgoingFriendInvitationEventArgs e = new ReceivedOutgoingFriendInvitationEventArgs(outgoingFriendInvitation);
				this.OnReceivedOutgoingFriendInvitation(this, e);
			}
		}

		public void ClearAlerts(IEnumerable<IAlert> alerts, Action<IClearAlertsResult> callback)
		{
			try
			{
				List<IInternalAlert> list = new List<IInternalAlert>();
				foreach (IAlert alert in alerts)
				{
					IInternalAlert internalAlert = alert as IInternalAlert;
					if (internalAlert == null)
					{
						logger.Critical("Can't clear unknown alert");
						callback(new ClearAlertsResult(false));
						return;
					}
					list.Add(internalAlert);
				}
				IWebCall<ClearAlertsRequest, ClearAlertsResponse> webCall = mixWebCallFactory.AlertsClearPut(new ClearAlertsRequest
				{
					AlertIds = list.Select((IInternalAlert a) => a.AlertId).Cast<long?>().ToList()
				});
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<ClearAlertsResponse> e)
				{
					ClearAlertsResponse response = e.Response;
					if (NotificationValidator.Validate(response.Notification))
					{
						notificationQueue.Dispatch(response.Notification, delegate
						{
							callback(new ClearAlertsResult(true));
						}, delegate
						{
							callback(new ClearAlertsResult(false));
						});
					}
					else
					{
						logger.Critical("Failed to validate clear alerts response: " + JsonParser.ToJson(response));
						callback(new ClearAlertsResult(false));
					}
				};
				webCall.OnError += delegate
				{
					callback(new ClearAlertsResult(false));
				};
				webCall.Execute();
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new ClearAlertsResult(false));
			}
		}

		private IInternalIncomingFriendInvitation RemoveIncomingFriendInvitation(long invitationId)
		{
			oldInvitationIds.Add(invitationId);
			IInternalIncomingFriendInvitation internalIncomingFriendInvitation = incomingFriendInvitations.FirstOrDefault((IInternalIncomingFriendInvitation i) => i.InvitationId == invitationId);
			if (internalIncomingFriendInvitation != null)
			{
				incomingFriendInvitations.Remove(internalIncomingFriendInvitation);
			}
			return internalIncomingFriendInvitation;
		}

		private IInternalOutgoingFriendInvitation RemoveOutgoingFriendInvitation(long invitationId)
		{
			oldInvitationIds.Add(invitationId);
			IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = outgoingFriendInvitations.FirstOrDefault((IInternalOutgoingFriendInvitation i) => i.InvitationId == invitationId);
			if (internalOutgoingFriendInvitation != null)
			{
				outgoingFriendInvitations.Remove(internalOutgoingFriendInvitation);
			}
			return internalOutgoingFriendInvitation;
		}

		public void AddFriendshipInvitation(FriendshipInvitation invitation, User friend)
		{
			long invitationId = invitation.FriendshipInvitationId.Value;
			if (oldInvitationIds.Contains(invitationId) || outgoingFriendInvitations.Any((IInternalOutgoingFriendInvitation i) => i.InvitationId == invitationId) || incomingFriendInvitations.Any((IInternalIncomingFriendInvitation i) => i.InvitationId == invitationId))
			{
				return;
			}
			if (invitation.IsInviter.Value)
			{
				IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = outgoingFriendInvitations.FirstOrDefault((IInternalOutgoingFriendInvitation i) => i.Invitee.DisplayName.Text == invitation.FriendDisplayName && i.RequestTrust != invitation.IsTrusted);
				if (internalOutgoingFriendInvitation != null)
				{
					RemoveOutgoingFriendInvitation(internalOutgoingFriendInvitation.InvitationId);
				}
				IInternalUnidentifiedUser invitee = RemoteUserFactory.CreateUnidentifiedUser(friend.DisplayName, friend.FirstName, userDatabase);
				OutgoingFriendInvitation outgoingFriendInvitation = new OutgoingFriendInvitation(this, invitee, invitation.IsTrusted.Value);
				outgoingFriendInvitation.SendComplete(invitationId);
				AddOutgoingFriendInvitation(outgoingFriendInvitation);
			}
			else
			{
				IInternalIncomingFriendInvitation internalIncomingFriendInvitation = incomingFriendInvitations.FirstOrDefault((IInternalIncomingFriendInvitation i) => i.Inviter.DisplayName.Text == invitation.FriendDisplayName && i.RequestTrust != invitation.IsTrusted);
				if (internalIncomingFriendInvitation != null)
				{
					RemoveIncomingFriendInvitation(internalIncomingFriendInvitation.InvitationId);
				}
				IInternalUnidentifiedUser inviter = RemoteUserFactory.CreateUnidentifiedUser(friend.DisplayName, friend.FirstName, userDatabase);
				IncomingFriendInvitation incomingFriendInvitation = new IncomingFriendInvitation(inviter, this, invitation.IsTrusted.Value);
				incomingFriendInvitation.SendComplete(invitationId);
				AddIncomingFriendInvitation(incomingFriendInvitation);
			}
		}

		public void RemoveFriendshipInvitation(long invitationId)
		{
			IInternalIncomingFriendInvitation internalIncomingFriendInvitation = RemoveIncomingFriendInvitation(invitationId);
			if (internalIncomingFriendInvitation != null)
			{
				internalIncomingFriendInvitation.Rejected();
				return;
			}
			IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = RemoveOutgoingFriendInvitation(invitationId);
			if (internalOutgoingFriendInvitation != null)
			{
				internalOutgoingFriendInvitation.Rejected();
			}
		}

		public void AddFriend(User domainFriend, bool isTrusted, long invitationId)
		{
			IInternalFriend friend = RemoteUserFactory.CreateFriend(domainFriend.UserId, isTrusted, domainFriend.DisplayName, domainFriend.FirstName, userDatabase);
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.Id == friend.Id && f.IsTrusted != friend.IsTrusted);
			if (internalFriend != null)
			{
				friends.Remove(internalFriend);
			}
			if (friends.Any((IInternalFriend f) => f.Id == friend.Id))
			{
				return;
			}
			friends.Add(friend);
			IInternalIncomingFriendInvitation internalIncomingFriendInvitation = RemoveIncomingFriendInvitation(invitationId);
			if (internalIncomingFriendInvitation != null)
			{
				internalIncomingFriendInvitation.Accepted(isTrusted, friend);
				return;
			}
			IInternalOutgoingFriendInvitation internalOutgoingFriendInvitation = RemoveOutgoingFriendInvitation(invitationId);
			if (internalOutgoingFriendInvitation != null)
			{
				internalOutgoingFriendInvitation.Accepted(isTrusted, friend);
			}
		}

		public void RemoveFriend(string friendSwid, bool sendEvent)
		{
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.Swid == friendSwid);
			friends.Remove(internalFriend);
			if (internalFriend != null && sendEvent)
			{
				UnfriendedEventArgs e = new UnfriendedEventArgs(internalFriend);
				this.OnUnfriended(this, e);
			}
		}

		public void UntrustFriend(string friendSwid)
		{
			IInternalFriend internalFriend = friends.FirstOrDefault((IInternalFriend f) => f.Swid == friendSwid);
			if (internalFriend != null)
			{
				UntrustFriend(internalFriend);
			}
		}

		public void UntrustFriend(IInternalFriend friend)
		{
			friend.ChangeTrust(false);
			UntrustedEventArgs e = new UntrustedEventArgs(friend);
			this.OnUntrusted(this, e);
		}

		public void AddFriend(IInternalFriend friend)
		{
			friends.Add(friend);
		}

		public void RemoveFriend(IInternalFriend friend)
		{
			friends.Remove(friend);
			this.OnUnfriended(this, new UnfriendedEventArgs(friend));
		}

		public void RemoveIncomingFriendInvitation(IInternalIncomingFriendInvitation invitation)
		{
			oldInvitationIds.Add(invitation.InvitationId);
			incomingFriendInvitations.Remove(invitation);
		}

		public void RemoveOutgoingFriendInvitation(IInternalOutgoingFriendInvitation invitation)
		{
			oldInvitationIds.Add(invitation.InvitationId);
			outgoingFriendInvitations.Remove(invitation);
		}

		public void GetAdultVerificationRequirements(Action<IGetAdultVerificationRequirementsResult> callback)
		{
			AdultVerificationRequirementsGetter.GetRequirements(logger, registrationProfile.CountryCode, mixWebCallFactory, delegate(bool required, bool available)
			{
				callback(new GetAdultVerificationRequirementsResult(true, required, available));
			}, delegate
			{
				callback(new GetAdultVerificationRequirementsResult(false, false, false));
			});
		}

		public void GetAdultVerificationStatus(Action<IGetAdultVerificationStatusResult> callback)
		{
			AdultVerificationStatusGetter.GetAdultVerificationStatus(logger, guestControllerClient, callback);
		}

		public void GetVerifyAdultForm(Action<IGetVerifyAdultFormResult> callback)
		{
			VerifyAdultFormUnitedStates form = new VerifyAdultFormUnitedStates();
			callback(new GetVerifyAdultFormResult(form));
		}

		public void VerifyAdult(IVerifyAdultFormUnitedStates form, Action<IVerifyAdultResult> callback)
		{
			if (registrationProfile.IsAdultVerified)
			{
				callback(new VerifyAdultResult(true, false));
			}
			else if (AgeBandType != AgeBandType.Adult)
			{
				callback(new VerifyAdultFailedNotAdultResult());
			}
			else if (string.IsNullOrEmpty(form.FirstName) || string.IsNullOrEmpty(form.LastName) || string.IsNullOrEmpty(form.PostalCode))
			{
				callback(new VerifyAdultFailedMissingInfoResult());
			}
			else
			{
				AdultVerifierUnitedStates.VerifyAdult(logger, guestControllerClient, form, delegate(IVerifyAdultResult r)
				{
					if (r.Success)
					{
						registrationProfile.IsAdultVerified = true;
					}
					callback(r);
				});
			}
		}

		public void AnswerVerifyAdultQuiz(IVerifyAdultQuizAnswers answers, Action<IVerifyAdultResult> callback)
		{
			VerifyAdultQuizAnswersSender.AnswerQuiz(logger, guestControllerClient, answers, delegate(IVerifyAdultResult r)
			{
				if (r.Success)
				{
					registrationProfile.IsAdultVerified = true;
				}
				callback(r);
			});
		}

		public void GetClaimableChildren(Action<IGetLinkedUsersResult> callback)
		{
			if (!registrationProfile.EmailVerified)
			{
				callback(new GetLinkedUsersFailedEmailNotVerifiedResult());
			}
			else if (AgeBandType != AgeBandType.Adult)
			{
				callback(new GetLinkedUsersFailedNotAdultResult());
			}
			else
			{
				ClaimableChildrenGetter.GetChildren(logger, guestControllerClient, mixWebCallFactory, callback);
			}
		}

		public void LinkChildAccount(ISession child, Action<ILinkChildResult> callback)
		{
			if (!(child is IInternalSession))
			{
				callback(new LinkChildResult(false));
				return;
			}
			if (child.LocalUser.AgeBandType != AgeBandType.Child)
			{
				callback(new LinkChildFailedNotChildResult());
				return;
			}
			if (AgeBandType != AgeBandType.Adult)
			{
				callback(new LinkChildFailedNotAdultResult());
				return;
			}
			IInternalSession internalSession = (IInternalSession)child;
			ChildLinker.LinkChild(logger, guestControllerClient, internalSession.InternalLocalUser.Swid, internalSession.GuestControllerAccessToken, callback);
		}

		public void LinkClaimableChildAccounts(IEnumerable<ILinkedUser> children, Action<ILinkChildResult> callback)
		{
			if (AgeBandType != AgeBandType.Adult)
			{
				callback(new LinkChildFailedNotAdultResult());
				return;
			}
			List<string> list = new List<string>();
			foreach (ILinkedUser child in children)
			{
				if (!(child is IInternalLinkedUser))
				{
					callback(new LinkChildResult(false));
					return;
				}
				list.Add(((IInternalLinkedUser)child).Swid);
			}
			ChildLinker.LinkClaimableChildren(logger, guestControllerClient, list, callback);
		}

		public void GetLinkedChildren(Action<IGetLinkedUsersResult> callback)
		{
			if (AgeBandType != AgeBandType.Adult)
			{
				callback(new GetLinkedUsersFailedNotAdultResult());
			}
			else
			{
				LinkedChildrenGetter.GetChildren(logger, guestControllerClient, mixWebCallFactory, callback);
			}
		}

		public void GetLinkedGuardians(Action<IGetLinkedUsersResult> callback)
		{
			LinkedGuardiansGetter.GetGuardians(logger, guestControllerClient, mixWebCallFactory, callback);
		}

		public void RequestTrustPermission(Action<IPermissionResult> callback)
		{
			if (AgeBandType == AgeBandType.Adult || AgeBandType == AgeBandType.Teen)
			{
				callback(new PermissionNotRequiredResult());
			}
			else
			{
				PermissionRequester.RequestPermission(logger, guestControllerClient, "MIX_TRUSTEDFRIENDSCOMMUNICATIONS", callback);
			}
		}

		public void RequestTrustPermissionForChild(ILinkedUser child, Action<IPermissionResult> callback)
		{
			if (AgeBandType != AgeBandType.Adult)
			{
				callback(new PermissionFailedNotAdultResult());
				return;
			}
			IInternalLinkedUser internalLinkedUser = child as IInternalLinkedUser;
			if (internalLinkedUser == null)
			{
				callback(new PermissionResult(false, ActivityApprovalStatus.Unknown));
			}
			else
			{
				PermissionRequester.RequestPermissionForChild(logger, guestControllerClient, "MIX_TRUSTEDFRIENDSCOMMUNICATIONS", internalLinkedUser.Swid, callback);
			}
		}

		public void ApproveChildTrustPermission(ISession child, ActivityApprovalStatus status, Action<IPermissionResult> callback)
		{
			ApproveChildTrustPermission(child.LocalUser.Id, child.LocalUser.AgeBandType, status, callback);
		}

		public void ApproveChildTrustPermission(ILinkedUser child, ActivityApprovalStatus status, Action<IPermissionResult> callback)
		{
			IInternalLinkedUser internalLinkedUser = child as IInternalLinkedUser;
			if (internalLinkedUser == null)
			{
				callback(new PermissionFailedInvalidResult());
			}
			else
			{
				ApproveChildTrustPermission(internalLinkedUser.Swid, child.AgeBand, status, callback);
			}
		}

		private void ApproveChildTrustPermission(string childSwid, AgeBandType childAgeBand, ActivityApprovalStatus status, Action<IPermissionResult> callback)
		{
			if (AgeBandType != AgeBandType.Adult)
			{
				callback(new PermissionFailedNotAdultResult());
			}
			else if (childAgeBand == AgeBandType.Adult || childAgeBand == AgeBandType.Teen)
			{
				callback(new PermissionNotRequiredResult());
			}
			else
			{
				PermissionApprover.ApprovePermission(logger, guestControllerClient, "MIX_TRUSTEDFRIENDSCOMMUNICATIONS", childSwid, status, callback);
			}
		}

		public void GetChildTrustPermission(ISession child, Action<IPermissionResult> callback)
		{
			GetChildTrustPermission(child.LocalUser.Id, child.LocalUser.AgeBandType, callback);
		}

		public void GetChildTrustPermission(ILinkedUser child, Action<IPermissionResult> callback)
		{
			IInternalLinkedUser internalLinkedUser = child as IInternalLinkedUser;
			if (internalLinkedUser == null)
			{
				callback(new PermissionFailedInvalidResult());
			}
			else
			{
				GetChildTrustPermission(internalLinkedUser.Swid, internalLinkedUser.AgeBand, callback);
			}
		}

		private void GetChildTrustPermission(string childSwid, AgeBandType childAgeBand, Action<IPermissionResult> callback)
		{
			if (childAgeBand == AgeBandType.Adult || childAgeBand == AgeBandType.Teen)
			{
				callback(new PermissionNotRequiredResult());
			}
			else
			{
				PermissionGetter.GetPermission(logger, guestControllerClient, "MIX_TRUSTEDFRIENDSCOMMUNICATIONS", childSwid, callback);
			}
		}
	}
}
