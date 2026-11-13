using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpContentType("text/plain")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/quest/v1/qa/{$questId}")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class QASetStatusOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("questId")]
		public string QuestId;

		[HttpRequestTextBody]
		public string RequestBody;

		[HttpResponseJsonBody]
		public QuestChangeResponse ResponseBody;

		public QASetStatusOperation(string questId, QuestStatus status)
		{
			QuestId = questId;
			RequestBody = Convert.ToInt32(status).ToString();
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QuestStatus questStatus = (QuestStatus)Enum.Parse(typeof(QuestStatus), RequestBody);
			if (questStatus != 0 && questStatus != QuestStatus.LOCKED)
			{
				ResponseBody = SetStatusOperation.SetStatus(questStatus, QuestId, offlineDatabase, offlineDefinitions);
			}
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
