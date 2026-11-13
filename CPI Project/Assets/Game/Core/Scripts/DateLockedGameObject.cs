using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Core
{
	public class DateLockedGameObject : MonoBehaviour
	{
		public ScheduledEventDateDefinition UnlockedDate;

		private void Awake()
		{
			if (Service.Get<ContentSchedulerService>().IsBeforeScheduleEventDates(UnlockedDate))
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
