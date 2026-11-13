using ClubPenguin.Analytics;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class ZoneConnectingStateHandler : AbstractStateHandler
	{
		private EventDispatcher eventDispatcher;

		private ZoneTransitionService zoneTransitionService;

		private GameStateController gameStateController;

		private bool isSceneLoaded = false;

		private bool isLocalPlayerInZone = false;

		private DataEntityHandle localPlayerHandle;

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			zoneTransitionService = Service.Get<ZoneTransitionService>();
			gameStateController = Service.Get<GameStateController>();
		}

		private void OnEnable()
		{
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerJoinedRoom);
			eventDispatcher.AddListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
		}

		private void OnDisable()
		{
			eventDispatcher.RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerJoinedRoom);
			eventDispatcher.RemoveListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
		}

		protected override void OnEnter()
		{
			if (!gameStateController.IsOnFtueIntro && !Service.Get<LoadingController>().HasLoadingSystem(this))
			{
				Service.Get<LoadingController>().AddLoadingSystem(this);
			}
			isSceneLoaded = (zoneTransitionService.TransitionState == ZoneTransitionEvents.ZoneTransition.States.Done);
			isLocalPlayerInZone = false;
			Service.Get<ICPSwrveService>().StartTimer("join_room_ready_to_waddle", "join_room_ready_to_waddle", null, zoneTransitionService.CurrentZone.SceneName);
		}

		protected override void OnExit()
		{
			if (Service.Get<LoadingController>().HasLoadingSystem(this))
			{
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
			}
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Done)
			{
				isSceneLoaded = true;
				prepareToSpawnIfReady();
			}
			else if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Cancel)
			{
				Service.Get<ICPSwrveService>().EndTimer("join_room_ready_to_waddle", null, "failure");
			}
			return false;
		}

		private bool onLocalPlayerJoinedRoom(NetworkControllerEvents.LocalPlayerJoinedRoomEvent evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			isLocalPlayerInZone = true;
			localPlayerHandle = evt.Handle;
			if (!Service.Get<SceneTransitionService>().AllowSceneActivation())
			{
				prepareToSpawnIfReady();
			}
			return false;
		}

		private void prepareToSpawnIfReady()
		{
			if (isSceneLoaded && isLocalPlayerInZone)
			{
				eventDispatcher.DispatchEvent(new PlayerSpawnedEvents.LocalPlayerReadyToSpawn(localPlayerHandle));
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				rootStateMachine.SendEvent(gameStateController.ZoneConnectedEvent);
			}
		}

		private bool onSessionPaused(SessionEvents.SessionPausedEvent evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (isLocalPlayerInZone)
			{
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				rootStateMachine.SendEvent(gameStateController.PausedEvent);
			}
			return false;
		}
	}
}
