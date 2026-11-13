using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestObjectiveListener : MonoBehaviour
	{
		public string QuestName;

		public string ObjectiveName;

		public bool IgnoreQuestCompletionCheck;

		public GameObject ObjectToToggle;

		public bool StateIfComplete;

		public bool HideIfQuestActive;

		private EventDispatcher dispatcher;

		private Quest quest;

		private bool isInitialized;

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
		}

		public void OnEnable()
		{
			isInitialized = false;
			if (string.IsNullOrEmpty(QuestName))
			{
				base.enabled = false;
			}
			else
			{
				CoroutineRunner.Start(initialize(), this, "ObjectiveListener");
			}
		}

		public void OnDisable()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (isInitialized)
			{
				dispatcher.RemoveListener<QuestEvents.QuestSyncCompleted>(onQuestSyncCompleted);
				dispatcher.RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			}
		}

		private IEnumerator initialize()
		{
			while ((quest = Service.Get<QuestService>().GetQuest(QuestName, false)) == null)
			{
				yield return null;
			}
			dispatcher.AddListener<QuestEvents.QuestSyncCompleted>(onQuestSyncCompleted);
			dispatcher.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			checkObjectiveState();
			isInitialized = true;
		}

		private bool onQuestSyncCompleted(QuestEvents.QuestSyncCompleted evt)
		{
			checkObjectiveState();
			return false;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			checkObjectiveState();
			return false;
		}

		private void checkObjectiveState()
		{
			if (quest != null && ObjectToToggle != null)
			{
				if (quest.State == Quest.QuestState.Active && HideIfQuestActive)
				{
					ObjectToToggle.SetActive(false);
				}
				else if (quest.IsObjectiveComplete(ObjectiveName) || (!IgnoreQuestCompletionCheck && (quest.State == Quest.QuestState.Completed || (quest.State == Quest.QuestState.Suspended && quest.TimesCompleted > 0))))
				{
					ObjectToToggle.SetActive(StateIfComplete);
				}
				else
				{
					ObjectToToggle.SetActive(!StateIfComplete);
				}
			}
		}
	}
}
