using SwrveUnity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public class SwrveMessagesCampaign : SwrveBaseCampaign
	{
		public List<SwrveMessage> Messages;

		private SwrveMessagesCampaign(DateTime initialisedTime)
			: base(initialisedTime)
		{
			Messages = new List<SwrveMessage>();
		}

		public SwrveMessage GetMessageForEvent(string triggerEvent, IDictionary<string, string> payload, SwrveQAUser qaUser)
		{
			int count = Messages.Count;
			if (count == 0)
			{
				LogAndAddReason("No messages in campaign " + Id, qaUser);
				return null;
			}
			if (CheckCampaignLimits(triggerEvent, payload, qaUser))
			{
				SwrveLog.Log(string.Format("[{0}] {1} matches a trigger in {2}", this, triggerEvent, Id));
				return GetNextMessage(count, qaUser);
			}
			return null;
		}

		public SwrveMessage GetMessageForId(int id)
		{
			for (int i = 0; i < Messages.Count; i++)
			{
				SwrveMessage swrveMessage = Messages[i];
				if (swrveMessage.Id == id)
				{
					return swrveMessage;
				}
			}
			return null;
		}

		protected SwrveMessage GetNextMessage(int messagesCount, SwrveQAUser qaUser)
		{
			if (RandomOrder)
			{
				List<SwrveMessage> list = new List<SwrveMessage>(Messages);
				list.Shuffle();
				for (int i = 0; i < list.Count; i++)
				{
					SwrveMessage swrveMessage = list[i];
					if (swrveMessage.IsDownloaded())
					{
						return swrveMessage;
					}
				}
			}
			else if (base.Next < messagesCount)
			{
				SwrveMessage swrveMessage = Messages[base.Next];
				if (swrveMessage.IsDownloaded())
				{
					return swrveMessage;
				}
			}
			LogAndAddReason("Campaign " + Id + " hasn't finished downloading.", qaUser);
			return null;
		}

		protected void AddMessage(SwrveMessage message)
		{
			Messages.Add(message);
		}

		public override bool AreAssetsReady()
		{
			return Messages.All((SwrveMessage m) => m.IsDownloaded());
		}

		public override bool SupportsOrientation(SwrveOrientation orientation)
		{
			return Messages.Any((SwrveMessage m) => m.SupportsOrientation(orientation));
		}

		public HashSet<SwrveAssetsQueueItem> GetImageAssets()
		{
			HashSet<SwrveAssetsQueueItem> hashSet = new HashSet<SwrveAssetsQueueItem>();
			for (int i = 0; i < Messages.Count; i++)
			{
				SwrveMessage swrveMessage = Messages[i];
				hashSet.UnionWith(swrveMessage.SetOfAssets());
			}
			return hashSet;
		}

		public void MessageWasShownToUser(SwrveMessageFormat messageFormat)
		{
			WasShownToUser();
			if (Messages.Count > 0)
			{
				if (!RandomOrder)
				{
					int num2 = base.Next = (base.Next + 1) % Messages.Count;
					SwrveLog.Log("Round Robin: Next message in campaign " + Id + " is " + num2);
				}
				else
				{
					SwrveLog.Log("Next message in campaign " + Id + " is random");
				}
			}
		}

		public static SwrveMessagesCampaign LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, Dictionary<string, object> campaignData, int id, DateTime initialisedTime, SwrveQAUser qaUser, Color? defaultBackgroundColor)
		{
			SwrveMessagesCampaign swrveMessagesCampaign = new SwrveMessagesCampaign(initialisedTime);
			object value = null;
			campaignData.TryGetValue("messages", out value);
			IList<object> list = null;
			try
			{
				list = (IList<object>)value;
			}
			catch (Exception ex)
			{
				swrveMessagesCampaign.LogAndAddReason("Campaign [" + id + "] invalid messages found, skipping.  Error: " + ex, qaUser);
			}
			if (list == null)
			{
				swrveMessagesCampaign.LogAndAddReason("Campaign [" + id + "] JSON messages are null, skipping.", qaUser);
				return null;
			}
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				Dictionary<string, object> messageData = (Dictionary<string, object>)list[i];
				SwrveMessage swrveMessage = SwrveMessage.LoadFromJSON(swrveAssetsManager, swrveMessagesCampaign, messageData, defaultBackgroundColor);
				if (swrveMessage.Formats.Count > 0)
				{
					swrveMessagesCampaign.AddMessage(swrveMessage);
				}
			}
			if (swrveMessagesCampaign.Messages.Count == 0)
			{
				swrveMessagesCampaign.LogAndAddReason("Campaign [" + id + "] no messages found, skipping.", qaUser);
			}
			return swrveMessagesCampaign;
		}
	}
}
