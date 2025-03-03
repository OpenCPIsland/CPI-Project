using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DI.Threading
{
	[ExecuteInEditMode]
	public class UnityThreadHelper : MonoBehaviour
	{
		private static UnityThreadHelper instance = null;

		private static object syncRoot = new object();

		private Dispatcher dispatcher;

		private TaskDistributor taskDistributor;

		private List<ThreadBase> registeredThreads = new List<ThreadBase>();

		private static UnityThreadHelper Instance
		{
			get
			{
				EnsureHelper();
				return instance;
			}
		}

		public static Dispatcher Dispatcher
		{
			get
			{
				return Instance.CurrentDispatcher;
			}
		}

		public static TaskDistributor TaskDistributor
		{
			get
			{
				return Instance.CurrentTaskDistributor;
			}
		}

		public Dispatcher CurrentDispatcher
		{
			get
			{
				return dispatcher;
			}
		}

		public TaskDistributor CurrentTaskDistributor
		{
			get
			{
				return taskDistributor;
			}
		}

		public static void EnsureHelper()
		{
			lock (syncRoot)
			{
				if ((object)null == instance)
				{
					instance = (UnityEngine.Object.FindObjectOfType(typeof(UnityThreadHelper)) as UnityThreadHelper);
					if ((object)null == instance)
					{
						GameObject gameObject = new GameObject("[UnityThreadHelper]");
						gameObject.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable);
						instance = gameObject.AddComponent<UnityThreadHelper>();
						instance.EnsureHelperInstance();
					}
				}
			}
		}

		private void EnsureHelperInstance()
		{
			dispatcher = (Dispatcher.MainNoThrow ?? new Dispatcher());
			taskDistributor = (TaskDistributor.MainNoThrow ?? new TaskDistributor("TaskDistributor"));
		}

		public static ActionThread CreateThread(Action<ActionThread> action, bool autoStartThread)
		{
			Instance.EnsureHelperInstance();
			Action<ActionThread> action2 = delegate(ActionThread currentThread)
			{
				try
				{
					action(currentThread);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			};
			ActionThread actionThread = new ActionThread(action2, autoStartThread);
			Instance.RegisterThread(actionThread);
			return actionThread;
		}

		public static ActionThread CreateThread(Action<ActionThread> action)
		{
			return CreateThread(action, true);
		}

		public static ActionThread CreateThread(Action action, bool autoStartThread)
		{
			return CreateThread((Action<ActionThread>)delegate
			{
				action();
			}, autoStartThread);
		}

		public static ActionThread CreateThread(Action action)
		{
			return CreateThread((Action<ActionThread>)delegate
			{
				action();
			}, true);
		}

		public static ThreadBase CreateThread(Func<ThreadBase, IEnumerator> action, bool autoStartThread)
		{
			Instance.EnsureHelperInstance();
			EnumeratableActionThread enumeratableActionThread = new EnumeratableActionThread(action, autoStartThread);
			Instance.RegisterThread(enumeratableActionThread);
			return enumeratableActionThread;
		}

		public static ThreadBase CreateThread(Func<ThreadBase, IEnumerator> action)
		{
			return CreateThread(action, true);
		}

		public static ThreadBase CreateThread(Func<IEnumerator> action, bool autoStartThread)
		{
			Func<ThreadBase, IEnumerator> action2 = (ThreadBase thread) => action();
			return CreateThread(action2, autoStartThread);
		}

		public static ThreadBase CreateThread(Func<IEnumerator> action)
		{
			Func<ThreadBase, IEnumerator> action2 = (ThreadBase thread) => action();
			return CreateThread(action2, true);
		}

		private void RegisterThread(ThreadBase thread)
		{
			if (!registeredThreads.Contains(thread))
			{
				registeredThreads.Add(thread);
			}
		}

		private void OnDestroy()
		{
			foreach (ThreadBase registeredThread in registeredThreads)
			{
				registeredThread.Dispose();
			}
			if (dispatcher != null)
			{
				dispatcher.Dispose();
			}
			dispatcher = null;
			if (taskDistributor != null)
			{
				taskDistributor.Dispose();
			}
			taskDistributor = null;
			if (instance == this)
			{
				instance = null;
			}
		}

		private void Update()
		{
			if (dispatcher != null)
			{
				dispatcher.ProcessTasks();
			}
			ThreadBase[] array = registeredThreads.Where((ThreadBase thread) => !thread.IsAlive).ToArray();
			ThreadBase[] array2 = array;
			foreach (ThreadBase threadBase in array2)
			{
				threadBase.Dispose();
				registeredThreads.Remove(threadBase);
			}
		}
	}
}
