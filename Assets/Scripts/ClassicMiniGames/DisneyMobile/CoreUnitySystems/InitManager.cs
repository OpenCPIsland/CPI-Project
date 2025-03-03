using DisneyMobile.CoreUnitySystems.Utility;
using DisneyMobile.CoreUnitySystems.Utility.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems
{
	public class InitManager
	{
		private Configurator m_configurator = null;

		private EventDispatcher m_eventDispatcher = null;

		private List<InitAction> m_initActions = null;

		private bool m_runAsCoroutine = true;

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
		}

		public void AddInitAction(InitAction action)
		{
			InitActions.Add(action);
		}

		public void Process(Configurator configurator, bool runAsCoroutine = true)
		{
			if (configurator != null)
			{
				m_configurator = configurator;
			}
			Logger.LogDebug(this, "Starting to run init actions", Logger.TagFlags.INIT);
			ValidateDependencies();
			List<ITopologicalNode> list = new List<ITopologicalNode>();
			list = m_initActions.ConvertAll((Converter<InitAction, ITopologicalNode>)((InitAction obj) => obj));
			list = TopologicalSort.Sort(list);
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
				BaseGameController.Instance.StartCoroutine(ExecuteReadyActionsCoroutine());
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
				if (action.CanStart())
				{
					action.Initialize(this, m_eventDispatcher, m_configurator);
					Logger.LogDebug(this, "Starting init action: " + action.ActionName, Logger.TagFlags.INIT);
					InitActions.RemoveAt(0);
					action.Start();
					if (m_runAsCoroutine)
					{
						yield return BaseGameController.Instance.StartCoroutine(action.Perform());
					}
					else
					{
						CoroutineHelper.RunCoroutineToCompletion(action.Perform());
					}
					yield return null;
				}
			}
		}

		private bool OnInitActionComplete(ActionCompleteEvent evt)
		{
			Logger.LogDebug(this, "Init action completed: " + evt.Action.ActionName, Logger.TagFlags.INIT);
			if (CheckAllComplete())
			{
				OnAllInitComplete();
			}
			return false;
		}

		private void OnAllInitComplete()
		{
			EventDispatcher.RemoveListener<ActionCompleteEvent>(OnInitActionComplete);
			EventDispatcher.DispatchEvent(new InitCompleteEvent());
			m_configurator = null;
			Logger.LogDebug(this, "All init actions completed", Logger.TagFlags.INIT);
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
				Logger.LogFatal(this, message, Logger.TagFlags.INIT);
				throw new ApplicationException(message);
			}
			foreach (string antecedentName in action.TopologicalDependencies)
			{
				List<InitAction> initActions = InitActions;
				Predicate<InitAction> match = (InitAction a) => a.GetType().ToString() == antecedentName;
				InitAction initAction = initActions.Find(match);
				if (initAction == null)
				{
					string message = "[ValidateDependencies] Missing Dependency: " + breadcrumbs + " -> " + antecedentName;
					Logger.LogFatal(this, message, Logger.TagFlags.INIT);
					throw new ApplicationException(message);
				}
				RecursiveValidateAction(initAction, dependencyType, breadcrumbs + " -> " + initAction.ActionName);
			}
		}
	}
}
