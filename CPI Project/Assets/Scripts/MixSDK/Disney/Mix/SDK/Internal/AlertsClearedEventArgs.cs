using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class AlertsClearedEventArgs : AbstractAlertsClearedEventArgs
	{
		public AlertsClearedEventArgs(IEnumerable<IAlert> alerts)
		{
			base.Alerts = alerts;
		}
	}
}
