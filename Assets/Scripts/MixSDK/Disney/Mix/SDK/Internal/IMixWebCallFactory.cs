using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IMixWebCallFactory : IDisposable
	{
		string GuestControllerAccessToken
		{
			get;
			set;
		}

		event EventHandler<AbstractAuthenticationLostEventArgs> OnAuthenticationLost;

		IWebCall<GetUsersByDisplayNameRequest, GetUsersResponse> UsersByDisplayNamePost(GetUsersByDisplayNameRequest request);

		IWebCall<GetUsersByUserIdRequest, GetUsersResponse> UsersByUserIdPost(GetUsersByUserIdRequest request);

		IWebCall<ClearAlertsRequest, ClearAlertsResponse> AlertsClearPut(ClearAlertsRequest request);

		IWebCall<SetDisplayNameRequest, SetDisplayNameResponse> DisplaynamePut(SetDisplayNameRequest request);

		IWebCall<ValidateDisplayNamesRequest, ValidateDisplayNamesResponse> DisplaynameValidatePost(ValidateDisplayNamesRequest request);

		IWebCall<ValidateDisplayNameRequest, ValidateDisplayNameResponse> DisplaynameValidateV2Post(ValidateDisplayNameRequest request);

		IWebCall<AddFriendshipRequest, AddFriendshipResponse> FriendshipPut(AddFriendshipRequest request);

		IWebCall<RemoveFriendshipRequest, RemoveFriendshipResponse> FriendshipDeletePost(RemoveFriendshipRequest request);

		IWebCall<BaseUserRequest, GetFriendshipRecommendationResponse> FriendshipRecommendPost(BaseUserRequest request);

		IWebCall<AddFriendshipInvitationRequest, AddFriendshipInvitationResponse> FriendshipInvitationPut(AddFriendshipInvitationRequest request);

		IWebCall<RemoveFriendshipInvitationRequest, RemoveFriendshipInvitationResponse> FriendshipInvitationDeletePost(RemoveFriendshipInvitationRequest request);

		IWebCall<BaseUserRequest, GetGeolocationResponse> GeolocationPost(BaseUserRequest request);

		IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportModerationTempBanPut(BaseUserRequest request);

		IWebCall<TriggerAlertRequest, BaseResponse> IntegrationTestSupportUserAlertPost(TriggerAlertRequest request);

		IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportUserAnonymizePost(BaseUserRequest request);

		IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportUserNotificationCounterDeletePost(BaseUserRequest request);

		IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportUserSessionExpirePost(BaseUserRequest request);

		IWebCall<SetLanguageRequest, BaseResponse> LanguagePreferencePost(SetLanguageRequest request);

		IWebCall<ReportPlayerRequest, BaseResponse> ModerationReportPlayerPut(ReportPlayerRequest request);

		IWebCall<ModerateTextRequest, ModerateTextResponse> ModerationTextPut(ModerateTextRequest request);

		IWebCall<GetNotificationsRequest, GetNotificationsResponse> NotificationsPost(GetNotificationsRequest request);

		IWebCall<GetNotificationsSinceSequenceRequest, GetNotificationsResponse> NotificationsSinceSequencePost(GetNotificationsSinceSequenceRequest request);

		IWebCall<PilCheckRequest, PilCheckResponse> PilCheckPost(PilCheckRequest request);

		IWebCall<SetPresenceRequest, BaseResponse> PresencePut(SetPresenceRequest request);

		IWebCall<BaseUserRequest, PostPresenceResponse> PresencePost(BaseUserRequest request);

		IWebCall<TogglePushNotificationRequest, BaseResponse> PushNotificationsSettingPost(TogglePushNotificationRequest request);

		IWebCall<BaseUserRequest, BaseResponse> PushNotificationsSettingDeletePost(BaseUserRequest request);

		IWebCall<GetRegistrationTextRequest, GetRegistrationTextResponse> RegistrationTextPost(GetRegistrationTextRequest request);

		IWebCall<DisplayNameSearchRequest, DisplayNameSearchResponse> SearchDisplaynamePost(DisplayNameSearchRequest request);

		IWebCall<StartUserSessionRequest, StartUserSessionResponse> SessionUserPut(StartUserSessionRequest request);

		IWebCall<BaseUserRequest, BaseResponse> SessionUserDeletePost(BaseUserRequest request);

		IWebCall<GetStateRequest, GetStateResponse> StatePost(GetStateRequest request);

		IWebCall<RemoveFriendshipTrustRequest, RemoveFriendshipTrustResponse> FriendshipTrustDeletePost(RemoveFriendshipTrustRequest request);
	}
}
