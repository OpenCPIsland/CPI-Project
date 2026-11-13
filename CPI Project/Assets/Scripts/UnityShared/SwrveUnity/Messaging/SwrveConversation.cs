using SwrveUnity.Helpers;
using SwrveUnityMiniJSON;
using System.Collections.Generic;
using System.Linq;

namespace SwrveUnity.Messaging
{
	public class SwrveConversation : SwrveBaseMessage
	{
		public string Conversation;

		public ISwrveAssetsManager SwrveAssetsManager;

		public int Priority = 9999;

		public HashSet<SwrveAssetsQueueItem> ConversationAssets
		{
			get;
			set;
		}

		private SwrveConversation(ISwrveAssetsManager swrveAssetsManager, SwrveConversationCampaign campaign)
		{
			SwrveAssetsManager = swrveAssetsManager;
			Campaign = campaign;
			ConversationAssets = new HashSet<SwrveAssetsQueueItem>();
		}

		public static SwrveConversation LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, SwrveConversationCampaign campaign, Dictionary<string, object> conversationData)
		{
			SwrveConversation swrveConversation = new SwrveConversation(swrveAssetsManager, campaign);
			swrveConversation.Id = MiniJsonHelper.GetInt(conversationData, "id");
			List<object> list = (List<object>)conversationData["pages"];
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)list[i];
				List<object> list2 = (List<object>)dictionary["content"];
				for (int j = 0; j < list2.Count; j++)
				{
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)list2[j];
					switch ((string)dictionary2["type"])
					{
					case "image":
						swrveConversation.queueImageAsset(dictionary2);
						break;
					case "html-fragment":
					case "star-rating":
						swrveConversation.queueFontAsset(dictionary2);
						break;
					case "multi-value-input":
					{
						swrveConversation.queueFontAsset(dictionary2);
						List<object> list3 = (List<object>)dictionary2["values"];
						for (int k = 0; k < list3.Count; k++)
						{
							Dictionary<string, object> content = (Dictionary<string, object>)list3[k];
							swrveConversation.queueFontAsset(content);
						}
						break;
					}
					}
				}
				List<object> list4 = (List<object>)dictionary["controls"];
				for (int j = 0; j < list4.Count; j++)
				{
					Dictionary<string, object> content2 = (Dictionary<string, object>)list4[j];
					swrveConversation.queueFontAsset(content2);
				}
			}
			swrveConversation.Conversation = Json.Serialize(conversationData);
			if (conversationData.ContainsKey("priority"))
			{
				swrveConversation.Priority = MiniJsonHelper.GetInt(conversationData, "priority");
			}
			return swrveConversation;
		}

		private void queueImageAsset(Dictionary<string, object> content)
		{
			string text = (string)content["value"];
			ConversationAssets.Add(new SwrveAssetsQueueItem(text, text, true));
		}

		private void queueFontAsset(Dictionary<string, object> content)
		{
			if (!content.ContainsKey("style"))
			{
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)content["style"];
			if (dictionary.ContainsKey("font_file") && dictionary.ContainsKey("font_digest"))
			{
				string text = (string)dictionary["font_file"];
				string text2 = (string)dictionary["font_digest"];
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && !text.Equals("_system_font_"))
				{
					ConversationAssets.Add(new SwrveAssetsQueueItem(text, text2, false));
				}
			}
		}

		public bool AreAssetsReady()
		{
			return ConversationAssets.All((SwrveAssetsQueueItem asset) => SwrveAssetsManager.AssetsOnDisk.Contains(asset.Name));
		}

		public override string GetBaseFormattedMessageType()
		{
			return "Conversation";
		}
	}
}
