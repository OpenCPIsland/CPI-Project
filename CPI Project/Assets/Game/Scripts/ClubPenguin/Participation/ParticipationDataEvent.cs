using UnityEngine;

namespace ClubPenguin.Participation
{
	public class ParticipationDataEvent
	{
		public struct StateChanged
		{
			public readonly ParticipationState State;

			public readonly GameObject Requestor;

			public StateChanged(ParticipationState state, GameObject requestor)
			{
				State = state;
				Requestor = requestor;
			}
		}
	}
}
