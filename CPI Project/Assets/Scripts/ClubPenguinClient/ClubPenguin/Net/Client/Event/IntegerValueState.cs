using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct IntegerValueState
	{
		public long SessionId
		{
			get;
			private set;
		}

		public int Value
		{
			get;
			private set;
		}

		public IntegerValueState(long sessionId, int value)
		{
			this = default(IntegerValueState);
			SessionId = sessionId;
			Value = value;
		}
	}
}
