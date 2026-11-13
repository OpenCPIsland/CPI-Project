using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpDELETE]
	[HttpPath("cp-api-base-uri", "/quest/v1/qa/locks/{$questId}")]
	public class QAUnlockQuestOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("questId")]
		public string QuestId;

		[HttpResponseJsonBody]
		public SignedResponse<QuestStateCollection> ResponseBody;

		public QAUnlockQuestOperation(string questId)
		{
			QuestId = questId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new SignedResponse<QuestStateCollection>
			{
				Data = SetProgressOperation.GetQuestStateCollection(offlineDatabase.Read<QuestStates>(), offlineDefinitions, false)
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SetProgressOperation.SetOfflineQuestStateCollection(offlineDatabase, ResponseBody.Data);
		}
	}
}
