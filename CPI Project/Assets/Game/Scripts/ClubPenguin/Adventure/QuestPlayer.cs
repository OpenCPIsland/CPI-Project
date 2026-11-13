using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestPlayer : MonoBehaviour
	{
		private EventDispatcher dispatcher;

		private QuestService questService;

		private Quest _activeQuest;

		private Quest activeQuest
		{
			get
			{
				return _activeQuest;
			}
			set
			{
				if (_activeQuest != value)
				{
					_activeQuest = value;
					if (_activeQuest != null)
					{
						_activeQuest.RestoreAsync(base.gameObject);
					}
				}
			}
		}

		public void Awake()
		{
			questService = Service.Get<QuestService>();
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			dispatcher.AddListener<HudEvents.HudInitComplete>(onHudInitCompleted);
		}

		public void OnDestroy()
		{
			dispatcher.RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			dispatcher.RemoveListener<HudEvents.HudInitComplete>(onHudInitCompleted);
			questService.QuestPlayerIsReady = false;
		}

		private bool onHudInitCompleted(HudEvents.HudInitComplete evt)
		{
			activeQuest = questService.ActiveQuest;
			questService.QuestPlayerIsReady = true;
			return false;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State == Quest.QuestState.Active)
			{
				activeQuest = evt.Quest;
			}
			else if (evt.Quest == activeQuest)
			{
				activeQuest = null;
			}
			return false;
		}
	}
}
