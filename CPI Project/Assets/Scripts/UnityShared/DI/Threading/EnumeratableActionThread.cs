using System;
using System.Collections;

namespace DI.Threading
{
	public sealed class EnumeratableActionThread : ThreadBase
	{
		private Func<ThreadBase, IEnumerator> enumeratableAction;

		public EnumeratableActionThread(Func<ThreadBase, IEnumerator> enumeratableAction)
			: this(enumeratableAction, true)
		{
		}

		public EnumeratableActionThread(Func<ThreadBase, IEnumerator> enumeratableAction, bool autoStartThread)
			: base("EnumeratableActionThread", Dispatcher.Current, false)
		{
			this.enumeratableAction = enumeratableAction;
			if (autoStartThread)
			{
				Start();
			}
		}

		protected override IEnumerator Do()
		{
			return enumeratableAction(this);
		}
	}
}
