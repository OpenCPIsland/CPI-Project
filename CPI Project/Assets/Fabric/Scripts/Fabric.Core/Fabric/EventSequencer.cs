using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class EventSequencer
	{
		[Serializable]
		public class EventSequencerEntry
		{
			[Serializable]
			public class EventEntry
			{
				[SerializeField]
				public EventNotificationType notificationType;

				[SerializeField]
				public string eventName;
			}

			[SerializeField]
			public EventNotificationType notificationType;

			[SerializeField]
			public List<EventEntry> events = new List<EventEntry>();

			[SerializeField]
			public bool _restartOnEvent;

			[SerializeField]
			public bool _loop;

			private bool _stopping;

			[NonSerialized]
			private int currentIndex;

			[NonSerialized]
			private GameObject _gameObject;

			public void ProcessEvent(Event postedEvent)
			{
				if (postedEvent.EventAction == EventAction.PlaySound || postedEvent.EventAction == EventAction.PlayScheduled)
				{
					postedEvent._onEventNotify = OnEventNotify;
					_stopping = false;
				}
				else if (postedEvent.EventAction == EventAction.StopSound)
				{
					EventManager.Instance.PostEvent(events[CurrentIndex()].eventName, EventAction.StopSound, _gameObject);
					_stopping = true;
				}
				_gameObject = postedEvent.parentGameObject;
			}

			public void OnEventNotify(EventNotificationType type, string eventName, object info, GameObject gameObject)
			{
				if (_stopping)
				{
					currentIndex = 0;
					return;
				}
				EventNotificationType eventNotificationType = (currentIndex != 0) ? events[currentIndex - 1].notificationType : notificationType;
				if (type == eventNotificationType)
				{
					PostEvent(GetNextEventName());
				}
			}

			public int CurrentIndex()
			{
				return currentIndex - 1;
			}

			private string GetNextEventName()
			{
				if (currentIndex >= events.Count)
				{
					if (!_loop)
					{
						currentIndex = 0;
						return null;
					}
					currentIndex = 0;
				}
				return events[currentIndex++].eventName;
			}

			private void PostEvent(string eventName)
			{
				if (eventName != null)
				{
					EventManager.Instance.PostEventNotify(eventName, _gameObject, OnEventNotify);
				}
			}
		}

		[Serializable]
		public class SequenceEntries : FastList<string, EventSequencerEntry>
		{
		}

		[SerializeField]
		public SequenceEntries _sequenceEntries = new SequenceEntries();

		public bool ProcessEvent(Event postedEvent)
		{
			EventSequencerEntry eventSequencerEntry = _sequenceEntries.FindItem(postedEvent._eventName);
			if (eventSequencerEntry != null)
			{
				eventSequencerEntry.ProcessEvent(postedEvent);
				return true;
			}
			return false;
		}

		public void Update()
		{
		}
	}
}
