using SwrveUnity.Helpers;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public abstract class SwrveBaseCampaign
	{
		private const string ID_KEY = "id";

		private const string CONVERSATION_KEY = "conversation";

		private const string MESSAGES_KEY = "messages";

		private const string SUBJECT_KEY = "subject";

		private const string MESSAGE_CENTER_KEY = "message_center";

		private const string TRIGGERS_KEY = "triggers";

		private const string EVENT_NAME_KEY = "event_name";

		private const string CONDITIONS_KEY = "conditions";

		private const string DISPLAY_ORDER_KEY = "display_order";

		private const string RULES_KEY = "rules";

		private const string RANDOM_KEY = "random";

		private const string DISMISS_AFTER_VIEWS_KEY = "dismiss_after_views";

		private const string DELAY_FIRST_MESSAGE_KEY = "delay_first_message";

		private const string MIN_DELAY_BETWEEN_MESSAGES_KEY = "min_delay_between_messages";

		private const string START_DATE_KEY = "start_date";

		private const string END_DATE_KEY = "end_date";

		protected const string WaitTimeFormat = "HH\\:mm\\:ss zzz";

		protected const int DefaultDelayFirstMessage = 180;

		protected const long DefaultMaxShows = 99999L;

		protected const int DefaultMinDelay = 60;

		protected readonly System.Random rnd = new System.Random();

		public int Id;

		protected string subject;

		protected List<SwrveTrigger> triggers;

		public DateTime StartDate;

		public DateTime EndDate;

		public bool RandomOrder = false;

		public SwrveCampaignState State;

		protected readonly DateTime swrveInitialisedTime;

		protected DateTime showMessagesAfterLaunch;

		protected int minDelayBetweenMessage;

		protected int delayFirstMessage = 180;

		protected int maxImpressions;

		public bool MessageCenter
		{
			get;
			protected set;
		}

		public int Impressions
		{
			get
			{
				return State.Impressions;
			}
			set
			{
				State.Impressions = value;
			}
		}

		public int Next
		{
			get
			{
				return State.Next;
			}
			set
			{
				State.Next = value;
			}
		}

		public SwrveCampaignState.Status Status
		{
			get
			{
				return State.CurStatus;
			}
			set
			{
				State.CurStatus = value;
			}
		}

		public string Subject
		{
			get
			{
				return subject;
			}
			protected set
			{
				subject = value;
			}
		}

		protected DateTime showMessagesAfterDelay
		{
			get
			{
				return State.ShowMessagesAfterDelay;
			}
			set
			{
				State.ShowMessagesAfterDelay = value;
			}
		}

		protected SwrveBaseCampaign(DateTime initialisedTime)
		{
			State = new SwrveCampaignState();
			swrveInitialisedTime = initialisedTime;
			triggers = new List<SwrveTrigger>();
			minDelayBetweenMessage = 60;
			showMessagesAfterLaunch = swrveInitialisedTime + TimeSpan.FromSeconds(180.0);
		}

		public bool CheckCampaignLimits(string triggerEvent, IDictionary<string, string> payload, SwrveQAUser qaUser)
		{
			DateTime now = SwrveHelper.GetNow();
			if (!CanTrigger(triggerEvent, payload, qaUser))
			{
				LogAndAddReason("There is no trigger in " + Id + " that matches " + triggerEvent, qaUser);
				return false;
			}
			if (!IsActive(qaUser))
			{
				return false;
			}
			if (!CheckImpressions(qaUser))
			{
				return false;
			}
			if (!string.Equals(triggerEvent, "Swrve.Messages.showAtSessionStart", StringComparison.OrdinalIgnoreCase) && IsTooSoonToShowMessageAfterLaunch(now))
			{
				LogAndAddReason("{Campaign throttle limit} Too soon after launch. Wait until " + showMessagesAfterLaunch.ToString("HH\\:mm\\:ss zzz"), qaUser);
				return false;
			}
			if (IsTooSoonToShowMessageAfterDelay(now))
			{
				LogAndAddReason("{Campaign throttle limit} Too soon after last message. Wait until " + showMessagesAfterDelay.ToString("HH\\:mm\\:ss zzz"), qaUser);
				return false;
			}
			return true;
		}

		public bool CheckImpressions(SwrveQAUser qaUser)
		{
			if (Impressions >= maxImpressions)
			{
				LogAndAddReason("{Campaign throttle limit} Campaign " + Id + " has been shown " + maxImpressions + " times already", qaUser);
				return false;
			}
			return true;
		}

		public bool IsActive(SwrveQAUser qaUser)
		{
			DateTime utcNow = SwrveHelper.GetUtcNow();
			if (StartDate > utcNow)
			{
				LogAndAddReason(string.Format("Campaign {0} not started yet (now: {1}, end: {2})", Id, utcNow, StartDate), qaUser);
				return false;
			}
			if (EndDate < utcNow)
			{
				LogAndAddReason(string.Format("Campaign {0} has finished (now: {1}, end: {2})", Id, utcNow, EndDate), qaUser);
				return false;
			}
			return true;
		}

		protected void LogAndAddReason(string reason, SwrveQAUser qaUser)
		{
			if (qaUser != null && !qaUser.campaignReasons.ContainsKey(Id))
			{
				qaUser.campaignReasons.Add(Id, reason);
			}
			SwrveLog.Log(string.Format("{0} {1}", this, reason));
		}

		protected void LogAndAddReason(int ident, string reason, SwrveQAUser qaUser)
		{
			LogAndAddReason(reason, qaUser);
		}

		public List<SwrveTrigger> GetTriggers()
		{
			return triggers;
		}

		public static SwrveBaseCampaign LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, Dictionary<string, object> campaignData, DateTime initialisedTime, SwrveQAUser qaUser, Color? defaultBackgroundColor)
		{
			int @int = MiniJsonHelper.GetInt(campaignData, "id");
			SwrveBaseCampaign swrveBaseCampaign = null;
			if (campaignData.ContainsKey("conversation"))
			{
				swrveBaseCampaign = SwrveConversationCampaign.LoadFromJSON(swrveAssetsManager, campaignData, @int, initialisedTime);
			}
			else if (campaignData.ContainsKey("messages"))
			{
				swrveBaseCampaign = SwrveMessagesCampaign.LoadFromJSON(swrveAssetsManager, campaignData, @int, initialisedTime, qaUser, defaultBackgroundColor);
			}
			if (swrveBaseCampaign == null)
			{
				return null;
			}
			swrveBaseCampaign.Id = @int;
			AssignCampaignTriggers(swrveBaseCampaign, campaignData);
			swrveBaseCampaign.MessageCenter = (campaignData.ContainsKey("message_center") && (bool)campaignData["message_center"]);
			if (!swrveBaseCampaign.MessageCenter && swrveBaseCampaign.GetTriggers().Count == 0)
			{
				swrveBaseCampaign.LogAndAddReason("Campaign [" + swrveBaseCampaign.Id + "], has no triggers. Skipping this campaign.", qaUser);
				return null;
			}
			AssignCampaignRules(swrveBaseCampaign, campaignData);
			AssignCampaignDates(swrveBaseCampaign, campaignData);
			swrveBaseCampaign.Subject = (campaignData.ContainsKey("subject") ? ((string)campaignData["subject"]) : "");
			if (swrveBaseCampaign.MessageCenter)
			{
				SwrveLog.Log(string.Format("message center campaign: {0}, {1}", swrveBaseCampaign.GetType(), swrveBaseCampaign.subject));
			}
			return swrveBaseCampaign;
		}

		public abstract bool AreAssetsReady();

		public abstract bool SupportsOrientation(SwrveOrientation orientation);

		protected static void AssignCampaignTriggers(SwrveBaseCampaign campaign, Dictionary<string, object> campaignData)
		{
			IList<object> list = (IList<object>)campaignData["triggers"];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				object obj = list[i];
				if (obj.GetType() == typeof(string))
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary.Add("event_name", obj);
					dictionary.Add("conditions", new Dictionary<string, object>());
					obj = dictionary;
				}
				try
				{
					SwrveTrigger item = SwrveTrigger.LoadFromJson((IDictionary<string, object>)obj);
					campaign.GetTriggers().Add(item);
				}
				catch (Exception ex)
				{
					SwrveLog.LogError("Unable to parse SwrveTrigger from json " + Json.Serialize(obj) + ", " + ex);
				}
			}
		}

		protected static void AssignCampaignRules(SwrveBaseCampaign campaign, Dictionary<string, object> campaignData)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)campaignData["rules"];
			campaign.RandomOrder = ((string)dictionary["display_order"]).Equals("random");
			if (dictionary.ContainsKey("dismiss_after_views"))
			{
				int num = campaign.maxImpressions = MiniJsonHelper.GetInt(dictionary, "dismiss_after_views");
			}
			if (dictionary.ContainsKey("delay_first_message"))
			{
				campaign.delayFirstMessage = MiniJsonHelper.GetInt(dictionary, "delay_first_message");
				campaign.showMessagesAfterLaunch = campaign.swrveInitialisedTime + TimeSpan.FromSeconds(campaign.delayFirstMessage);
			}
			if (dictionary.ContainsKey("min_delay_between_messages"))
			{
				int num2 = campaign.minDelayBetweenMessage = MiniJsonHelper.GetInt(dictionary, "min_delay_between_messages");
			}
		}

		protected static void AssignCampaignDates(SwrveBaseCampaign campaign, Dictionary<string, object> campaignData)
		{
			DateTime unixEpoch = SwrveHelper.UnixEpoch;
			campaign.StartDate = unixEpoch.AddMilliseconds(MiniJsonHelper.GetLong(campaignData, "start_date"));
			campaign.EndDate = unixEpoch.AddMilliseconds(MiniJsonHelper.GetLong(campaignData, "end_date"));
		}

		public void IncrementImpressions()
		{
			Impressions++;
		}

		protected bool IsTooSoonToShowMessageAfterLaunch(DateTime now)
		{
			return now < showMessagesAfterLaunch;
		}

		protected bool IsTooSoonToShowMessageAfterDelay(DateTime now)
		{
			return now < showMessagesAfterDelay;
		}

		protected void SetMessageMinDelayThrottle()
		{
			showMessagesAfterDelay = SwrveHelper.GetNow() + TimeSpan.FromSeconds(minDelayBetweenMessage);
		}

		public void WasShownToUser()
		{
			Status = SwrveCampaignState.Status.Seen;
			IncrementImpressions();
			SetMessageMinDelayThrottle();
		}

		public void MessageDismissed()
		{
			SetMessageMinDelayThrottle();
		}

		public bool IsA<T>() where T : SwrveBaseCampaign
		{
			return GetType() == typeof(T);
		}

		public bool CanTrigger(string eventName, IDictionary<string, string> payload = null, SwrveQAUser qaUser = null)
		{
			return GetTriggers().Any((SwrveTrigger trig) => trig.CanTrigger(eventName, payload));
		}
	}
}
