using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class StartFTUEStateHandler : AbstractStateHandler
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
			eventDispatcher.AddListener<PlaymakerExternalEvents.FTUEPlayerMoved>(onFTUEPlayerMoved);
		}

		private void OnDisable()
		{
			eventDispatcher.RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.RemoveListener<PlaymakerExternalEvents.FTUEPlayerMoved>(onFTUEPlayerMoved);
		}

		protected override void OnEnter()
		{
			Service.Get<LoadingController>().AddLoadingSystem(this);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.ShowAvailableMarketingLoadingScreen.ToString(), true);
			Service.Get<ZoneTransitionService>().LoadZoneOffline(gameStateController.FTUEConfig.FtueSceneName, gameStateController.SceneConfig.TransitionSceneName, null, dictionary);
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (!base.IsInHandledState)
			{
				return false;
			}
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Done)
			{
				Quest quest = Service.Get<QuestService>().GetQuest(gameStateController.FTUEConfig.FtueQuestId);
				Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.StartQuest(quest));
				quest.SetOffline();
				quest.Activate();
			}
			return false;
		}

		private bool onFTUEPlayerMoved(PlaymakerExternalEvents.FTUEPlayerMoved evt)
		{
			Service.Get<LoadingController>().RemoveLoadingSystem(this);
			return false;
		}
	}
}
