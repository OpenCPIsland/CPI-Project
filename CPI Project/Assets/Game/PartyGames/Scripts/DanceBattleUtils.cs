using ClubPenguin.Analytics;
using Disney.MobileNetwork;

namespace ClubPenguin.Game.PartyGames
{
	public static class DanceBattleUtils
	{
		public static void LogPlayerJoinDanceBI(int pNumPlayers)
		{
			Service.Get<ICPSwrveService>().Action("dance_battle", "join", pNumPlayers.ToString());
		}

		public static void LogDanceStartBI(int pNumPlayers)
		{
			Service.Get<ICPSwrveService>().Action("dance_battle", "start", pNumPlayers.ToString());
		}

		public static void LogRoundEndBI(int roundNumber, float points, int teamId)
		{
			Service.Get<ICPSwrveService>().Action("dance_battle", "round_complete", roundNumber.ToString(), points.ToString(), teamId.ToString());
		}

		public static void LogDanceFullBI()
		{
			Service.Get<ICPSwrveService>().Action("dance_battle", "game full");
		}

		public static void LogNotEnoughPlayersBI()
		{
			Service.Get<ICPSwrveService>().Action("dance_battle", "not enough players");
		}

		public static void LogGameEndBI(int teamIdWinner)
		{
			Service.Get<ICPSwrveService>().Action("dance_battle", "game_over", teamIdWinner.ToString());
		}
	}
}
