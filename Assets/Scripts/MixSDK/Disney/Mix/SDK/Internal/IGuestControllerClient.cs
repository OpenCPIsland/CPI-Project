using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IGuestControllerClient : IDisposable
	{
		event EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> OnAccessTokenChanged;

		event EventHandler<AbstractLegalMarketingUpdateRequiredEventArgs> OnLegalMarketingUpdateRequired;

		event EventHandler<AbstractAuthenticationLostEventArgs> OnAuthenticationLost;

		void LogIn(LogInRequest request, Action<GuestControllerResult<LogInResponse>> callback);

		void Refresh(Action<GuestControllerResult<RefreshResponse>> callback);

		void GetSiteConfiguration(Action<GuestControllerResult<SiteConfigurationResponse>> callback);

		void GetSiteConfiguration(string countryCode, Action<GuestControllerResult<SiteConfigurationResponse>> callback);

		void Validate(ValidateRequest request, Action<GuestControllerResult<ValidateResponse>> callback);

		void Register(RegisterRequest request, Action<GuestControllerResult<LogInResponse>> callback);

		void UpdateProfile(UpdateProfileRequest request, Action<GuestControllerResult<ProfileResponse>> callback);

		void GetProfile(Action<GuestControllerResult<ProfileResponse>> callback);

		void RecoverPassword(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback);

		void RecoverUsername(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback);

		void ResolveMase(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback);

		void UpgradeNrt(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback);

		void SendParentalApprovalEmail(string languageCode, Action<GuestControllerResult<NotificationResponse>> callback);

		void SendVerificationEmail(string languageCode, Action<GuestControllerResult<NotificationResponse>> callback);

		void GetAdultVerificationStatus(Action<GuestControllerResult<AdultVerificationStatusResponse>> callback);

		void VerifyAdultUnitedStates(AdultVerificationRequest request, Action<GuestControllerResult<AdultVerificationResponse>> callback);

		void SendAdultVerificationQuiz(AdultVerificationQuizRequest request, Action<GuestControllerResult<AdultVerificationQuizResponse>> callback);

		void GetClaimableChildren(Action<GuestControllerResult<ChildrenResponse>> callback);

		void GetLinkedChildren(Action<GuestControllerResult<ChildrenResponse>> callback);

		void LinkChild(LinkChildRequest request, string childSwid, Action<GuestControllerResult<GuestControllerWebCallResponse>> callback);

		void LinkClaimableChildren(LinkClaimableChildrenRequest request, Action<GuestControllerResult<GuestControllerWebCallResponse>> callback);

		void GetGuardians(Action<GuestControllerResult<GuardiansResponse>> callback);

		void RequestPermission(RequestPermissionRequest request, Action<GuestControllerResult<PermissionResponse>> callback);

		void RequestPermissionForChild(RequestPermissionRequest request, string childSwid, Action<GuestControllerResult<PermissionResponse>> callback);

		void ApprovePermission(ApprovePermissionRequest request, string childSwid, Action<GuestControllerResult<PermissionResponse>> callback);

		void GetPermission(string childSwid, Action<GuestControllerResult<GetPermissionsResponse>> callback);
	}
}
