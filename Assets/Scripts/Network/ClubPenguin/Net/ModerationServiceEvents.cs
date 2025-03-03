using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public static class ModerationServiceEvents
	{
		public struct ShowAlerts
		{
			public readonly IEnumerable<IModerationAlert> Alerts;

			public ShowAlerts(IEnumerable<IModerationAlert> alerts)
			{
				Alerts = alerts;
			}
		}
	}
}
