using ClubPenguin.Net;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class PausedStateHandler : AbstractStateHandler
	{
		private EventDispatcher eventDispatcher;

		private NetworkController networkController;

		private GameStateController gameStateController;

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			networkController = Service.Get<NetworkController>();
			gameStateController = Service.Get<GameStateController>();
		}

		private void OnEnable()
		{
			eventDispatcher.AddListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
		}

		private void OnDisable()
		{
			eventDispatcher.RemoveListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
		}

		protected override void OnEnter()
		{
			networkController.PauseGameServer();
		}

		private bool onSessionResumed(SessionEvents.SessionResumedEvent evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (!Service.Get<MembershipService>().IsPurchaseInProgress)
			{
				networkController.ResumeGameServer();
				rootStateMachine.SendEvent(gameStateController.ZoneConnectingEvent);
			}
			return false;
		}
	}
}
