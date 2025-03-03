using System;
using System.Collections.Generic;
using System.Threading;

namespace DI.Threading
{
	public class Dispatcher : DispatcherBase
	{
		[ThreadStatic]
		private static Task currentTask;

		[ThreadStatic]
		internal static Dispatcher currentDispatcher;

		protected static Dispatcher mainDispatcher;

		public static Task CurrentTask
		{
			get
			{
				if (currentTask == null)
				{
					throw new InvalidOperationException("No task is currently running.");
				}
				return currentTask;
			}
		}

		public static Dispatcher Current
		{
			get
			{
				if (currentDispatcher == null)
				{
					throw new InvalidOperationException("No Dispatcher found for the current thread, please create a new Dispatcher instance before calling this property.");
				}
				return currentDispatcher;
			}
			set
			{
				if (currentDispatcher != null)
				{
					currentDispatcher.Dispose();
				}
				currentDispatcher = value;
			}
		}

		public static Dispatcher CurrentNoThrow
		{
			get
			{
				return currentDispatcher;
			}
		}

		public static Dispatcher Main
		{
			get
			{
				if (mainDispatcher == null)
				{
					throw new InvalidOperationException("No Dispatcher found for the main thread, please create a new Dispatcher instance before calling this property.");
				}
				return mainDispatcher;
			}
		}

		public static Dispatcher MainNoThrow
		{
			get
			{
				return mainDispatcher;
			}
		}

		public static Func<T> CreateSafeFunction<T>(Func<T> function)
		{
			return delegate
			{
				try
				{
					return function();
				}
				catch
				{
					CurrentTask.Abort();
					return default(T);
				}
			};
		}

		public static Action CreateSafeAction<T>(Action action)
		{
			return delegate
			{
				try
				{
					action();
				}
				catch
				{
					CurrentTask.Abort();
				}
			};
		}

		public Dispatcher()
			: this(true)
		{
		}

		public Dispatcher(bool setThreadDefaults)
		{
			if (setThreadDefaults)
			{
				if (currentDispatcher != null)
				{
					throw new InvalidOperationException("Only one Dispatcher instance allowed per thread.");
				}
				currentDispatcher = this;
				if (mainDispatcher == null)
				{
					mainDispatcher = this;
				}
			}
		}

		public void ProcessTasks()
		{
			if (dataEvent.InterWaitOne(0))
			{
				ProcessTasksInternal();
			}
		}

		public bool ProcessTasks(WaitHandle exitHandle)
		{
			if (WaitHandle.WaitAny(new WaitHandle[2]
			{
				exitHandle,
				dataEvent
			}) == 0)
			{
				return false;
			}
			ProcessTasksInternal();
			return true;
		}

		public bool ProcessNextTask()
		{
			Task task;
			lock (taskListSyncRoot)
			{
				if (taskList.Count == 0)
				{
					return false;
				}
				task = taskList.Dequeue();
			}
			ProcessSingleTask(task);
			if (TaskCount == 0)
			{
				dataEvent.Reset();
			}
			return true;
		}

		public bool ProcessNextTask(WaitHandle exitHandle)
		{
			if (WaitHandle.WaitAny(new WaitHandle[2]
			{
				exitHandle,
				dataEvent
			}) == 0)
			{
				return false;
			}
			Task task;
			lock (taskListSyncRoot)
			{
				if (taskList.Count == 0)
				{
					return false;
				}
				task = taskList.Dequeue();
			}
			ProcessSingleTask(task);
			if (TaskCount == 0)
			{
				dataEvent.Reset();
			}
			return true;
		}

		private void ProcessTasksInternal()
		{
			List<Task> list;
			lock (taskListSyncRoot)
			{
				list = new List<Task>(taskList);
				taskList.Clear();
			}
			while (list.Count != 0)
			{
				Task task = list[0];
				list.RemoveAt(0);
				ProcessSingleTask(task);
			}
			if (TaskCount == 0)
			{
				dataEvent.Reset();
			}
		}

		private void ProcessSingleTask(Task task)
		{
			RunTask(task);
			if (TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
			{
				lock (taskListSyncRoot)
				{
					ReorderTasks();
				}
			}
		}

		internal void RunTask(Task task)
		{
			Task task2 = currentTask;
			currentTask = task;
			currentTask.DoInternal();
			currentTask = task2;
		}

		protected override void CheckAccessLimitation()
		{
			if (AllowAccessLimitationChecks && currentDispatcher == this)
			{
				throw new InvalidOperationException("Dispatching a Task with the Dispatcher associated to the current thread is prohibited. You can run these Tasks without the need of a Dispatcher.");
			}
		}

		public override void Dispose()
		{
			while (true)
			{
				bool flag = true;
				lock (taskListSyncRoot)
				{
					if (taskList.Count != 0)
					{
						currentTask = taskList.Dequeue();
						goto IL_0044;
					}
				}
				break;
				IL_0044:
				currentTask.Dispose();
			}
			dataEvent.Close();
			dataEvent = null;
			if (currentDispatcher == this)
			{
				currentDispatcher = null;
			}
			if (mainDispatcher == this)
			{
				mainDispatcher = null;
			}
		}
	}
}
