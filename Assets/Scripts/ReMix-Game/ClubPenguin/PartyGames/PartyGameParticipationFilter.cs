using ClubPenguin.Participation;

namespace ClubPenguin.PartyGames
{
	public class PartyGameParticipationFilter : AbstractParticipationFilter
	{
		public const string FILTER_ID = "party_game";

		public override bool doesRequestPassFilter(ParticipationRequest request)
		{
			return request.Requestor.GetComponentInChildren<PartyGameBehaviourTag>() != null;
		}

		public override string getId()
		{
			return "party_game";
		}
	}
}
