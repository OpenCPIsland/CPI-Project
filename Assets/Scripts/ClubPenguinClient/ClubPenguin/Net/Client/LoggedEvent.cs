using System;

namespace ClubPenguin.Net.Client
{
	[Serializable]
	public class LoggedEvent
	{
		public GameServerEvent GameServerEvent;

		public object Data;

		public long Ticks;

		public LoggedEvent(GameServerEvent gameServerEvent, object data)
		{
			GameServerEvent = gameServerEvent;
			Data = data;
			Ticks = DateTime.Now.Ticks;
		}
	}
}
