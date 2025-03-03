using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public interface IOfflineDefinitionLoader
	{
		List<QuestState> AvailableQuests(QuestStateCollection quests);

		QuestRewardsCollection QuestRewards(string questId);

		void AddReward(Reward reward, CPResponse responseBody);

		void SetReward(Reward reward, CPResponse responseBody);

		Reward GetClaimableReward(int rewardId);

		Reward GetInRoomReward(List<string> newRewards);

		void SubtractEquipmentCost(int definitionId);

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
