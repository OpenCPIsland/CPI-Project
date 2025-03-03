using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DI.Threading
{
	public abstract class DispatcherBase : IDisposable
	{
		protected int lockCount = 0;

		protected object taskListSyncRoot = new object();

		protected Queue<Task> taskList = new Queue<Task>();

		protected Queue<Task> delayedTaskList = new Queue<Task>();

		protected ManualResetEvent dataEvent = new ManualResetEvent(false);

		public bool AllowAccessLimitationChecks;

		public TaskSortingSystem TaskSortingSystem;

		public bool IsWorking
		{
			get
			{
				return dataEvent.InterWaitOne(0);
			}
		}

		public virtual int TaskCount
		{
			get
			{
				lock (taskListSyncRoot)
				{
					return taskList.Count;
				}
			}
		}

		public DispatcherBase()
		{
		}

		public void Lock()
		{
			lock (taskListSyncRoot)
			{
				lockCount++;
			}
		}

		public void Unlock()
		{
			lock (taskListSyncRoot)
			{
				lockCount--;
				if (lockCount == 0 && delayedTaskList.Count > 0)
				{
					while (delayedTaskList.Count > 0)
					{
						taskList.Enqueue(delayedTaskList.Dequeue());
					}
					if (TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded || TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
					{
						ReorderTasks();
					}
					TasksAdded();
				}
			}
		}

		public Task<T> Dispatch<T>(Func<T> function)
		{
			CheckAccessLimitation();
			Task<T> task = new Task<T>(function);
			AddTask(task);
			return task;
		}

		public Task Dispatch(Action action)
		{
			CheckAccessLimitation();
			Task task = Task.Create(action);
			AddTask(task);
			return task;
		}

		public Task Dispatch(Task task)
		{
			CheckAccessLimitation();
			AddTask(task);
			return task;
		}

		internal virtual void AddTask(Task task)
		{
			lock (taskListSyncRoot)
			{
				if (lockCount > 0)
				{
					delayedTaskList.Enqueue(task);
					return;
				}
				taskList.Enqueue(task);
				if (TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded || TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
				{
					ReorderTasks();
				}
			}
			TasksAdded();
		}

		internal void AddTasks(IEnumerable<Task> tasks)
		{
			lock (taskListSyncRoot)
			{
				if (lockCount > 0)
				{
					foreach (Task task in tasks)
					{
						delayedTaskList.Enqueue(task);
					}
					return;
				}
				foreach (Task task2 in tasks)
				{
					taskList.Enqueue(task2);
				}
				if (TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded || TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
				{
					ReorderTasks();
				}
			}
			TasksAdded();
		}

		internal virtual void TasksAdded()
		{
			dataEvent.Set();
		}

		protected void ReorderTasks()
		{
			taskList = new Queue<Task>(taskList.OrderBy((Task t) => t.Priority));
		}

		internal IEnumerable<Task> SplitTasks(int divisor)
		{
			if (divisor == 0)
			{
				divisor = 2;
			}
			int count = TaskCount / divisor;
			return IsolateTasks(count);
		}

		internal IEnumerable<Task> IsolateTasks(int count)
		{
			Queue<Task> queue = new Queue<Task>();
			if (count == 0)
			{
				count = taskList.Count;
			}
			lock (taskListSyncRoot)
			{
				for (int i = 0; i < count && i < taskList.Count; i++)
				{
					queue.Enqueue(taskList.Dequeue());
				}
				if (TaskCount == 0)
				{
					dataEvent.Reset();
				}
			}
			return queue;
		}

		protected abstract void CheckAccessLimitation();

		public virtual void Dispose()
		{
			while (true)
			{
				bool flag = true;
				Task task;
				lock (taskListSyncRoot)
				{
					if (taskList.Count != 0)
					{
						task = taskList.Dequeue();
						goto IL_0040;
					}
				}
				break;
				IL_0040:
				task.Dispose();
			}
			dataEvent.Close();
			dataEvent = null;
		}
	}
}
