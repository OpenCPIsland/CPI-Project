using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PartyGameStartEventV2
	{
		public string playerData;

		public int sessionId;

		public int templateId;
	}
}
