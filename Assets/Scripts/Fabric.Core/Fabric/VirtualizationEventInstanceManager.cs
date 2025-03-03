using System.Collections.Generic;

namespace Fabric
{
	internal class VirtualizationEventInstanceManager
	{
		public Queue<VirtualizationEventInstance> _eventInstancePool = new Queue<VirtualizationEventInstance>();

		public void Initialise(int size, Component component)
		{
			for (int i = 0; i < size; i++)
			{
				_eventInstancePool.Enqueue(new VirtualizationEventInstance(component));
			}
		}

		public void Shutdown()
		{
			_eventInstancePool.Clear();
		}

		public VirtualizationEventInstance Alloc(Event e)
		{
			if (_eventInstancePool.Count == 0)
			{
				DebugLog.Print("VirtualizationEventInstance: Failed to allocate event instance for Event: " + e._eventName, DebugLevel.Error);
				return null;
			}
			VirtualizationEventInstance virtualizationEventInstance = _eventInstancePool.Dequeue();
			Event @event = new Event();
			@event.Copy(e);
			virtualizationEventInstance._event = @event;
			return virtualizationEventInstance;
		}

		public void Free(VirtualizationEventInstance instance)
		{
			instance._componentInstance = null;
			instance._event = null;
			instance._isPlaying = false;
			instance._dspTime = 0.0;
			instance._time = 0f;
			_eventInstancePool.Enqueue(instance);
		}
	}
}
