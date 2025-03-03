using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestAudio : MonoBehaviour
	{
		public void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			Service.Get<EventDispatcher>().AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State != Quest.QuestState.Active && evt.Quest == Service.Get<QuestService>().ActiveQuest)
			{
				Service.Get<EventDispatcher>().RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
				Object.Destroy(base.gameObject);
			}
			return false;
		}
	}
}
