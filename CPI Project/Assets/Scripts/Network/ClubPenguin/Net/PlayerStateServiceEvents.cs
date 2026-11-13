using ClubPenguin.Net.Client.Smartfox.SFSObject;
using ClubPenguin.Net.Domain;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class PlayerStateServiceEvents
	{
		public struct PlayerLocoStateChanged
		{
			public readonly long SessionId;

			public readonly LocomotionState State;

			public PlayerLocoStateChanged(long sessionId, LocomotionState state)
			{
				SessionId = sessionId;
				State = state;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PlayerOutfitSaved
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PlayerReferralSet
		{
		}

		public struct PlayerOutfitChanged
		{
			public readonly PlayerOutfitDetails Outfit;

			public PlayerOutfitChanged(PlayerOutfitDetails outfit)
			{
				Outfit = outfit;
			}
		}

		public struct PlayerProfileChanged
		{
			public readonly long SessionId;

			public readonly Profile Profile;

			public PlayerProfileChanged(long sessionId, Profile profile)
			{
				SessionId = sessionId;
				Profile = profile;
			}
		}

		public struct PlayerMembershipStatusChanged
		{
			public readonly bool IsMember;

			public PlayerMembershipStatusChanged(bool isMember)
			{
				IsMember = isMember;
			}
		}

		public struct LocalPlayerDataReceived
		{
			public readonly LocalPlayerData Data;

			public LocalPlayerDataReceived(LocalPlayerData data)
			{
				Data = data;
			}
		}

		public struct MigrationDataRecieved
		{
			public readonly MigrationData Data;

			public MigrationDataRecieved(MigrationData data)
			{
				Data = data;
			}
		}

		public struct PreregistrationDataReceived
		{
			public readonly PreregistrationData Data;

			public PreregistrationDataReceived(PreregistrationData data)
			{
				Data = data;
			}
		}

		public struct OtherPlayerDataReceived
		{
			public readonly OtherPlayerData Data;

			public OtherPlayerDataReceived(OtherPlayerData data)
			{
				Data = data;
			}
		}

		public struct OtherPlayerDataListReceived
		{
			public readonly IList<OtherPlayerData> Data;

			public OtherPlayerDataListReceived(IList<OtherPlayerData> data)
			{
				Data = data;
			}
		}

		public struct OnlinePlayerSwidListReceived
		{
			public readonly IList<string> Swids;

			public OnlinePlayerSwidListReceived(IList<string> swids)
			{
				Swids = swids;
			}
		}

		public struct DispensableEquipped
		{
			public readonly long SessionId;

			public readonly string Type;

			public DispensableEquipped(long sessionId, string type)
			{
				SessionId = sessionId;
				Type = type;
			}
		}

		public struct ConsumableEquipped
		{
			public readonly long SessionId;

			public readonly string Type;

			public ConsumableEquipped(long sessionId, string type)
			{
				SessionId = sessionId;
				Type = type;
			}
		}

		public struct DurableEquipped
		{
			public readonly long SessionId;

			public readonly string Type;

			public DurableEquipped(long sessionId, string type)
			{
				SessionId = sessionId;
				Type = type;
			}
		}

		public struct PartyGameEquipped
		{
			public readonly long SessionId;

			public readonly string Type;

			public PartyGameEquipped(long sessionId, string type)
			{
				SessionId = sessionId;
				Type = type;
			}
		}

		public struct TubeSelected
		{
			public readonly long SessionId;

			public readonly int TubeId;

			public TubeSelected(long sessionId, int tubeId)
			{
				SessionId = sessionId;
				TubeId = tubeId;
			}
		}

		public struct HeldObjectDequipped
		{
			public readonly long SessionId;

			public HeldObjectDequipped(long sessionId)
			{
				SessionId = sessionId;
			}
		}

		public struct AirBubbleChanged
		{
			public readonly long SessionId;

			public readonly AirBubble AirBubble;

			public AirBubbleChanged(long sessionId, AirBubble airBubble)
			{
				SessionId = sessionId;
				AirBubble = airBubble;
			}
		}

		public struct AwayFromKeyboardStateChanged
		{
			public readonly long SessionId;

			public readonly int Value;

			public readonly EquippedObject EquippedObject;

			public AwayFromKeyboardStateChanged(long sessionId, int value, EquippedObject equippedObject)
			{
				SessionId = sessionId;
				Value = value;
				EquippedObject = equippedObject;
			}
		}

		public struct TemporaryHeadStatusChanged
		{
			public readonly long SessionId;

			public readonly int Type;

			public TemporaryHeadStatusChanged(long sessionId, int type)
			{
				SessionId = sessionId;
				Type = type;
			}
		}
	}
}
