namespace Disney.LaunchPadFramework
{
	public class ReadonlyEventDispatcher
	{
		private EventDispatcher eventDispatcher = new EventDispatcher();

		public ReadonlyEventDispatcher(EventDispatcher eventDispatcher)
		{
			this.eventDispatcher = eventDispatcher;
		}

		public void AddListener<T>(EventHandlerDelegate<T> handler, EventDispatcher.Priority priority = EventDispatcher.Priority.DEFAULT) where T : struct
		{
			eventDispatcher.AddListener(handler, priority);
		}

		public bool RemoveListener<T>(EventHandlerDelegate<T> handler) where T : struct
		{
			return eventDispatcher.RemoveListener(handler);
		}
	}
}
