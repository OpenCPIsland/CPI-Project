using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/quest/v1/progress")]
	[HttpTimeout(65f)]
	[HttpPOST]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[RequestQueue("Quest")]
	public class SetProgressOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public SignedResponse<QuestObjectives> RequestBody;

		[HttpResponseJsonBody]
		public QuestChangeResponse ResponseBody;

		public SetProgressOperation(SignedResponse<QuestObjectives> objectives)
		{
			RequestBody = objectives;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = SetProgress(RequestBody.Data, offlineDatabase, offlineDefinitions);
		}

		public static QuestChangeResponse SetProgress(QuestObjectives objectives, OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QuestStates.QuestState questState = null;
			QuestStates questStates = offlineDatabase.Read<QuestStates>();
			QuestChangeResponse questChangeResponse = new QuestChangeResponse();
			foreach (QuestStates.QuestState quest in questStates.Quests)
			{
				if (quest.status == QuestStatus.ACTIVE)
				{
					questState = quest;
					break;
				}
			}
			if (questState == null)
			{
				return questChangeResponse;
			}
			if (questState.completedObjectives == null)
			{
				questState.completedObjectives = new QuestObjectives();
			}
			Reward reward = null;
			if (questState.timesCompleted == 0)
			{
				Dictionary<string, Reward> objectiveRewards = offlineDefinitions.QuestRewards(questState.questId).ObjectiveRewards;
				foreach (string objective in objectives)
				{
					if (!questState.completedObjectives.Contains(objective) && objectiveRewards.ContainsKey(objective))
					{
						if (reward == null)
						{
							reward = new Reward();
						}
						reward.AddReward(objectiveRewards[objective]);
					}
				}
			}
			if (reward != null)
			{
				offlineDefinitions.AddReward(reward, questChangeResponse);
			}
			questState.completedObjectives = objectives;
			offlineDatabase.Write(questStates);
			questChangeResponse.questId = questState.questId;
			questChangeResponse.questStateCollection = new SignedResponse<QuestStateCollection>
			{
				Data = GetQuestStateCollection(questStates, offlineDefinitions, false)
			};
			if (reward != null)
			{
				JsonService jsonService = Service.Get<JsonService>();
				questChangeResponse.reward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(reward)));
			}
			return questChangeResponse;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			QuestChangeResponse responseBody = new QuestChangeResponse();
			if (ResponseBody.reward != null)
			{
				offlineDefinitions.AddReward(ResponseBody.reward.ToReward(), responseBody);
			}
			SetOfflineQuestStateCollection(offlineDatabase, ResponseBody.questStateCollection.Data);
		}

		public static void SetOfflineQuestStateCollection(OfflineDatabase offlineDatabase, QuestStateCollection quests)
		{
			QuestStates value = offlineDatabase.Read<QuestStates>();
			foreach (QuestState quest in quests)
			{
				bool flag = false;
				foreach (QuestStates.QuestState quest2 in value.Quests)
				{
					if (quest2.questId == quest.questId)
					{
						quest2.completedObjectives = quest.completedObjectives;
						quest2.unlockTime = quest.unlockTime;
						quest2.status = quest.status;
						quest2.timesCompleted = quest.timesCompleted;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					QuestStates.QuestState questState = new QuestStates.QuestState();
					questState.questId = quest.questId;
					questState.completedObjectives = quest.completedObjectives;
					questState.timesCompleted = quest.timesCompleted;
					questState.status = quest.status;
					questState.unlockTime = quest.unlockTime;
					QuestStates.QuestState current2 = questState;
					if (current2.status == QuestStatus.COMPLETED)
					{
						current2.completedTime = DateTime.UtcNow;
					}
					value.Quests.Add(current2);
				}
			}
			offlineDatabase.Write(value);
		}

		public static QuestStateCollection GetQuestStateCollection(QuestStates states, IOfflineDefinitionLoader offlineDefinitions, bool includeComplete)
		{
			QuestStateCollection questStateCollection = new QuestStateCollection();
			QuestStateCollection questStateCollection2 = new QuestStateCollection();
			foreach (QuestStates.QuestState quest in states.Quests)
			{
				questStateCollection2.Add(quest);
				if (includeComplete || quest.status != QuestStatus.COMPLETED)
				{
					questStateCollection.Add(quest);
				}
			}
			foreach (QuestState item in offlineDefinitions.AvailableQuests(questStateCollection2))
			{
				questStateCollection.Add(item);
			}
			return questStateCollection;
		}
	}
}
