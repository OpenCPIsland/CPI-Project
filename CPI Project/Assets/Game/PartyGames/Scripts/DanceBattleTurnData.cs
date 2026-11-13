using System.Collections.Generic;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleTurnData
	{
		public List<int> Moves;

		public long StartTime;

		public int RoundNum;

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			DanceBattleTurnData danceBattleTurnData = other as DanceBattleTurnData;
			if (danceBattleTurnData == null)
			{
				return false;
			}
			return danceBattleTurnData.StartTime == StartTime;
		}

		public override int GetHashCode()
		{
			return (int)StartTime;
		}
	}
}
