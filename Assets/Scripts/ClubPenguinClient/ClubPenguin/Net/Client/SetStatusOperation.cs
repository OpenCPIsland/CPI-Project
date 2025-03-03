using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[RequestQueue("Quest")]
	[HttpTimeout(65f)]
	[HttpAccept("application/json")]
	[HttpContentType("text/plain")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/quest/v1/{$questId}")]
	public class SetStatusOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("questId")]
		public string QuestId;

		[HttpRequestTextBody]
		public string RequestBody;

		[HttpResponseJsonBody]
		public QuestChangeResponse ResponseBody;

		public SetStatusOperation(string questId, QuestStatus status)
		{
			QuestId = questId;
			RequestBody = Convert.ToInt32(status).ToString();
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QuestStatus status = (QuestStatus)Enum.Parse(typeof(QuestStatus), RequestBody);
			ResponseBody = SetStatus(status, QuestId, offlineDatabase, offlineDefinitions);
		}

		public static QuestChangeResponse SetStatus(QuestStatus status, string questId, OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QuestChangeResponse questChangeResponse = new QuestChangeResponse();
			QuestStates questStates = offlineDatabase.Read<QuestStates>();
			QuestStates.QuestState questState = null;
			int num = -1;
			for (int i = 0; i < questStates.Quests.Count; i++)
			{
				if (questStates.Quests[i].questId == questId)
				{
					questState = questStates.Quests[i];
					num = i;
					break;
				}
			}
			QuestRewardsCollection questRewardsCollection = offlineDefinitions.QuestRewards(questId);
			Reward reward = null;
			if (questState == null)
			{
				reward = questRewardsCollection.StartReward;
				if (reward != null)
				{
					if (reward.isEmpty())
					{
						reward = null;
					}
					else
					{
						offlineDefinitions.AddReward(reward, questChangeResponse);
					}
				}
				questState = new QuestStates.QuestState();
				questState.questId = questId;
			}
			if (status == QuestStatus.ACTIVE)
			{
				for (int i = 0; i < questStates.Quests.Count; i++)
				{
					if (questStates.Quests[i].status == QuestStatus.ACTIVE)
					{
						questStates.Quests[i].status = QuestStatus.SUSPENDED;
					}
				}
				if (questState.status == QuestStatus.COMPLETED)
				{
					questState.completedObjectives.Clear();
				}
			}
			if (status == QuestStatus.COMPLETED)
			{
				int timesCompleted = questState.timesCompleted;
				if (timesCompleted == 0)
				{
					questState.completedTime = DateTime.UtcNow;
					reward = questRewardsCollection.CompleteReward;
					if (reward != null)
					{
						if (reward.isEmpty())
						{
							reward = null;
						}
						else
						{
							offlineDefinitions.AddReward(reward, questChangeResponse);
						}
					}
				}
				questState.timesCompleted = timesCompleted + 1;
			}
			questState.status = status;
			if (num >= 0)
			{
				questStates.Quests[num] = questState;
			}
			else
			{
				questStates.Quests.Add(questState);
			}
			offlineDatabase.Write(questStates);
			JsonService jsonService = Service.Get<JsonService>();
			if (reward != null)
			{
				questChangeResponse.reward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(reward)));
			}
			questChangeResponse.questId = questId;
			questChangeResponse.questStateCollection = new SignedResponse<QuestStateCollection>
			{
				Data = SetProgressOperation.GetQuestStateCollection(questStates, offlineDefinitions, false)
			};
			return questChangeResponse;
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
