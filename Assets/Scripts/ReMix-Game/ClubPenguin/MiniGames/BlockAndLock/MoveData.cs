using UnityEngine;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	public class MoveData : Component
	{
		public Grid2 stopPos;

		public bool hitPiece;

		public MoveData(Grid2 stopPos, bool hitPiece)
		{
			this.stopPos = stopPos;
			this.hitPiece = hitPiece;
		}
	}
}
