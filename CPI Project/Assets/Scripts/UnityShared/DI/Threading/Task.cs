using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace DI.Threading
{
	public abstract class Task
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Unit
		{
		}

		private object syncRoot = new object();

		private bool hasEnded = false;

		public string Name;

		public volatile int Priority;

		private ManualResetEvent abortEvent = new ManualResetEvent(false);

		private ManualResetEvent endedEvent = new ManualResetEvent(false);

		private ManualResetEvent endingEvent = new ManualResetEvent(false);

		private bool hasStarted = false;

		[ThreadStatic]
		private static Task current;

		private bool disposed = false;

		public static Task Current
		{
			get
			{
				return current;
			}
		}

		public bool ShouldAbort
		{
			get
			{
				return abortEvent.InterWaitOne(0);
			}
		}

		public bool HasEnded
		{
			get
			{
				return hasEnded || endedEvent.InterWaitOne(0);
			}
		}

		public bool IsEnding
		{
			get
			{
				return endingEvent.InterWaitOne(0);
			}
		}

		public bool IsSucceeded
		{
			get
			{
				return endingEvent.InterWaitOne(0) && !abortEvent.InterWaitOne(0);
			}
		}

		public bool IsFailed
		{
			get
			{
				return endingEvent.InterWaitOne(0) && abortEvent.InterWaitOne(0);
			}
		}

		public abstract object RawResult
		{
			get;
		}

		private event TaskEndedEventHandler taskEnded;

		public event TaskEndedEventHandler TaskEnded
		{
			add
			{
				lock (syncRoot)
				{
					if (endingEvent.InterWaitOne(0))
					{
						value(this);
					}
					else
					{
						taskEnded += value;
					}
				}
			}
			remove
			{
				lock (syncRoot)
				{
					taskEnded -= value;
				}
			}
		}

		public Task()
		{
		}

		~Task()
		{
			Dispose();
		}

		private void End()
		{
			lock (syncRoot)
			{
				endingEvent.Set();
				if (this.taskEnded != null)
				{
					this.taskEnded(this);
				}
				endedEvent.Set();
				if (current == this)
				{
					current = null;
				}
				hasEnded = true;
			}
		}

		protected abstract IEnumerator Do();

		public void Abort()
		{
			abortEvent.Set();
			if (!hasStarted)
			{
				End();
			}
		}

		public void AbortWait()
		{
			Abort();
			if (hasStarted)
			{
				Wait();
			}
		}

		public void AbortWaitForSeconds(float seconds)
		{
			Abort();
			if (hasStarted)
			{
				WaitForSeconds(seconds);
			}
		}

		public void Wait()
		{
			if (!hasEnded)
			{
				Priority--;
				endedEvent.WaitOne();
			}
		}

		public void WaitForSeconds(float seconds)
		{
			if (!hasEnded)
			{
				Priority--;
				endedEvent.InterWaitOne(TimeSpan.FromSeconds(seconds));
			}
		}

		public abstract TResult Wait<TResult>();

		public abstract TResult WaitForSeconds<TResult>(float seconds);

		public abstract TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue);

		internal void DoInternal()
		{
			current = this;
			hasStarted = true;
			if (!ShouldAbort)
			{
				try
				{
					IEnumerator enumerator = Do();
					if (enumerator == null)
					{
						End();
						return;
					}
					RunEnumerator(enumerator);
				}
				catch (Exception ex)
				{
					Abort();
					if (string.IsNullOrEmpty(Name))
					{
						Debug.LogError("Error while processing task:\n" + ex.ToString());
					}
					else
					{
						Debug.LogError("Error while processing task '" + Name + "':\n" + ex.ToString());
					}
				}
			}
			End();
		}

		private void RunEnumerator(IEnumerator enumerator)
		{
			ThreadBase currentThread = ThreadBase.CurrentThread;
			do
			{
				if (enumerator.Current is Task)
				{
					Task taskBase = (Task)enumerator.Current;
					currentThread.DispatchAndWait(taskBase);
				}
				else if (enumerator.Current is SwitchTo)
				{
					SwitchTo switchTo = (SwitchTo)enumerator.Current;
					if (switchTo.Target == SwitchTo.TargetType.Main && currentThread != null)
					{
						Task taskBase = Create((Action)delegate
						{
							if (enumerator.MoveNext() && !ShouldAbort)
							{
								RunEnumerator(enumerator);
							}
						});
						currentThread.DispatchAndWait(taskBase);
					}
					else if (switchTo.Target == SwitchTo.TargetType.Thread && currentThread == null)
					{
						break;
					}
				}
			}
			while (enumerator.MoveNext() && !ShouldAbort);
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				if (hasStarted)
				{
					Wait();
				}
				endingEvent.Close();
				endedEvent.Close();
				abortEvent.Close();
			}
		}

		public Task Run(DispatcherBase target)
		{
			if (target == null)
			{
				return Run();
			}
			target.Dispatch(this);
			return this;
		}

		public Task Run()
		{
			Run(UnityThreadHelper.TaskDistributor);
			return this;
		}

		public static Task Create(Action<Task> action)
		{
			return new Task<Unit>(action);
		}

		public static Task Create(Action action)
		{
			return new Task<Unit>(action);
		}

		public static Task<T> Create<T>(Func<Task, T> func)
		{
			return new Task<T>(func);
		}

		public static Task<T> Create<T>(Func<T> func)
		{
			return new Task<T>(func);
		}

		public static Task Create(IEnumerator enumerator)
		{
			return new Task<IEnumerator>(enumerator);
		}

		public static Task<T> Create<T>(Type type, string methodName, params object[] args)
		{
			return new Task<T>(type, methodName, args);
		}

		public static Task<T> Create<T>(object that, string methodName, params object[] args)
		{
			return new Task<T>(that, methodName, args);
		}
	}
	public class Task<T> : Task
	{
		private Func<Task, T> function;

		private T result;

		public override object RawResult
		{
			get
			{
				if (!base.IsEnding)
				{
					Wait();
				}
				return result;
			}
		}

		public T Result
		{
			get
			{
				if (!base.IsEnding)
				{
					Wait();
				}
				return result;
			}
		}

		public Task(Func<Task, T> function)
		{
			this.function = function;
		}

		public Task(Func<T> function)
		{
			Func<Task, T> func = this.function = ((Task t) => function());
		}

		public Task(Action<Task> action)
		{
			Func<Task, T> func = function = delegate(Task t)
			{
				action(t);
				return default(T);
			};
		}

		public Task(Action action)
		{
			Func<Task, T> func = function = delegate
			{
				action();
				return default(T);
			};
		}

		public Task(IEnumerator enumerator)
		{
			Func<Task, T> func = function = ((Task t) => (T)enumerator);
		}

		public Task(Type type, string methodName, params object[] args)
		{
			MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
			if (methodInfo == null)
			{
				throw new ArgumentException("methodName", "Fitting method with the given name was not found.");
			}
			function = ((Task t) => (T)methodInfo.Invoke(null, args));
		}

		public Task(object that, string methodName, params object[] args)
		{
			MethodInfo methodInfo = that.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
			if (methodInfo == null)
			{
				throw new ArgumentException("methodName", "Fitting method with the given name was not found.");
			}
			function = ((Task t) => (T)methodInfo.Invoke(that, args));
		}

		protected override IEnumerator Do()
		{
			result = function(this);
			if (result is IEnumerator)
			{
				return (IEnumerator)(object)result;
			}
			return null;
		}

		public override TResult Wait<TResult>()
		{
			Priority--;
			return (TResult)(object)Result;
		}

		public override TResult WaitForSeconds<TResult>(float seconds)
		{
			Priority--;
			return ((Task)this).WaitForSeconds(seconds, default(TResult));
		}

		public override TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue)
		{
			if (!base.HasEnded)
			{
				WaitForSeconds(seconds);
			}
			if (base.IsSucceeded)
			{
				return (TResult)(object)result;
			}
			return defaultReturnValue;
		}

		public new Task<T> Run(DispatcherBase target)
		{
			base.Run(target);
			return this;
		}

		public new Task<T> Run()
		{
			base.Run();
			return this;
		}
	}
}
