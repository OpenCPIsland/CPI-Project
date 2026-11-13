using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
    public interface IOfflineDefinitionLoader
    {
        List<QuestState> AvailableQuests(QuestStateCollection quests);

        QuestRewardsCollection QuestRewards(string questId);

        Reward GetTaskReward(string taskId);

        // Whether the task has actually been finished. Offline there is nobody
        // else to ask: the reward is handed out by the same machine that claims
        // it, so without this the only thing standing between a player and an
        // unearned reward is the button being greyed out.
        bool IsTaskComplete(string taskId);

        // The day daily challenges belong to. Not the calendar day: the game picks
        // the set of challenges by its own content date, so counters have to be
        // filed against the same day or they reset while the tasks stay put.
        long GetCurrentDay();

        void AddReward(Reward reward, CPResponse responseBody);

        void SetReward(Reward reward, CPResponse responseBody);

        Reward GetClaimableReward(int rewardId);

        Reward GetInRoomReward(List<string> newRewards);

        void SubtractEquipmentCost(int definitionId);

        int GetEquipmentTemplateDefinitionCost(int definitionId);

        void SubtractConsumableCost(string consumableId, int count);

        int GetSpinResult(Reward spinReward, Reward chestReward);

        OfflineGameServerClient.IConsumable GetConsumable(string type);

        Reward GetQuickNotificationReward();

        int GetCoinsForExchange(Dictionary<string, int> collectibleCurrencies);

        Dictionary<string, string> GetRandomFishingPrizes();

        Reward GetFishingReward(string v);

        Reward GetDisneyStoreItemReward(int itemId, int count);

        void SubtractDisneyStoreItemCost(int itemId, int count);

        void SubtractDecorationCost(DecorationId decoration, int count);

        bool IsOwnIgloo(ZoneId iglooId);
    }
}
