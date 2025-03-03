using Disney.LaunchPadFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public class StateMachine : MonoBehaviour
	{
		public StateMachineDefinition Definition;

		public Queue<string> pendingStateChanges;

		private bool stateChanging;

		private State currentState;

		private StateMachineContext context;

		private bool isStarted;

		public State CurrentState
		{
			get
			{
				return currentState;
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("CurrentState", "Attempted to set a null state. Current state: " + currentState.Name);
				}
				leaveState(currentState);
				currentState = value;
				if (Definition != null && Definition.PersistState)
				{
					context.SetPersistentState(base.name, currentState);
				}
				enterState(currentState);
			}
		}

		public string CurrentStateName
		{
			get
			{
				return CurrentState.Name;
			}
		}

		internal event Action<StateMachine> StateChanged;

		internal event Action<StateMachine> InitialStateSet;

		public static StateMachine Create(GameObject go, StateMachineDefinition definition)
		{
			StateMachine stateMachine = go.AddComponent<StateMachine>();
			stateMachine.Definition = definition;
			stateMachine.init();
			return stateMachine;
		}

		public void Awake()
		{
			init();
		}

		public void Start()
		{
			isStarted = true;
			OnEnable();
			enterState(currentState);
		}

		private void init()
		{
			pendingStateChanges = new Queue<string>();
			if (context == null)
			{
				context = GetComponentInParent<StateMachineContext>();
				context.AddStateMachine(this);
			}
			if (Definition != null)
			{
				if (Definition.PersistState && context.GetPersistentState(base.name) != null)
				{
					currentState = context.GetPersistentState(base.name);
				}
				else
				{
					currentState = Definition.States[0];
				}
				if (this.InitialStateSet != null)
				{
					this.InitialStateSet(this);
				}
			}
		}

		public void OnDestroy()
		{
			context.RemoveStateMachine(base.name);
		}

		public void OnEnable()
		{
			if (isStarted)
			{
				sendExternalEvents(Definition.OnEnableEvents);
			}
		}

		public void OnDisable()
		{
			sendExternalEvents(Definition.OnDisableEvents);
		}

		public void SendEvent(string evt)
		{
			if (stateChanging)
			{
				pendingStateChanges.Enqueue(evt);
			}
			switch (evt)
			{
			case "enable":
				base.gameObject.SetActive(true);
				return;
			case "disable":
				base.gameObject.SetActive(false);
				return;
			case "suspend":
				base.enabled = false;
				return;
			case "resume":
				base.enabled = true;
				return;
			case "reset":
				context.ResetState(base.name);
				return;
			}
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			string targetStateName = currentState.GetTargetStateName(evt);
			if (targetStateName != null)
			{
				State state = Definition.GetState(targetStateName);
				if (state == null)
				{
					throw new Exception(string.Format("Could not get a state by the name {0} in state machine {1}", targetStateName, base.name));
				}
				CurrentState = state;
			}
		}

		public void SendEvent(ExternalEvent evt)
		{
			if (stateChanging)
			{
				throw new Exception(string.Format("FSM '{0}' is trying to send external event {1}.{2} during a state change to {3}", base.name, evt.Target, evt.Event, currentState.Name));
			}
			switch (evt.Target)
			{
			case "parent":
				sendEventToParent(evt.Event);
				break;
			case "ancestors":
				sendEventToAncestors(evt.Event);
				break;
			case "siblings":
				sendEventToSiblings(evt.Event);
				break;
			case "children":
				sendEventToChildren(evt.Event);
				break;
			case "descendants":
				sendEventToDescendants(evt.Event);
				break;
			default:
				context.SendEvent(evt);
				break;
			}
		}

		private void sendEventToParent(string evt)
		{
			StateMachine component = base.transform.parent.GetComponent<StateMachine>();
			component.SendEvent(evt);
		}

		private void sendEventToAncestors(string evt)
		{
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				StateMachine component = parent.GetComponent<StateMachine>();
				if (component != null)
				{
					component.SendEvent(evt);
				}
				parent = parent.parent;
			}
		}

		private void sendEventToSiblings(string evt)
		{
			Transform parent = base.transform.parent;
			for (int i = 0; i < parent.childCount; i++)
			{
				StateMachine component = parent.GetChild(i).GetComponent<StateMachine>();
				if (component != null && component != this)
				{
					component.SendEvent(evt);
				}
			}
		}

		private void sendEventToChildren(string evt)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				StateMachine component = base.transform.GetChild(i).GetComponent<StateMachine>();
				if (component != null)
				{
					component.SendEvent(evt);
				}
			}
		}

		private void sendEventToDescendants(string evt)
		{
			StateMachine[] componentsInChildren = GetComponentsInChildren<StateMachine>(true);
			foreach (StateMachine stateMachine in componentsInChildren)
			{
				if (stateMachine != this)
				{
					stateMachine.SendEvent(evt);
				}
			}
		}

		private void sendExternalEvents(ExternalEvent[] events)
		{
			for (int i = 0; i < events.Length; i++)
			{
				SendEvent(events[i]);
			}
		}

		private void leaveState(State state)
		{
			sendExternalEvents(state.OnExitEvents);
		}

		private void enterState(State state)
		{
			sendExternalEvents(state.OnEntryEvents);
			try
			{
				base.gameObject.SendMessage("OnStateChanged", state.Name, SendMessageOptions.DontRequireReceiver);
			}
			catch (Exception ex)
			{
				Log.LogErrorFormatted(this, "Error in OnStateChange entering state: {0}", state.Name);
				Log.LogException(this, ex);
			}
			stateChanging = true;
			PassiveStateHandler[] components = GetComponents<PassiveStateHandler>();
			for (int i = 0; i < components.Length; i++)
			{
				try
				{
					components[i].HandleStateChange(state.Name);
				}
				catch (Exception ex)
				{
					Log.LogErrorFormatted(components[i], "Error in HandleStateChange entering state: {0} on handler of type {1}", state.Name, components[i].GetType());
					Log.LogException(components[i], ex);
				}
			}
			string text = null;
			ActiveStateHandler[] components2 = GetComponents<ActiveStateHandler>();
			for (int i = 0; i < components2.Length; i++)
			{
				if (components2[i].HandledState == state.Name)
				{
					text = components2[i].HandleStateChange();
					break;
				}
			}
			if (this.StateChanged != null)
			{
				this.StateChanged(this);
			}
			stateChanging = false;
			if (text != null)
			{
				SendEvent(text);
			}
			if (pendingStateChanges.Count > 0)
			{
				SendEvent(pendingStateChanges.Dequeue());
			}
		}

		[Conditional("DO_LOGGING")]
		private void trace(string format, params object[] args)
		{
		}

		public IEnumerator CheckEventDependencies(ExternalEvent evt)
		{
			if (!base.isActiveAndEnabled)
			{
				yield break;
			}
			string stateName = currentState.GetTargetStateName(evt.Event);
			if (stateName != null)
			{
				State state = Definition.GetState(stateName);
				if (state == null)
				{
					throw new Exception(string.Format("Could not get a state by the name {0} in state machine {1}", stateName, base.name));
				}
				IList<string> eventTargets = getOnEntryTargets(state.OnEntryEvents);
				int count = dependentStateMachineCount(eventTargets);
				while (eventTargets.Count > count)
				{
					yield return null;
					count = dependentStateMachineCount(eventTargets);
				}
				context.SendEvent(evt);
			}
		}

		private IList<string> getOnEntryTargets(ExternalEvent[] events)
		{
			IList<string> list = new List<string>();
			for (int i = 0; i < events.Length; i++)
			{
				list.Add(events[i].Target);
			}
			return list;
		}

		private int dependentStateMachineCount(IList<string> targetFilters)
		{
			int num = 0;
			for (int i = 0; i < targetFilters.Count; i++)
			{
				if (context.ContainsStateMachine(targetFilters[i]))
				{
					num++;
				}
			}
			return num;
		}
	}
}
