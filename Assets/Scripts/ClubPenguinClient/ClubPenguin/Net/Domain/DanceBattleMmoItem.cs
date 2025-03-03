using ClubPenguin.Net.Client.Smartfox;
using Sfs2X.Entities;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class DanceBattleMmoItem : CPMMOItem
	{
		private string scores;

		private string turnData;

		private string turnOutcomeDanceMoveData;

		public DanceBattleMmoItem(IMMOItem sfsItem)
		{
			scores = getScores(sfsItem);
			turnData = getTurnData(sfsItem);
			turnOutcomeDanceMoveData = getTurnOutcomeDanceMoveData(sfsItem);
		}

		public string getScores()
		{
			return scores;
		}

		private string getScores(IMMOItem sfsItem)
		{
			string result = string.Empty;
			if (sfsItem.ContainsVariable(SocketItemVars.SCORE_DATA.GetKey()))
			{
				result = sfsItem.GetVariable(SocketItemVars.SCORE_DATA.GetKey()).GetStringValue();
			}
			return result;
		}

		public string getTurnData()
		{
			return turnData;
		}

		private string getTurnData(IMMOItem sfsItem)
		{
			string result = string.Empty;
			if (sfsItem.ContainsVariable(SocketItemVars.TURN_DATA.GetKey()))
			{
				result = sfsItem.GetVariable(SocketItemVars.TURN_DATA.GetKey()).GetStringValue();
			}
			return result;
		}

		public string getTurnOutcomeDanceMoveData()
		{
			return turnOutcomeDanceMoveData;
		}

		private string getTurnOutcomeDanceMoveData(IMMOItem sfsItem)
		{
			string result = string.Empty;
			if (sfsItem.ContainsVariable(SocketItemVars.DANCE_MOVE_DATA.GetKey()))
			{
				result = sfsItem.GetVariable(SocketItemVars.DANCE_MOVE_DATA.GetKey()).GetStringValue();
			}
			return result;
		}
	}
}
