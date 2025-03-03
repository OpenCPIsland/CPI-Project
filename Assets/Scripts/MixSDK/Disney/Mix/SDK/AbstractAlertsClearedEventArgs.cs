using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public abstract class AbstractAlertsClearedEventArgs : EventArgs
	{
		public IEnumerable<IAlert> Alerts
		{
			get;
			protected set;
		}
	}
}
