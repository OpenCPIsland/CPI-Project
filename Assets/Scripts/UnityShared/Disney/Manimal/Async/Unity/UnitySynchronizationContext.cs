using System.Threading;
using UnityEngine;

namespace Disney.Manimal.Async.Unity
{
	public sealed class UnitySynchronizationContext : SynchronizationContext
	{
		private MonoBehaviour monoBehaviour;

		private readonly IConcurrentScheduler _scheduler;

		public MonoBehaviour MonoBehaviour
		{
			get
			{
				return monoBehaviour;
			}
		}

		public UnitySynchronizationContext(MonoBehaviour behavior)
		{
			monoBehaviour = behavior;
			_scheduler = new UnityThreadScheduler(behavior);
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			_scheduler.QueueAction(delegate
			{
				d(state);
			});
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			if (_scheduler.OwningThread == Thread.CurrentThread)
			{
				d(state);
				return;
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			_scheduler.QueueAction(delegate
			{
				try
				{
					d(state);
				}
				finally
				{
					wait.Set();
				}
			});
			wait.WaitOne();
		}
	}
}
