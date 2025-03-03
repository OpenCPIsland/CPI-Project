using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class MixLoginController : AbstractLoginController
	{
		public string MissingInfoEvent;

		public string LegalUpdateEvent;

		public string LoginSuccessEvent;

		public string ParentalConsentRequiredEvent;

		private string nextEvent;

		protected StateMachine rootStateMachine;

		private MixLoginCreateService loginService;

		private void Awake()
		{
			loginService = Service.Get<MixLoginCreateService>();
		}

		private void OnEnable()
		{
			loginService.OnLoginSuccess += onLoginSuccess;
			loginService.OnLoginFailed += onLoginFailed;
			loginService.OnRecoverySuccess += onRecoverySuccess;
			loginService.OnUsernameRecoveryFailure += onUsernameRecoveryFailed;
			loginService.OnPasswordRecoveryFailure += onPasswordRecoveryFailed;
			loginService.OnMissingInfoLoginSuccess += onMissingInfo;
			loginService.OnRequiresLegalMarketingUpdateLoginSuccess += onLegalUpdate;
			loginService.OnParentalConsentRequiredLoginSuccess += onParentalConsentRequired;
		}

		private void Start()
		{
			rootStateMachine = GetComponent<StateMachine>();
		}

		private void OnDisable()
		{
			loginService.OnLoginSuccess -= onLoginSuccess;
			loginService.OnLoginFailed -= onLoginFailed;
			loginService.OnUsernameRecoveryFailure -= onUsernameRecoveryFailed;
			loginService.OnPasswordRecoveryFailure -= onPasswordRecoveryFailed;
			loginService.OnRecoverySuccess -= onRecoverySuccess;
			loginService.OnMissingInfoLoginSuccess -= onMissingInfo;
			loginService.OnRequiresLegalMarketingUpdateLoginSuccess -= onLegalUpdate;
		}

		public override void Login(DLoginPayload payload)
		{
			loginService.Login(payload.Username, payload.Password);
		}

		public override void PasswordRecoverySend(string lookupValue)
		{
			loginService.PasswordRecoverySend(lookupValue);
		}

		public override void UsernameRecoverySend(string lookupValue)
		{
			loginService.UsernameRecoverySend(lookupValue);
		}

		private void onLoginSuccess(ISession session)
		{
			nextEvent = LoginSuccessEvent;
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			Service.Get<SessionManager>().AddMixSession(session);
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			if (nextEvent == null)
			{
				nextEvent = LoginSuccessEvent;
			}
			rootStateMachine.SendEvent(nextEvent);
			return false;
		}

		private void onLoginFailed(ILoginResult result)
		{
			dispatchLoginError(result);
		}

		private void onUsernameRecoveryFailed(ISendUsernameRecoveryResult result)
		{
			if (result is ISendUsernameRecoveryRateLimitedResult)
			{
				string type = "";
				string format = "Account.General.Error.RateLimited";
				Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error(type, format));
			}
			dispatchRecoveryFailed();
		}

		private void onPasswordRecoveryFailed(ISendPasswordRecoveryResult result)
		{
			if (result is ISendPasswordRecoveryRateLimitedResult)
			{
				string type = "";
				string format = "Account.General.Error.RateLimited";
				Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error(type, format));
			}
			dispatchRecoveryFailed();
		}

		private void onRecoverySuccess()
		{
			dispatchRecoverySuccess();
		}

		private void onMissingInfo(ISession session)
		{
			nextEvent = MissingInfoEvent;
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			Service.Get<SessionManager>().AddMixSession(session);
		}

		private void onLegalUpdate(ISession session)
		{
			nextEvent = LegalUpdateEvent;
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			Service.Get<SessionManager>().AddMixSession(session);
		}

		private void onParentalConsentRequired(ISession session)
		{
			nextEvent = ParentalConsentRequiredEvent;
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			Service.Get<SessionManager>().AddMixSession(session);
		}
	}
}
