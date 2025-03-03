using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class ChatMessageSender : MonoBehaviour
	{
		public struct SendChatMessage
		{
			public readonly string Message;

			public readonly SizzleClipDefinition SizzleClip;

			public readonly bool IsChatPhrase;

			public readonly ChatServiceEvents.ChatMessageQuestObjective Quest;

			public SendChatMessage(string message, SizzleClipDefinition sizzleClip, bool isChatPhrase = false)
			{
				Message = message;
				SizzleClip = sizzleClip;
				IsChatPhrase = isChatPhrase;
				Quest = new ChatServiceEvents.ChatMessageQuestObjective();
			}
		}

		private EventChannel eventChannel;

		private EventDispatcher eventDispatcher;

		private void OnEnable()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			eventChannel.AddListener<ChatServiceEvents.SendChatActivity>(onSendChatActivity);
			eventChannel.AddListener<ChatServiceEvents.SendChatActivityCancel>(onSendChatActivityCancel);
			eventChannel.AddListener<SendChatMessage>(onSendChatMessage, EventDispatcher.Priority.LAST);
		}

		private void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onSendChatActivity(ChatServiceEvents.SendChatActivity evt)
		{
			Service.Get<INetworkServicesManager>().ChatService.SendActivity();
			return false;
		}

		private bool onSendChatActivityCancel(ChatServiceEvents.SendChatActivityCancel evt)
		{
			Service.Get<INetworkServicesManager>().ChatService.SendActivityCancel();
			return false;
		}

		private bool onSendChatMessage(SendChatMessage evt)
		{
			Service.Get<INetworkServicesManager>().ChatService.SendMessage(evt.Message, (!(evt.SizzleClip == null)) ? evt.SizzleClip.Id : 0, evt.Quest.QuestId, evt.Quest.Objective);
			return false;
		}
	}
}
