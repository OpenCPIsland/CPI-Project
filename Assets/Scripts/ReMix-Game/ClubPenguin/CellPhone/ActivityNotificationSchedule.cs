using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class ActivityNotificationSchedule : StaticGameDataDefinition
	{
		public RewardDefinition NotificationReward;

		public ActivityNotificationScheduleBlock[] NotificationBlocks;
	}
}
