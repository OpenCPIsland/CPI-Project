using System.Collections.Generic;
using System.Diagnostics;

namespace ClubPenguin.Compete
{
	public class CompetitionResults<T>
	{
		public readonly Competitor<T> Winner;

		public CompetitionResults(List<Competitor<T>> competitors)
		{
			Winner = ((competitors.Count > 0 && competitors[0].Points > 0) ? competitors[0] : null);
		}

		[Conditional("UNITY_EDITOR")]
		private void setCompetitors(List<Competitor<T>> competitors)
		{
		}
	}
}
