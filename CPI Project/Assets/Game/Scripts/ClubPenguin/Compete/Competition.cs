using System.Collections.Generic;

namespace ClubPenguin.Compete
{
	public class Competition<T>
	{
		private CompetitionRules<T> Rules;

		public List<Competitor<T>> Competitors;

		public List<CompetitionResults<T>> Results;

		public CompetitionResults<T> LatestResults
		{
			get
			{
				return (Results.Count > 0) ? Results[Results.Count - 1] : null;
			}
		}

		public Competition(CompetitionRules<T> rules)
		{
			Rules = rules;
			Results = new List<CompetitionResults<T>>();
			Competitors = new List<Competitor<T>>();
		}

		public void RemoveAllCompetitorsAndResults()
		{
			Competitors.Clear();
			Results.Clear();
		}

		public Competitor<T> EnterCompetitor(T competitor)
		{
			if (competitor == null)
			{
				return null;
			}
			foreach (Competitor<T> competitor3 in Competitors)
			{
				if (competitor.Equals(competitor3.theObject))
				{
					return competitor3;
				}
			}
			Competitor<T> competitor2 = new Competitor<T>(competitor);
			Competitors.Add(competitor2);
			return competitor2;
		}

		public Competitor<T> GetCompetitor(T competitor)
		{
			if (competitor == null)
			{
				return null;
			}
			foreach (Competitor<T> competitor2 in Competitors)
			{
				if (competitor.Equals(competitor2.theObject))
				{
					return competitor2;
				}
			}
			return null;
		}

		public bool RemoveCompetitor(Competitor<T> competitor)
		{
			return RemoveCompetitor(competitor.Value);
		}

		public bool RemoveCompetitor(T competitor)
		{
			bool flag = Competitors.Count == 0;
			foreach (Competitor<T> competitor2 in Competitors)
			{
				if (competitor.Equals(competitor2.theObject))
				{
					Competitors.Remove(competitor2);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Decide();
			}
			return flag;
		}

		public CompetitionResults<T> Decide()
		{
			ResetScores();
			Rules.AssignPointsToCompetitors(Competitors);
			Competitors.Sort((Competitor<T> a, Competitor<T> b) => -a.CompareTo(b));
			CompetitionResults<T> competitionResults = new CompetitionResults<T>(Competitors);
			Results.Add(competitionResults);
			return competitionResults;
		}

		private void ResetScores()
		{
			Results.Clear();
			for (int i = 0; i < Competitors.Count; i++)
			{
				Competitors[i].Points = 0;
			}
		}
	}
}
