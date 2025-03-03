using ClubPenguin.Net.Domain;
using ClubPenguin.PartyGames;
using System.Collections.Generic;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameTeamEndGamePopupData
	{
		public enum LocalPlayerResult
		{
			Lose,
			Win,
			Tie
		}

		public class PartyGameTeamEndGamePopupTeamData
		{
			public readonly float Score;

			public readonly string TeamNameToken;

			public readonly bool IsWinningTeam;

			public readonly bool IsLocalPlayersTeam;

			public readonly int TeamThemeId;

			public readonly bool IsShowingScore;

			public PartyGameTeamEndGamePopupTeamData(float score, string teamNameToken, bool isWinningTeam, bool isLocalPlayersTeam, int teamThemeId, bool isShowingScore)
			{
				Score = score;
				TeamNameToken = teamNameToken;
				IsWinningTeam = isWinningTeam;
				IsLocalPlayersTeam = isLocalPlayersTeam;
				TeamThemeId = teamThemeId;
				IsShowingScore = isShowingScore;
			}
		}

		public readonly List<PartyGameTeamEndGamePopupTeamData> TeamData;

		public readonly int ThemeId;

		public readonly Reward LocalPlayerReward;

		public readonly int GameSessionId;

		public readonly PartyGameEndPlacement LocalPlayerPlacement;

		public PartyGameTeamEndGamePopupData(List<PartyGameTeamEndGamePopupTeamData> teamData, int themeId, PartyGameEndPlacement localPlayerPlacement, Reward localPlayerReward, int gameSessionId)
		{
			TeamData = teamData;
			ThemeId = themeId;
			LocalPlayerPlacement = localPlayerPlacement;
			LocalPlayerReward = localPlayerReward;
			GameSessionId = gameSessionId;
		}

		public LocalPlayerResult getLocalPlayerResult()
		{
			LocalPlayerResult result = LocalPlayerResult.Lose;
			if (LocalPlayerPlacement == PartyGameEndPlacement.FIRST)
			{
				result = LocalPlayerResult.Win;
			}
			else
			{
				float num = float.MinValue;
				foreach (PartyGameTeamEndGamePopupTeamData teamDatum in TeamData)
				{
					if (teamDatum.IsLocalPlayersTeam)
					{
						if (teamDatum.Score >= num)
						{
							result = LocalPlayerResult.Tie;
							num = teamDatum.Score;
						}
					}
					else if (teamDatum.Score > num)
					{
						result = LocalPlayerResult.Lose;
						num = teamDatum.Score;
					}
				}
			}
			return result;
		}
	}
}
