using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Scene;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Net
{
	public static class WorldServiceEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct WorldJoinedEvent
		{
		}

		public struct ItemSpawned
		{
			public readonly CPMMOItem Item;

			public ItemSpawned(CPMMOItem item)
			{
				Item = item;
			}
		}

		public struct ItemChanged
		{
			public readonly CPMMOItem Item;

			public ItemChanged(CPMMOItem item)
			{
				Item = item;
			}
		}

		public struct ItemDestroyed
		{
			public readonly CPMMOItemId ItemId;

			public ItemDestroyed(CPMMOItemId itemId)
			{
				ItemId = itemId;
			}
		}

		public struct ItemMoved
		{
			public readonly CPMMOItemId ItemId;

			public readonly Vector3 Position;

			public ItemMoved(CPMMOItemId itemId, Vector3 position)
			{
				ItemId = itemId;
				Position = position;
			}
		}

		public struct RoomsFoundEvent
		{
			public readonly IList<RoomIdentifier> Rooms;

			public RoomsFoundEvent(IList<RoomIdentifier> rooms)
			{
				Rooms = rooms;
			}
		}

		public struct RoomFoundEvent
		{
			public readonly RoomIdentifier Room;

			public RoomFoundEvent(RoomIdentifier room)
			{
				Room = room;
			}
		}

		public struct SessionEstablishedEvent
		{
			public readonly SignedResponse<SessionData> SignedSessionData;

			public SessionEstablishedEvent(SignedResponse<SessionData> signedSessionData)
			{
				SignedSessionData = signedSessionData;
			}
		}

		public struct PlayerJoinRoomEvent
		{
			public readonly long SessionId;

			public readonly string Name;

			public PlayerJoinRoomEvent(long sessionId, string name)
			{
				SessionId = sessionId;
				Name = name;
			}
		}

		public struct PlayerLeaveRoomEvent
		{
			public readonly long SessionId;

			public PlayerLeaveRoomEvent(long sessionId)
			{
				SessionId = sessionId;
			}
		}

		public struct SelfRoomJoinedEvent
		{
			public readonly long SessionId;

			public readonly string Name;

			public readonly RoomIdentifier Room;

			public readonly SceneLayout ExtraLayoutData;

			public readonly string RoomOwnerName;

			public readonly bool IsRoomOwner;

			public SelfRoomJoinedEvent(long sessionId, string name, RoomIdentifier room, SceneLayout extraLayoutData, string roomOwnerName, bool isRoomOwner)
			{
				SessionId = sessionId;
				Name = name;
				Room = room;
				ExtraLayoutData = extraLayoutData;
				RoomOwnerName = roomOwnerName;
				IsRoomOwner = isRoomOwner;
			}
		}

		public struct RoomPopulationReceivedEvent
		{
			public readonly List<RoomPopulation> RoomPopulations;

			public RoomPopulationReceivedEvent(List<RoomPopulation> roomPopulations)
			{
				RoomPopulations = roomPopulations;
			}
		}

		public struct WorldsWithRoomPopulationReceivedEvent
		{
			public List<WorldRoomPopulation> WorldRoomPopulations;

			public WorldsWithRoomPopulationReceivedEvent(List<WorldRoomPopulation> worldRoomPopulations)
			{
				WorldRoomPopulations = worldRoomPopulations;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SelfLeaveRoomEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SelfWillLeaveRoomEvent
		{
		}

		public struct ContentDateChanged
		{
			public readonly DateTime ContentDate;

			public ContentDateChanged(DateTime contentDate)
			{
				ContentDate = contentDate;
			}
		}
	}
}
