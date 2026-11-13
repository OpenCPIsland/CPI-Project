using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PlayerLocomotionStateEvent
	{
		public long SessionId;

		public LocomotionState State;
	}
}
