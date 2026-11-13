using System.Collections;
using System.Threading;

namespace DI.Threading
{
	internal sealed class TaskWorker : ThreadBase
	{
		public Dispatcher Dispatcher;

		public TaskDistributor TaskDistributor
		{
			get;
			private set;
		}

		public bool IsWorking
		{
			get
			{
				return Dispatcher.IsWorking;
			}
		}

		public TaskWorker(string name, TaskDistributor taskDistributor)
			: base(name, false)
		{
			TaskDistributor = taskDistributor;
			Dispatcher = new Dispatcher(false);
		}

		protected override IEnumerator Do()
		{
			while (!exitEvent.InterWaitOne(0))
			{
				if (Dispatcher.ProcessNextTask())
				{
					continue;
				}
				TaskDistributor.FillTasks(Dispatcher);
				if (Dispatcher.TaskCount == 0)
				{
					if (WaitHandle.WaitAny(new WaitHandle[2]
					{
						exitEvent,
						TaskDistributor.NewDataWaitHandle
					}) == 0)
					{
						return null;
					}
					TaskDistributor.FillTasks(Dispatcher);
				}
			}
			return null;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (Dispatcher != null)
			{
				Dispatcher.Dispose();
			}
			Dispatcher = null;
		}
	}
}
