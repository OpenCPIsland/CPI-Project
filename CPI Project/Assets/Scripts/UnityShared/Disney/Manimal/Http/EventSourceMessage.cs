using System;

namespace Disney.Manimal.Http
{
	public class EventSourceMessage : EventArgs
	{
		public string Type
		{
			get;
			set;
		}

		public string LastEventID
		{
			get;
			set;
		}

		public string Data
		{
			get;
			set;
		}

		public EventSourceMessage()
		{
		}

		public EventSourceMessage(string type, string lastEventID, string data)
		{
			Type = type;
			LastEventID = lastEventID;
			Data = data;
		}
	}
}
