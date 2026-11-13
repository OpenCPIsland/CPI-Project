using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class SendQuestEventOnAnimationEvent : MonoBehaviour
	{
		public AnimationQuestEvent[] events;

		private QuestService questService;

		private void Start()
		{
			questService = Service.Get<QuestService>();
		}

		public void OnAnimationEvent(string eventName)
		{
			for (int i = 0; i < events.Length; i++)
			{
				if (events[i].animationEventName == eventName)
				{
					questService.SendEvent(events[i].questEventName);
				}
			}
		}
	}
}
