using ClubPenguin.Net.Client.Attributes;
using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[Encrypted]
	[HttpPath("cp-api-base-uri", "/chat/v1/message/verify")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpAccept("application/json")]
	[HttpContentType("application/json")]
	public class VerifyChatMessageOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public ChatMessage RequestBody;

		[HttpResponseJsonBody]
		public SignedResponse<ChatMessage> ResponseBody;

		public VerifyChatMessageOperation(ChatMessage chatMessage)
		{
			RequestBody = chatMessage;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new SignedResponse<ChatMessage>
			{
				Data = RequestBody
			};
		}
	}
}
