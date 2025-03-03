using HutongGames.PlayMaker;
using System.Runtime.InteropServices;

namespace ClubPenguin.Adventure
{
	public static class QuestEvents
	{
		public struct SuspendQuest
		{
			public readonly Quest Quest;

			public SuspendQuest(Quest quest)
			{
				Quest = quest;
			}
		}

		public struct StartQuest
		{
			public readonly Quest Quest;

			public StartQuest(Quest quest)
			{
				Quest = quest;
			}
		}

		public struct ReplayQuest
		{
			public readonly Quest Quest;

			public ReplayQuest(Quest quest)
			{
				Quest = quest;
			}
		}

		public struct ResumeQuest
		{
			public readonly Quest Quest;

			public ResumeQuest(Quest quest)
			{
				Quest = quest;
			}
		}

		public struct RestartQuest
		{
			public readonly Quest Quest;

			public RestartQuest(Quest quest)
			{
				Quest = quest;
			}
		}

		public struct SetPlayerOutOfWorld
		{
			public bool IsOutOfWorld;

			public SetPlayerOutOfWorld(bool isOutOfWorld)
			{
				IsOutOfWorld = isOutOfWorld;
			}
		}

		public struct RegisterQuestSubFsm
		{
			public Fsm QuestFsm;

			public RegisterQuestSubFsm(Fsm questFsm)
			{
				QuestFsm = questFsm;
			}
		}

		public struct SetQuestHint
		{
			public QuestHint HintData;

			public SetQuestHint(QuestHint hintData)
			{
				HintData = hintData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CancelQuestHint
		{
		}

		public struct QuestUpdated
		{
			public readonly Quest Quest;

			public QuestUpdated(Quest quest)
			{
				Quest = quest;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct QuestSyncCompleted
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct QuestStarted
		{
		}

		public struct QuestCompleted
		{
			public readonly Quest Quest;

			public QuestCompleted(Quest quest)
			{
				Quest = quest;
			}
		}

		public struct QuestWaypointTriggerEntered
		{
			public readonly string WaypointName;

			public QuestWaypointTriggerEntered(string waypointName)
			{
				WaypointName = waypointName;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct QuestInitializationComplete
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct MascotAudioComplete
		{
		}

		public struct OnMascotInteract
		{
			public Mascot Mascot;

			public OnMascotInteract(Mascot mascot)
			{
				Mascot = mascot;
			}
		}
	}
}
