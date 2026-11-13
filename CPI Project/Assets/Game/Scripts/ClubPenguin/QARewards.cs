using ClubPenguin.Chat;
using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Tubes;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using Tweaker.Core;
using UnityEngine;
using ClubPenguin.Collectibles;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Props;

namespace ClubPenguin
{
	public class QARewards
	{
		private const int ALL_TEMPLATE_DEFINITIONS = -1;

		private const int ALL_TUBE_DEFINITIONS = -1;

		private const string ALL_EMOTE_DEFINITIONS = "__ALL_EMOTES__";

		public class TemplateDefinitionGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				DevonLocalization.Core.Localizer localizer = Service.Get<DevonLocalization.Core.Localizer>();
				List<NamedToggleValueAttribute.NamedToggleValue> options = LoadDefinitions<TemplateDefinition>()
					.Where(template => template != null)
					.OrderBy(template => template.Name)
					.Select(template => new NamedToggleValueAttribute.NamedToggleValue(
						localizer.GetTokenTranslation(template.Name),
						template.Id))
					.ToList();
				options.Insert(0, new NamedToggleValueAttribute.NamedToggleValue("All Templates", ALL_TEMPLATE_DEFINITIONS));
				return options;
			}
		}

		public class TubeDefinitionGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				DevonLocalization.Core.Localizer localizer = Service.Get<DevonLocalization.Core.Localizer>();
				List<NamedToggleValueAttribute.NamedToggleValue> options = LoadDefinitions<TubeDefinition>()
					.Where(tube => tube != null)
					.OrderBy(tube => tube.DisplayNameToken)
					.Select(tube => new NamedToggleValueAttribute.NamedToggleValue(
						localizer.GetTokenTranslation(tube.DisplayNameToken),
						tube.Id))
					.ToList();
				options.Insert(0, new NamedToggleValueAttribute.NamedToggleValue("All Tubes", ALL_TUBE_DEFINITIONS));
				return options;
			}
		}

		public class EmoteDefinitionGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				DevonLocalization.Core.Localizer localizer = Service.Get<DevonLocalization.Core.Localizer>();
				List<EmoteDefinition> emotes = LoadDefinitions<EmoteDefinition>()
					.Where(emote => emote != null && emote.Id != ALL_EMOTE_DEFINITIONS)
					.OrderBy(emote => localizer.GetTokenTranslation(emote.Token))
					.ToList();
				List<NamedToggleValueAttribute.NamedToggleValue> options = emotes
					.Select(emote => new NamedToggleValueAttribute.NamedToggleValue(
						localizer.GetTokenTranslation(emote.Token),
						emote.Id))
					.ToList();
				options.Insert(0, new NamedToggleValueAttribute.NamedToggleValue("All Emotes", ALL_EMOTE_DEFINITIONS));
				return options;
			}
		}

		private static List<TDefinition> LoadDefinitions<TDefinition>() where TDefinition : StaticGameDataDefinition
		{
			Manifest manifest = Content.LoadImmediate<Manifest>(StaticGameDataUtils.GetManifestContentKey(typeof(TDefinition)));
			if (manifest == null || manifest.Assets == null)
			{
				throw new InvalidOperationException("The definition manifest is unavailable for " + typeof(TDefinition).Name + ".");
			}
			return manifest.Assets.OfType<TDefinition>().ToList();
		}

		private static ChatActivityService getChatActivityService()
		{
			GameObject serviceObject = Service.Get<GameObject>();
			if (serviceObject == null)
			{
				throw new InvalidOperationException("The shared game object is unavailable.");
			}
			ChatActivityService service = serviceObject.GetComponent<ChatActivityService>();
			if (service == null)
			{
				throw new InvalidOperationException("Chat activity service is unavailable.");
			}
			return service;
		}

		[Invokable("Inventory.AddTemplate", Description = "Add a template to your inventory, or choose All Templates")]
		[PublicTweak]
		public static void AddTemplateToInventory([NamedToggleValue(typeof(TemplateDefinitionGenerator), 0u)] int templateId)
		{
			if (templateId == ALL_TEMPLATE_DEFINITIONS)
			{
				getChatActivityService().AddAllEquipmentFromTweaker();
			}
			else
			{
				getChatActivityService().AddEquipmentFromTweaker(templateId);
			}
		}

		[Invokable("Inventory.AddTube", Description = "Add a tube to your inventory, or choose All Tubes")]
		[PublicTweak]
		public static void AddTubeToInventory([NamedToggleValue(typeof(TubeDefinitionGenerator), 0u)] int tubeId)
		{
			if (tubeId == ALL_TUBE_DEFINITIONS)
			{
				getChatActivityService().AddAllTubesFromTweaker();
			}
			else
			{
				getChatActivityService().AddTubeFromTweaker(tubeId);
			}
		}

		[Invokable("Inventory.AddEmote", Description = "Add an emote to your inventory, or choose All Emotes")]
		[PublicTweak]
		public static void AddEmoteToInventory([NamedToggleValue(typeof(EmoteDefinitionGenerator), 0u)] string emoteId)
		{
			Dictionary<string, EmoteDefinition> emoteDefinitions = Service.Get<GameData>().Get<Dictionary<string, EmoteDefinition>>();
			if (emoteDefinitions == null)
			{
				throw new InvalidOperationException("Emote definitions are unavailable.");
			}

			Reward reward = new Reward();
			if (emoteId == ALL_EMOTE_DEFINITIONS)
			{
				foreach (EmoteDefinition emote in emoteDefinitions.Values)
				{
					if (emote != null && emote.Id != ALL_EMOTE_DEFINITIONS)
					{
						reward.Add(new EmoteReward(emote.Id));
					}
				}
			}
			else
			{
				EmoteDefinition emote;
				if (!emoteDefinitions.TryGetValue(emoteId, out emote) || emote == null)
				{
					throw new InvalidOperationException("The requested emote is unavailable: " + emoteId);
				}
				reward.Add(new EmoteReward(emote.Id));
			}

			if (reward.isEmpty())
			{
				throw new InvalidOperationException("No emotes are available to add.");
			}
			Service.Get<INetworkServicesManager>().RewardService.QA_SetReward(reward);
		}

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

		[Invokable("Reward.QuickJackpot", Description = "Loads you up with party supplies, completes all quests, 999999 XP and 999999 Coins. Warning: This is not ideal for an everyday playthrough PlayerData, this will cause many problems with the adventures. This is only useful for quick testing.")]
		[PublicTweak]
		public static void QA_AddGoodiesForPlayTestQuick()
		{
			QA_AddGoodiesForPlayTest(999999, 999999);
		}

		[PublicTweak]
		[Invokable("Reward.Jackpot", Description = "Loads you up with party supplies, completes all quests, XP for all mascots, and Coins. Warning: This is not ideal for an everyday playthrough PlayerData, this will cause many problems with the adventures. This is only useful for quick testing.")]
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
		[Invokable("Reward.MaxLevel", Description = "Puts the user at the max level. Warning: This is not ideal for an everyday playthrough PlayerData, this will cause many problems with the adventures. This is only useful for quick testing.")]
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
