using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct OnQuestState
	{
		public long SessionId;

		public string MascotName;
	}
}
