using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hg.ApiWebKit.tinyworkflow
{
	public class Workflow<T> : IWorkflow where T : WorkflowStateObject
	{
		public Dictionary<string, List<Action<T>>> Workflows;

		private Action<T> _currentStep = null;

		private int _currentStepNumber = -1;

		private Queue<Action<T>> _steps;

		private MonoBehaviour _behaviorHost;

		public T StateObject
		{
			get;
			private set;
		}

		public string CurrentWorkflowName
		{
			get;
			private set;
		}

		public bool IsRunning
		{
			get;
			private set;
		}

		public Workflow()
		{
			Configuration.Log("[TinyWorkflow] Initializing", LogSeverity.VERBOSE);
		}

		public void StartWorkflow(string name, object stateObject)
		{
			Configuration.Log("[TinyWorkflow] Starting '" + name + "' workflow.", LogSeverity.VERBOSE);
			if (IsRunning)
			{
				Configuration.Log("[TinyWorkflow] This workflow instance is currently executing the '" + CurrentWorkflowName + "' workflow.", LogSeverity.ERROR);
				return;
			}
			if (!Workflows.ContainsKey(name))
			{
				Configuration.Log("[TinyWorkflow] This workflow '" + name + "' could not be located.", LogSeverity.ERROR);
				return;
			}
			if (stateObject == null)
			{
				Configuration.Log("[TinyWorkflow] Cannot start workflow with a null state object.", LogSeverity.ERROR);
				return;
			}
			IsRunning = true;
			if (_behaviorHost == null)
			{
				_behaviorHost = (MonoBehaviour)Configuration.Bootstrap().AddComponent(typeof(MonoBehaviour));
			}
			CurrentWorkflowName = name;
			StateObject = (T)stateObject;
			StateObject.Workflow = this;
			_steps = new Queue<Action<T>>();
			_currentStepNumber = -1;
			_currentStep = null;
			foreach (Action<T> item in Workflows[name])
			{
				_steps.Enqueue(item);
			}
			Configuration.Log("[TinyWorkflow] Queued " + _steps.Count + " steps.", LogSeverity.VERBOSE);
			T stateObject2 = StateObject;
			stateObject2.OnWorkflowStart();
			NextStep(false, false);
		}

		public void StepComplete(bool success = true)
		{
			Configuration.Log("[TinyWorkflow] Step Completed", LogSeverity.VERBOSE);
			try
			{
				T stateObject = StateObject;
				stateObject.StepResultCallbacks[_currentStepNumber](success);
			}
			catch
			{
			}
			if (_currentStep != null)
			{
				T stateObject2 = StateObject;
				stateObject2.OnWorkflowStepCompletion(_currentStepNumber, success, StateObject);
			}
		}

		public void RepeatStep()
		{
			Configuration.Log("[TinyWorkflow] Repeating Step", LogSeverity.VERBOSE);
			if (_currentStep != null)
			{
				_currentStep(StateObject);
			}
		}

		public void NextStep(bool success = true, bool notifyCompletion = true)
		{
			if (notifyCompletion)
			{
				StepComplete(success);
			}
			Configuration.Log("[TinyWorkflow] Next Step", LogSeverity.VERBOSE);
			if (_steps.Count > 0)
			{
				_currentStep = _steps.Dequeue();
				_currentStepNumber++;
				_currentStep(StateObject);
			}
		}

		public void Stop()
		{
			Configuration.Log("[TinyWorkflow] Stopping", LogSeverity.VERBOSE);
			if (!IsRunning)
			{
				Configuration.Log("[TinyWorkflow] Workflow is not running.", LogSeverity.WARNING);
				return;
			}
			if (StateObject != null)
			{
				T stateObject = StateObject;
				stateObject.OnWorkflowStop();
			}
			killBehaviorHost();
			IsRunning = false;
			Configuration.Log("[TinyWorkflow] Stopped", LogSeverity.VERBOSE);
		}

		public Coroutine StartCoroutine(IEnumerator routine)
		{
			Configuration.Log("[TinyWorkflow] Starting Coroutine", LogSeverity.VERBOSE);
			return _behaviorHost.StartCoroutine(routine);
		}

		private void killBehaviorHost()
		{
			if (_behaviorHost != null)
			{
				_behaviorHost.StopAllCoroutines();
				UnityEngine.Object.Destroy(_behaviorHost);
			}
		}
	}
}
