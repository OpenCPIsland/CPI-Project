using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public static class QuestServiceEvents
	{
		public struct QuestStatesRecieved
		{
			public readonly QuestStateCollection QuestStates;

			public QuestStatesRecieved(QuestStateCollection questStates)
			{
				QuestStates = questStates;
			}
		}

		public struct PlayerOnQuest
		{
			public readonly long SessionId;

			public readonly string MascotName;

			public PlayerOnQuest(long sessionId, string mascotName)
			{
				SessionId = sessionId;
				MascotName = mascotName;
			}
		}
	}
}
