using ClubPenguin.Adventure;
using ClubPenguin.CellPhone;
using ClubPenguin.Chat;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Collectibles;
using ClubPenguin.Consumable;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.DisneyStore;
using ClubPenguin.Durable;
using ClubPenguin.Igloo;
using ClubPenguin.MiniGames;
using ClubPenguin.MiniGames.Fishing;
using ClubPenguin.Net;
using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Offline;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using ClubPenguin.Props;
using ClubPenguin.Rewards;
using ClubPenguin.Tubes;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class OfflineDefinitionLoader : IOfflineDefinitionLoader
	{
		private OfflineDatabase offlineDatabase;

		private CPDataEntityCollection dataEntityCollection;

		public OfflineDefinitionLoader()
		{
			offlineDatabase = Service.Get<OfflineDatabase>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (!Service.Get<GameSettings>().OfflineMode)
			{
				Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			}
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			if (offlineDatabase.Read<CustomEquipmentCollection>().Equipment.Count == 0)
			{
				INetworkServicesManager network = Service.Get<INetworkServicesManager>();
				network.InventoryService.GetEquipmentInventory();
				network.IglooService.GetDecorations();
				dataEntityCollection.When(dataEntityCollection.LocalPlayerHandle, delegate(SavedIgloosMetaData savedIgloosMetaData)
				{
					CoroutineRunner.StartPersistent(loadIglooEntity(network, savedIgloosMetaData), this, "loadIglooEntity");
				});
			}
			return false;
		}

		private IEnumerator loadIglooEntity(INetworkServicesManager network, SavedIgloosMetaData savedIgloosMetaData)
		{
			yield return null;
			network.IglooService.UpdateIglooData(savedIgloosMetaData.IglooVisibility, savedIgloosMetaData.ActiveIglooId);
			foreach (SavedIglooMetaData savedIgloo in savedIgloosMetaData.SavedIgloos)
			{
				network.IglooService.GetIglooLayout(savedIgloo.LayoutId);
			}
		}

		public void AddReward(Reward reward, CPResponse responseBody)
		{
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			ProgressionService progressionService = Service.Get<ProgressionService>();
			CoinReward rewardable;
			if (reward.TryGetValue(out rewardable))
			{
				value.Assets.coins += rewardable.Coins;
			}
			MascotXPReward rewardable2;
			if (reward.TryGetValue(out rewardable2))
			{
				foreach (KeyValuePair<string, int> item in rewardable2.XP)
				{
					if (value.Assets.mascotXP.ContainsKey(item.Key))
					{
						value.Assets.mascotXP[item.Key] = progressionService.addXp(item.Key, item.Value, value.Assets.mascotXP[item.Key]);
					}
					else
					{
						value.Assets.mascotXP[item.Key] = progressionService.addXp(item.Key, item.Value, 0L);
					}
				}
				int level = progressionService.Level;
				int num = 0;
				foreach (long value5 in value.Assets.mascotXP.Values)
				{
					num += ProgressionService.GetMascotLevelFromXP(value5);
				}
				if (num > level)
				{
					if (responseBody.wsEvents == null)
					{
						responseBody.wsEvents = new List<SignedResponse<WebServiceEvent>>();
					}
					responseBody.wsEvents.Add(new SignedResponse<WebServiceEvent>
					{
						Data = new WebServiceEvent
						{
							details = num,
							type = 3
						}
					});
				}
			}
			CollectibleReward rewardable3;
			if (reward.TryGetValue(out rewardable3))
			{
				foreach (KeyValuePair<string, int> collectible in rewardable3.Collectibles)
				{
					if (value.Assets.collectibleCurrencies.ContainsKey(collectible.Key))
					{
						value.Assets.collectibleCurrencies[collectible.Key] += collectible.Value;
					}
					else
					{
						value.Assets.collectibleCurrencies[collectible.Key] = collectible.Value;
					}
				}
			}
			DecalReward rewardable4;
			if (reward.TryGetValue(out rewardable4))
			{
				value.Assets.decals.AddRange(rewardable4.Decals);
			}
			FabricReward rewardable5;
			if (reward.TryGetValue(out rewardable5))
			{
				value.Assets.fabrics.AddRange(rewardable5.Fabrics);
			}
			EmoteReward rewardable6;
			if (reward.TryGetValue(out rewardable6))
			{
				value.Assets.emotePacks.AddRange(rewardable6.Emotes);
			}
			EquipmentTemplateReward rewardable7;
			if (reward.TryGetValue(out rewardable7))
			{
				value.Assets.equipmentTemplates.AddRange(rewardable7.EquipmentTemplates);
			}
			EquipmentInstanceReward rewardable8;
			if (reward.TryGetValue(out rewardable8))
			{
				System.Random random = new System.Random();
				byte[] array = new byte[8];
				CustomEquipmentCollection value2 = offlineDatabase.Read<CustomEquipmentCollection>();
				foreach (CustomEquipment equipmentInstance in rewardable8.EquipmentInstances)
				{
					random.NextBytes(array);
					value2.Equipment.Add(new CustomEquipment
					{
						dateTimeCreated = DateTime.UtcNow.GetTimeInMilliseconds(),
						definitionId = equipmentInstance.definitionId,
						equipmentId = BitConverter.ToInt64(array, 0),
						parts = equipmentInstance.parts
					});
				}
				offlineDatabase.Write(value2);
			}
			LotReward rewardable9;
			if (reward.TryGetValue(out rewardable9))
			{
				value.Assets.lots.AddRange(rewardable9.Lots);
			}
			DecorationInstanceReward rewardable10;
			if (reward.TryGetValue(out rewardable10))
			{
				DecorationInventoryEntity value3 = offlineDatabase.Read<DecorationInventoryEntity>();
				foreach (KeyValuePair<int, int> decoration in rewardable10.Decorations)
				{
					DecorationId decorationId = new DecorationId(decoration.Key, DecorationType.Decoration);
					if (value3.Inventory.ContainsKey(decorationId))
					{
						value3.Inventory[decorationId] += decoration.Value;
					}
					else
					{
						value3.Inventory[decorationId] = decoration.Value;
					}
				}
				offlineDatabase.Write(value3);
			}
			StructureInstanceReward rewardable11;
			if (reward.TryGetValue(out rewardable11))
			{
				DecorationInventoryEntity value3 = offlineDatabase.Read<DecorationInventoryEntity>();
				foreach (KeyValuePair<int, int> decoration2 in rewardable11.Decorations)
				{
					DecorationId decorationId = new DecorationId(decoration2.Key, DecorationType.Structure);
					if (value3.Inventory.ContainsKey(decorationId))
					{
						value3.Inventory[decorationId] += decoration2.Value;
					}
					else
					{
						value3.Inventory[decorationId] = decoration2.Value;
					}
				}
				offlineDatabase.Write(value3);
			}
			DecorationReward rewardable12;
			if (reward.TryGetValue(out rewardable12))
			{
				value.Assets.decorations.AddRange(rewardable12.Decorations);
			}
			StructureReward rewardable13;
			if (reward.TryGetValue(out rewardable13))
			{
				value.Assets.structures.AddRange(rewardable13.Structures);
			}
			MusicTrackReward rewardable14;
			if (reward.TryGetValue(out rewardable14))
			{
				value.Assets.musicTracks.AddRange(rewardable14.MusicTracks);
			}
			LightingReward rewardable15;
			if (reward.TryGetValue(out rewardable15))
			{
				value.Assets.lighting.AddRange(rewardable15.Lighting);
			}
			DurableReward rewardable16;
			if (reward.TryGetValue(out rewardable16))
			{
				value.Assets.durables.AddRange(rewardable16.Durables);
			}
			IglooSlotsReward rewardable17;
			if (reward.TryGetValue(out rewardable17))
			{
				value.IglooSlots += rewardable17.IglooSlots;
			}
			ConsumableReward rewardable18;
			if (reward.TryGetValue(out rewardable18))
			{
				value.Assets.partySupplies.AddRange(rewardable18.Consumable);
			}
			TubeReward rewardable19;
			if (reward.TryGetValue(out rewardable19))
			{
				value.Assets.tubes.AddRange(rewardable19.Tubes);
			}
			ConsumableInstanceReward rewardable20;
			if (reward.TryGetValue(out rewardable20))
			{
				ClubPenguin.Net.Offline.ConsumableInventory value4 = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
				foreach (KeyValuePair<string, int> consumable in rewardable20.Consumables)
				{
					if (value4.Inventory.ContainsKey(consumable.Key))
					{
						value4.Inventory[consumable.Key].itemCount += consumable.Value;
					}
					else
					{
						value4.Inventory[consumable.Key] = new InventoryItemStock
						{
							itemCount = consumable.Value
						};
					}
					value4.Inventory[consumable.Key].lastPurchaseTimestamp = DateTime.UtcNow.GetTimeInMilliseconds();
				}
				offlineDatabase.Write(value4);
			}
			offlineDatabase.Write(value);
		}

		public void SetReward(Reward reward, CPResponse responseBody)
		{
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			ProgressionService progressionService = Service.Get<ProgressionService>();
			CoinReward rewardable;
			if (reward.TryGetValue(out rewardable))
			{
				value.Assets.coins = rewardable.Coins;
			}
			MascotXPReward rewardable2;
			if (reward.TryGetValue(out rewardable2))
			{
				foreach (KeyValuePair<string, int> item in rewardable2.XP)
				{
					value.Assets.mascotXP[item.Key] = progressionService.addXp(item.Key, item.Value, 0L);
				}
				int level = progressionService.Level;
				int num = 0;
				foreach (long value5 in value.Assets.mascotXP.Values)
				{
					num += ProgressionService.GetMascotLevelFromXP(value5);
				}
				if (num > level)
				{
					if (responseBody.wsEvents == null)
					{
						responseBody.wsEvents = new List<SignedResponse<WebServiceEvent>>();
					}
					responseBody.wsEvents.Add(new SignedResponse<WebServiceEvent>
					{
						Data = new WebServiceEvent
						{
							details = num,
							type = 3
						}
					});
				}
			}
			CollectibleReward rewardable3;
			if (reward.TryGetValue(out rewardable3))
			{
				foreach (KeyValuePair<string, int> collectible in rewardable3.Collectibles)
				{
					value.Assets.collectibleCurrencies[collectible.Key] = collectible.Value;
				}
			}
			DecalReward rewardable4;
			if (reward.TryGetValue(out rewardable4))
			{
				value.Assets.decals = rewardable4.Decals;
			}
			FabricReward rewardable5;
			if (reward.TryGetValue(out rewardable5))
			{
				value.Assets.fabrics = rewardable5.Fabrics;
			}
			EmoteReward rewardable6;
			if (reward.TryGetValue(out rewardable6))
			{
				value.Assets.emotePacks = rewardable6.Emotes;
			}
			EquipmentTemplateReward rewardable7;
			if (reward.TryGetValue(out rewardable7))
			{
				value.Assets.equipmentTemplates = rewardable7.EquipmentTemplates;
			}
			EquipmentInstanceReward rewardable8;
			if (reward.TryGetValue(out rewardable8))
			{
				CustomEquipmentCollection value2 = offlineDatabase.Read<CustomEquipmentCollection>();
				value2.Equipment.Clear();
				System.Random random = new System.Random();
				byte[] array = new byte[8];
				foreach (CustomEquipment equipmentInstance in rewardable8.EquipmentInstances)
				{
					random.NextBytes(array);
					value2.Equipment.Add(new CustomEquipment
					{
						dateTimeCreated = DateTime.UtcNow.GetTimeInMilliseconds(),
						definitionId = equipmentInstance.definitionId,
						equipmentId = BitConverter.ToInt64(array, 0),
						parts = equipmentInstance.parts
					});
				}
				offlineDatabase.Write(value2);
			}
			LotReward rewardable9;
			if (reward.TryGetValue(out rewardable9))
			{
				value.Assets.lots = rewardable9.Lots;
			}
			DecorationInstanceReward rewardable10;
			if (reward.TryGetValue(out rewardable10))
			{
				DecorationInventoryEntity value3 = offlineDatabase.Read<DecorationInventoryEntity>();
				foreach (KeyValuePair<int, int> decoration in rewardable10.Decorations)
				{
					DecorationId id = new DecorationId(decoration.Key, DecorationType.Decoration);
					value3.Inventory[id] = decoration.Value;
				}
				offlineDatabase.Write(value3);
			}
			StructureInstanceReward rewardable11;
			if (reward.TryGetValue(out rewardable11))
			{
				DecorationInventoryEntity value3 = offlineDatabase.Read<DecorationInventoryEntity>();
				foreach (KeyValuePair<int, int> decoration2 in rewardable11.Decorations)
				{
					DecorationId id = new DecorationId(decoration2.Key, DecorationType.Structure);
					value3.Inventory[id] = decoration2.Value;
				}
				offlineDatabase.Write(value3);
			}
			DecorationReward rewardable12;
			if (reward.TryGetValue(out rewardable12))
			{
				value.Assets.decorations = rewardable12.Decorations;
			}
			StructureReward rewardable13;
			if (reward.TryGetValue(out rewardable13))
			{
				value.Assets.structures = rewardable13.Structures;
			}
			MusicTrackReward rewardable14;
			if (reward.TryGetValue(out rewardable14))
			{
				value.Assets.musicTracks = rewardable14.MusicTracks;
			}
			LightingReward rewardable15;
			if (reward.TryGetValue(out rewardable15))
			{
				value.Assets.lighting = rewardable15.Lighting;
			}
			DurableReward rewardable16;
			if (reward.TryGetValue(out rewardable16))
			{
				value.Assets.durables = rewardable16.Durables;
			}
			IglooSlotsReward rewardable17;
			if (reward.TryGetValue(out rewardable17))
			{
				value.IglooSlots = rewardable17.IglooSlots;
			}
			ConsumableReward rewardable18;
			if (reward.TryGetValue(out rewardable18))
			{
				value.Assets.partySupplies = rewardable18.Consumable;
			}
			TubeReward rewardable19;
			if (reward.TryGetValue(out rewardable19))
			{
				value.Assets.tubes = rewardable19.Tubes;
			}
			ConsumableInstanceReward rewardable20;
			if (reward.TryGetValue(out rewardable20))
			{
				ClubPenguin.Net.Offline.ConsumableInventory value4 = offlineDatabase.Read<ClubPenguin.Net.Offline.ConsumableInventory>();
				foreach (KeyValuePair<string, int> consumable in rewardable20.Consumables)
				{
					value4.Inventory[consumable.Key] = new InventoryItemStock
					{
						itemCount = consumable.Value
					};
					value4.Inventory[consumable.Key].lastPurchaseTimestamp = DateTime.UtcNow.GetTimeInMilliseconds();
				}
				offlineDatabase.Write(value4);
			}
			offlineDatabase.Write(value);
		}

		public List<QuestState> AvailableQuests(QuestStateCollection quests)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (QuestState quest in quests)
			{
				hashSet.Add(quest.questId);
			}
			ClubPenguin.Net.Offline.PlayerAssets playerAssets = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			List<QuestState> list = new List<QuestState>();
			Dictionary<string, QuestDefinition> knownQuests = Service.Get<QuestService>().knownQuests;
			foreach (KeyValuePair<string, QuestDefinition> item in knownQuests)
			{
				if (!hashSet.Contains(item.Key))
				{
					DateTime? t = questAvailableDate(item.Value, playerAssets.Assets.mascotXP);
					if (t.HasValue)
					{
						QuestState questState = new QuestState();
						questState.questId = item.Key;
						questState.completedObjectives = new QuestObjectives();
						if (t <= DateTime.UtcNow)
						{
							questState.status = QuestStatus.AVAILABLE;
						}
						else
						{
							questState.status = QuestStatus.LOCKED;
							questState.unlockTime = t.Value.GetTimeInMilliseconds();
						}
						list.Add(questState);
					}
				}
			}
			return list;
		}

		public Reward GetClaimableReward(int rewardId)
		{
			Dictionary<int, ClaimableRewardDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, ClaimableRewardDefinition>>();
			ClaimableRewardDefinition value;
			if (dictionary.TryGetValue(rewardId, out value) && value.Reward != null)
			{
				return value.Reward.ToReward();
			}
			return null;
		}

		public Reward GetInRoomReward(List<string> newRewards)
		{
			Reward reward = new Reward();
			foreach (string newReward in newRewards)
			{
				GameObject gameObject = GameObject.Find(newReward);
				if (gameObject != null)
				{
					Collectible[] array = gameObject.GetComponents<Collectible>();
					if (array.Length == 0)
					{
						ShakeCollectible componentInParent = gameObject.GetComponentInParent<ShakeCollectible>();
						if (componentInParent == null)
						{
							continue;
						}
						SceneryCollectible[] array2 = UnityEngine.Object.FindObjectsOfType<SceneryCollectible>();
						List<Collectible> list = new List<Collectible>();
						SceneryCollectible[] array3 = array2;
						foreach (SceneryCollectible sceneryCollectible in array3)
						{
							if (sceneryCollectible.originalParent == componentInParent.transform)
							{
								list.Add(sceneryCollectible);
							}
						}
						array = list.ToArray();
					}
					Collectible[] array4 = array;
					foreach (Collectible collectible in array4)
					{
						RewardDefinition rewardDef = collectible.RewardDef;
						if (rewardDef != null)
						{
							reward.AddReward(rewardDef.ToReward());
						}
					}
				}
			}
			return reward;
		}

		public QuestRewardsCollection QuestRewards(string questId)
		{
			QuestRewardsCollection result = default(QuestRewardsCollection);
			Dictionary<string, QuestDefinition> knownQuests = Service.Get<QuestService>().knownQuests;
			if (knownQuests.ContainsKey(questId))
			{
				QuestDefinition questDefinition = knownQuests[questId];
				if ((bool)questDefinition.StartReward)
				{
					result.StartReward = questDefinition.StartReward.ToReward();
				}
				if ((bool)questDefinition.CompleteReward)
				{
					result.CompleteReward = questDefinition.CompleteReward.ToReward();
				}
				result.ObjectiveRewards = new Dictionary<string, Reward>();
				if (questDefinition.ObjectiveRewards.Length > 0)
				{
					Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
					if (activeQuest != null)
					{
						foreach (KeyValuePair<string, int> objectiveRewardIndex in activeQuest.ObjectiveRewardIndexes)
						{
							if (objectiveRewardIndex.Value >= 0 && objectiveRewardIndex.Value < questDefinition.ObjectiveRewards.Length)
							{
								result.ObjectiveRewards.Add(objectiveRewardIndex.Key, questDefinition.ObjectiveRewards[objectiveRewardIndex.Value].ToReward());
							}
						}
					}
				}
			}
			return result;
		}

		public void SubtractEquipmentCost(int templateDefinitionId)
		{
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			TemplateDefinition value;
			if (dictionary.TryGetValue(templateDefinitionId, out value))
			{
				ClubPenguin.Net.Offline.PlayerAssets value2 = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
				value2.Assets.coins -= value.Cost;
				if (value2.Assets.coins < 0)
				{
					value2.Assets.coins = 0;
				}
				offlineDatabase.Write(value2);
			}
		}

		public void SubtractConsumableCost(string consumableId, int count)
		{
			Dictionary<int, PropDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			foreach (PropDefinition value2 in dictionary.Values)
			{
				if (consumableId == value2.NameOnServer)
				{
					ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
					value.Assets.coins -= value2.Cost * count;
					if (value.Assets.coins < 0)
					{
						value.Assets.coins = 0;
					}
					offlineDatabase.Write(value);
				}
			}
		}

		public void SubtractDecorationCost(DecorationId decoration, int count)
		{
			int num = 0;
			switch (decoration.type)
			{
			case DecorationType.Decoration:
			{
				Dictionary<int, DecorationDefinition> dictionary2 = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
				if (dictionary2.ContainsKey(decoration.definitionId))
				{
					num = dictionary2[decoration.definitionId].Cost;
				}
				break;
			}
			case DecorationType.Structure:
			{
				Dictionary<int, StructureDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, StructureDefinition>>();
				if (dictionary.ContainsKey(decoration.definitionId))
				{
					num = dictionary[decoration.definitionId].Cost;
				}
				break;
			}
			}
			ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
			value.Assets.coins -= num * count;
			if (value.Assets.coins < 0)
			{
				value.Assets.coins = 0;
			}
			offlineDatabase.Write(value);
		}

		private DateTime? questAvailableDate(QuestDefinition def, Dictionary<string, long> mascotsXp)
		{
			if (def.LevelRequirement > 0)
			{
				long value = 0L;
				mascotsXp.TryGetValue(def.Mascot.name, out value);
				if (def.LevelRequirement > ProgressionService.GetMascotLevelFromXP(value))
				{
					return null;
				}
			}
			DateTime dateTime = DateTime.UtcNow.AddMinutes(-1.0);
			if (def.CompletedQuestRequirement != null && def.CompletedQuestRequirement.Length > 0)
			{
				List<QuestStates.QuestState> quests = Service.Get<OfflineDatabase>().Read<QuestStates>().Quests;
				Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>(quests.Count);
				foreach (QuestStates.QuestState item in quests)
				{
					if (item.timesCompleted > 0)
					{
						dictionary.Add(item.questId, item.completedTime);
					}
				}
				QuestDefinition[] completedQuestRequirement = def.CompletedQuestRequirement;
				foreach (QuestDefinition questDefinition in completedQuestRequirement)
				{
					if (!dictionary.ContainsKey(questDefinition.name))
					{
						return null;
					}
					if (def.TimeLock.TimeSpan.TotalSeconds > 0.0)
					{
						DateTime dateTime2 = dictionary[questDefinition.name].Add(def.TimeLock.TimeSpan);
						if (dateTime < dateTime2)
						{
							dateTime = dateTime2;
						}
					}
				}
			}
			return dateTime;
		}

		public int GetSpinResult(Reward spinReward, Reward chestReward)
		{
			CellPhoneDailySpinActivityDefinition widgetData = ClubPenguin.Core.SceneRefs.Get<CellPhoneActivityScreenDailySpinWidget>().WidgetData;
			ClubPenguin.Net.Offline.DailySpinData dailySpinData = offlineDatabase.Read<ClubPenguin.Net.Offline.DailySpinData>();
			bool flag = dailySpinData.CurrentChestId == 0 && dailySpinData.NumPunchesOnCurrentChest == 0 && dailySpinData.NumChestsReceivedOfCurrentChestId == 0;
			CellPhoneDailySpinActivityDefinition.ChestDefinition chestDefinition = getChestDefinitionForId(dailySpinData.CurrentChestId, widgetData) ?? default(CellPhoneDailySpinActivityDefinition.ChestDefinition);
			dailySpinData.NumPunchesOnCurrentChest++;
			int num = -1;
			if (dailySpinData.NumPunchesOnCurrentChest >= chestDefinition.NumPunchesPerChest)
			{
				addChestReward(chestReward, widgetData, dailySpinData, chestDefinition);
			}
			if (flag && !widgetData.FirstTimeSpinReward.Reward.ToReward().isEmpty())
			{
				num = widgetData.FirstTimeSpinReward.SpinOutcomeId;
				spinReward.AddReward(widgetData.FirstTimeSpinReward.Reward.ToReward());
			}
			else
			{
				num = addWeightedRandomSpinReward(spinReward, widgetData, dailySpinData, chestDefinition);
			}
			if (num == widgetData.ChestSpinOutcomeId)
			{
				dailySpinData.NumSpinsSinceReceivedChest = 0;
				dailySpinData.NumSpinsSinceReceivedExtraSpin++;
				addChestReward(spinReward, widgetData, dailySpinData, chestDefinition);
			}
			else if (num == widgetData.RespinSpinOutcomeId)
			{
				dailySpinData.NumSpinsSinceReceivedExtraSpin = 0;
				if (isChestValidSpinReward(dailySpinData, chestDefinition))
				{
					dailySpinData.NumSpinsSinceReceivedChest++;
				}
			}
			else
			{
				dailySpinData.NumSpinsSinceReceivedExtraSpin++;
				if (isChestValidSpinReward(dailySpinData, chestDefinition))
				{
					dailySpinData.NumSpinsSinceReceivedChest++;
				}
			}
			if (num == widgetData.ChestSpinOutcomeId || !chestReward.isEmpty())
			{
				dailySpinData.NumChestsReceivedOfCurrentChestId++;
				dailySpinData.NumPunchesOnCurrentChest = 0;
				if (dailySpinData.NumChestsReceivedOfCurrentChestId >= chestDefinition.NumChestsToNextLevel)
				{
					CellPhoneDailySpinActivityDefinition.ChestDefinition? chestDefinitionForId = getChestDefinitionForId(chestDefinition.ChestId + 1, widgetData);
					if (chestDefinitionForId.HasValue)
					{
						dailySpinData.CurrentChestId = chestDefinitionForId.Value.ChestId;
						dailySpinData.NumChestsReceivedOfCurrentChestId = 0;
					}
				}
			}
			MascotXPReward rewardable;
			if (spinReward.TryGetValue(out rewardable))
			{
				bool flag2 = false;
				ProgressionService progressionService = Service.Get<ProgressionService>();
				foreach (KeyValuePair<string, int> item in rewardable.XP)
				{
					if (progressionService.IsMascotMaxLevel(item.Key))
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					rewardable.XP.Clear();
					spinReward.AddReward(widgetData.DefaultReward.ToReward());
				}
			}
			if (num != widgetData.RespinSpinOutcomeId)
			{
				dailySpinData.TimeOfLastSpinInMilliseconds = DateTime.UtcNow.GetTimeInMilliseconds();
			}
			offlineDatabase.Write(dailySpinData);
			return num;
		}

		private static int addWeightedRandomSpinReward(Reward spinReward, CellPhoneDailySpinActivityDefinition dailySpinDefinition, ClubPenguin.Net.Offline.DailySpinData dailySpinData, CellPhoneDailySpinActivityDefinition.ChestDefinition chestDefinition)
		{
			int num = 0;
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
			Dictionary<int, Reward> dictionary = new Dictionary<int, Reward>();
			int num2 = dailySpinDefinition.InitialRespinWeight + dailySpinDefinition.RespinWeightIncreasePerSpin * dailySpinData.NumSpinsSinceReceivedExtraSpin;
			dictionary.Add(dailySpinDefinition.RespinSpinOutcomeId, dailySpinDefinition.RespinReward.ToReward());
			list.Add(new KeyValuePair<int, int>(dailySpinDefinition.RespinSpinOutcomeId, num2));
			num += num2;
			if (isChestValidSpinReward(dailySpinData, chestDefinition))
			{
				int num3 = dailySpinDefinition.InitialChestWeight + dailySpinDefinition.ChestWeightIncreasePerSpin * dailySpinData.NumSpinsSinceReceivedChest;
				dictionary.Add(dailySpinDefinition.ChestSpinOutcomeId, new Reward());
				list.Add(new KeyValuePair<int, int>(dailySpinDefinition.ChestSpinOutcomeId, num3));
				num += num3;
			}
			foreach (CellPhoneDailySpinActivityDefinition.SpinReward spinReward2 in dailySpinDefinition.SpinRewards)
			{
				dictionary.Add(spinReward2.SpinOutcomeId, spinReward2.Reward.ToReward());
				list.Add(new KeyValuePair<int, int>(spinReward2.SpinOutcomeId, spinReward2.Weight));
				num += spinReward2.Weight;
			}
			int num4 = UnityEngine.Random.Range(0, num);
			int num5 = 0;
			foreach (KeyValuePair<int, int> item in list)
			{
				num5 += item.Value;
				if (num5 > num4)
				{
					spinReward.AddReward(dictionary[item.Key]);
					return item.Key;
				}
			}
			return -1;
		}

		private static bool isChestValidSpinReward(ClubPenguin.Net.Offline.DailySpinData dailySpinData, CellPhoneDailySpinActivityDefinition.ChestDefinition chestDefinition)
		{
			return !chestDefinition.IsChestSpinNotAllowed && dailySpinData.NumPunchesOnCurrentChest < chestDefinition.NumPunchesPerChest;
		}

		private static CellPhoneDailySpinActivityDefinition.ChestDefinition? getChestDefinitionForId(int chestId, CellPhoneDailySpinActivityDefinition dailySpinDefinition)
		{
			foreach (CellPhoneDailySpinActivityDefinition.ChestDefinition chestDefinition in dailySpinDefinition.ChestDefinitions)
			{
				if (chestDefinition.ChestId == chestId)
				{
					return chestDefinition;
				}
			}
			return null;
		}

		private static void addChestReward(Reward chestReward, CellPhoneDailySpinActivityDefinition dailySpinDefinition, ClubPenguin.Net.Offline.DailySpinData dailySpinData, CellPhoneDailySpinActivityDefinition.ChestDefinition chestDefinition)
		{
			if (dailySpinData.NumChestsReceivedOfCurrentChestId == 0)
			{
				CellPhoneDailySpinActivityDefinition.ChestReward repeatableChestReward = getRepeatableChestReward(dailySpinData, chestDefinition);
				chestReward.AddReward(repeatableChestReward.Reward.ToReward());
				chestReward.AddReward(chestDefinition.FirstTimeClaimedReward.ToReward());
			}
			else
			{
				CellPhoneDailySpinActivityDefinition.ChestReward repeatableChestReward = getRepeatableChestReward(dailySpinData, chestDefinition);
				chestReward.AddReward(repeatableChestReward.Reward.ToReward());
				CellPhoneDailySpinActivityDefinition.ChestReward nonRepeatableChestReward = getNonRepeatableChestReward(dailySpinData, chestDefinition);
				chestReward.AddReward(nonRepeatableChestReward.Reward.ToReward());
			}
		}

		private static CellPhoneDailySpinActivityDefinition.ChestReward getNonRepeatableChestReward(ClubPenguin.Net.Offline.DailySpinData dailySpinData, CellPhoneDailySpinActivityDefinition.ChestDefinition chestDefinition)
		{
			List<CellPhoneDailySpinActivityDefinition.ChestReward> nonRepeatableChestRewards = chestDefinition.NonRepeatableChestRewards;
			List<CellPhoneDailySpinActivityDefinition.ChestReward> list = filterRewardsAlreadyReceived(nonRepeatableChestRewards, dailySpinData.EarnedNonRepeatableRewardIds);
			if (list.Count > 0)
			{
				return getRepeatableChestReward(dailySpinData, chestDefinition);
			}
			nonRepeatableChestRewards = list;
			dailySpinData.EarnedNonRepeatableRewardIds.Add(nonRepeatableChestRewards[0].RewardId);
			return nonRepeatableChestRewards[0];
		}

		private static CellPhoneDailySpinActivityDefinition.ChestReward getRepeatableChestReward(ClubPenguin.Net.Offline.DailySpinData dailySpinData, CellPhoneDailySpinActivityDefinition.ChestDefinition chestDefinition)
		{
			List<CellPhoneDailySpinActivityDefinition.ChestReward> list = new List<CellPhoneDailySpinActivityDefinition.ChestReward>(chestDefinition.RepeatableChestRewards);
			List<CellPhoneDailySpinActivityDefinition.ChestReward> list2 = filterRewardsAlreadyReceived(list, dailySpinData.EarnedRepeatableRewardIds);
			if (list2.Count > 0)
			{
				dailySpinData.EarnedRepeatableRewardIds.Clear();
			}
			else
			{
				list = list2;
			}
			dailySpinData.EarnedRepeatableRewardIds.Add(list[0].RewardId);
			return list[0];
		}

		private static List<CellPhoneDailySpinActivityDefinition.ChestReward> filterRewardsAlreadyReceived(List<CellPhoneDailySpinActivityDefinition.ChestReward> chestRewards, List<int> receivedRewardIds)
		{
			List<CellPhoneDailySpinActivityDefinition.ChestReward> list = new List<CellPhoneDailySpinActivityDefinition.ChestReward>();
			foreach (CellPhoneDailySpinActivityDefinition.ChestReward chestReward in chestRewards)
			{
				if (!receivedRewardIds.Contains(chestReward.RewardId))
				{
					list.Add(chestReward);
				}
			}
			return list;
		}

		public OfflineGameServerClient.IConsumable GetConsumable(string type)
		{
			Dictionary<int, PropDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			foreach (PropDefinition value in dictionary.Values)
			{
				if (type == value.NameOnServer)
				{
					switch (value.ExperienceType)
					{
					case PropDefinition.ConsumableExperience.OneShot:
						return new OfflineGameServerClient.EchoConsumable();
					case PropDefinition.ConsumableExperience.PlayerHeld:
						if (value.Shareable)
						{
							return new OfflineGameServerClient.SharedConsumable(value.TotalItemQuantity, value.RecipientConsumable);
						}
						return new OfflineGameServerClient.PlayerHeldConsumable(value.ActionCount);
					case PropDefinition.ConsumableExperience.World:
						if (value.ActionCount > 0)
						{
							return new OfflineGameServerClient.ActionedConsumable(value.TimeToLive, value.RewardRadius, value.TotalReward.ToReward(), value.MinPerPlayerReward.ToReward(), value.ActionCount);
						}
						return new OfflineGameServerClient.TimedConsumable(value.TimeToLive, value.RewardRadius, value.TotalReward.ToReward(), value.MinPerPlayerReward.ToReward());
					case PropDefinition.ConsumableExperience.PartyGameLobby:
						return new OfflineGameServerClient.SharedConsumable(value.TotalItemQuantity, value.RecipientConsumable);
					}
					break;
				}
			}
			return null;
		}

		public Reward GetQuickNotificationReward()
		{
			ActivityNotificationManager activityNotificationManager = UnityEngine.Object.FindObjectOfType<ActivityNotificationManager>();
			if (activityNotificationManager != null)
			{
				return activityNotificationManager.Schedule.NotificationReward.ToReward();
			}
			return new Reward();
		}

		public int GetCoinsForExchange(Dictionary<string, int> collectibleCurrencies)
		{
			int num = 0;
			CollectibleDefinitionService collectibleDefinitionService = Service.Get<CollectibleDefinitionService>();
			foreach (KeyValuePair<string, int> collectibleCurrency in collectibleCurrencies)
			{
				CollectibleDefinition collectibleDefinition = collectibleDefinitionService.Get(collectibleCurrency.Key);
				if (collectibleDefinition != null)
				{
					num += (int)Math.Ceiling(collectibleDefinition.ExchangeRate * (float)collectibleCurrency.Value);
				}
			}
			return num;
		}

		public Dictionary<string, string> GetRandomFishingPrizes()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			MinigameService minigameService = Service.Get<MinigameService>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			foreach (LootTableRewardDefinition value in minigameService.Loot.Values)
			{
				if (value.Bucket.Weight != 0)
				{
					if (dictionary2.ContainsKey(value.Bucket.BucketName))
					{
						dictionary2[value.Bucket.BucketName] += value.Weight;
					}
					else
					{
						dictionary2[value.Bucket.BucketName] = value.Weight;
					}
				}
			}
			foreach (KeyValuePair<string, int> item in dictionary2)
			{
				int num = UnityEngine.Random.Range(0, item.Value);
				int num2 = 0;
				foreach (LootTableRewardDefinition value2 in minigameService.Loot.Values)
				{
					if (value2.Weight != 0 && value2.Bucket.BucketName == item.Key)
					{
						num2 += value2.Weight;
						if (num2 > num)
						{
							dictionary.Add(item.Key, value2.Id);
							break;
						}
					}
				}
			}
			return dictionary;
		}

		public Reward GetFishingReward(string lootTableRewardId)
		{
			MinigameService minigameService = Service.Get<MinigameService>();
			foreach (LootTableRewardDefinition value in minigameService.Loot.Values)
			{
				if (value.Id == lootTableRewardId)
				{
					return value.Reward.ToReward();
				}
			}
			return new Reward();
		}

		public Reward GetDisneyStoreItemReward(int itemId, int count)
		{
			Dictionary<int, DisneyStoreFranchiseDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DisneyStoreFranchiseDefinition>>();
			foreach (DisneyStoreFranchiseDefinition value in dictionary.Values)
			{
				if (value.Items != null)
				{
					foreach (DisneyStoreItemDefinition item in value.Items)
					{
						if (item.Id == itemId)
						{
							Reward reward = item.Reward.ToReward();
							if (count > 1)
							{
								ConsumableInstanceReward rewardable;
								if (reward.TryGetValue(out rewardable))
								{
									Dictionary<string, int> dictionary2 = new Dictionary<string, int>(rewardable.Consumables);
									foreach (KeyValuePair<string, int> item2 in dictionary2)
									{
										rewardable.Consumables[item2.Key] = item2.Value * count;
									}
								}
								DecorationInstanceReward rewardable2;
								if (reward.TryGetValue(out rewardable2))
								{
									Dictionary<int, int> dictionary3 = new Dictionary<int, int>(rewardable2.Decorations);
									foreach (KeyValuePair<int, int> item3 in dictionary3)
									{
										rewardable2.Decorations[item3.Key] = item3.Value * count;
									}
								}
								StructureInstanceReward rewardable3;
								if (reward.TryGetValue(out rewardable3))
								{
									Dictionary<int, int> dictionary3 = new Dictionary<int, int>(rewardable3.Decorations);
									foreach (KeyValuePair<int, int> item4 in dictionary3)
									{
										rewardable3.Decorations[item4.Key] = item4.Value * count;
									}
								}
							}
							return reward;
						}
					}
				}
			}
			return new Reward();
		}

		public void SubtractDisneyStoreItemCost(int itemId, int count)
		{
			Dictionary<int, DisneyStoreFranchiseDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DisneyStoreFranchiseDefinition>>();
			foreach (DisneyStoreFranchiseDefinition value2 in dictionary.Values)
			{
				if (value2.Items != null)
				{
					foreach (DisneyStoreItemDefinition item in value2.Items)
					{
						if (item.Id == itemId)
						{
							ClubPenguin.Net.Offline.PlayerAssets value = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
							value.Assets.coins -= item.Cost * count;
							if (value.Assets.coins < 0)
							{
								value.Assets.coins = 0;
							}
							offlineDatabase.Write(value);
						}
					}
				}
			}
		}

		public bool IsOwnIgloo(ZoneId iglooId)
		{
			ProfileData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				return component.ZoneId.Equals(iglooId);
			}
			return false;
		}
	}
}
