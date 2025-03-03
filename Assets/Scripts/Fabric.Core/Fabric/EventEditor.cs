using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class EventEditor
	{
		[Serializable]
		public class EventItem
		{
			[NonSerialized]
			private Event _runtimeEvent = new Event();

			[SerializeField]
			public Event _event = new Event();

			[SerializeField]
			public Component _component;

			[SerializeField]
			public bool _ignoreEventAction;

			[SerializeField]
			public ParameterData _parameterData;

			[SerializeField]
			public DSPParameterData _dspParameter;

			[SerializeField]
			public SwitchPresetData _switchPresetData;

			[SerializeField]
			public GlobalParameterData _globalParameterData;

			[SerializeField]
			public GlobalSwitchParameterData _globalSwitchParameterData;

			[SerializeField]
			public TransitionToSnapshotData _transitionToSnapshotData;

			[SerializeField]
			public string _eventValue = "";

			[SerializeField]
			public object _parameter;

			[SerializeField]
			public float _eventParameter = 1f;

			[SerializeField]
			public double _eventScheduleParameter;

			[SerializeField]
			public string _eventParameterName = "";

			[SerializeField]
			public DSPType _dspType;

			[SerializeField]
			public float _timeToTarget;

			[SerializeField]
			public float _curve = 0.5f;

			[SerializeField]
			public float _min;

			[SerializeField]
			public float _max = 1f;

			public EventStatus ProcessEvent(Event zEvent)
			{
				_runtimeEvent.Copy(_event);
				_runtimeEvent.parentGameObject = zEvent.parentGameObject;
				_runtimeEvent._eventName = zEvent._eventName;
				if (_ignoreEventAction)
				{
					_runtimeEvent.Copy(zEvent);
					_runtimeEvent.EventAction = zEvent.EventAction;
				}
				else
				{
					if (zEvent._forceEventAction)
					{
						_runtimeEvent.EventAction = zEvent.EventAction;
					}
					BuildEvent();
				}
				if (_runtimeEvent.EventAction == EventAction.SetGlobalParameter || _runtimeEvent.EventAction == EventAction.SetGlobalSwitch || _runtimeEvent.EventAction == EventAction.AddPreset || _runtimeEvent.EventAction == EventAction.RemovePreset || _runtimeEvent.EventAction == EventAction.SwitchPreset || _runtimeEvent.EventAction == EventAction.ResetDynamicMixer || _runtimeEvent.EventAction == EventAction.TransitionToSnapshot || _runtimeEvent.EventAction == EventAction.LoadAudioMixer || _runtimeEvent.EventAction == EventAction.UnloadAudioMixer || _runtimeEvent.EventAction == EventAction.MicStart || _runtimeEvent.EventAction == EventAction.MicStop)
				{
					EventManager.Instance.ProcessEvent(_runtimeEvent);
				}
				else if (_component != null)
				{
					_runtimeEvent.eventStatus = ((IEventListener)_component).Process(_runtimeEvent);
				}
				if (Application.isEditor && _runtimeEvent.eventStatus != EventStatus.Not_Handled && _runtimeEvent.EventAction == EventAction.PlaySound)
				{
					EventManager.Instance.AddActiveEvent(_runtimeEvent, _component);
				}
				zEvent.Copy(_runtimeEvent);
				return _runtimeEvent.eventStatus;
			}

			public void BuildEvent()
			{
				if (_runtimeEvent.EventAction == EventAction.SetPitch || _runtimeEvent.EventAction == EventAction.SetVolume || _runtimeEvent.EventAction == EventAction.SetPan || _runtimeEvent.EventAction == EventAction.SetTime || _runtimeEvent.EventAction == EventAction.SetVolumeProperty || _runtimeEvent.EventAction == EventAction.SetPitchProperty)
				{
					_runtimeEvent._parameter = _eventParameter;
				}
				else if (_runtimeEvent.EventAction == EventAction.SetParameter)
				{
					_parameterData._parameter = _eventParameterName.GetHashCode();
					_parameterData._value = _eventParameter;
					_runtimeEvent._parameter = _parameterData;
				}
				else if (_runtimeEvent.EventAction == EventAction.SetDSPParameter)
				{
					_dspParameter._dspType = _dspType;
					_dspParameter._parameter = _eventParameterName;
					_dspParameter._value = _eventParameter;
					_dspParameter._time = _timeToTarget;
					_dspParameter._curve = _curve;
					_runtimeEvent._parameter = _dspParameter;
				}
				else if (_runtimeEvent.EventAction == EventAction.SwitchPreset)
				{
					_runtimeEvent._parameter = _switchPresetData;
				}
				else if (_runtimeEvent.EventAction == EventAction.SetGlobalParameter)
				{
					if (_globalParameterData != null)
					{
						_runtimeEvent._parameter = _globalParameterData;
					}
				}
				else if (_runtimeEvent.EventAction == EventAction.SetGlobalSwitch)
				{
					if (_globalSwitchParameterData != null)
					{
						_runtimeEvent._parameter = _globalSwitchParameterData;
					}
				}
				else if (_runtimeEvent.EventAction == EventAction.SetRegion || _runtimeEvent.EventAction == EventAction.QueueRegion)
				{
					_runtimeEvent._parameter = _eventParameterName;
				}
				else if (_runtimeEvent.EventAction == EventAction.LoadAudioMixer || _runtimeEvent.EventAction == EventAction.UnloadAudioMixer)
				{
					_runtimeEvent._parameter = _eventParameterName;
				}
				else if (_runtimeEvent.EventAction == EventAction.TransitionToSnapshot)
				{
					_runtimeEvent._parameter = _transitionToSnapshotData;
				}
				else if (_runtimeEvent.EventAction == EventAction.PlayScheduled || _runtimeEvent.EventAction == EventAction.StopScheduled)
				{
					_runtimeEvent._parameter = _eventScheduleParameter;
				}
				else
				{
					_runtimeEvent._parameter = _eventValue;
				}
			}
		}

		[Serializable]
		public class EventEntry : IEventListener
		{
			[SerializeField]
			public float _delay;

			[SerializeField]
			public int _probability = 100;

			[NonSerialized]
			public int _postCount;

			[SerializeField]
			public int _postCountMax;

			[SerializeField]
			public bool _addToQueue;

			[SerializeField]
			public string _eventName;

			[SerializeField]
			public bool _ignoreIncomingGameObject;

			[SerializeField]
			public List<EventItem> _eventList = new List<EventItem>();

			[NonSerialized]
			private List<Event> _eventQueue = new List<Event>();

			bool IEventListener.IsDestroyed
			{
				get
				{
					return this == null;
				}
			}

			private System.Random rnd
			{
				get
				{
					return Generic._random;
				}
			}

			public void Initialise()
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.RegisterListener(this, _eventName);
				}
			}

			public void Shutdown()
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.UnregisterListener(this, _eventName);
				}
			}

			public bool IsComponentPresent(Component component)
			{
				for (int i = 0; i < _eventList.Count; i++)
				{
					if (_eventList[i]._component == component)
					{
						return true;
					}
				}
				return false;
			}

			EventStatus IEventListener.Process(Event zEvent)
			{
				if (_probability < 100)
				{
					int num = (int)(rnd.NextDouble() * 100.0);
					if (num > _probability)
					{
						return EventStatus.Not_Handled_Probability;
					}
				}
				if (_postCountMax > 0)
				{
					if (_postCount >= _postCountMax)
					{
						return EventStatus.Not_Handled_PostCountMax;
					}
					_postCount++;
				}
				if (_ignoreIncomingGameObject)
				{
					zEvent.parentGameObject = FabricManager.Instance.gameObject;
				}
				if (_delay == 0f)
				{
					return ProcessEvent(zEvent);
				}
				_eventQueue.Add(zEvent);
				zEvent._delay = _delay;
				zEvent._delayTimer = 0f;
				return EventStatus.InQueue;
			}

			private EventStatus ProcessEvent(Event zEvent)
			{
				EventStatus result = EventStatus.Not_Handled;
				for (int i = 0; i < _eventList.Count; i++)
				{
					EventStatus eventStatus = _eventList[i].ProcessEvent(zEvent);
					if (eventStatus != EventStatus.Not_Handled)
					{
						result = EventStatus.Handled;
					}
				}
				return result;
			}

			public void Update()
			{
				for (int i = 0; i < _eventQueue.Count; i++)
				{
					Event @event = _eventQueue[i];
					if (@event._delayTimer < @event._delay)
					{
						@event._delayTimer += FabricTimer.GetRealtimeDelta();
						continue;
					}
					ProcessEvent(@event);
					@event._delay = 0f;
					@event._delayTimer = 0f;
					@event.eventStatus = EventStatus.Handled;
					_eventQueue.RemoveAt(i);
				}
			}

			bool IEventListener.IsActive(GameObject parentGameObject)
			{
				for (int i = 0; i < _eventList.Count; i++)
				{
					if (_eventList[i]._component != null && _eventList[i]._component.IsComponentActive())
					{
						return true;
					}
				}
				return false;
			}

			bool IEventListener.GetEventListeners(string eventName, List<EventListener> listeners)
			{
				return false;
			}

			bool IEventListener.GetEventListeners(int eventID, List<EventListener> listeners)
			{
				return false;
			}

			bool IEventListener.GetEventInfo(GameObject parentGameObject, ref EventInfo eventInfo)
			{
				return false;
			}
		}

		[SerializeField]
		public List<string> _eventNames = new List<string>();

		[SerializeField]
		public List<EventEntry> _eventList = new List<EventEntry>();

		[NonSerialized]
		private Component _cachedComponent;

		public void Initialise()
		{
			for (int i = 0; i < _eventList.Count; i++)
			{
				_eventList[i].Initialise();
			}
		}

		public void Shutdown()
		{
			for (int i = 0; i < _eventList.Count; i++)
			{
				_eventList[i].Shutdown();
			}
		}

		public void AddEventEntry(EventEntry eventEntry)
		{
			_eventList.Add(eventEntry);
			eventEntry.Initialise();
		}

		public void RemoveEventEntry(EventEntry eventEntry)
		{
			eventEntry.Shutdown();
			_eventList.Remove(eventEntry);
		}

		public void RemoveEventName(string eventName)
		{
			for (int i = 0; i < _eventList.Count; i++)
			{
				if (_eventList[i]._eventName == eventName)
				{
					_eventList.RemoveAt(i);
					return;
				}
			}
			int num = 0;
			while (true)
			{
				if (num < _eventNames.Count)
				{
					if (_eventNames[num] == eventName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_eventNames.RemoveAt(num);
		}

		public void RenameEventName(string oldEventName, string newEventName)
		{
			for (int i = 0; i < _eventList.Count; i++)
			{
				if (_eventList[i]._eventName == oldEventName)
				{
					_eventList[i]._eventName = newEventName;
					return;
				}
			}
			int num = 0;
			while (true)
			{
				if (num < _eventNames.Count)
				{
					if (_eventNames[num] == oldEventName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_eventNames[num] = newEventName;
		}

		public void Update()
		{
			for (int i = 0; i < _eventList.Count; i++)
			{
				_eventList[i].Update();
			}
		}

		public bool IsComponentPresent(Component component)
		{
			if (_cachedComponent != null && component == _cachedComponent)
			{
				return true;
			}
			for (int i = 0; i < _eventList.Count; i++)
			{
				if (_eventList[i].IsComponentPresent(component))
				{
					_cachedComponent = component;
					return true;
				}
			}
			return false;
		}

		public EventEntry GetEntryByEventName(string eventName)
		{
			for (int i = 0; i < _eventList.Count; i++)
			{
				if (_eventList[i]._eventName == eventName)
				{
					return _eventList[i];
				}
			}
			return null;
		}

		public void RemoveComponent(Component component)
		{
			_cachedComponent = null;
		}

		public EventStatus ProcessEvent(Event zEvent)
		{
			return EventStatus.Not_Handled;
		}
	}
}
