using System;
using System.Threading;

namespace Disney.Manimal.Async
{
	internal interface IConcurrentScheduler
	{
		Thread OwningThread
		{
			get;
		}

		void QueueAction(Action action);
	}
}
