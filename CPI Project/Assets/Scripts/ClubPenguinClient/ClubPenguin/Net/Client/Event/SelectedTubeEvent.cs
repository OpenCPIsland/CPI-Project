using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct SelectedTubeEvent
	{
		public long SessionId;

		public int TubeId;
	}
}
