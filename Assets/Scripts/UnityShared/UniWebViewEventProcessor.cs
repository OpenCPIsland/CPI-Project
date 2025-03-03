using System;
using System.Collections.Generic;
using UnityEngine;

public class UniWebViewEventProcessor : MonoBehaviour
{
	private object _queueLock = new object();

	private List<Action> _queuedEvents = new List<Action>();

	private List<Action> _executingEvents = new List<Action>();

	private static UniWebViewEventProcessor _instance = null;

	public static UniWebViewEventProcessor instance
	{
		get
		{
			if (!_instance)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(UniWebViewEventProcessor)) as UniWebViewEventProcessor);
				if (!_instance)
				{
					GameObject gameObject = new GameObject("UniWebViewEventProcessor");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					_instance = gameObject.AddComponent<UniWebViewEventProcessor>();
				}
			}
			return _instance;
		}
	}

	public void QueueEvent(Action action)
	{
		lock (_queueLock)
		{
			_queuedEvents.Add(action);
		}
	}

	private void Update()
	{
		MoveQueuedEventsToExecuting();
		while (_executingEvents.Count > 0)
		{
			Action action = _executingEvents[0];
			_executingEvents.RemoveAt(0);
			action();
		}
	}

	private void MoveQueuedEventsToExecuting()
	{
		lock (_queueLock)
		{
			while (_queuedEvents.Count > 0)
			{
				Action item = _queuedEvents[0];
				_executingEvents.Add(item);
				_queuedEvents.RemoveAt(0);
			}
		}
	}
}
