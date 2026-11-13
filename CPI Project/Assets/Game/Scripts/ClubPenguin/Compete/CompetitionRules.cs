using System.Collections.Generic;

namespace ClubPenguin.Compete
{
	public abstract class CompetitionRules<T>
	{
		public abstract void AssignPointsToCompetitors(List<Competitor<T>> competitors);
	}
}
