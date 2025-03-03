using SwrveUnity.Helpers;
using System;
using System.Collections.Generic;

namespace SwrveUnity.Messaging
{
	public class SwrveCampaignState
	{
		public enum Status
		{
			Unseen,
			Seen,
			Deleted
		}

		private const string SEEN_KEY = "seen";

		private const string DELETED_KEY = "deleted";

		public int Impressions;

		public int Next;

		public DateTime ShowMessagesAfterDelay;

		public Status CurStatus;

		public SwrveCampaignState()
		{
			ShowMessagesAfterDelay = SwrveHelper.GetNow();
		}

		public SwrveCampaignState(int campaignId, Dictionary<string, object> savedStatesJson)
		{
			string key = "Next" + campaignId;
			if (savedStatesJson.ContainsKey(key))
			{
				Next = MiniJsonHelper.GetInt(savedStatesJson, key);
			}
			key = "Impressions" + campaignId;
			if (savedStatesJson.ContainsKey(key))
			{
				Impressions = MiniJsonHelper.GetInt(savedStatesJson, key);
			}
			key = "Status" + campaignId;
			if (savedStatesJson.ContainsKey(key))
			{
				CurStatus = ParseStatus(MiniJsonHelper.GetString(savedStatesJson, key));
			}
			else
			{
				CurStatus = Status.Unseen;
			}
		}

		public static Status ParseStatus(string status)
		{
			if (status.ToLower().Equals("seen"))
			{
				return Status.Seen;
			}
			if (status.ToLower().Equals("deleted"))
			{
				return Status.Deleted;
			}
			return Status.Unseen;
		}

		public override string ToString()
		{
			return string.Format("[SwrveCampaignState] Impressions: {0}, Next: {1}, ShowMessagesAfterDelay: {2}, CurStatus: {3}", Impressions, Next, ShowMessagesAfterDelay, CurStatus);
		}
	}
}
