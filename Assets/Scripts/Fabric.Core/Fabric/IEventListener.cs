using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	public interface IEventListener
	{
		bool IsDestroyed
		{
			get;
		}

		EventStatus Process(Event postedEvent);

		bool GetEventListeners(string eventName, List<EventListener> listeners);

		bool GetEventListeners(int eventID, List<EventListener> listeners);

		bool IsActive(GameObject parentGameObject);

		bool GetEventInfo(GameObject parentGameObject, ref EventInfo eventInfo);
	}
}
