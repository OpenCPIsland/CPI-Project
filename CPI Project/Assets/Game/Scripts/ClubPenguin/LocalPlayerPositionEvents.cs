using UnityEngine;

namespace ClubPenguin
{
	public static class LocalPlayerPositionEvents
	{
		public struct PlayerPositionChangedEvent
		{
			public readonly Vector3 PreviousPosition;

			public readonly GameObject Player;

			public PlayerPositionChangedEvent(Vector3 previousPosiiton, GameObject player)
			{
				PreviousPosition = previousPosiiton;
				Player = player;
			}
		}
	}
}
