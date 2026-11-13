namespace ClubPenguin.Participation
{
	public abstract class AbstractParticipationFilter
	{
		public abstract bool doesRequestPassFilter(ParticipationRequest request);

		public abstract string getId();
	}
}
