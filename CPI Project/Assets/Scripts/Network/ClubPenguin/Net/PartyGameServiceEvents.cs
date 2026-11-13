using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public static class PartyGameServiceEvents
	{
		public struct PartyGameStarted
		{
			public readonly long Owner;

			public readonly long[] Players;

			public readonly int SessionId;

			public readonly int GameTemplateId;

			public PartyGameStarted(long owner, long[] players, int sessionId, int gameTemplateId)
			{
				Owner = owner;
				Players = players;
				SessionId = sessionId;
				GameTemplateId = gameTemplateId;
			}
		}

		public struct PartyGameStartedV2
		{
			public readonly string PlayerData;

			public readonly int SessionId;

			public readonly int GameTemplateId;

			public PartyGameStartedV2(int sessionId, int gameTemplateId, string playerData)
			{
				SessionId = sessionId;
				GameTemplateId = gameTemplateId;
				PlayerData = playerData;
			}
		}

		public struct PartyGameEnded
		{
			public readonly int SessionId;

			public readonly Dictionary<long, int> PlayerSessionIdToPlacement;

			public PartyGameEnded(Dictionary<long, int> playerSessionIdToPlacement, int sessionId)
			{
				PlayerSessionIdToPlacement = playerSessionIdToPlacement;
				SessionId = sessionId;
			}
		}

		public struct PartyGameSessionMessage
		{
			public readonly int SessionId;

			public readonly int Type;

			public readonly string Data;

			public PartyGameSessionMessage(int sessionId, int type, string data)
			{
				SessionId = sessionId;
				Type = type;
				Data = data;
			}
		}
	}
}
