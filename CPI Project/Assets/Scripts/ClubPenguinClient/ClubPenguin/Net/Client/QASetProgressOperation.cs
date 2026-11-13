using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/quest/v1/qa/progress")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	public class QASetProgressOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public QuestObjectives RequestBody;

		[HttpResponseJsonBody]
		public QuestChangeResponse ResponseBody;

		public QASetProgressOperation(QuestObjectives objectives)
		{
			RequestBody = objectives;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = SetProgressOperation.SetProgress(RequestBody, offlineDatabase, offlineDefinitions);
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QuestChangeResponse responseBody = new QuestChangeResponse();
			if (ResponseBody.reward != null)
			{
				offlineDefinitions.AddReward(ResponseBody.reward.ToReward(), responseBody);
			}
			SetProgressOperation.SetOfflineQuestStateCollection(offlineDatabase, ResponseBody.questStateCollection.Data);
		}
	}
}
