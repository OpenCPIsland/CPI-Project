using UnityEngine;

namespace ClubPenguin.MiniGames.TiltATube
{
	public class TiltATubesEvents : MonoBehaviour
	{
		public struct AddPlayer
		{
			public readonly string VolumeId;

			public readonly GameObject PlayerGameObj;

			public readonly bool IsTubing;

			public AddPlayer(string volumeId, GameObject playerObj, bool isTubing)
			{
				VolumeId = volumeId;
				PlayerGameObj = playerObj;
				IsTubing = isTubing;
			}
		}

		public struct RemovePlayer
		{
			public readonly string VolumeId;

			public readonly GameObject PlayerGameObj;

			public readonly bool IsTubing;

			public RemovePlayer(string volumeId, GameObject playerObj, bool isTubing)
			{
				VolumeId = volumeId;
				PlayerGameObj = playerObj;
				IsTubing = isTubing;
			}
		}

		public struct DisconnectPlayer
		{
			public readonly string VolumeId;

			public readonly string PlayerName;

			public DisconnectPlayer(string volumeId, string playerName)
			{
				VolumeId = volumeId;
				PlayerName = playerName;
			}
		}
	}
}
