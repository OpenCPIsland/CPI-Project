using ClubPenguin.Compete;
using System;
using UnityEngine;

namespace ClubPenguin.Participation
{
	public class ParticipationRequest
	{
		public enum Type
		{
			Prepare,
			Start,
			ForcedInteraction,
			Lock,
			Stop,
			Internal
		}

		public readonly DateTime Timestamp;

		public readonly Type RequestType;

		public readonly string RequestorName;

		public readonly GameObject Requestor;

		public readonly Competitor<GameObject> CompetitorReference;

		public readonly string Source;

		public ParticipationRequest(Type type, GameObject requestor, string source, Competitor<GameObject> competitor = null)
		{
			RequestType = type;
			Requestor = requestor;
			RequestorName = ((requestor != null) ? requestor.name : "null.name");
			Timestamp = DateTime.Now;
			Source = source;
			CompetitorReference = competitor;
		}

		public ParticipationRequest(Type type, Competitor<GameObject> competitor, string source)
		{
			RequestType = type;
			if (competitor != null)
			{
				Requestor = competitor.Value;
				RequestorName = ((competitor.Value != null) ? competitor.Value.name : "null.name");
			}
			Timestamp = DateTime.Now;
			Source = source;
			CompetitorReference = competitor;
		}
	}
}
