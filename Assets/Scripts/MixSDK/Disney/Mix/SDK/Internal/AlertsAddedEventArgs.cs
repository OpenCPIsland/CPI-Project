using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class AlertsAddedEventArgs : AbstractAlertsAddedEventArgs
	{
		public AlertsAddedEventArgs(IEnumerable<IAlert> alerts)
		{
			base.Alerts = alerts;
		}
	}
}
