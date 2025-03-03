using ClubPenguin.Net.Client.Smartfox;
using Sfs2X.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class IslandTargetPlaygroundStatsMMOItem : CPMMOItem
	{
		private string path;

		private int currentWinStreak;

		private int bestWinStreak;

		public string GetPath()
		{
			return path;
		}

		private string getPath(IMMOItem sfsItem)
		{
			if (sfsItem.ContainsVariable(SocketItemVars.GAME_OBJECT_PATH.GetKey()))
			{
				return sfsItem.GetVariable(SocketItemVars.GAME_OBJECT_PATH.GetKey()).GetStringValue();
			}
			return string.Empty;
		}

		public int GetBestWinStreakToday()
		{
			return bestWinStreak;
		}

		private int getBestWinStreakToday(IMMOItem sfsItem)
		{
			if (sfsItem.ContainsVariable(SocketItemVars.ACTION_COUNT.GetKey()))
			{
				return sfsItem.GetVariable(SocketItemVars.ACTION_COUNT.GetKey()).GetIntValue();
			}
			return 0;
		}

		public int GetCurrentWinStreakToday()
		{
			return currentWinStreak;
		}

		private int getCurrentWinStreakToday(IMMOItem sfsItem)
		{
			if (sfsItem.ContainsVariable(SocketItemVars.INTEGER_A.GetKey()))
			{
				return sfsItem.GetVariable(SocketItemVars.INTEGER_A.GetKey()).GetIntValue();
			}
			return 0;
		}

		public IslandTargetPlaygroundStatsMMOItem(IMMOItem sfsItem)
		{
			path = getPath(sfsItem);
			bestWinStreak = getBestWinStreakToday(sfsItem);
			currentWinStreak = getCurrentWinStreakToday(sfsItem);
		}

		public override string ToString()
		{
			return string.Format("[IslandTargetPlaygroundStatsMMOItem: {0}, Current Win Streak: {1}, Best Win Streak: {2}", GetPath(), GetCurrentWinStreakToday(), GetBestWinStreakToday());
		}
	}
}
