using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/EventsManager")]
	[ExecuteInEditMode]
	public class EventManager : MonoBehaviour
	{
		public class ActiveEvent
		{
			[NonSerialized]
			public string _eventName;

			[NonSerialized]
			public GameObject _parentGameObject;

			[NonSerialized]
			public Component _component;

			[NonSerialized]
			public bool _componentInstanceFoldout = true;

			[NonSerialized]
			public bool _virtualEventsFoldout;

			[NonSerialized]
			public float _time;
		}

		public delegate void OnLogEventHandler(Event postedEvent);

		private static EventManager _instance = null;

		private Dictionary<string, List<IEventListener>> _listeners = new Dictionary<string, List<IEventListener>>();

		private Dictionary<int, List<IEventListener>> _listenersHashed = new Dictionary<int, List<IEventListener>>();

		[SerializeField]
		public GlobalParameterManager _globalParameterManager = new GlobalParameterManager();

		private List<Event> _eventQueue = new List<Event>();

		[HideInInspector]
		[SerializeField]
		public List<string> _eventList = new List<string>();

		[HideInInspector]
		[SerializeField]
		public EventEditor _eventEditor = new EventEditor();

		[NonSerialized]
		[HideInInspector]
		public List<ActiveEvent> _activeEvents = new List<ActiveEvent>();

		[NonSerialized]
		[HideInInspector]
		public float _activeEventPersistenceTime = 1f;

		[HideInInspector]
		public CodeProfiler profiler = new CodeProfiler();

		[HideInInspector]
		[SerializeField]
		public bool _eventMenuListFoldout = true;

		[HideInInspector]
		[SerializeField]
		public bool _eventListFoldout;

		[SerializeField]
		[HideInInspector]
		public bool _forceQueueAllEvents;

		[HideInInspector]
		[SerializeField]
		public EventSequencer _eventSequencer = new EventSequencer();

		private Event newEvent = new Event();

		private Event overrideEvent = new Event();

		private ParameterData parameter = new ParameterData();

		private DSPParameterData dspParameter = new DSPParameterData();

		private GlobalParameterData globalParameter = new GlobalParameterData();

		private GlobalSwitchParameterData globalSwitch = new GlobalSwitchParameterData();

		private List<EventListener> eventListeners = new List<EventListener>(32);
		
////////////////////Fabric 2.5.0 Structs//////////////////		
		[HideInInspector]
		[SerializeField]
		public bool _deployWithActiveEventList;

		public static EventManager Instance
		{
			get
			{
				if (_instance == null)
				{
					if (FabricManager.Instance != null)
					{
						_instance = FabricManager.Instance.GetComponent<EventManager>();
						if (_instance == null)
						{
							_instance = (EventManager)FabricManager.Instance.gameObject.AddComponent(typeof(EventManager));
						}
					}
					if (_instance == null)
					{
						_instance = (EventManager)UnityEngine.Object.FindObjectOfType(typeof(EventManager));
					}
				}
				return _instance;
			}
		}

		public static event OnLogEventHandler OnLogEvent;

		private void Awake()
		{
			Init();
		}
		
		public void Init()
		{
			_instance = this;
			_eventEditor.Initialise();
			_globalParameterManager.Init();
		}

		private void OnEnable()
		{
			if (Application.isEditor && !Application.isPlaying)
			{
				Awake();
			}
		}

		public void Shutdown()
		{
			_eventEditor.Shutdown();
			_globalParameterManager.Shutdown();
			_instance = null;
		}
		
		private void OnDestroy()
		{
			Shutdown();
		}

		public static bool IsInitialised()
		{
			if (_instance == null)
			{
				_instance = Instance;
			}
			if (!(_instance != null))
			{
				return false;
			}
			return true;
		}

		public bool RegisterListener(IEventListener listener, string eventName)
		{
			if (IsNullOrDestroyed(listener) || eventName == null)
			{
				DebugLog.Print("EventManager register listener failed", DebugLevel.Error);
				return false;
			}
			List<IEventListener> list = null;
			if (!_listeners.ContainsKey(eventName))
			{
				list = new List<IEventListener>();
				_listeners.Add(eventName, list);
				_listenersHashed.Add(GetIDFromEventName(eventName), list);
			}
			list = _listeners[eventName];
			if (list.Contains(listener))
			{
				DebugLog.Print("Listener " + listener.GetType().ToString() + " is already registered to listen for event " + eventName);
				return false;
			}
			list.Add(listener);
			return true;
		}

		public bool UnregisterListener(IEventListener listener, string eventName)
		{
			if (!_listeners.ContainsKey(eventName))
			{
				return false;
			}
			List<IEventListener> list = _listeners[eventName];
			if (!list.Contains(listener))
			{
				DebugLog.Print("failed to find listener", DebugLevel.Error);
				return false;
			}
			list.Remove(listener);
			if (list.Count == 0)
			{
				_listeners.Remove(eventName);
				_listenersHashed.Remove(GetIDFromEventName(eventName));
			}
			return true;
		}

		public int NumOfEventsInQueue()
		{
			return _eventQueue.Count;
		}

		public void AddEventNames(string[] events)
		{
			for (int i = 0; i < events.Length; i++)
			{
				int num = _eventList.IndexOf(events[i]);
				if (num < 0)
				{
					_eventList.Add(events[i]);
				}
			}
		}

		public void AddEventNamesFromFile(string filename)
		{
			StreamReader streamReader = new StreamReader(filename, Encoding.Default);
			if (streamReader == null)
			{
				return;
			}
			List<string> list = new List<string>();
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text != null)
				{
					list.Add(text);
				}
			}
			while (text != null);
			streamReader.Close();
			AddEventNames(list.ToArray());
		}

		public void ExportEventNamesToFile(string filename)
		{
			StreamWriter streamWriter = new StreamWriter(filename);
			if (streamWriter != null)
			{
				for (int i = 1; i < _eventList.Count; i++)
				{
					streamWriter.WriteLine(_eventList[i]);
				}
				streamWriter.Close();
			}
		}

		public void RemoveEventNamesFromFile(string filename)
		{
			StreamReader streamReader = new StreamReader(filename, Encoding.Default);
			if (streamReader == null)
			{
				return;
			}
			List<string> list = new List<string>();
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text != null)
				{
					list.Add(text);
				}
			}
			while (text != null);
			streamReader.Close();
			RemoveEventNames(list.ToArray());
		}

		public void RemoveEventNames(string[] events)
		{
			for (int i = 0; i < events.Length; i++)
			{
				int num = _eventList.IndexOf(events[i]);
				if (num >= 0)
				{
					_eventList.RemoveAt(num);
				}
			}
		}

		private void LogEvent(Event postedEvent)
		{
			if (EventManager.OnLogEvent != null)
			{
				EventManager.OnLogEvent(postedEvent);
			}
		}

		public bool PostEvent(string eventName)
		{
			return PostEvent(eventName, null);
		}

		public bool PostEvent(string eventName, GameObject parentGameObject)
		{
			return PostEvent(eventName, parentGameObject, null);
		}

		public bool PostEvent(string eventName, GameObject parentGameObject, InitialiseParameters initialiseParameters)
		{
			return PostEvent(eventName, EventAction.PlaySound, null, parentGameObject, initialiseParameters, false);
		}

		public bool PostEvent(string eventName, EventAction eventAction)
		{
			return PostEvent(eventName, eventAction, null, null, null, false);
		}

		public bool PostEvent(string eventName, EventAction eventAction, object parameter)
		{
			return PostEvent(eventName, eventAction, parameter, null, null, false);
		}

		public bool PostEvent(string eventName, EventAction eventAction, object parameter, GameObject parentGameObject)
		{
			return PostEvent(eventName, eventAction, parameter, parentGameObject, null, false);
		}

		public bool PostEvent(string eventName, EventAction eventAction, GameObject parentGameObject)
		{
			return PostEvent(eventName, eventAction, null, parentGameObject, null, false);
		}

		public bool PostEvent(string eventName, EventAction eventAction, object parameter, GameObject parentGameObject, InitialiseParameters initialiseParameters)
		{
			return PostEvent(eventName, eventAction, null, parentGameObject, initialiseParameters, false);
		}

		public bool PostEvent(string eventName, EventAction eventAction, object parameter, GameObject parentGameObject, InitialiseParameters initialiseParameters, bool addToQueue)
		{
			return PostEvent(eventName, eventAction, parameter, parentGameObject, initialiseParameters, addToQueue, null);
		}

		public bool PostEventNotify(string eventName, OnEventNotify onEventNotify)
		{
			return PostEvent(eventName, EventAction.PlaySound, null, null, null, false, onEventNotify);
		}

		public bool PostEventNotify(string eventName, GameObject parentGameObject, OnEventNotify onEventNotify)
		{
			return PostEvent(eventName, EventAction.PlaySound, null, parentGameObject, null, false, onEventNotify);
		}

		public bool PostEventNotify(string eventName, GameObject parentGameObject, InitialiseParameters initialiseParameters, OnEventNotify onEventNotify)
		{
			return PostEvent(eventName, EventAction.PlaySound, null, parentGameObject, initialiseParameters, false, onEventNotify);
		}

		public bool PostEventNotify(string eventName, EventAction eventAction, OnEventNotify onEventNotify)
		{
			return PostEvent(eventName, eventAction, null, null, null, false, onEventNotify);
		}

		public bool PostEventNotify(string eventName, EventAction eventAction, object parameter, OnEventNotify onEventNotify)
		{
			return PostEvent(eventName, eventAction, parameter, null, null, false, onEventNotify);
		}

		public bool PostEventNotify(string eventName, EventAction eventAction, object parameter, GameObject parentGameObject, OnEventNotify onEventNotify)
		{
			return PostEvent(eventName, eventAction, parameter, parentGameObject, null, false, onEventNotify);
		}

		public bool PostEvent(string eventName, EventAction eventAction, object parameter, GameObject parentGameObject, InitialiseParameters initialiseParameters, bool addToQueue, OnEventNotify onEventNotify)
		{
			if (parentGameObject != null)
			{
				newEvent.parentGameObject = parentGameObject;
			}
			else
			{
				newEvent.parentGameObject = null;
			}
			newEvent._eventName = eventName;
			newEvent.EventAction = eventAction;
			newEvent._parameter = parameter;
			newEvent._initialiseParameters = initialiseParameters;
			newEvent._onEventNotify = onEventNotify;
			return PostEvent(newEvent, addToQueue);
		}

		public bool PostEvent(Event postedEvent, bool addToQueue = false)
		{
			if (postedEvent._eventName == "" || postedEvent._eventName == "Event_Unset")
			{
				return false;
			}
			if (!addToQueue && !_forceQueueAllEvents && postedEvent._delay == 0f)
			{
				ProcessEvent(postedEvent);
			}
			else
			{
				_eventQueue.Add(postedEvent);
				postedEvent.eventStatus = EventStatus.InQueue;
				postedEvent._delayTimer = 0f;
			}
			return true;
		}

		public bool SetParameter(string eventName, string parameterName, float value, GameObject parentGameObject = null)
		{
			if (_forceQueueAllEvents)
			{
				parameter = new ParameterData();
			}
			parameter._parameter = parameterName.GetHashCode();
			parameter._index = -1;
			parameter._value = value;
			return PostEvent(eventName, EventAction.SetParameter, parameter, parentGameObject);
		}

		public bool SetParameter(string eventName, int parameterIndex, float value, GameObject parentGameObject = null)
		{
			if (_forceQueueAllEvents)
			{
				parameter = new ParameterData();
			}
			parameter._index = parameterIndex;
			parameter._value = value;
			return PostEvent(eventName, EventAction.SetParameter, parameter, parentGameObject);
		}

		public bool SetDSPParameter(string eventName, DSPType dspType, string parameterName, float value, float time = 0f, float curve = 0.5f, GameObject parentGameObject = null)
		{
			if (_forceQueueAllEvents)
			{
				dspParameter = new DSPParameterData();
			}
			dspParameter._dspType = dspType;
			dspParameter._parameter = parameterName;
			dspParameter._value = value;
			dspParameter._time = time;
			dspParameter._curve = curve;
			return PostEvent(eventName, EventAction.SetDSPParameter, dspParameter, parentGameObject);
		}

		public bool SetGlobalParameter(string parameterName, float value)
		{
			if (_forceQueueAllEvents)
			{
				globalParameter = new GlobalParameterData();
			}
			globalParameter._name = parameterName;
			globalParameter._value = value;
			return PostEvent("GlobalParameter", EventAction.SetGlobalParameter, globalParameter);
		}

		public float GetGlobalParameter(string parameterName)
		{
			return _globalParameterManager.GetGlobalParameter(parameterName);
		}

		public bool SetGlobalSwitch(string globalSwitchName, string switchName)
		{
			globalSwitch._name = globalSwitchName;
			globalSwitch._switch = switchName;
			return PostEvent("GlobalParameter", EventAction.SetGlobalSwitch, globalSwitch);
		}

		public bool SetTransition(TransitionData transition, GameObject gameObject)
		{
			bool flag = false;
			if (transition == null)
			{
				return flag;
			}
			flag |= PostEvent(transition._stopEvent, EventAction.StopScheduled, transition._scheduleStop, gameObject);
			return flag | PostEvent(transition._startEvent, EventAction.PlayScheduled, transition._scheduleStart, gameObject);
		}

		public bool ProcessEvent(Event postedEvent)
		{
			bool result = false;
			string eventName = postedEvent._eventName;
			if (eventName == null)
			{
				return false;
			}
			if (!_listeners.ContainsKey(eventName))
			{
				DebugLog.Print(DebugLevel.Warning, "Event \"", eventName, "\" triggered has no listeners!");
				postedEvent.eventStatus = EventStatus.Failed_No_Listeners;
			}
			else
			{
				if (_eventSequencer != null)
				{
					_eventSequencer.ProcessEvent(postedEvent);
				}
				List<IEventListener> list = _listeners[eventName];
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					IEventListener eventListener = list[i];
					if (IsNullOrDestroyed(eventListener))
					{
						continue;
					}
					if (eventListener.GetEventListeners(eventName, eventListeners))
					{
						Event @event = ApplyOverrideEventActions(eventListeners, postedEvent, overrideEvent);
						if (@event != null)
						{
							postedEvent.eventStatus = eventListener.Process(@event);
						}
					}
					else
					{
						postedEvent.eventStatus = eventListener.Process(postedEvent);
					}
					if ((Application.isEditor || _deployWithActiveEventList) && postedEvent.EventAction == EventAction.PlaySound)
					{
						AddActiveEvent(postedEvent, eventListener as Component);
					}
				}
				result = ((postedEvent.eventStatus == EventStatus.Handled || postedEvent.eventStatus == EventStatus.Handled_Virtualized) ? true : false);
			}
			LogEvent(postedEvent);
			return result;
		}

		public void AddActiveEvent(Event postedEvent, Component component)
		{
			if (component != null && (postedEvent.eventStatus == EventStatus.Handled || postedEvent.eventStatus == EventStatus.Handled_Virtualized))
			{
				ActiveEvent activeEvent = _activeEvents.Find((ActiveEvent x) => component == x._component);
				if (activeEvent == null)
				{
					activeEvent = new ActiveEvent();
					activeEvent._eventName = postedEvent._eventName;
					activeEvent._component = component;
					_activeEvents.Add(activeEvent);
				}
			}
		}

		public static int GetIDFromEventName(string eventName)
		{
			if (eventName != null)
			{
				return eventName.GetHashCode();
			}
			return 0;
		}

		public bool PostEvent(int eventID)
		{
			return PostEvent(eventID, null, null);
		}

		public bool PostEvent(int eventID, GameObject parentGameObject)
		{
			return PostEvent(eventID, parentGameObject, null);
		}

		public bool PostEvent(int eventID, GameObject parentGameObject, InitialiseParameters initialiseParameters)
		{
			return PostEvent(eventID, EventAction.PlaySound, null, parentGameObject, initialiseParameters, false, null);
		}

		public bool PostEvent(int eventID, EventAction eventAction)
		{
			return PostEvent(eventID, eventAction, null, null, null, false, null);
		}

		public bool PostEvent(int eventID, EventAction eventAction, object parameter)
		{
			return PostEvent(eventID, eventAction, parameter, null, null, false, null);
		}

		public bool PostEvent(int eventID, EventAction eventAction, object parameter, GameObject parentGameObject)
		{
			return PostEvent(eventID, eventAction, parameter, parentGameObject, null, false, null);
		}

		public bool PostEvent(int eventID, EventAction eventAction, GameObject parentGameObject)
		{
			return PostEvent(eventID, eventAction, null, parentGameObject, null, false, null);
		}

		public bool SetParameter(int eventID, string parameterName, float value, GameObject parentGameObject = null)
		{
			if (_forceQueueAllEvents)
			{
				parameter = new ParameterData();
			}
			parameter._parameter = parameterName.GetHashCode();
			parameter._index = -1;
			parameter._value = value;
			return PostEvent(eventID, EventAction.SetParameter, parameter, parentGameObject);
		}

		public bool SetParameter(int eventID, int parameterIndex, float value, GameObject parentGameObject = null)
		{
			if (_forceQueueAllEvents)
			{
				parameter = new ParameterData();
			}
			parameter._index = parameterIndex;
			parameter._value = value;
			return PostEvent(eventID, EventAction.SetParameter, parameter, parentGameObject);
		}

		public void SetDSPParameter(int eventID, DSPType dspType, string parameterName, float value, float time = 0f, float curve = 0.5f, GameObject parentGameObject = null)
		{
			if (_forceQueueAllEvents)
			{
				dspParameter = new DSPParameterData();
			}
			dspParameter._dspType = dspType;
			dspParameter._parameter = parameterName;
			dspParameter._value = value;
			dspParameter._time = time;
			dspParameter._curve = curve;
			PostEvent(eventID, EventAction.SetDSPParameter, dspParameter, parentGameObject);
		}

		public bool PostEvent(int eventID, EventAction eventAction, object parameter, GameObject parentGameObject, InitialiseParameters initialiseParameters, bool addToQueue, OnEventNotify onEventNotify)
		{
			if (parentGameObject != null)
			{
				newEvent.parentGameObject = parentGameObject;
			}
			else
			{
				newEvent.parentGameObject = null;
			}
			newEvent._eventID = eventID;
			newEvent.EventAction = eventAction;
			newEvent._parameter = parameter;
			newEvent._initialiseParameters = initialiseParameters;
			newEvent._onEventNotify = onEventNotify;
			return PostEventID(newEvent, addToQueue);
		}

		public bool PostEventID(Event postedEvent, bool addToQueue = false)
		{
			if (postedEvent._eventID == 0)
			{
				return false;
			}
			if (postedEvent.parentGameObject == null)
			{
				postedEvent.parentGameObject = base.gameObject;
			}
			if (!addToQueue && !_forceQueueAllEvents && postedEvent._delay == 0f)
			{
				ProcessEventID(postedEvent);
			}
			else
			{
				_eventQueue.Add(postedEvent);
				postedEvent.eventStatus = EventStatus.InQueue;
				postedEvent._delayTimer = 0f;
			}
			return true;
		}

		private bool ProcessEventID(Event postedEvent)
		{
			bool result = false;
			int eventID = postedEvent._eventID;
			if (!_listenersHashed.ContainsKey(eventID))
			{
				DebugLog.Print("Event \"" + eventID + "\" triggered has no listeners!", DebugLevel.Warning);
				postedEvent.eventStatus = EventStatus.Failed_No_Listeners;
			}
			else
			{
				if (_eventSequencer != null)
				{
					_eventSequencer.ProcessEvent(postedEvent);
				}
				List<IEventListener> list = _listenersHashed[eventID];
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					IEventListener eventListener = list[i];
					if (IsNullOrDestroyed(eventListener))
					{
						continue;
					}
					if (eventListener.GetEventListeners(eventID, eventListeners))
					{
						Event @event = ApplyOverrideEventActions(eventListeners, postedEvent, overrideEvent);
						if (@event != null)
						{
							postedEvent.eventStatus = eventListener.Process(@event);
						}
					}
					else
					{
						postedEvent.eventStatus = eventListener.Process(postedEvent);
					}
				}
				result = ((postedEvent.eventStatus == EventStatus.Handled) ? true : false);
			}
			LogEvent(postedEvent);
			return result;
		}

		public void UpdateInternal()
		{
			profiler.Begin();
			for (int i = 0; i < _eventQueue.Count; i++)
			{
				Event @event = _eventQueue[i];
				if (@event.parentGameObject == null)
				{
					_eventQueue.RemoveAt(i);
				}
				else if (@event._delayTimer < @event._delay)
				{
					@event._delayTimer += FabricTimer.GetRealtimeDelta();
				}
				else if (ProcessEvent(@event))
				{
					_eventQueue.RemoveAt(i);
				}
			}
			_eventEditor.Update();
			_globalParameterManager.Update();
			if (Application.isEditor || _deployWithActiveEventList)
			{
				for (int j = 0; j < _activeEvents.Count; j++)
				{
					ActiveEvent activeEvent = _activeEvents[j];
					if (activeEvent._component == null || !activeEvent._component.IsComponentActive())
					{
						if (activeEvent._time == 0f)
						{
							activeEvent._time = _activeEventPersistenceTime;
						}
						else
						{
							activeEvent._time -= FabricTimer.GetRealtimeDelta();
						}
						if (activeEvent._time <= 0f)
						{
							_activeEvents.Remove(activeEvent);
							activeEvent._time = 0f;
						}
					}
				}
			}
			profiler.End();
		}

		public bool IsEventActive(string eventName, GameObject parentGameObject)
		{
			if (_listeners.ContainsKey(eventName))
			{
				List<IEventListener> list = _listeners[eventName];
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					IEventListener eventListener = list[i];
					if (!IsNullOrDestroyed(eventListener) && eventListener.IsActive(parentGameObject))
					{
						return true;
					}
				}
			}
			return false;
		}

		public EventInfo GetEventInfo(string eventName, GameObject parentGameObject = null, EventInfo eventInfo = null)
		{
			if (eventInfo == null)
			{
				eventInfo = new EventInfo();
			}
			if (_listeners.ContainsKey(eventName))
			{
				List<IEventListener> list = _listeners[eventName];
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					IEventListener eventListener = list[i];
					if (!IsNullOrDestroyed(eventListener) && eventListener.IsActive(parentGameObject))
					{
						eventListener.GetEventInfo(parentGameObject, ref eventInfo);
					}
				}
			}
			return eventInfo;
		}

		public int FindEventNameIndexByName(string eventName)
		{
			for (int i = 0; i < _eventList.Count; i++)
			{
				string b = _eventList[i];
				if (eventName == b)
				{
					return i;
				}
			}
			return 0;
		}

		private Event ApplyOverrideEventActions(List<EventListener> eventListeners, Event originalEvent, Event overrideEvent)
		{
			bool flag = true;
			for (int i = 0; i < eventListeners.Count; i++)
			{
				EventListener eventListener = eventListeners[i];
				if (eventListener._overrideParameters == null || !eventListener._overrideEventTriggerAction)
				{
					continue;
				}
				flag = true;
				overrideEvent.Copy(originalEvent);
				if (eventListener._overrideParameters._overrideIncomingEventAction)
				{
					if (eventListener._overrideParameters._incomingEventAction != originalEvent.EventAction)
					{
						flag = false;
						continue;
					}
					overrideEvent.EventAction = eventListener._overrideParameters._overrideEventAction;
				}
				else
				{
					overrideEvent.EventAction = eventListener._overrideParameters._overrideEventAction;
				}
				if (eventListener._overrideParameters._type == OverrideParameterType.Float)
				{
					overrideEvent._parameter = eventListener._overrideParameters.FloatParameter;
				}
				else if (eventListener._overrideParameters._type == OverrideParameterType.String)
				{
					overrideEvent._parameter = eventListener._overrideParameters.StringParameter;
				}
				else if (eventListener._overrideParameters._type == OverrideParameterType.SwitchPresetData)
				{
					overrideEvent._parameter = eventListener._overrideParameters.SwitchPresetData;
				}
				else if (eventListener._overrideParameters._type == OverrideParameterType.DSPParameterData)
				{
					overrideEvent._parameter = eventListener._overrideParameters.DSPParameterData;
				}
				else if (eventListener._overrideParameters._type == OverrideParameterType.TransitionToSnapshotData)
				{
					overrideEvent._parameter = eventListener._overrideParameters.TransitionToSnapshotData;
				}
				if (flag)
				{
					return overrideEvent;
				}
			}
			if (!flag)
			{
				return null;
			}
			return originalEvent;
		}

		public bool IsNullOrDestroyed(object obj)
		{
			if (obj is IEventListener)
			{
				return (obj as IEventListener).IsDestroyed;
			}
			if (obj is Component)
			{
				return obj as Component == null;
			}
			return obj == null;
		}
	}
}
