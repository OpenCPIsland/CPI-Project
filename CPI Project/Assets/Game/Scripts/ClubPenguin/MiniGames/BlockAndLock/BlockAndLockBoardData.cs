using UnityEngine;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	public class BlockAndLockBoardData : Component
	{
		public PieceCategory Category;

		public int Id;

		public BlockAndLockBoardData(PieceCategory category, int id)
		{
			Category = category;
			Id = id;
		}
	}
}
