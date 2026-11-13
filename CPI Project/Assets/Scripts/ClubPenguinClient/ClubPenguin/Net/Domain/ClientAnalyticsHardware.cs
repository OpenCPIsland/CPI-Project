using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct ClientAnalyticsHardware
	{
		public string platform;

		public string model;

		public string osVersion;

		public string network;
	}
}
