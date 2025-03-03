using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct ClientAnalyticsAction
	{
		public string context;

		public string action;

		public string location;

		public string type;
	}
}
