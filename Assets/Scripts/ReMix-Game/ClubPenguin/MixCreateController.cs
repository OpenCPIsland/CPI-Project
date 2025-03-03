using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class MixCreateController : AbstractCreateController
	{
		private const int MaxDisplayNameLength = 14;

		public string CreateSuccessEvent;

		public string LocalPlayerLoadedEvent;

		public string UpdateDisplayNameSuccessEvent;

		public string UpdateProfileSuccessEvent;

		public string LegalUpdateEvent;

		public string ProfileUpdateEvent;

		private MixLoginCreateService loginService;

		private SessionManager sessionManager;

		protected StateMachine rootStateMachine;

		private string currentReferer;

		public override bool IsOnline
		{
			get
			{
				return Service.Get<ConnectionManager>().ConnectionState == ConnectionManager.NetworkConnectionState.BasicConnection;
			}
		}

		public override bool IsConfigReady
		{
			get
			{
				return !loginService.RegistrationConfigIsNotSet;
			}
		}

		private IEnumerator Start()
		{
			loginService = Service.Get<MixLoginCreateService>();
			loginService.OnCreateSuccess += onCreateSuccess;
			loginService.OnCreateFailed += onCreateFailed;
			loginService.OnProfileUpdateSuccess += onProfileUpdateSuccess;
			loginService.OnProfileUpdateFailed += onProfileUpdateFailed;
			loginService.OnRegistrationConfigError += onRegistrationConfigError;
			sessionManager = Service.Get<SessionManager>();
			while (GetComponent<StateMachine>() == null)
			{
				yield return null;
			}
			rootStateMachine = GetComponent<StateMachine>();
		}

		private void OnDestroy()
		{
			if (loginService != null)
			{
				loginService.OnCreateSuccess -= onCreateSuccess;
				loginService.OnCreateFailed -= onCreateFailed;
				loginService.OnProfileUpdateSuccess -= onProfileUpdateSuccess;
				loginService.OnProfileUpdateFailed -= onProfileUpdateFailed;
				loginService.OnRegistrationConfigError -= onRegistrationConfigError;
			}
		}

		public override void Create(DCreateAccountPayload payload)
		{
			Dictionary<IMarketingItem, bool> marketing = new Dictionary<IMarketingItem, bool>();
			loginService.CreateChildAccount(payload.FirstName, payload.Username, payload.ParentEmail, payload.Password, payload.LangPref, marketing, payload.AcceptedLegalDocs);
		}

		public override void UpdateProfile(DUpdateProfilePayload payload)
		{
			loginService.UpdateProfile(payload.FirstName, payload.ParentEmail, payload.AcceptedLegalDocs, Service.Get<SessionManager>().LocalUser);
		}

		public override void ValidateDisplayName(string displayName)
		{
			sessionManager.LocalUser.ValidateDisplayName(displayName, delegate(IValidateDisplayNameResult validateResult)
			{
				if (validateResult.Success)
				{
					dispatchValidateDisplayNameSuccess();
				}
				else
				{
					dispatchValidateDisplayNameError(validateResult);
				}
			});
		}

		public override void UpdateDisplayName(string displayName, string referer)
		{
			if (displayName == sessionManager.LocalUser.DisplayName.Text)
			{
				rootStateMachine.SendEvent(UpdateDisplayNameSuccessEvent);
				return;
			}
			currentReferer = referer;
			sessionManager.LocalUser.UpdateDisplayName(displayName, delegate(IUpdateDisplayNameResult updateResult)
			{
				if (updateResult.Success)
				{
					Service.Get<ICPSwrveService>().Action("game.name_change", displayName);
					Service.Get<ICPSwrveService>().Action("game.display_name_form.submit", "success");
					setReferer();
				}
				else
				{
					dispatchUpdateDisplayNameError(updateResult);
				}
			});
		}

		private void setReferer()
		{
			if (!string.IsNullOrEmpty(currentReferer))
			{
				Service.Get<EventDispatcher>().AddListener<PlayerStateServiceEvents.PlayerReferralSet>(onPlayerReferralSet);
				Service.Get<EventDispatcher>().AddListener<PlayerStateServiceErrors.PlayerReferralError>(onPlayerReferralError);
				Service.Get<INetworkServicesManager>().PlayerStateService.SetReferral(currentReferer);
				currentReferer = null;
			}
			else
			{
				rootStateMachine.SendEvent(UpdateDisplayNameSuccessEvent);
			}
		}

		private bool onPlayerReferralSet(PlayerStateServiceEvents.PlayerReferralSet evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<PlayerStateServiceEvents.PlayerReferralSet>(onPlayerReferralSet);
			Service.Get<EventDispatcher>().RemoveListener<PlayerStateServiceErrors.PlayerReferralError>(onPlayerReferralError);
			rootStateMachine.SendEvent(UpdateDisplayNameSuccessEvent);
			return false;
		}

		private bool onPlayerReferralError(PlayerStateServiceErrors.PlayerReferralError evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<PlayerStateServiceEvents.PlayerReferralSet>(onPlayerReferralSet);
			Service.Get<EventDispatcher>().RemoveListener<PlayerStateServiceErrors.PlayerReferralError>(onPlayerReferralError);
			rootStateMachine.SendEvent(UpdateDisplayNameSuccessEvent);
			return false;
		}

		private void onCreateSuccess(ISession session)
		{
			rootStateMachine.SendEvent(CreateSuccessEvent);
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			bool canPrepopulateDisplayName = session.LocalUser.RegistrationProfile.Username.Length <= 14;
			Service.Get<SessionManager>().AddMixSession(session, canPrepopulateDisplayName);
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			if (sessionManager.LocalUser.RegistrationProfile.AccountStatus == AccountStatus.AwaitingParentalConsent)
			{
				Service.Get<MixLoginCreateService>().ParentalApprovalEmailSend(Service.Get<SessionManager>().LocalUser);
			}
			rootStateMachine.SendEvent(LocalPlayerLoadedEvent);
			return false;
		}

		private void onCreateFailed(IRegisterResult result)
		{
			dispatchCreateError(result);
		}

		private void onProfileUpdateSuccess()
		{
			rootStateMachine.SendEvent(UpdateProfileSuccessEvent);
		}

		private void onProfileUpdateFailed(IUpdateProfileResult result)
		{
			List<IInvalidProfileItemError> list = result.Errors as List<IInvalidProfileItemError>;
			if (list != null && list[0] is ILoginRequiresLegalMarketingUpdateResult)
			{
				rootStateMachine.SendEvent(LegalUpdateEvent);
			}
			else if (list != null)
			{
				rootStateMachine.SendEvent(ProfileUpdateEvent);
			}
			dispatchProfileUpdateError(result);
		}

		public override bool CheckRegConfigReady()
		{
			if (loginService.RegistrationConfigIsNotSet)
			{
				if (loginService.IsEmbargoed)
				{
					string type = "GlobalUI.Homescreen.GeoGate.Title";
					string format = "GlobalUI.Homescreen.GeoGate.Body";
					Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error(type, format));
				}
				else
				{
					onRegistrationConfigError("config");
					if (!loginService.IsFetchingRegConfig)
					{
						loginService.GetRegistrationConfig();
					}
				}
				return false;
			}
			return true;
		}

		private void onRegistrationConfigError(string step)
		{
			if (base.CanShowAccountError)
			{
				string titleToken = "Account.Create.Error.OneID.Title";
				string messageToken = "Account.Create.Error.OneID";
				Service.Get<EventDispatcher>().DispatchEvent(new SessionErrorEvents.RegistrationConfigError(titleToken, messageToken, step));
			}
		}
	}
}
