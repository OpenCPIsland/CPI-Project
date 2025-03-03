using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct ClientAnalytics
	{
		public ClientAnalyticsHardware hardware;

		public ClientAnalyticsAction action;
	}
}
