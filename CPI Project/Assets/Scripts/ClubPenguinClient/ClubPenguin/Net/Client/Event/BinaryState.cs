using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct BinaryState
	{
		public long SessionId
		{
			get;
			private set;
		}

		public bool IsTrue
		{
			get;
			private set;
		}

		public BinaryState(long sessionId, bool isTrue)
		{
			this = default(BinaryState);
			SessionId = sessionId;
			IsTrue = isTrue;
		}
	}
}
