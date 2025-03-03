using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tweaker.Core;
using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public class StateMachineContext : MonoBehaviour
	{
		public class StateMachineContextNameGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				return getNameToggleValues<StateMachineContext>();
			}
		}

		public class StateMachineNameGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				return getNameToggleValues<StateMachine>();
			}
		}

		private readonly Dictionary<string, StateMachine> stateMachines = new Dictionary<string, StateMachine>();

		private readonly Dictionary<string, State> persistentStates = new Dictionary<string, State>();

		private readonly Dictionary<string, StateMachineProxy> stateMachineProxies = new Dictionary<string, StateMachineProxy>();

		public event Action<string> StateMachineLoaded;

		public event Action<string, string> StateChanged;

		public void SendEvent(ExternalEvent evt)
		{
			SendEvent(evt, false);
		}

		public void SendEvent(ExternalEvent evt, bool checkDependencies)
		{
			if (!stateMachines.ContainsKey(evt.Target))
			{
				if (!stateMachineProxies.ContainsKey(evt.Target))
				{
					throw new Exception("State machine of name, " + evt.Target + ", was not found in StateMachineContext: " + base.gameObject.name);
				}
				if (stateMachineProxies[evt.Target] != null)
				{
					stateMachineProxies[evt.Target].AddEvent(evt.Event);
					return;
				}
				stateMachineProxies.Remove(evt.Target);
			}
			StateMachine stateMachine = stateMachines[evt.Target];
			if (checkDependencies)
			{
				CoroutineRunner.StopAllForOwner(stateMachine);
				CoroutineRunner.Start(stateMachine.CheckEventDependencies(evt), stateMachine, "Check event dependencies");
			}
			else
			{
				stateMachine.SendEvent(evt.Event);
			}
		}

		public bool ContainsStateMachine(string stateMachineName)
		{
			return stateMachines.ContainsKey(stateMachineName);
		}

		public string GetStateMachineState(string stateMachineName)
		{
			if (ContainsStateMachine(stateMachineName))
			{
				return stateMachines[stateMachineName].CurrentState.Name;
			}
			return null;
		}

		internal void AddStateMachine(StateMachine sm)
		{
			if (stateMachines.ContainsKey(sm.name))
			{
				throw new Exception("State machine of name, " + sm.name + ", already exists in the context.");
			}
			stateMachines.Add(sm.name, sm);
			sm.StateChanged += onStateChanged;
			if (stateMachineProxies.ContainsKey(sm.name))
			{
				sm.InitialStateSet += onInitialStateSet;
			}
			if (this.StateMachineLoaded != null)
			{
				this.StateMachineLoaded(sm.name);
			}
		}

		private void onInitialStateSet(StateMachine stateMachine)
		{
			stateMachine.InitialStateSet -= onInitialStateSet;
			foreach (string item in stateMachineProxies[stateMachine.name].DequeueEvents())
			{
				stateMachine.SendEvent(item);
			}
		}

		private void onStateChanged(StateMachine stateMachine)
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(stateMachine.name, stateMachine.CurrentStateName);
			}
		}

		internal void RemoveStateMachine(string name)
		{
			if (stateMachines.ContainsKey(name))
			{
				stateMachines[name].StateChanged -= onStateChanged;
			}
			stateMachines.Remove(name);
		}

		internal void AddStateMachineProxy(string target, StateMachineProxy proxy)
		{
			stateMachineProxies.Add(target, proxy);
		}

		internal void SetPersistentState(string stateMachineName, State state)
		{
			persistentStates[stateMachineName] = state;
		}

		internal State GetPersistentState(string stateMachineName)
		{
			State value;
			if (persistentStates.TryGetValue(stateMachineName, out value))
			{
			}
			return value;
		}

		internal bool ResetState(string name)
		{
			return persistentStates.Remove(name);
		}

		[Conditional("DO_LOGGING")]
		private void trace(string format, params object[] args)
		{
			Console.WriteLine("Ctx '{0}': {1}", base.name, string.Format(format, args));
		}

		private void OnDestroy()
		{
			foreach (KeyValuePair<string, StateMachine> stateMachine in stateMachines)
			{
				stateMachine.Value.StateChanged -= onStateChanged;
			}
			this.StateMachineLoaded = null;
		}

		[Invokable("UI.StateMachine.SendEvent")]
		private static void tweakerSendEvent([NamedToggleValue(typeof(StateMachineContextNameGenerator), 0u)] string contextName, [NamedToggleValue(typeof(StateMachineNameGenerator), 0u)] string targetString, string eventString)
		{
			StateMachineContext stateMachineContext = null;
			StateMachineContext[] array = UnityEngine.Object.FindObjectsOfType<StateMachineContext>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name == contextName)
				{
					stateMachineContext = array[i];
					break;
				}
			}
			if (stateMachineContext != null)
			{
				if (stateMachineContext.ContainsStateMachine(targetString))
				{
					stateMachineContext.SendEvent(new ExternalEvent(targetString, eventString));
				}
				else
				{
					Log.LogErrorFormatted(typeof(StateMachineContext), "StateMachineContext {0} does not contain StateMachine {1}", contextName, targetString);
				}
			}
			else
			{
				Log.LogErrorFormatted(typeof(StateMachineContext), "Could not find a StateMachineContext with name {0}", contextName);
			}
		}

		private static List<NamedToggleValueAttribute.NamedToggleValue> getNameToggleValues<T>() where T : Component
		{
			List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
			T[] array = UnityEngine.Object.FindObjectsOfType<T>();
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(new NamedToggleValueAttribute.NamedToggleValue(array[i].name, array[i].name));
			}
			return list;
		}
	}
}
