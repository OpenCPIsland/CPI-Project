#define UNITY_ASSERTIONS
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Player
{
	[DisallowMultipleComponent]
	public class PlayerTallyController : MonoBehaviour
	{
		private int playerRoomCount = 0;

		private EventChannel eventChannel;

		public int PlayerRoomCount
		{
			get
			{
				return playerRoomCount;
			}
			private set
			{
				Assert.IsTrue(value >= 0, "Room population should never be negative!");
				if (playerRoomCount != value)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PlayerSpawnedEvents.RoomPopulationChanged(value));
				}
				playerRoomCount = value;
			}
		}

		[Invokable("Igloo.Debug.SetRoomPopulation")]
		public static void SetRoomPopulation(int count)
		{
			PlayerTallyController playerTallyController = Object.FindObjectOfType<PlayerTallyController>();
			if (playerTallyController != null)
			{
				playerTallyController.PlayerRoomCount = count;
			}
		}

		public void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<NetworkControllerEvents.RemotePlayerJoinedRoomEvent>(onRemotePlayerJoinedRoom, EventDispatcher.Priority.LAST);
			eventChannel.AddListener<NetworkControllerEvents.RemotePlayerLeftRoomEvent>(onRemotePlayerLeftRoom, EventDispatcher.Priority.LAST);
			updateUserCount();
		}

		public void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		private bool onRemotePlayerJoinedRoom(NetworkControllerEvents.RemotePlayerJoinedRoomEvent evt)
		{
			updateUserCount();
			return false;
		}

		private bool onRemotePlayerLeftRoom(NetworkControllerEvents.RemotePlayerLeftRoomEvent evt)
		{
			updateUserCount();
			return false;
		}

		private void updateUserCount()
		{
			PlayerRoomCount = Service.Get<INetworkServicesManager>().ServerUserCount;
		}
	}
}
