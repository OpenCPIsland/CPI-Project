using Disney.LaunchPadFramework.Utility;
using Disney.LaunchPadFramework.Utility.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public class InitManager
	{
		private EventDispatcher m_eventDispatcher = null;

		private List<InitAction> m_initActions = null;

		private List<CompletedInitAction> m_completedActions = null;

		private bool m_runAsCoroutine = true;

		private MonoBehaviour m_coroutineOwner = null;

		private List<InitAction> InitActions
		{
			get
			{
				if (m_initActions == null)
				{
					m_initActions = new List<InitAction>();
				}
				return m_initActions;
			}
		}

		public EventDispatcher EventDispatcher
		{
			get
			{
				return m_eventDispatcher;
			}
			set
			{
				m_eventDispatcher = value;
			}
		}

		public InitManager(EventDispatcher eventDispatcher)
		{
			EventDispatcher = eventDispatcher;
			m_completedActions = new List<CompletedInitAction>();
		}

		public void AddInitAction(InitAction action)
		{
			InitActions.Add(action);
		}

		public void Process(bool runAsCoroutine = true, MonoBehaviour coroutineOwner = null)
		{
			m_coroutineOwner = coroutineOwner;
			ValidateDependencies();
			List<ITopologicalNode> list = new List<ITopologicalNode>();
			list = m_initActions.ConvertAll((Converter<InitAction, ITopologicalNode>)((InitAction obj) => obj));
			list.AddRange(m_completedActions.ConvertAll((Converter<CompletedInitAction, ITopologicalNode>)((CompletedInitAction obj) => obj)));
			list = TopologicalSort.Sort(list);
			list.RemoveAll((ITopologicalNode obj) => obj is CompletedInitAction);
			m_initActions = list.ConvertAll((ITopologicalNode obj) => obj as InitAction);
			EventDispatcher.AddListener<ActionCompleteEvent>(OnInitActionComplete);
			EventDispatcher.DispatchEvent(new InitStartEvent(InitActions.Count));
			if (CheckAllComplete())
			{
				OnAllInitComplete();
				return;
			}
			m_runAsCoroutine = runAsCoroutine;
			if (m_runAsCoroutine)
			{
				m_coroutineOwner.StartCoroutine(ExecuteReadyActionsCoroutine());
			}
			else
			{
				CoroutineHelper.RunCoroutineToCompletion(ExecuteReadyActionsCoroutine());
			}
		}

		public void Clear()
		{
			InitActions.Clear();
		}

		private bool CheckAllComplete()
		{
			return InitActions.Count == 0;
		}

		private IEnumerator ExecuteReadyActionsCoroutine()
		{
			while (InitActions.Count > 0)
			{
				InitAction action = InitActions[0];
				if (action.CanBegin())
				{
					action.Initialize(this, m_eventDispatcher);
					InitActions.RemoveAt(0);
					action.Begin();
					if (m_runAsCoroutine)
					{
						yield return m_coroutineOwner.StartCoroutine(action.Perform());
					}
					else
					{
						CoroutineHelper.RunCoroutineToCompletion(action.Perform());
					}
					yield return null;
				}
				else if (action.completed)
				{
					InitActions.RemoveAt(0);
				}
			}
		}

		private bool OnInitActionComplete(ActionCompleteEvent evt)
		{
			InitAction initAction = evt.Action as InitAction;
			if (initAction != null)
			{
				m_completedActions.AddIfUnique(new CompletedInitAction(initAction.TopologicalIdentifier, initAction.TopologicalDependencies));
			}
			if (CheckAllComplete())
			{
				OnAllInitComplete();
			}
			return false;
		}

		private void OnAllInitComplete()
		{
			EventDispatcher.RemoveListener<ActionCompleteEvent>(OnInitActionComplete);
			EventDispatcher.DispatchEvent(default(InitCompleteEvent));
		}

		private void ValidateDependencies()
		{
			for (int i = 0; i < InitActions.Count; i++)
			{
				InitAction initAction = InitActions[i];
				RecursiveValidateAction(initAction, initAction.GetType(), initAction.ActionName);
			}
		}

		private void RecursiveValidateAction(InitAction action, Type dependencyType, string breadcrumbs)
		{
			if (action.TopologicalDependencies.Contains(dependencyType.ToString()))
			{
				string message = "[ValidateDependencies] Found Circular Dependency: " + breadcrumbs;
				Log.LogFatal(this, message);
				throw new ApplicationException(message);
			}
			foreach (string antecedentName in action.TopologicalDependencies)
			{
				List<InitAction> initActions = InitActions;
				Predicate<InitAction> match = (InitAction a) => a.GetType().ToString() == antecedentName;
				InitAction initAction = initActions.Find(match);
				if (initAction == null)
				{
					CompletedInitAction completedInitAction = m_completedActions.Find((CompletedInitAction a) => a.TopologicalIdentifier.Equals(antecedentName));
					if (completedInitAction != null)
					{
						break;
					}
					string message = "[ValidateDependencies] Missing Dependency: " + breadcrumbs + " -> " + antecedentName;
					Log.LogFatal(this, message);
					throw new ApplicationException(message);
				}
				RecursiveValidateAction(initAction, dependencyType, breadcrumbs + " -> " + initAction.ActionName);
			}
		}
	}
}
