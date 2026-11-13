using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PCSessionStartRequest
	{
		public string DistributionChannelId;

		public string SourceIp;

		public string Language;

		public string DeviceType;
	}
}
