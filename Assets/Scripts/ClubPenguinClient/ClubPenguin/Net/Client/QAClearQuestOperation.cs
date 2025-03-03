using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpDELETE]
	[HttpAccept("application/json")]
	[HttpPath("cp-api-base-uri", "/quest/v1/qa/{$questId}")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class QAClearQuestOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("questId")]
		public string QuestId;

		[HttpResponseJsonBody]
		public SignedResponse<QuestStateCollection> ResponseBody;

		public QAClearQuestOperation(string questId)
		{
			QuestId = questId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QuestStates questStates = offlineDatabase.Read<QuestStates>();
			for (int i = 0; i < questStates.Quests.Count; i++)
			{
				if (questStates.Quests[i].questId == QuestId)
				{
					questStates.Quests.RemoveAt(i);
					break;
				}
			}
			offlineDatabase.Write(questStates);
			ResponseBody = new SignedResponse<QuestStateCollection>
			{
				Data = SetProgressOperation.GetQuestStateCollection(questStates, offlineDefinitions, false)
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SetProgressOperation.SetOfflineQuestStateCollection(offlineDatabase, ResponseBody.Data);
		}
	}
}
