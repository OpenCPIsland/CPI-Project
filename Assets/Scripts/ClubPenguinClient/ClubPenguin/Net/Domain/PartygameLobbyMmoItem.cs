using ClubPenguin.Net.Client.Smartfox;
using Sfs2X.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class PartygameLobbyMmoItem : CPMMOItem
	{
		public string playerData;

		public long timeStartedInSeconds;

		public long timeToLive;

		public int gameTemplateId;

		public PartygameLobbyMmoItem()
		{
		}

		public PartygameLobbyMmoItem(IMMOItem sfsItem)
		{
			playerData = getPlayerData(sfsItem);
			timeStartedInSeconds = getTimeStartedInSecondsSinceEpoc(sfsItem);
			timeToLive = getTimeToLiveInSeconds(sfsItem);
			gameTemplateId = getGameTemplateId(sfsItem);
		}

		public string GetPlayerData()
		{
			return playerData;
		}

		private string getPlayerData(IMMOItem sfsItem)
		{
			string result = string.Empty;
			if (sfsItem.ContainsVariable(SocketItemVars.SCORE_DATA.GetKey()))
			{
				result = sfsItem.GetVariable(SocketItemVars.SCORE_DATA.GetKey()).GetStringValue();
			}
			return result;
		}

		public long GetTimeStartedInSecondsSinceEpoc()
		{
			return timeStartedInSeconds;
		}

		private long getTimeStartedInSecondsSinceEpoc(IMMOItem sfsItem)
		{
			long result = 0L;
			if (sfsItem.ContainsVariable(SocketItemVars.STATE_TIMESTAMP.GetKey()))
			{
				result = long.Parse(sfsItem.GetVariable(SocketItemVars.STATE_TIMESTAMP.GetKey()).GetStringValue());
			}
			return result;
		}

		public long GetTimeToLiveInSeconds()
		{
			return timeToLive;
		}

		private long getTimeToLiveInSeconds(IMMOItem sfsItem)
		{
			long result = 0L;
			if (sfsItem.ContainsVariable(SocketItemVars.TIME_TO_LIVE.GetKey()))
			{
				result = sfsItem.GetVariable(SocketItemVars.TIME_TO_LIVE.GetKey()).GetIntValue();
			}
			return result;
		}

		public int GetGameTemplateId()
		{
			return gameTemplateId;
		}

		private int getGameTemplateId(IMMOItem sfsItem)
		{
			int result = 0;
			if (sfsItem.ContainsVariable(SocketItemVars.INTEGER_A.GetKey()))
			{
				result = sfsItem.GetVariable(SocketItemVars.INTEGER_A.GetKey()).GetIntValue();
			}
			return result;
		}
	}
}
