using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class ZoneConnectedStateHandler : AbstractStateHandler
	{
		private EventDispatcher eventDispatcher;

		private ZoneTransitionService zoneTransitionService;

		private GameStateController gameStateController;

		private QuestService questService;

		private void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			zoneTransitionService = Service.Get<ZoneTransitionService>();
			gameStateController = Service.Get<GameStateController>();
			questService = Service.Get<QuestService>();
		}

		private void OnEnable()
		{
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionStart);
			eventDispatcher.AddListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
			eventDispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onPlayerSpawned);
		}

		private void OnDisable()
		{
			eventDispatcher.RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionStart);
			eventDispatcher.RemoveListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
			eventDispatcher.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onPlayerSpawned);
		}

		protected override void OnEnter()
		{
			string str = Service.Get<MembershipService>().GetAccountFlowData().FlowType.ToString();
			Service.Get<ICPSwrveService>().EndTimer("GetInTheGame", "home_to_world." + str);
			if (gameStateController.DoFTUECheckOnZoneChange)
			{
				checkFTUEProgress();
				return;
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			PausedStateData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				Quest activeQuest = questService.ActiveQuest;
				if (activeQuest != null)
				{
					resetCamera();
					Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.ResumeQuest(activeQuest));
				}
			}
		}

		private bool onPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			Service.Get<ICPSwrveService>().EndTimer("join_room_ready_to_waddle", null, "success");
			return false;
		}

		private void checkFTUEProgress()
		{
			Quest activeQuest = questService.ActiveQuest;
			if (activeQuest != null && activeQuest.Id == gameStateController.FTUEConfig.FtueQuestId)
			{
				if (gameStateController.IsFTUEComplete)
				{
					resetCamera();
					questService.EndQuest(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject, Service.Get<GameStateController>().FTUEConfig.FtueQuestId);
				}
				else if (!gameStateController.IsFTUEComplete && !gameStateController.IsOnFtueIntro)
				{
					resetCamera();
					questService.EndQuest(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject, Service.Get<GameStateController>().FTUEConfig.FtueQuestId);
					resumeFirstTimeUserExperienceQuest();
					resetGUI();
				}
				else
				{
					activeQuest.SetOnline();
				}
			}
			else if (questService.ActiveQuest == null && !gameStateController.IsFTUEComplete)
			{
				if (SceneRefs.PopupManager == null)
				{
					eventDispatcher.AddListener<PopupEvents.PopupManagerReady>(onPopupManagerReady);
				}
				else
				{
					resumeFirstTimeUserExperienceQuest();
					resetGUI();
				}
			}
			gameStateController.DoFTUECheckOnZoneChange = false;
		}

		private bool onPopupManagerReady(PopupEvents.PopupManagerReady evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			eventDispatcher.RemoveListener<PopupEvents.PopupManagerReady>(onPopupManagerReady);
			resumeFirstTimeUserExperienceQuest();
			resetGUI();
			return false;
		}

		private void resumeFirstTimeUserExperienceQuest()
		{
			Quest quest = Service.Get<QuestService>().GetQuest(gameStateController.FTUEConfig.FtueQuestId);
			Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.ResumeQuest(quest));
		}

		private void resetGUI()
		{
			eventDispatcher.DispatchEvent(new TrayEvents.SelectTrayScreen("ControlsScreen"));
			eventDispatcher.DispatchEvent(new HudEvents.SuppressQuestNotifier(false));
		}

		private void resetCamera()
		{
			Director director = ClubPenguin.Core.SceneRefs.Get<Director>();
			if (director != null)
			{
				director.SoftResetCamera();
			}
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

		private bool onSceneTransitionStart(SceneTransitionEvents.TransitionStart evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (zoneTransitionService.GetZoneBySceneName(evt.SceneName) == null)
			{
				rootStateMachine.SendEvent(gameStateController.SceneWithoutZoneEvent);
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
	}
}
