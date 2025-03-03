using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public abstract class AbstractAlertsAddedEventArgs : EventArgs
	{
		public IEnumerable<IAlert> Alerts
		{
			get;
			protected set;
		}
	}
}
