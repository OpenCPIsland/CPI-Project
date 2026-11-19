#if !UNITY_WEBGL
using System.Collections.Generic;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanPlayer
	{
		private static int nextUserId = 1;

		public readonly int UserId;

		public readonly LanConnection Connection;

		public string ZoneName = "ClubPenguin";

		public string RoomName = "ClubPenguin_default_room";

		public string RoomKey = "";

		public string UserName = "guest";

		public string PlayerSessionId;

		public string OutfitJson;

		public int? Colour;

		public int? SelectedTubeId;

		public int? DaysOld;

		public Dictionary<string, long> MascotXP;

		public string Swid;

		public bool LoggedIn;

		public bool Registered;

		public float[] LastPosition;

		public bool Announced;

		public bool Placed;

		public byte[] AesKey;

		public string EquippedItemName;

		public string EquippedConsumable;

		public int? HeldActionCount;

		public string HeldObject;

		public int LocomotionState;

		public long? ThrowChargingSince;

		public int? PendingLobbyItem;

		public long LastTakeAt = long.MinValue / 2;

		public readonly List<string> CompletedObjectives = new List<string>();

		public readonly Dictionary<string, long> RoomCollected = new Dictionary<string, long>();

		public LanPlayer(LanConnection connection)
		{
			Connection = connection;
			UserId = System.Threading.Interlocked.Increment(ref nextUserId);
		}

		public string AccountKey
		{
			get
			{
				return Swid ?? ("user" + UserId);
			}
		}

		public void Send(byte[] packet)
		{
			Connection.Send(packet);
		}
	}
}
#endif
