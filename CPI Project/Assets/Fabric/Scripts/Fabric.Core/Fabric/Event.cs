using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class Event
	{
		[SerializeField]
		public string _eventName = "";

		[SerializeField]
		public int _eventID;

		[SerializeField]
		public EventAction EventAction;

		[NonSerialized]
		private EventStatus _status;

		[SerializeField]
		public GameObject parentGameObject;

		[SerializeField]
		public object _parameter;

		[SerializeField]
		public float _delay;

		[SerializeField]
		public float _delayTimer;

		[SerializeField]
		public InitialiseParameters _initialiseParameters;

		[SerializeField]
		public OnEventNotify _onEventNotify;

		[SerializeField]
		public string _eventCategory = "";

		[SerializeField]
		public int _priority;

		[NonSerialized]
		public bool _forceEventAction;

		public EventStatus eventStatus
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
			}
		}

		public void Copy(Event fromEvent)
		{
			_eventName = fromEvent._eventName;
			EventAction = fromEvent.EventAction;
			eventStatus = fromEvent.eventStatus;
			parentGameObject = fromEvent.parentGameObject;
			_parameter = fromEvent._parameter;
			_delay = fromEvent._delay;
			_delayTimer = fromEvent._delayTimer;
			_initialiseParameters = fromEvent._initialiseParameters;
			_onEventNotify = fromEvent._onEventNotify;
			_eventCategory = fromEvent._eventCategory;
			_priority = fromEvent._priority;
		}

		public void Reset()
		{
			_eventName = "";
			EventAction = EventAction.PlaySound;
			eventStatus = EventStatus.Not_Handled;
			parentGameObject = null;
			_parameter = null;
			_delay = 0f;
			_delayTimer = 0f;
			_initialiseParameters = null;
			_onEventNotify = null;
			_eventCategory = null;
			_priority = 0;
		}
	}
}
