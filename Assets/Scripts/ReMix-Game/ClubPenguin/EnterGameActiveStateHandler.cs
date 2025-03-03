using ClubPenguin.Net;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class EnterGameActiveStateHandler : ActiveStateHandler
	{
		private SessionManager sessionManager;

		private GameStateController gameStateController;

		private void Awake()
		{
			sessionManager = Service.Get<SessionManager>();
			gameStateController = Service.Get<GameStateController>();
		}

		public override string HandleStateChange()
		{
			string result = gameStateController.MixLoginEvent;
			if (Service.Get<ConnectionManager>().ConnectionState == ConnectionManager.NetworkConnectionState.NoConnection)
			{
				result = gameStateController.DefaultEvent;
				Service.Get<EventDispatcher>().DispatchEvent(default(AccountSystemEvents.AccountSystemEnded));
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.NoConnectionError));
			}
			else if (sessionManager.HasSession)
			{
				gameStateController.ReconnectFromHome();
				result = gameStateController.ZoneConnectingEvent;
			}
			return result;
		}
	}
}
