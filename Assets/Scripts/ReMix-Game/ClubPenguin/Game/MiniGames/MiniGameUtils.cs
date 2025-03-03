using ClubPenguin.Analytics;
using Disney.MobileNetwork;

namespace ClubPenguin.Game.MiniGames
{
	public static class MiniGameUtils
	{
		public static void LogGameStartBi(string miniGameName)
		{
			Service.Get<ICPSwrveService>().Action("game.minigame", miniGameName);
		}

		public static void StartBiTimer(string miniGameName)
		{
			string timerID = string.Format("{0}_time", miniGameName);
			Service.Get<ICPSwrveService>().StartTimer(timerID, "minigame", null, miniGameName);
		}

		public static void StopBiTimer(string miniGameName)
		{
			string timerID = string.Format("{0}_time", miniGameName);
			Service.Get<ICPSwrveService>().EndTimer(timerID);
		}
	}
}
