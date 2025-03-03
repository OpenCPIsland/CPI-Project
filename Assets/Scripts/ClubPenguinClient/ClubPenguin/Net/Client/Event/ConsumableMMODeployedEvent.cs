using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct ConsumableMMODeployedEvent
	{
		public long SessionId;

		public long ExperienceId;
	}
}
