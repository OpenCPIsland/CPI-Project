using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class ChatService : BaseNetworkService, IChatService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CHAT_ACTIVITY_RECEIVED, onChatActivityReceived);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CHAT_ACTIVITY_CANCEL_RECEIVED, onChatActivityCancelReceived);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CHAT_MESSAGE_RECEIVED, onChatMessageReceived);
		}

		public void SendActivity()
		{
			clubPenguinClient.GameServer.SendChatActivity();
		}

		public void SendActivityCancel()
		{
			clubPenguinClient.GameServer.SendChatActivityCancel();
		}

		public void SendMessage(string message, int sizzleClipID, string questId = null, string objective = null)
		{
			ChatMessage chatMessage = default(ChatMessage);
			chatMessage.senderSessionId = clubPenguinClient.PlayerSessionId;
			chatMessage.message = message;
			chatMessage.emotion = sizzleClipID;
			chatMessage.questId = questId;
			chatMessage.objective = objective;
			APICall<VerifyChatMessageOperation> aPICall = clubPenguinClient.ChatApi.VerifyChatMessage(chatMessage);
			aPICall.OnResponse += delegate(VerifyChatMessageOperation op, HttpResponse httpResponse)
			{
				onChatMessageVerified(op.ResponseBody);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onChatMessageVerified(SignedResponse<ChatMessage> signedChatMessage)
		{
			if (signedChatMessage.Data.moderated)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ChatServiceEvents.ChatMessageBlockedReceived(signedChatMessage.Data.senderSessionId));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ChatServiceEvents.ChatMessageReceived(signedChatMessage.Data.senderSessionId, signedChatMessage.Data.message, signedChatMessage.Data.emotion));
			}
			clubPenguinClient.GameServer.SendChatMessage(signedChatMessage);
		}

		private void onChatActivityReceived(GameServerEvent evt, object data)
		{
			long sessionId = (long)data;
			Service.Get<EventDispatcher>().DispatchEvent(new ChatServiceEvents.ChatActivityReceived(sessionId));
		}

		private void onChatActivityCancelReceived(GameServerEvent evt, object data)
		{
			long sessionId = (long)data;
			Service.Get<EventDispatcher>().DispatchEvent(new ChatServiceEvents.ChatActivityCancelReceived(sessionId));
		}

		private void onChatMessageReceived(GameServerEvent evt, object data)
		{
			ReceivedChatMessage receivedChatMessage = (ReceivedChatMessage)data;
			Service.Get<EventDispatcher>().DispatchEvent(new ChatServiceEvents.ChatMessageReceived(receivedChatMessage.senderSessionId, receivedChatMessage.message, receivedChatMessage.emotion));
		}
	}
}
