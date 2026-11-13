using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.SpecialEvents
{
	public class ScheduledCore : MonoBehaviour
	{
		private const string FTUE_QUEST_NAME = "AAC001Q001LeakyShip";

		private const bool QUEST_IS_ACTIVE = true;

		private const bool QUEST_IS_NOT_ACTIVE = false;

		public string EventName;

		public bool HideWhenQuestActive = true;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		protected EventChannel eventChannel;

		private ScheduledEventDateDefinition dateDefinition;

		public virtual void Awake()
		{
			dateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[DateDefinitionKey.Id];
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			eventChannel.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			checkFTUEState();
			return false;
		}

		private void checkFTUEState()
		{
			if (Service.Get<GameStateController>().IsFTUEComplete)
			{
				checkWithinDateRange();
			}
			else
			{
				eventChannel.AddListener<QuestEvents.QuestUpdated>(onFTUEQuestUpdated);
			}
		}

		private bool onFTUEQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.Id == "AAC001Q001LeakyShip" && evt.Quest.State == Quest.QuestState.Completed)
			{
				eventChannel.RemoveListener<QuestEvents.QuestUpdated>(onFTUEQuestUpdated);
				checkWithinDateRange();
			}
			return false;
		}

		private void checkWithinDateRange()
		{
			if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(dateDefinition))
			{
				addScheduledCoreEventListeners();
				if (HideWhenQuestActive)
				{
					addReactiveQuestListeners();
				}
				DisplayAdjustments();
			}
		}

		private void addScheduledCoreEventListeners()
		{
			eventChannel.AddListener<ScheduledCoreEvents.ShowAdjustments>(onShowAdjustments);
			eventChannel.AddListener<ScheduledCoreEvents.HideAdjustments>(onHideAdjustments);
		}

		private bool onShowAdjustments(ScheduledCoreEvents.ShowAdjustments evt)
		{
			AdjustSceneForQuest(false);
			return false;
		}

		private bool onHideAdjustments(ScheduledCoreEvents.HideAdjustments evt)
		{
			AdjustSceneForQuest(true);
			return false;
		}

		private void addReactiveQuestListeners()
		{
			eventChannel.AddListener<QuestEvents.SuspendQuest>(onSuspendQuest);
			eventChannel.AddListener<QuestEvents.StartQuest>(onStartQuest);
			eventChannel.AddListener<QuestEvents.ReplayQuest>(onReplayQuest);
			eventChannel.AddListener<QuestEvents.ResumeQuest>(onResumeQuest);
			eventChannel.AddListener<QuestEvents.RestartQuest>(onRestartQuest);
			eventChannel.AddListener<QuestEvents.QuestCompleted>(onCompleteQuest);
			eventChannel.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
		}

		private bool onSuspendQuest(QuestEvents.SuspendQuest evt)
		{
			AdjustSceneForQuest(false);
			return false;
		}

		private bool onStartQuest(QuestEvents.StartQuest evt)
		{
			AdjustSceneForQuest(true);
			return false;
		}

		private bool onReplayQuest(QuestEvents.ReplayQuest evt)
		{
			AdjustSceneForQuest(true);
			return false;
		}

		private bool onResumeQuest(QuestEvents.ResumeQuest evt)
		{
			AdjustSceneForQuest(true);
			return false;
		}

		private bool onRestartQuest(QuestEvents.RestartQuest evt)
		{
			AdjustSceneForQuest(true);
			return false;
		}

		private bool onCompleteQuest(QuestEvents.QuestCompleted evt)
		{
			AdjustSceneForQuest(false);
			return false;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State == Quest.QuestState.Completed || evt.Quest.State == Quest.QuestState.Suspended)
			{
				AdjustSceneForQuest(false);
			}
			else if (evt.Quest.State == Quest.QuestState.Active)
			{
				AdjustSceneForQuest(true);
			}
			return false;
		}

		public virtual void DisplayAdjustments()
		{
		}

		public virtual void AdjustSceneForQuest(bool isQuestActive)
		{
		}

		public virtual bool ShouldAdjustmentsBeHidden()
		{
			if (HideWhenQuestActive && Service.IsSet<QuestService>())
			{
				return Service.Get<QuestService>().ActiveQuest != null;
			}
			return false;
		}

		public virtual void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
