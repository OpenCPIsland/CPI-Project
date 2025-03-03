using System;
using System.Linq;
using System.Threading;

namespace DI.Threading
{
	public class TaskDistributor : DispatcherBase
	{
		private TaskWorker[] workerThreads;

		private static TaskDistributor mainTaskDistributor;

		public int MaxAdditionalWorkerThreads = 0;

		private string name;

		private bool isDisposed = false;

		private ThreadPriority priority = ThreadPriority.BelowNormal;

		internal WaitHandle NewDataWaitHandle
		{
			get
			{
				return dataEvent;
			}
		}

		public static TaskDistributor Main
		{
			get
			{
				if (mainTaskDistributor == null)
				{
					throw new InvalidOperationException("No default TaskDistributor found, please create a new TaskDistributor instance before calling this property.");
				}
				return mainTaskDistributor;
			}
		}

		public static TaskDistributor MainNoThrow
		{
			get
			{
				return mainTaskDistributor;
			}
		}

		public override int TaskCount
		{
			get
			{
				int num = base.TaskCount;
				lock (workerThreads)
				{
					for (int i = 0; i < workerThreads.Length; i++)
					{
						num += workerThreads[i].Dispatcher.TaskCount;
					}
				}
				return num;
			}
		}

		public ThreadPriority Priority
		{
			get
			{
				return priority;
			}
			set
			{
				priority = value;
				TaskWorker[] array = workerThreads;
				foreach (TaskWorker taskWorker in array)
				{
					taskWorker.Priority = value;
				}
			}
		}

		public TaskDistributor(string name)
			: this(name, 0)
		{
		}

		public TaskDistributor(string name, int workerThreadCount)
			: this(name, workerThreadCount, true)
		{
		}

		public TaskDistributor(string name, int workerThreadCount, bool autoStart)
		{
			this.name = name;
			if (workerThreadCount <= 0)
			{
				workerThreadCount = ThreadBase.AvailableProcessors * 2;
			}
			workerThreads = new TaskWorker[workerThreadCount];
			lock (workerThreads)
			{
				for (int i = 0; i < workerThreadCount; i++)
				{
					workerThreads[i] = new TaskWorker(name, this);
				}
			}
			if (mainTaskDistributor == null)
			{
				mainTaskDistributor = this;
			}
			if (autoStart)
			{
				Start();
			}
		}

		public void Start()
		{
			lock (workerThreads)
			{
				for (int i = 0; i < workerThreads.Length; i++)
				{
					if (!workerThreads[i].IsAlive)
					{
						workerThreads[i].Start();
					}
				}
			}
		}

		public void SpawnAdditionalWorkerThread()
		{
			lock (workerThreads)
			{
				Array.Resize(ref workerThreads, workerThreads.Length + 1);
				workerThreads[workerThreads.Length - 1] = new TaskWorker(name, this);
				workerThreads[workerThreads.Length - 1].Priority = priority;
				workerThreads[workerThreads.Length - 1].Start();
			}
		}

		internal void FillTasks(Dispatcher target)
		{
			target.AddTasks(IsolateTasks(1));
		}

		protected override void CheckAccessLimitation()
		{
			if (MaxAdditionalWorkerThreads > 0 || !AllowAccessLimitationChecks || ThreadBase.CurrentThread == null || !(ThreadBase.CurrentThread is TaskWorker) || ((TaskWorker)ThreadBase.CurrentThread).TaskDistributor != this)
			{
				return;
			}
			throw new InvalidOperationException("Access to TaskDistributor prohibited when called from inside a TaskDistributor thread. Dont dispatch new Tasks through the same TaskDistributor. If you want to distribute new tasks create a new TaskDistributor and use the new created instance. Remember to dispose the new instance to prevent thread spamming.");
		}

		internal override void TasksAdded()
		{
			if (MaxAdditionalWorkerThreads > 0 && (workerThreads.All((TaskWorker worker) => worker.Dispatcher.TaskCount > 0 || worker.IsWorking) || taskList.Count > workerThreads.Length))
			{
				Interlocked.Decrement(ref MaxAdditionalWorkerThreads);
				SpawnAdditionalWorkerThread();
			}
			base.TasksAdded();
		}

		public override void Dispose()
		{
			if (!isDisposed)
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
							goto IL_0052;
						}
					}
					break;
					IL_0052:
					task.Dispose();
				}
				lock (workerThreads)
				{
					for (int i = 0; i < workerThreads.Length; i++)
					{
						workerThreads[i].Dispose();
					}
					workerThreads = new TaskWorker[0];
				}
				dataEvent.Close();
				dataEvent = null;
				if (mainTaskDistributor == this)
				{
					mainTaskDistributor = null;
				}
				isDisposed = true;
			}
		}
	}
}
