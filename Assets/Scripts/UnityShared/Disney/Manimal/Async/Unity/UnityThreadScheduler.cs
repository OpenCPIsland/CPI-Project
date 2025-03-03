using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Disney.Manimal.Async.Unity
{
	internal sealed class UnityThreadScheduler : IConcurrentScheduler, IDisposable
	{
		private readonly Queue<Action> _actionQueue;

		private readonly object _sync = new object();

		private MonoBehaviour _behavior;

		private Thread _owningThread;

		private bool _isDisposed;

		Thread IConcurrentScheduler.OwningThread
		{
			get
			{
				return _owningThread;
			}
		}

		public UnityThreadScheduler(MonoBehaviour behavior)
		{
			_behavior = behavior;
			_owningThread = Thread.CurrentThread;
			_actionQueue = new Queue<Action>();
			_behavior.StartCoroutine(ProcessQueue());
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void IConcurrentScheduler.QueueAction(Action action)
		{
			lock (_sync)
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException(GetType().FullName);
				}
				if (((IConcurrentScheduler)this).OwningThread == Thread.CurrentThread)
				{
					action();
				}
				else
				{
					_actionQueue.Enqueue(action);
				}
			}
		}

		private IEnumerator ProcessQueue()
		{
			while (!_isDisposed)
			{
				object sync;
				object obj = sync = _sync;
				Monitor.Enter(sync);
				try
				{
					while (!_isDisposed && _actionQueue.Count > 0)
					{
						Action action = _actionQueue.Dequeue();
						try
						{
							action();
						}
						catch (Exception exception)
						{
							Debug.LogWarning("Failed to invoke scheduled action. It will be ignored so subsequent actions may be processed.");
							Debug.LogException(exception);
						}
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
				yield return null;
			}
		}

		~UnityThreadScheduler()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				lock (_sync)
				{
					if (disposing)
					{
						_actionQueue.Clear();
					}
					_isDisposed = true;
				}
			}
		}
	}
}
