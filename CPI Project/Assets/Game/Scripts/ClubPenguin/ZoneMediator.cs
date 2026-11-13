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
			// evt.ContentDate is whatever date the room identifier carried, and offline that
			// is just today's real date - there is no schedule for it, so the reload finds
			// nothing. The scheduler already maps today onto a day we do have content for,
			// which is what onZoneTransition below passes.
			dailyChallengeService.ReloadChallenges(contentSchedulerService.CurrentContentDate());
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
