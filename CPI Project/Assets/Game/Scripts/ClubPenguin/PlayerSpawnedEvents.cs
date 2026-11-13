using Disney.Kelowna.Common.DataModel;
using UnityEngine;

namespace ClubPenguin
{
	public static class PlayerSpawnedEvents
	{
		public struct LocalPlayerReadyToSpawn
		{
			public readonly DataEntityHandle Handle;

			public LocalPlayerReadyToSpawn(DataEntityHandle handle)
			{
				Handle = handle;
			}
		}

		public struct LocalPlayerSpawned
		{
			public GameObject LocalPlayerGameObject;

			public DataEntityHandle Handle;

			public LocalPlayerSpawned(GameObject localPlayerGameObject, DataEntityHandle handle)
			{
				LocalPlayerGameObject = localPlayerGameObject;
				Handle = handle;
			}
		}

		public struct RemotePlayerSpawned
		{
			public GameObject RemotePlayerGameObject;

			public DataEntityHandle Handle;

			public RemotePlayerSpawned(GameObject remotePlayerGameObject, DataEntityHandle handle)
			{
				RemotePlayerGameObject = remotePlayerGameObject;
				Handle = handle;
			}
		}

		public struct RoomPopulationChanged
		{
			public readonly int Count;

			public RoomPopulationChanged(int count)
			{
				Count = count;
			}
		}
	}
}
