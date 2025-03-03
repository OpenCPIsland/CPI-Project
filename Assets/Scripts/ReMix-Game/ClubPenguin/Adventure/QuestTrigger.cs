using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[RequireComponent(typeof(Collider))]
	public class QuestTrigger : MonoBehaviour
	{
		public delegate void EventHook(string ev);

		public string QuestName;

		public string EnterEvent;

		public string StayEvent;

		public string ExitEvent;

		public string Tag;

		private EventHook eventHook;

		private void SendEvent(string evt)
		{
			QuestService questService = Service.Get<QuestService>();
			Quest activeQuest = questService.ActiveQuest;
			if (activeQuest != null && activeQuest.Definition.name == QuestName)
			{
				questService.SendEvent(evt);
				if (eventHook != null)
				{
					eventHook(evt);
				}
			}
		}

		public void SetEventHook(EventHook hook)
		{
			eventHook = hook;
		}

		public void OnTriggerEnter(Collider col)
		{
			if (!string.IsNullOrEmpty(EnterEvent) && col.CompareTag(Tag))
			{
				SendEvent(EnterEvent);
			}
		}

		public void OnTriggerStay(Collider col)
		{
			if (!string.IsNullOrEmpty(StayEvent) && col.CompareTag(Tag))
			{
				SendEvent(StayEvent);
			}
		}

		public void OnTriggerExit(Collider col)
		{
			if (!string.IsNullOrEmpty(ExitEvent) && col.CompareTag(Tag))
			{
				SendEvent(ExitEvent);
			}
		}
	}
}
