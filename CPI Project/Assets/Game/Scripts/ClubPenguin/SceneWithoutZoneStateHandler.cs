using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class SceneWithoutZoneStateHandler : AbstractStateHandler
	{
		private EventDispatcher eventDispatcher;

		private ZoneTransitionService zoneTransitionService;

		private DataEntityHandle localPlayerHandle;

		private GameStateController gameStateController;

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			zoneTransitionService = Service.Get<ZoneTransitionService>();
			localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			gameStateController = Service.Get<GameStateController>();
		}

		private void OnEnable()
		{
			eventDispatcher.AddListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
			eventDispatcher.AddListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
		}

		private void OnDisable()
		{
			eventDispatcher.RemoveListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
			eventDispatcher.RemoveListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
			eventDispatcher.RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
		}

		protected override void OnEnter()
		{
		}

		private bool onSceneTransitionComplete(SceneTransitionEvents.TransitionComplete evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (evt.SceneName == zoneTransitionService.CurrentZone.SceneName)
			{
				eventDispatcher.DispatchEvent(new PlayerSpawnedEvents.LocalPlayerReadyToSpawn(localPlayerHandle));
				rootStateMachine.SendEvent(gameStateController.ZoneConnectedEvent);
			}
			return false;
		}

		private bool onSessionPaused(SessionEvents.SessionPausedEvent evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			rootStateMachine.SendEvent(gameStateController.PausedEvent);
			return false;
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Begin)
			{
				rootStateMachine.SendEvent(gameStateController.ZoneConnectingEvent);
			}
			return false;
		}
	}
}
