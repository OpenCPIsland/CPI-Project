using ClubPenguin.Net.Domain.Igloo;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	public class LocalPlayerData : OtherPlayerData
	{
		public List<MinigameProgress> minigameProgress;

		public QuestStateCollection quests;

		public long membershipExpireDate;

		public string subscriptionVendor;

		public string subscriptionProductId;

		public bool subscriptionPaymentPending;

		public MigrationData migrationData;

		public List<sbyte> tutorialData;

		public BreadcrumbsResponse breadcrumbs;

		public List<int> claimedRewardIds;

		public SavedIglooLayoutsSummary iglooLayouts;

		public bool trialAvailable;

		public bool recurring;

		public DailySpinData dailySpinData;
	}
}
