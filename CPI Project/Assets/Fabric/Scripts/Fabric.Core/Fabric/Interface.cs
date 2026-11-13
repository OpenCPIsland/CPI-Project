using UnityEngine;

namespace Fabric
{
	public class Interface
	{
		private static bool _enable = true;

		public void Enable(bool enable)
		{
			_enable = enable;
		}

		public bool PostEvent(string eventName, EventAction eventAction = EventAction.PlaySound, GameObject parentGameObject = null, object parameter = null, OnEventNotify onEventNotify = null, InitialiseParameters initialiseParameters = null, bool addToQueue = false)
		{
			if (_enable && EventManager.Instance != null)
			{
				return EventManager.Instance.PostEvent(eventName, eventAction, parameter, parentGameObject, initialiseParameters, addToQueue, onEventNotify);
			}
			return false;
		}

		public bool PostEvent(int eventID, EventAction eventAction = EventAction.PlaySound, GameObject parentGameObject = null, object parameter = null, OnEventNotify onEventNotify = null, InitialiseParameters initialiseParameters = null, bool addToQueue = false)
		{
			if (_enable && EventManager.Instance != null)
			{
				return EventManager.Instance.PostEvent(eventID, eventAction, parameter, parentGameObject, initialiseParameters, addToQueue, onEventNotify);
			}
			return false;
		}

		public static void SetParameter(string eventName, string parameterName, float value, GameObject parentGameObject = null)
		{
			if (_enable && EventManager.Instance != null)
			{
				EventManager.Instance.SetParameter(eventName, parameterName, value, parentGameObject);
			}
		}
	}
}
