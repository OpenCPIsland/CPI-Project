using ClubPenguin.DailyChallenge;
using ClubPenguin.Net;
using ClubPenguin.PartyGames;
using Disney.LaunchPadFramework;

namespace ClubPenguin
{
	public class ZoneMediator
	{
		private ContentSchedulerService contentSchedulerService;

		private DailyChallengeService dailyChallengeService;

		private PartyGameManager partyGameManager;

		public ZoneMediator(EventDispatcher eventDispatcher, DailyChallengeService dailyChallengeService, ContentSchedulerService contentSchedulerService, PartyGameManager partyGameManager)
		{
			this.dailyChallengeService = dailyChallengeService;
			this.partyGameManager = partyGameManager;
			this.contentSchedulerService = contentSchedulerService;
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.AddListener<WorldServiceEvents.ContentDateChanged>(onContentDateChanged);
		}

		private bool onContentDateChanged(WorldServiceEvents.ContentDateChanged evt)
		{
			dailyChallengeService.ReloadChallenges(evt.ContentDate);
			return false;
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			switch (evt.State)
			{
			case ZoneTransitionEvents.ZoneTransition.States.Done:
				dailyChallengeService.ReloadChallenges(contentSchedulerService.CurrentContentDate());
				break;
			case ZoneTransitionEvents.ZoneTransition.States.Begin:
				dailyChallengeService.ClearLoadedDailies();
				partyGameManager.Reset();
				break;
			}
			return false;
		}
	}
}
