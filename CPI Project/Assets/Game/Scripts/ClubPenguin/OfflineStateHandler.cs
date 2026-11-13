using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class OfflineStateHandler : AbstractStateHandler
	{
		private EventDispatcher eventDispatcher;

		private GameStateController gameStateController;

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			gameStateController = Service.Get<GameStateController>();
		}

		private void OnEnable()
		{
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
		}

		private void OnDisable()
		{
			eventDispatcher.RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
		}

		protected override void OnEnter()
		{
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Request)
			{
				rootStateMachine.SendEvent(gameStateController.ZoneConnectingEvent);
			}
			return false;
		}
	}
}
