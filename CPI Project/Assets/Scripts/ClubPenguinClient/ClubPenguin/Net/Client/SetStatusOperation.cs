using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;
using UnityEngine;

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

            if (offlineDatabase == null)
            {
                Debug.LogError($"[SetStatusOperation] offlineDatabase is NULL for questId '{questId}'");
                return questChangeResponse;
            }

            QuestStates questStates = offlineDatabase.Read<QuestStates>();
            if (questStates.Equals(default(QuestStates)))
            {
                Debug.LogWarning($"[SetStatusOperation] QuestStates is default for questId '{questId}' — creating new");
                questStates = new QuestStates();
            }

            if (questStates.Quests == null)
            {
                Debug.LogWarning($"[SetStatusOperation] questStates.Quests is NULL for questId '{questId}' — creating list");
                questStates.Quests = new List<QuestStates.QuestState>();
            }

            if (offlineDefinitions == null)
            {
                Debug.LogError($"[SetStatusOperation] offlineDefinitions is NULL for questId '{questId}'");
                return questChangeResponse;
            }

            QuestStates.QuestState questState = null;
            int num = -1;
            for (int i = 0; i < questStates.Quests.Count; i++)
            {
                if (questStates.Quests[i] == null)
                {
                    Debug.LogError($"[SetStatusOperation] questStates.Quests[{i}] is NULL for questId '{questId}'");
                    continue;
                }

                if (questStates.Quests[i].questId == questId)
                {
                    questState = questStates.Quests[i];
                    num = i;
                    break;
                }
            }

            QuestRewardsCollection questRewardsCollection = offlineDefinitions.QuestRewards(questId);
            if (questRewardsCollection.Equals(default(QuestRewardsCollection)))
            {
                Debug.LogWarning($"[SetStatusOperation] QuestRewardsCollection is default for questId '{questId}' — creating empty collection");
                questRewardsCollection = new QuestRewardsCollection();
            }
            else
            {
                Debug.Log($"[SetStatusOperation] QuestRewardsCollection for questId '{questId}': {SafeJson(questRewardsCollection)}");
            }

            Reward reward = null;
            string rewardSource = "None";

            if (questState == null)
            {
                rewardSource = "StartReward";
                reward = questRewardsCollection.StartReward;
                if (reward == null || reward.isEmpty())
                {
                    Debug.LogWarning($"[SetStatusOperation] {rewardSource} is empty or null for questId '{questId}' — {SafeJson(reward)}");
                    reward = null;
                }
                else
                {
                    Debug.Log($"[SetStatusOperation] Adding {rewardSource} for questId '{questId}' — {SafeJson(reward)}");
                    offlineDefinitions.AddReward(reward, questChangeResponse);
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

                if (questState.status == QuestStatus.COMPLETED && questState.completedObjectives != null)
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

                    rewardSource = "CompleteReward";
                    reward = questRewardsCollection.CompleteReward;
                    if (reward == null || reward.isEmpty())
                    {
                        Debug.LogWarning($"[SetStatusOperation] {rewardSource} is empty or null for questId '{questId}' — {SafeJson(reward)}");
                        reward = null;
                    }
                    else
                    {
                        Debug.Log($"[SetStatusOperation] Adding {rewardSource} for questId '{questId}' — {SafeJson(reward)}");
                        offlineDefinitions.AddReward(reward, questChangeResponse);
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

            if (reward != null)
            {
                try
                {
                    JsonService jsonService = Service.Get<JsonService>();
                    if (jsonService != null)
                    {
                        questChangeResponse.reward = jsonService.Deserialize<RewardJsonReader>(
                            jsonService.Serialize(RewardJsonWritter.FromReward(reward))
                        );
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[SetStatusOperation] Exception serializing reward for questId '{questId}': {ex}");
                }
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
            if (ResponseBody?.reward != null)
            {
                Debug.Log($"[SetStatusOperation] Applying ResponseBody.reward — {SafeJson(ResponseBody.reward)}");
                offlineDefinitions.AddReward(ResponseBody.reward.ToReward(), new QuestChangeResponse());
            }
        }

        private static string SafeJson(object obj)
        {
            if (obj == null) return "NULL";
            try
            {
                return JsonUtility.ToJson(obj, true);
            }
            catch
            {
                return "<Non-serializable object>";
            }
        }
    }
}
