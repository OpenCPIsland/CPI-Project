using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hg.ApiWebKit.tinyfsm
{
	[Serializable]
	public class TinyStateMachine
	{
		private MonoBehaviour _behaviorHost = null;

		private IEnumerator _previousState = null;

		private IEnumerator _currentState = null;

		private Queue<IEnumerator> _nextStatesQueue = null;

		private bool _nextStatesQueueBlocked = false;

		private bool _nextStateAllowed = false;

		private Action<TinyStateMachine, string, string> _onStateChangeNotification;

		private Action _currentGuiRoutine = delegate
		{
		};

		private bool _canUpdate = false;

		public bool NextStateRequested
		{
			get;
			private set;
		}

		public bool CanTransition
		{
			get
			{
				return !_nextStatesQueueBlocked;
			}
		}

		public string CurrentStateName
		{
			get
			{
				return stateName(_currentState);
			}
		}

		public TinyStateMachine(Action<TinyStateMachine, string, string> onStateChange)
		{
			Configuration.Log("[TinyFsm] Initializing", LogSeverity.VERBOSE);
			_nextStatesQueue = new Queue<IEnumerator>();
			_onStateChangeNotification = onStateChange;
			generateBehaviourHost();
		}

		private void generateBehaviourHost()
		{
			if (!(_behaviorHost != null))
			{
				GameObject gameObject = new GameObject();
				gameObject.name = Configuration.GetSetting<string>("persistent-game-object-name") + ":tiny-fsm:" + gameObject.GetInstanceID();
				gameObject.hideFlags = Configuration.GetSetting<HideFlags>("tiny-fsm-game-object-flags");
				_behaviorHost = gameObject.AddComponent<MonoBehaviour>();
			}
		}

		public void Update()
		{
			generateBehaviourHost();
			if (!_canUpdate)
			{
				Configuration.Log("[TinyFsm] Not yet updateable.", LogSeverity.VERBOSE);
			}
			else if (_nextStatesQueue.Count != 0 && !_nextStatesQueueBlocked)
			{
				_nextStatesQueueBlocked = true;
				IEnumerator state = _nextStatesQueue.Dequeue();
				_behaviorHost.StartCoroutine(spinUpNextState(state));
			}
		}

		public void Stop(bool destroyFsm = false)
		{
			_behaviorHost.StartCoroutine(spinUpNextState(null));
		}

		public bool TryGoto(IEnumerator nextState)
		{
			_canUpdate = true;
			if (_nextStatesQueueBlocked)
			{
				Configuration.Log("[TinyFsm] Denied Requested State Change  => " + stateName(nextState), LogSeverity.VERBOSE);
				return false;
			}
			Configuration.Log("[TinyFsm] Accepted Requested State Change  => " + stateName(nextState), LogSeverity.VERBOSE);
			_nextStatesQueue.Enqueue(nextState);
			return true;
		}

		public bool Goto(IEnumerator nextState)
		{
			_canUpdate = true;
			Configuration.Log("[TinyFsm] Accepted Requested State Change  => " + stateName(nextState), LogSeverity.VERBOSE);
			_nextStatesQueue.Enqueue(nextState);
			return true;
		}

		public void CurrentStateCompleted()
		{
			_nextStateAllowed = true;
		}

		private IEnumerator spinUpNextState(IEnumerator state)
		{
			Configuration.Log("[TinyFsm] Spinning Up State : " + stateName(state), LogSeverity.VERBOSE);
			NextStateRequested = true;
			if (_currentState != null)
			{
				Configuration.Log("[TinyFsm] " + stateName(state) + " is waiting for " + stateName(_currentState) + " to finish.", LogSeverity.VERBOSE);
				while (!_nextStateAllowed)
				{
					yield return null;
				}
				Configuration.Log("[TinyFsm] " + stateName(_currentState) + " has finished.", LogSeverity.VERBOSE);
			}
			_nextStateAllowed = false;
			NextStateRequested = false;
			if (state == null)
			{
				Configuration.Log("[TinyFsm] Stopping...", LogSeverity.VERBOSE);
				UnityEngine.Object.Destroy(_behaviorHost.gameObject);
				Configuration.Log("[TinyFsm] Stopped!", LogSeverity.VERBOSE);
			}
			else
			{
				_behaviorHost.StartCoroutine(state);
				_previousState = _currentState;
				_currentState = state;
				if (_onStateChangeNotification != null)
				{
					_onStateChangeNotification(this, stateName(_previousState), stateName(_currentState));
				}
				Configuration.Log("[TinyFsm] " + stateName(_currentState) + " is now the current state.", LogSeverity.VERBOSE);
			}
			_nextStatesQueueBlocked = false;
			yield return null;
		}

		public void OnGUI()
		{
			_currentGuiRoutine();
		}

		public void SetGui(Action guiRoutine)
		{
			_currentGuiRoutine = guiRoutine;
		}

		public bool IsNextState(IEnumerator state)
		{
			try
			{
				return stateName(_nextStatesQueue.Peek()) == stateName(state);
			}
			catch
			{
				return false;
			}
		}

		public bool WasPreviousState(IEnumerator state)
		{
			Configuration.Log("[TinyFsm] Previous State Check", LogSeverity.VERBOSE);
			return stateName(_previousState) == stateName(state);
		}

		public bool IsInState(IEnumerator state)
		{
			Configuration.Log("[TinyFsm] Current State Check", LogSeverity.VERBOSE);
			return CurrentStateName == stateName(state);
		}

		private string stateName(IEnumerator state)
		{
			if (state == null)
			{
				return "(null-state)";
			}
			string name = state.GetType().Name;
			return name.Remove(name.IndexOf(">") + 1, name.Length - name.IndexOf(">") - 1);
		}
	}
}
