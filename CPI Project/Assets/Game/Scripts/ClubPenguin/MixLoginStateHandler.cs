using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;

namespace ClubPenguin
{
	public class MixLoginStateHandler : AbstractStateHandler
	{
		private EventDispatcher eventDispatcher;

		private GameStateController gameStateController;

		private MixLoginCreateService loginService;

		private SessionManager sessionManager;

		public string MissingInfoEvent;

		public string LegalUpdateEvent;

		public string ParentalConsentRequiredEvent;

		private string nextAccountSystemEvent;

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			gameStateController = Service.Get<GameStateController>();
			loginService = Service.Get<MixLoginCreateService>();
			sessionManager = Service.Get<SessionManager>();
		}

		protected override void OnEnter()
		{
			bool firstSession = Service.Get<GameSettings>().FirstSession;
			if (LoginController.AutoLoginEnabled && !LoginController.SkipAutoLogin && !firstSession)
			{
				Service.Get<ICPSwrveService>().StartTimer("GetInTheGame", "home_to_world.auto_login");
				addListeners();
				CoroutineRunner.StopAllForOwner(this);
				CoroutineRunner.Start(doSoftLogin(), this, "SoftLogin Async delay");
				return;
			}
			if (firstSession && !MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
			{
				Service.Get<ICPSwrveService>().StartTimer("GetInTheGame", "home_to_world.create");
				gameStateController.ShowAccountSystemCreate();
			}
			else
			{
				Service.Get<ICPSwrveService>().StartTimer("GetInTheGame", "home_to_world.login");
				gameStateController.ShowAccountSystemLogin();
			}
			LoginController.SkipAutoLogin = false;
		}

		private IEnumerator doSoftLogin()
		{
			yield return null;
			loginService.SoftLogin();
		}

		private void onSoftLoginSuccess(ISession session)
		{
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			accountFlowData.FlowType = AccountFlowType.autologin;
			if (base.IsInHandledState)
			{
				removeListeners();
				nextAccountSystemEvent = gameStateController.LoginSuccessEvent;
				eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
				sessionManager.AddMixSession(session);
			}
		}

		private void onSoftLoginFailed(IRestoreLastSessionResult result)
		{
			if (base.IsInHandledState)
			{
				removeListeners();
				if (!Service.Get<GameSettings>().FirstSession)
				{
					Service.Get<ICPSwrveService>().StartTimer("GetInTheGame", "home_to_world.login");
					gameStateController.ShowAccountSystemLogin();
				}
				else
				{
					Service.Get<ICPSwrveService>().StartTimer("GetInTheGame", "home_to_world.create");
					gameStateController.ShowAccountSystemCreate();
				}
			}
		}

		private void onSoftLoginMissingInfo(ISession session)
		{
			if (base.IsInHandledState)
			{
				removeListeners();
				nextAccountSystemEvent = MissingInfoEvent;
				eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
				sessionManager.AddMixSession(session);
			}
		}

		private void onSoftLoginParentalApproval(ISession session)
		{
			if (base.IsInHandledState)
			{
				removeListeners();
				nextAccountSystemEvent = ParentalConsentRequiredEvent;
				eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
				sessionManager.AddMixSession(session);
			}
		}

		private void onSoftLoginRequiresLegalUpdate(ISession session)
		{
			if (base.IsInHandledState)
			{
				removeListeners();
				nextAccountSystemEvent = LegalUpdateEvent;
				eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
				sessionManager.AddMixSession(session);
			}
		}

		private void addListeners()
		{
			loginService.OnLoginSuccess += onSoftLoginSuccess;
			loginService.OnSoftLoginFailed += onSoftLoginFailed;
			loginService.OnMissingInfoLoginSuccess += onSoftLoginMissingInfo;
			loginService.OnParentalConsentRequiredLoginSuccess += onSoftLoginParentalApproval;
			loginService.OnRequiresLegalMarketingUpdateLoginSuccess += onSoftLoginRequiresLegalUpdate;
		}

		private void removeListeners()
		{
			loginService.OnLoginSuccess -= onSoftLoginSuccess;
			loginService.OnSoftLoginFailed -= onSoftLoginFailed;
			loginService.OnMissingInfoLoginSuccess -= onSoftLoginMissingInfo;
			loginService.OnParentalConsentRequiredLoginSuccess -= onSoftLoginParentalApproval;
			loginService.OnRequiresLegalMarketingUpdateLoginSuccess -= onSoftLoginRequiresLegalUpdate;
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			if (!base.IsInHandledState)
			{
				return false;
			}
			gameStateController.ShowAccountSystem(nextAccountSystemEvent);
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			accountFlowData.FlowType = AccountFlowType.autologin;
			return false;
		}
	}
}
