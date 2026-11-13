using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class GuestControllerErrorParser
	{
		private static readonly List<KeyValuePair<string, Type>> registerErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("GUEST_GATED_EMBARGOED", typeof(RegisterEmbargoedCountryResult)),
			new KeyValuePair<string, Type>("GUEST_GATED_LOCATION", typeof(RegisterGatedLocationResult)),
			new KeyValuePair<string, Type>("RATE_LIMITED", typeof(RegisterRateLimitedResult))
		};

		private static readonly List<KeyValuePair<string, Type>> registerProfileInputNameErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("profile.email", typeof(InvalidEmailError)),
			new KeyValuePair<string, Type>("profile.parentEmail", typeof(InvalidParentEmailError)),
			new KeyValuePair<string, Type>("profile.firstName", typeof(InvalidFirstNameError)),
			new KeyValuePair<string, Type>("profile.lastName", typeof(InvalidLastNameError)),
			new KeyValuePair<string, Type>("GTOU", typeof(InvalidTermsOfUseError)),
			new KeyValuePair<string, Type>("ppV2", typeof(InvalidPrivacyPolicyError)),
			new KeyValuePair<string, Type>("password", typeof(InvalidPasswordError)),
			new KeyValuePair<string, Type>("profile.username", typeof(InvalidUsernameError)),
			new KeyValuePair<string, Type>("displayName.proposedDisplayName", typeof(InvalidDisplayNameError)),
			new KeyValuePair<string, Type>("profile.authCredential.password", typeof(PasswordMatchesOtherProfileInfoError))
		};

		private static readonly List<KeyValuePair<string, Type>> registerProfileCodeErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("UNABLE_TO_SEND_PARENT_NOTIFICATION", typeof(InvalidParentEmailError)),
			new KeyValuePair<string, Type>("PARENT_EMAIL_BLACKLISTED", typeof(ParentEmailBannedError)),
			new KeyValuePair<string, Type>("SERVICE_ERROR", typeof(InvalidEmailError)),
			new KeyValuePair<string, Type>("INVALID_PASSWORD_USING_PROFILE_INFORMATION", typeof(PasswordUsesProfileInformationError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_LIKE_PHONE_NUMBER", typeof(PasswordLikePhoneNumberError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_MISSING_EXPECTED_CHARS", typeof(PasswordMissingExpectedCharactersError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_SIZE", typeof(PasswordSizeError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_TOO_COMMON", typeof(PasswordTooCommonError))
		};

		private static readonly List<KeyValuePair<string, Type>> loginErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("MISSING_VALUE", typeof(LoginMissingInfoResult)),
			new KeyValuePair<string, Type>("PPU_MARKETING", typeof(LoginRequiresLegalMarketingUpdateResult)),
			new KeyValuePair<string, Type>("PPU_LEGAL", typeof(LoginRequiresLegalMarketingUpdateResult)),
			new KeyValuePair<string, Type>("PPU_SECURITY", typeof(LoginSecurityUpdateResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_MASE_ERROR", typeof(LoginFailedMultipleAccountsResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_PARENTAL_CONSENT_REQUIRED", typeof(LoginFailedParentalConsentResult)),
			new KeyValuePair<string, Type>("PROFILE_USER_BANNED", typeof(LoginFailedTemporaryBanResult)),
			new KeyValuePair<string, Type>("PROFILE_DISABLED", typeof(LoginFailedProfileDisabledResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_ACCOUNT_LOCKED_OUT", typeof(LoginFailedAccountLockedOutResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_CREDENTIALS", typeof(LoginFailedAuthenticationResult)),
			new KeyValuePair<string, Type>("MALFORMED_INPUT", typeof(LoginFailedAuthenticationResult)),
			new KeyValuePair<string, Type>("RATE_LIMITED", typeof(LoginFailedRateLimitedResult)),
			new KeyValuePair<string, Type>("GUEST_GATED_LOCATION", typeof(LoginFailedGatedLocationResult))
		};

		private static readonly List<KeyValuePair<string, Type>> validateErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("GUEST_GATED_LOCATION", typeof(ValidateNewAccountGatedLocationResult)),
			new KeyValuePair<string, Type>("GUEST_GATED_EMBARGOED", typeof(ValidateNewAccountEmbargoedCountryResult))
		};

		private static readonly List<KeyValuePair<string, Type>> regConfigErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("GUEST_GATED_EMBARGOED", typeof(GetRegistrationConfigurationEmbargoedCountryResult))
		};

		private static readonly List<KeyValuePair<string, Type>> refreshErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("AUTHORIZATION_CREDENTIALS", typeof(RefreshAuthenticationFailedError)),
			new KeyValuePair<string, Type>("MALFORMED_INPUT", typeof(RefreshAuthenticationFailedError)),
			new KeyValuePair<string, Type>("PPU_MARKETING", typeof(RefreshRequiresLegalMarketingUpdateError)),
			new KeyValuePair<string, Type>("PPU_LEGAL", typeof(RefreshRequiresLegalMarketingUpdateError)),
			new KeyValuePair<string, Type>("AUTHORIZATION_PARENTAL_CONSENT_REQUIRED", typeof(RefreshParentalConsentError)),
			new KeyValuePair<string, Type>("PROFILE_USER_BANNED", typeof(RefreshTemporaryBanError)),
			new KeyValuePair<string, Type>("PROFILE_DISABLED", typeof(RefreshProfileDisabledError)),
			new KeyValuePair<string, Type>("AUTHORIZATION_ACCOUNT_LOCKED_OUT", typeof(RefreshAccountLockedOutError)),
			new KeyValuePair<string, Type>("AUTHORIZATION_INVALID_REFRESH_TOKEN", typeof(RefreshInvalidRefreshTokenError)),
			new KeyValuePair<string, Type>("AUTHORIZATION_INVALID_OR_EXPIRED_TOKEN", typeof(RefreshInvalidAccessTokenError)),
			new KeyValuePair<string, Type>("GUEST_GATED_LOCATION", typeof(RefreshTokenGatedLocationError))
		};

		private static readonly List<KeyValuePair<string, Type>> restoreErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("AUTHORIZATION_CREDENTIALS", typeof(RestoreLastSessionFailedAuthenticationResult)),
			new KeyValuePair<string, Type>("MALFORMED_INPUT", typeof(RestoreLastSessionFailedAuthenticationResult)),
			new KeyValuePair<string, Type>("MISSING_VALUE", typeof(RestoreLastSessionSuccessMissingInfoResult)),
			new KeyValuePair<string, Type>("PPU_MARKETING", typeof(RestoreLastSessionSuccessRequiresLegalMarketingUpdateResult)),
			new KeyValuePair<string, Type>("PPU_LEGAL", typeof(RestoreLastSessionSuccessRequiresLegalMarketingUpdateResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_MASE_ERROR", typeof(RestoreLastSessionFailedMultipleAccountsResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_PARENTAL_CONSENT_REQUIRED", typeof(RestoreLastSessionFailedParentalConsentResult)),
			new KeyValuePair<string, Type>("PROFILE_USER_BANNED", typeof(RestoreLastSessionFailedTemporaryBanResult)),
			new KeyValuePair<string, Type>("PROFILE_DISABLED", typeof(RestoreLastSessionFailedProfileDisabledResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_ACCOUNT_LOCKED_OUT", typeof(RestoreLastSessionFailedAccountLockedOutResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_INVALID_REFRESH_TOKEN", typeof(RestoreLastSessionFailedInvalidRefreshTokenResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_INVALID_OR_EXPIRED_TOKEN", typeof(RestoreLastSessionFailedInvalidOrExpiredTokenResult))
		};

		private static readonly List<KeyValuePair<string, Type>> validationErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_MISSING_EXPECTED_CHARS", typeof(ValidateNewAccountPassswordMissingCharactersError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_SIZE", typeof(ValidateNewAccountPasswordSizeError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_TOO_COMMON", typeof(ValidateNewAccountPasswordCommonError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_PASSWORD_LIKE_PHONE_NUMBER", typeof(ValidateNewAccountPasswordPhoneError)),
			new KeyValuePair<string, Type>("ACCOUNT_FOUND", typeof(ValidateNewAccountAccountFoundError)),
			new KeyValuePair<string, Type>("INUSE_VALUE", typeof(ValidateNewAccountAccountInUseError)),
			new KeyValuePair<string, Type>("AUTHORIZATION_MASE_ERROR", typeof(ValidateNewAccountMultipleAccountsError)),
			new KeyValuePair<string, Type>("DISALLOWED_VALUE", typeof(ValidateNewAccountUsernameError)),
			new KeyValuePair<string, Type>("INVALID_VALUE", typeof(ValidateNewAccountUsernameError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_FILTHY", typeof(ValidateNewAccountUsernameError)),
			new KeyValuePair<string, Type>("INVALID_VALUE_USERNAME_SIZE", typeof(ValidateNewAccountUsernameError)),
			new KeyValuePair<string, Type>("NRT_ACCOUNT", typeof(ValidateNewAccountNotRegisteredTransactorError)),
			new KeyValuePair<string, Type>("INVALID_OR_MALFORMED_REQUEST", typeof(ValidateNewAccountNothingToValidateError)),
			new KeyValuePair<string, Type>("RATE_LIMITED", typeof(ValidateNewAccountRateLimitedError))
		};

		private static readonly List<KeyValuePair<string, Type>> verifyAdultErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("VERIFICATION_ADDITIONAL_INFORMATION_REQUIRED", typeof(VerifyAdultFailedQuestionsResult)),
			new KeyValuePair<string, Type>("VERIFICATION_FAILED", typeof(VerifyAdultFailedMaximumAttemptsResult)),
			new KeyValuePair<string, Type>("MISSING_VALUE", typeof(VerifyAdultFailedMissingInfoResult)),
			new KeyValuePair<string, Type>("INVALID_DATA", typeof(VerifyAdultFailedInvalidDataResult)),
			new KeyValuePair<string, Type>("DISALLOWED_VALUE", typeof(VerifyAdultFailedNotAdultResult)),
			new KeyValuePair<string, Type>("SWID_AUTHORIZATION_FAILED", typeof(VerifyAdultFailedInvalidDataResult))
		};

		private static readonly List<KeyValuePair<string, Type>> linkChildErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("OPC_MAX_PARENTS", typeof(LinkChildFailedMaxParentsResult)),
			new KeyValuePair<string, Type>("FORBIDDEN_VALUE", typeof(LinkChildFailedNotChildResult))
		};

		private static readonly List<KeyValuePair<string, Type>> permissionErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("ACTIVITY_PERMISSION_ALREADY_APPROVED", typeof(PermissionAlreadyApprovedResult)),
			new KeyValuePair<string, Type>("ACTIVITY_PERMISSION_ALREADY_DENIED", typeof(PermissionFailedAlreadyDeniedResult)),
			new KeyValuePair<string, Type>("ACTIVITY_PERMISSION_NOT_REQUIRED", typeof(PermissionNotRequiredResult)),
			new KeyValuePair<string, Type>("ACTIVITY_PERMISSION_PARENTAL_VERIFICATION", typeof(PermissionFailedNotVerifiedAsAdultResult)),
			new KeyValuePair<string, Type>("ACTIVITY_PERMISSION_NOT_FOUND", typeof(PermissionFailedNotFoundResult)),
			new KeyValuePair<string, Type>("ACTIVITY_PERMISSION_INVALID", typeof(PermissionFailedInvalidResult)),
			new KeyValuePair<string, Type>("AUTHORIZATION_CREDENTIALS", typeof(PermissionFailedInvalidResult))
		};

		private static readonly List<KeyValuePair<string, Type>> recoverUsernameErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("RATE_LIMITED", typeof(SendUsernameRecoveryRateLimitedResult))
		};

		private static readonly List<KeyValuePair<string, Type>> recoverPasswordErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("RATE_LIMITED", typeof(SendPasswordRecoveryRateLimitedResult))
		};

		private static readonly List<KeyValuePair<string, Type>> upgradeNrtErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("RATE_LIMITED", typeof(SendNonRegisteredTransactorUpgradeRateLimitedResult))
		};

		private static readonly List<KeyValuePair<string, Type>> resolveMaseErrors = new List<KeyValuePair<string, Type>>
		{
			new KeyValuePair<string, Type>("RATE_LIMITED", typeof(SendMultipleAccountsResolutionRateLimitedResult))
		};

		public static IEnumerable<IValidateNewAccountError> GetValidationErrors(GuestApiErrorCollection collection)
		{
			List<IValidateNewAccountError> list = new List<IValidateNewAccountError>();
			foreach (GuestApiError apiError in collection.errors)
			{
				if (apiError != null && apiError.code != null)
				{
					KeyValuePair<string, Type> keyValuePair = validationErrors.FirstOrDefault((KeyValuePair<string, Type> p) => p.Key == apiError.code);
					if (keyValuePair.Key != null)
					{
						if (keyValuePair.Key == "DISALLOWED_VALUE")
						{
							string key = registerProfileInputNameErrors.FirstOrDefault((KeyValuePair<string, Type> p) => p.Key == apiError.inputName).Key;
							if (key != null && key != "profile.username")
							{
								continue;
							}
						}
						IValidateNewAccountError item = (IValidateNewAccountError)Activator.CreateInstance(keyValuePair.Value);
						list.Add(item);
					}
				}
			}
			return (list.Count == 0) ? null : list;
		}

		public static IList<IInvalidProfileItemError> GetRegisterProfileItemErrors(GuestApiErrorCollection collection)
		{
			if (collection == null)
			{
				return null;
			}
			List<IInvalidProfileItemError> list = new List<IInvalidProfileItemError>();
			foreach (GuestApiError apiError in collection.errors)
			{
				if (apiError != null)
				{
					if (apiError.inputName != null)
					{
						KeyValuePair<string, Type> keyValuePair = registerProfileInputNameErrors.FirstOrDefault((KeyValuePair<string, Type> p) => p.Key == apiError.inputName);
						if (keyValuePair.Key != null)
						{
							object[] args = new object[1]
							{
								apiError.developerMessage
							};
							list.Add((IInvalidProfileItemError)Activator.CreateInstance(keyValuePair.Value, args));
						}
					}
					if (apiError.code != null)
					{
						KeyValuePair<string, Type> keyValuePair2 = registerProfileCodeErrors.FirstOrDefault((KeyValuePair<string, Type> p) => p.Key == apiError.code);
						if (keyValuePair2.Key != null)
						{
							object[] args = new object[1]
							{
								apiError.developerMessage
							};
							list.Add((IInvalidProfileItemError)Activator.CreateInstance(keyValuePair2.Value, args));
						}
					}
				}
			}
			if (list.Count == 0)
			{
				list = null;
			}
			return list;
		}

		public static IRegisterResult GetRegisterResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<IRegisterResult>(error, registerErrors);
		}

		public static ILoginResult GetLoginResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<ILoginResult>(error, loginErrors);
		}

		public static IValidateNewAccountResult GetValidateResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<IValidateNewAccountResult>(error, validateErrors);
		}

		public static IInternalGetRegistrationConfigurationResult GetRegConfigResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<IInternalGetRegistrationConfigurationResult>(error, regConfigErrors);
		}

		public static IRestoreLastSessionResult GetRestoreLastSessionResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<IRestoreLastSessionResult>(error, restoreErrors);
		}

		public static IRefreshGuestControllerTokenError GetGuestControllerTokenRefreshError(GuestApiErrorCollection error)
		{
			return GetWorstResult<IRefreshGuestControllerTokenError>(error, refreshErrors);
		}

		public static IVerifyAdultResult GetVerifyAdultResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<IVerifyAdultResult>(error, verifyAdultErrors);
		}

		public static ILinkChildResult GetLinkChildResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<ILinkChildResult>(error, linkChildErrors);
		}

		public static IPermissionResult GetPermissionResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<IPermissionResult>(error, permissionErrors);
		}

		public static ISendUsernameRecoveryResult GetRecoverUsernameResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<ISendUsernameRecoveryResult>(error, recoverUsernameErrors);
		}

		public static ISendPasswordRecoveryResult GetRecoverPasswordResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<ISendPasswordRecoveryResult>(error, recoverPasswordErrors);
		}

		public static ISendNonRegisteredTransactorUpgradeResult GetUpgradeNrtResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<ISendNonRegisteredTransactorUpgradeResult>(error, upgradeNrtErrors);
		}

		public static ISendMultipleAccountsResolutionResult GetResolveMaseResult(GuestApiErrorCollection error)
		{
			return GetWorstResult<ISendMultipleAccountsResolutionResult>(error, resolveMaseErrors);
		}

		public static string GetAccountStatusCode(GuestApiErrorCollection error, string currentStatusCode)
		{
			IRefreshGuestControllerTokenError guestControllerTokenRefreshError = GetGuestControllerTokenRefreshError(error);
			if (guestControllerTokenRefreshError is IRefreshParentalConsentError)
			{
				if (currentStatusCode == "ACTIVE")
				{
					return "AWAIT_PARENT_CONSENT";
				}
			}
			else if (currentStatusCode == "AWAIT_PARENT_CONSENT")
			{
				return "ACTIVE";
			}
			return currentStatusCode;
		}

		private static TResult GetWorstResult<TResult>(GuestApiErrorCollection errorCollection, List<KeyValuePair<string, Type>> errors)
		{
			if (errorCollection == null || errorCollection.errors == null)
			{
				return default(TResult);
			}
			KeyValuePair<string, Type> keyValuePair = errors.LastOrDefault((KeyValuePair<string, Type> p) => errorCollection.errors.Any((GuestApiError e) => e.code == p.Key));
			return (keyValuePair.Key == null) ? default(TResult) : ((TResult)Activator.CreateInstance(keyValuePair.Value));
		}
	}
}
