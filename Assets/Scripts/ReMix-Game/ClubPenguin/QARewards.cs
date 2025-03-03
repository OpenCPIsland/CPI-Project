using ClubPenguin.Adventure;
using ClubPenguin.Collectibles;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Props;
using Disney.MobileNetwork;
using Tweaker.Core;

namespace ClubPenguin
{
	public class QARewards
	{
		public static void AddCoinsToAccount(int coinsToAdd)
		{
			CoinsData component;
			if (Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component))
			{
				QA_SetCoins(coinsToAdd + component.Coins);
			}
		}

		[Invokable("Reward.SetCoins", Description = "Set your desired number of coins")]
		[PublicTweak]
		public static void QA_SetCoins(int coins)
		{
			Reward reward = new Reward();
			reward.Add(new CoinReward(coins));
			Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
		}

		[Invokable("Reward.QuickJackpot", Description = "Loads you up with party supplies, completes all quests, 999999 XP and 999999 Coins")]
		[PublicTweak]
		public static void QA_AddGoodiesForPlayTestQuick()
		{
			QA_AddGoodiesForPlayTest(999999, 999999);
		}

		[PublicTweak]
		[Invokable("Reward.Jackpot", Description = "Loads you up with party supplies, completes all quests, XP for all mascots, and Coins")]
		public static void QA_AddGoodiesForPlayTest(int coins, int xp)
		{
			setXpForAllMascots(xp);
			QA_SetCoins(coins);
			foreach (PropDefinition value in Service.Get<PropService>().Props.Values)
			{
				if (value.PropType == PropDefinition.PropTypes.Consumable && !value.ServerAddedItem)
				{
					Service.Get<INetworkServicesManager>().ConsumableService.QA_SetTypeCount(value.GetNameOnServer(), 999);
				}
			}
			CompleteAllQuests();
		}

		[Invokable("Reward.ClearRewards", Description = "Resets xp, coins, and consumables count to zero")]
		[PublicTweak]
		public static void QA_ClearRewards()
		{
			QA_SetZeroXP();
			QA_SetCoins(0);
			foreach (PropDefinition value in Service.Get<PropService>().Props.Values)
			{
				if (value.PropType == PropDefinition.PropTypes.Consumable && !value.ServerAddedItem)
				{
					Service.Get<INetworkServicesManager>().ConsumableService.QA_SetTypeCount(value.GetNameOnServer(), 0);
				}
			}
		}

		public static void CompleteAllQuests()
		{
			foreach (QuestDefinition value in Service.Get<QuestService>().knownQuests.Values)
			{
				if (!value.Prototyped)
				{
					Service.Get<INetworkServicesManager>().QuestService.QA_SetStatus(value.name, QuestStatus.COMPLETED);
				}
			}
		}

		[PublicTweak]
		[Invokable("Reward.SetXP", Description = "Set your desired xp level for a given mascot")]
		public static void QA_SetMascotXP([NamedToggleValue(typeof(MascotService.MascotNameGenerator), 0u)] string mascot, int xp)
		{
			Reward reward = new Reward();
			reward.Add(new MascotXPReward(mascot, xp));
			Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
		}

		[PublicTweak]
		[Invokable("Reward.MaxLevel", Description = "Puts the user at the max level.")]
		public static void QA_SetMaxXP()
		{
			setXpForAllMascots(999999);
		}

		[PublicTweak]
		[Invokable("Reward.ResetLevel", Description = "Puts the user at level 0")]
		public static void QA_SetZeroXP()
		{
			setXpForAllMascots(0);
		}

		private static void setXpForAllMascots(int xp)
		{
			Reward reward = new Reward();
			foreach (Mascot mascot in Service.Get<MascotService>().Mascots)
			{
				reward.Add(new MascotXPReward(mascot.Name, xp));
			}
			Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
		}

		[PublicTweak]
		[Invokable("Reward.SetCollectibleCount", Description = "Set the count of the given collectible type")]
		public static void QA_SetCollectibleCount([NamedToggleValue(typeof(CollectibleDefinitionService.CollectibleTypeGenerator), 0u)] string type, int count)
		{
			Reward reward = new Reward();
			reward.Add(new CollectibleReward(type, count));
			Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
		}
	}
}
