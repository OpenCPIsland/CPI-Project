using System;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public struct ActivityNotificationScheduleBlock
	{
		public int TriggerTime;

		public CellPhoneActivityDefinition[] Notifications;
	}
}
