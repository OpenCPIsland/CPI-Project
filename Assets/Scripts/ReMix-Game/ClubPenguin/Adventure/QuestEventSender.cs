using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestEventSender : MonoBehaviour
	{
		public void SendQuestEvent(string questEvent)
		{
			Service.Get<QuestService>().SendEvent(questEvent);
		}
	}
}
