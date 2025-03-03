using ClubPenguin.Net.Client;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin.Net
{
	public class LeaveRoomSequence
	{
		private ClubPenguinClient clubPenguinClient;

		private System.Action successHandler;

		public LeaveRoomSequence(ClubPenguinClient client, System.Action successHandler = null)
		{
			clubPenguinClient = client;
			this.successHandler = successHandler;
		}

		public void LeaveRoom(bool immediately = false)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(WorldServiceEvents.SelfWillLeaveRoomEvent));
			if (immediately)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(WorldServiceEvents.SelfLeaveRoomEvent));
				if (successHandler != null)
				{
					successHandler();
				}
			}
			else
			{
				clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ROOM_LEAVE, onRoomLeave);
			}
			clubPenguinClient.GameServer.LeaveRoom();
		}

		private void onRoomLeave(GameServerEvent evt, object data)
		{
			removeSFSListeners();
			Service.Get<EventDispatcher>().DispatchEvent(default(WorldServiceEvents.SelfLeaveRoomEvent));
			if (successHandler != null)
			{
				successHandler();
			}
		}

		private void removeSFSListeners()
		{
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.ROOM_LEAVE, onRoomLeave);
		}
	}
}
