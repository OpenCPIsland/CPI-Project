using System;
using System.Collections;

namespace DI.Threading
{
	public sealed class ActionThread : ThreadBase
	{
		private Action<ActionThread> action;

		public ActionThread(Action<ActionThread> action)
			: this(action, true)
		{
		}

		public ActionThread(Action<ActionThread> action, bool autoStartThread)
			: base("ActionThread", Dispatcher.Current, false)
		{
			this.action = action;
			if (autoStartThread)
			{
				Start();
			}
		}

		protected override IEnumerator Do()
		{
			action(this);
			return null;
		}
	}
}
