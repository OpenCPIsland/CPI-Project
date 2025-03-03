using System;

namespace Disney.Manimal.Http
{
	public interface IEventSource
	{
		event EventHandler<EventSourceMessage> OnMessage;
	}
}
