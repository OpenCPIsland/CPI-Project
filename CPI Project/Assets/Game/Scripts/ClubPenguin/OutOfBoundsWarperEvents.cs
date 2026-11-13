using UnityEngine;

namespace ClubPenguin
{
	public class OutOfBoundsWarperEvents
	{
		public struct ResetPlayer
		{
			public GameObject Player;

			public ResetPlayer(GameObject player)
			{
				Player = player;
			}
		}
	}
}
