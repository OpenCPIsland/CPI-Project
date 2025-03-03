using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace DI.Threading
{
	public abstract class ThreadBase : IDisposable
	{
		protected Dispatcher targetDispatcher;

		protected Thread thread;

		protected ManualResetEvent exitEvent = new ManualResetEvent(false);

		[ThreadStatic]
		private static ThreadBase currentThread;

		private string threadName;

		private System.Threading.ThreadPriority priority = System.Threading.ThreadPriority.BelowNormal;

		public static int AvailableProcessors
		{
			get
			{
				return SystemInfo.processorCount;
			}
		}

		public static ThreadBase CurrentThread
		{
			get
			{
				return currentThread;
			}
		}

		public bool IsAlive
		{
			get
			{
				return thread != null && thread.IsAlive;
			}
		}

		public bool ShouldStop
		{
			get
			{
				return exitEvent.InterWaitOne(0);
			}
		}

		public System.Threading.ThreadPriority Priority
		{
			get
			{
				return priority;
			}
			set
			{
				priority = value;
				if (thread != null)
				{
					thread.Priority = priority;
				}
			}
		}

		public ThreadBase(string threadName)
			: this(threadName, true)
		{
		}

		public ThreadBase(string threadName, bool autoStartThread)
			: this(threadName, Dispatcher.CurrentNoThrow, autoStartThread)
		{
		}

		public ThreadBase(string threadName, Dispatcher targetDispatcher)
			: this(threadName, targetDispatcher, true)
		{
		}

		public ThreadBase(string threadName, Dispatcher targetDispatcher, bool autoStartThread)
		{
			this.threadName = threadName;
			this.targetDispatcher = targetDispatcher;
			if (autoStartThread)
			{
				Start();
			}
		}

		public void Start()
		{
			if (thread != null)
			{
				Abort();
			}
			exitEvent.Reset();
			thread = new Thread(DoInternal);
			thread.Name = threadName;
			thread.Priority = priority;
			thread.Start();
		}

		public void Exit()
		{
			if (thread != null)
			{
				exitEvent.Set();
			}
		}

		public void Abort()
		{
			Exit();
			if (thread != null)
			{
				thread.Join();
			}
		}

		public void AbortWaitForSeconds(float seconds)
		{
			Exit();
			if (thread != null)
			{
				thread.Join((int)(seconds * 1000f));
				if (thread.IsAlive)
				{
					thread.Abort();
				}
			}
		}

		public Task<T> Dispatch<T>(Func<T> function)
		{
			return targetDispatcher.Dispatch(function);
		}

		public T DispatchAndWait<T>(Func<T> function)
		{
			Task<T> task = Dispatch(function);
			task.Wait();
			return task.Result;
		}

		public T DispatchAndWait<T>(Func<T> function, float timeOutSeconds)
		{
			Task<T> task = Dispatch(function);
			task.WaitForSeconds(timeOutSeconds);
			return task.Result;
		}

		public Task Dispatch(Action action)
		{
			return targetDispatcher.Dispatch(action);
		}

		public void DispatchAndWait(Action action)
		{
			Task task = Dispatch(action);
			task.Wait();
		}

		public void DispatchAndWait(Action action, float timeOutSeconds)
		{
			Task task = Dispatch(action);
			task.WaitForSeconds(timeOutSeconds);
		}

		public Task Dispatch(Task taskBase)
		{
			return targetDispatcher.Dispatch(taskBase);
		}

		public void DispatchAndWait(Task taskBase)
		{
			Task task = Dispatch(taskBase);
			task.Wait();
		}

		public void DispatchAndWait(Task taskBase, float timeOutSeconds)
		{
			Task task = Dispatch(taskBase);
			task.WaitForSeconds(timeOutSeconds);
		}

		protected void DoInternal()
		{
			currentThread = this;
			IEnumerator enumerator = Do();
			if (enumerator != null)
			{
				RunEnumerator(enumerator);
			}
		}

		private void RunEnumerator(IEnumerator enumerator)
		{
			do
			{
				if (enumerator.Current is Task)
				{
					Task taskBase = (Task)enumerator.Current;
					DispatchAndWait(taskBase);
				}
				else if (enumerator.Current is SwitchTo)
				{
					SwitchTo switchTo = (SwitchTo)enumerator.Current;
					if (switchTo.Target == SwitchTo.TargetType.Main && CurrentThread != null)
					{
						Task taskBase = Task.Create((Action)delegate
						{
							if (enumerator.MoveNext() && !ShouldStop)
							{
								RunEnumerator(enumerator);
							}
						});
						DispatchAndWait(taskBase);
					}
					else if (switchTo.Target == SwitchTo.TargetType.Thread && CurrentThread == null)
					{
						break;
					}
				}
			}
			while (enumerator.MoveNext() && !ShouldStop);
		}

		protected abstract IEnumerator Do();

		public virtual void Dispose()
		{
			AbortWaitForSeconds(1f);
		}
	}
}
