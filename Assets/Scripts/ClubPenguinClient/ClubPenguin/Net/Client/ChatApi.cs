using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class ChatApi
	{
		private ClubPenguinClient clubPenguinClient;

		public ChatApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<VerifyChatMessageOperation> VerifyChatMessage(ChatMessage chatMessage)
		{
			VerifyChatMessageOperation operation = new VerifyChatMessageOperation(chatMessage);
			return new APICall<VerifyChatMessageOperation>(clubPenguinClient, operation);
		}
	}
}
