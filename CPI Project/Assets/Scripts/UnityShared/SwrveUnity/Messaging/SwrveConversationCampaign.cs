using System;
using System.Collections.Generic;

namespace SwrveUnity.Messaging
{
	public class SwrveConversationCampaign : SwrveBaseCampaign
	{
		public SwrveConversation Conversation;

		private SwrveConversationCampaign(DateTime initialisedTime)
			: base(initialisedTime)
		{
		}

		public SwrveConversation GetConversationForEvent(string triggerEvent, IDictionary<string, string> payload, SwrveQAUser qaUser)
		{
			if (null == Conversation)
			{
				LogAndAddReason("No conversation in campaign " + Id, qaUser);
				return null;
			}
			if (CheckCampaignLimits(triggerEvent, payload, qaUser))
			{
				SwrveLog.Log(string.Format("[{0}] {1} matches a trigger in {2}", this, triggerEvent, Id));
				if (AreAssetsReady())
				{
					return Conversation;
				}
				LogAndAddReason("Assets not downloaded to show conversation in campaign " + Id, qaUser);
			}
			return null;
		}

		public override bool AreAssetsReady()
		{
			return Conversation.AreAssetsReady();
		}

		public override bool SupportsOrientation(SwrveOrientation orientation)
		{
			return true;
		}

		public static SwrveConversationCampaign LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, Dictionary<string, object> campaignData, int campaignId, DateTime initialisedTime)
		{
			SwrveConversationCampaign swrveConversationCampaign = new SwrveConversationCampaign(initialisedTime);
			swrveConversationCampaign.Conversation = SwrveConversation.LoadFromJSON(swrveAssetsManager, swrveConversationCampaign, (Dictionary<string, object>)campaignData["conversation"]);
			return swrveConversationCampaign;
		}
	}
}
