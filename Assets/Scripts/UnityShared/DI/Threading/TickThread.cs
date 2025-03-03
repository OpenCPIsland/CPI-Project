using System;
using System.Collections;
using System.Threading;

namespace DI.Threading
{
	public sealed class TickThread : ThreadBase
	{
		private Action action;

		private int tickLengthInMilliseconds;

		private ManualResetEvent tickEvent = new ManualResetEvent(false);

		public TickThread(Action action, int tickLengthInMilliseconds)
			: this(action, tickLengthInMilliseconds, true)
		{
		}

		public TickThread(Action action, int tickLengthInMilliseconds, bool autoStartThread)
			: base("TickThread", Dispatcher.CurrentNoThrow, false)
		{
			this.tickLengthInMilliseconds = tickLengthInMilliseconds;
			this.action = action;
			if (autoStartThread)
			{
				Start();
			}
		}

		protected override IEnumerator Do()
		{
			while (!exitEvent.InterWaitOne(0))
			{
				action();
				if (WaitHandle.WaitAny(new WaitHandle[2]
				{
					exitEvent,
					tickEvent
				}, tickLengthInMilliseconds) == 0)
				{
					return null;
				}
			}
			return null;
		}
	}
}
