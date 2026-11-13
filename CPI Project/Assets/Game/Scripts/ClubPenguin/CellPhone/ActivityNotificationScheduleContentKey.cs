using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class ActivityNotificationScheduleContentKey : TypedAssetContentKey<ActivityNotificationSchedule>
	{
		public ActivityNotificationScheduleContentKey()
		{
		}

		public ActivityNotificationScheduleContentKey(string key)
			: base(key)
		{
		}

		public ActivityNotificationScheduleContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
