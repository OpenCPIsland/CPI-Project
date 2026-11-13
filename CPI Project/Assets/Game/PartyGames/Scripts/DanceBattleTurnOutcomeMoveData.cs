using System.Collections.Generic;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleTurnOutcomeMoveData
	{
		public class PlayerMoveData
		{
			public long PlayerSessionId;

			public List<int> DanceMoveIds;

			public PlayerMoveData()
			{
			}

			public PlayerMoveData(long playerSessionId, List<int> danceMoveIds)
			{
				PlayerSessionId = playerSessionId;
				DanceMoveIds = danceMoveIds;
			}
		}

		public List<PlayerMoveData> PlayerMoveDatas;

		public long StartTimeInSeconds;

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			DanceBattleTurnOutcomeMoveData danceBattleTurnOutcomeMoveData = other as DanceBattleTurnOutcomeMoveData;
			if (danceBattleTurnOutcomeMoveData == null)
			{
				return false;
			}
			return danceBattleTurnOutcomeMoveData.StartTimeInSeconds == StartTimeInSeconds;
		}

		public override int GetHashCode()
		{
			return (int)StartTimeInSeconds;
		}
	}
}
