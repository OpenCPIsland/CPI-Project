using ClubPenguin.Chat;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Consumable;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Props;
using ClubPenguin.Tubes;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ClubPenguin.Rewards
{
	public static class RewardUtils
	{
		public static Reward GetRewardForProgressionLevel(int level)
		{
			Reward reward = new Reward();
			Dictionary<ProgressionUnlockCategory, string[]> unlockedIDsForLevel = Service.Get<ProgressionService>().GetUnlockedIDsForLevel(level);
			Dictionary<ProgressionUnlockCategory, ProgressionService.UnlockDefinition> unlockedDefinitionsForLevel = Service.Get<ProgressionService>().GetUnlockedDefinitionsForLevel(level);
			Dictionary<ProgressionUnlockCategory, int> unlockedCountsForLevel = Service.Get<ProgressionService>().GetUnlockedCountsForLevel(level);
			foreach (KeyValuePair<ProgressionUnlockCategory, string[]> item in unlockedIDsForLevel)
			{
				if (RewardableLoader.RewardableTypeMap.ContainsKey(item.Key.ToString()))
				{
					for (int i = 0; i < item.Value.Length; i++)
					{
						reward.Add(RewardableLoader.GenerateRewardable(item.Key.ToString(), item.Value));
					}
				}
			}
			foreach (KeyValuePair<ProgressionUnlockCategory, ProgressionService.UnlockDefinition> item2 in unlockedDefinitionsForLevel)
			{
				string text = item2.Key.ToString();
				Type elementType = typeof(ProgressionUnlockDefinition).GetField(text).FieldType.GetElementType();
				if (elementType != null)
				{
					FieldInfo attributedField = StaticGameDataDefinitionIdAttribute.GetAttributedField(elementType);
					if (attributedField != null)
					{
						if (RewardableLoader.RewardableTypeMap.ContainsKey(text) && item2.Value.Definitions.Length > 0)
						{
							Type type = attributedField.GetValue(item2.Value.Definitions[0]).GetType();
							Type type2 = typeof(List<>).MakeGenericType(type);
							IList list = (IList)Activator.CreateInstance(type2);
							for (int i = 0; i < item2.Value.Definitions.Length; i++)
							{
								list.Add(attributedField.GetValue(item2.Value.Definitions[i]));
							}
							for (int i = 0; i < list.Count; i++)
							{
								reward.Add(RewardableLoader.GenerateRewardable(text, list[i]));
							}
						}
					}
					else
					{
						Log.LogErrorFormatted(typeof(RewardUtils), "Could not find a StaticGameDataDefinitionId field on type {0}", elementType);
					}
				}
				else
				{
					Log.LogErrorFormatted(typeof(RewardUtils), "Could not find an element type for the field {0}, the field must be an array", text);
				}
			}
			foreach (KeyValuePair<ProgressionUnlockCategory, int> item3 in unlockedCountsForLevel)
			{
				if (RewardableLoader.RewardableTypeMap.ContainsKey(item3.Key.ToString()))
				{
					reward.Add(RewardableLoader.GenerateRewardable(item3.Key.ToString(), item3.Value));
				}
			}
			return reward;
		}

		public static string GetUnlockText(RewardCategory category)
		{
			Localizer localizer = Service.Get<Localizer>();
			string tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Items");
			switch (category)
			{
			case RewardCategory.decals:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Decals");
				break;
			case RewardCategory.sizzleClips:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Animations");
				break;
			case RewardCategory.equipmentInstances:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.SpecialItems");
				break;
			case RewardCategory.fabrics:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Fabrics");
				break;
			case RewardCategory.equipmentTemplates:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Templates");
				break;
			case RewardCategory.savedOutfitSlots:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.OutfitSlots");
				break;
			case RewardCategory.emotePacks:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Emotes");
				break;
			case RewardCategory.durables:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Gear");
				break;
			case RewardCategory.partySupplies:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.PartySupplies");
				break;
			case RewardCategory.tubes:
				tokenTranslation = localizer.GetTokenTranslation("RewardScreen.UnlockedText.Tubes");
				break;
			case RewardCategory.lots:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.Lots");
				break;
			case RewardCategory.lighting:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.Lighting");
				break;
			case RewardCategory.musicTracks:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.Effects");
				break;
			case RewardCategory.iglooSlots:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.IglooSlot");
				break;
			case RewardCategory.decorationInstances:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.Items");
				break;
			case RewardCategory.decorationPurchaseRights:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.ItemsAvailable");
				break;
			case RewardCategory.structureInstances:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.Structures");
				break;
			case RewardCategory.structurePurchaseRights:
				tokenTranslation = localizer.GetTokenTranslation("Rewards.Igloos.StructuresAvailable");
				break;
			}
			return tokenTranslation;
		}

		public static List<DReward> GetDRewardFromReward(Reward reward)
		{
			Type typeFromHandle = typeof(IList);
			List<DReward> list = new List<DReward>();
			foreach (IRewardable item in reward)
			{
				if (!item.IsEmpty())
				{
					if (item is ConsumableInstanceReward)
					{
						ConsumableInstanceReward consumableInstanceReward = (ConsumableInstanceReward)item;
						using (Dictionary<string, int>.Enumerator enumerator2 = consumableInstanceReward.Consumables.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								DReward dReward = new DReward();
								dReward.Category = RewardCategory.consumables;
								dReward.UnlockID = GetConsumableIdByServerName(enumerator2.Current.Key);
								list.Add(dReward);
							}
						}
					}
					else if (item is EquipmentInstanceReward)
					{
						EquipmentInstanceReward equipmentInstanceReward = (EquipmentInstanceReward)item;
						for (int i = 0; i < equipmentInstanceReward.EquipmentInstances.Count; i++)
						{
							DReward dReward = new DReward();
							dReward.Category = RewardCategory.equipmentInstances;
							dReward.EquipmentRequest = equipmentInstanceReward.EquipmentInstances[i];
							list.Add(dReward);
						}
					}
					else if (item is DecorationInstanceReward)
					{
						DecorationInstanceReward decorationInstanceReward = (DecorationInstanceReward)item;
						using (Dictionary<int, int>.Enumerator enumerator3 = decorationInstanceReward.Decorations.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								DReward dReward = new DReward();
								dReward.Category = RewardCategory.decorationInstances;
								dReward.UnlockID = enumerator3.Current.Key;
								list.Add(dReward);
							}
						}
					}
					else if (item is StructureInstanceReward)
					{
						StructureInstanceReward structureInstanceReward = (StructureInstanceReward)item;
						using (Dictionary<int, int>.Enumerator enumerator3 = structureInstanceReward.Decorations.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								DReward dReward = new DReward();
								dReward.Category = RewardCategory.structureInstances;
								dReward.UnlockID = enumerator3.Current.Key;
								list.Add(dReward);
							}
						}
					}
					else if (Enum.IsDefined(typeof(RewardCategory), item.RewardType) && typeFromHandle.IsAssignableFrom(item.Reward.GetType()))
					{
						IList list2 = item.Reward as IList;
						if (list2 != null)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								DReward dReward = new DReward();
								dReward.UnlockID = list2[i];
								dReward.Category = (RewardCategory)Enum.Parse(typeof(RewardCategory), item.RewardType);
								list.Add(dReward);
							}
						}
					}
				}
			}
			return list;
		}

		public static int GetConsumableIdByServerName(string serverName)
		{
			Dictionary<int, PropDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			int result = -1;
			using (Dictionary<int, PropDefinition>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Value.NameOnServer == serverName)
					{
						result = enumerator.Current.Value.Id;
					}
				}
			}
			return result;
		}

		public static string GetConsumableServerNameById(int id)
		{
			Dictionary<int, PropDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			string result = "";
			using (Dictionary<int, PropDefinition>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Value.Id == id)
					{
						result = enumerator.Current.Value.NameOnServer;
					}
				}
			}
			return result;
		}

		public static bool IsConsumablePartyGame(string serverName)
		{
			Dictionary<int, PropDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			bool result = false;
			using (Dictionary<int, PropDefinition>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Value.NameOnServer == serverName)
					{
						result = (enumerator.Current.Value.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby);
						break;
					}
				}
			}
			return result;
		}

		public static bool IsMemberLockableItemMemberOnly<TItemDefinition, UIdType>(UIdType itemId, bool allowNonMemberTemplateCreation = false) where TItemDefinition : IMemberLocked
		{
			bool result = true;
			Dictionary<UIdType, TItemDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<UIdType, TItemDefinition>>();
			if (dictionary != null && dictionary.ContainsKey(itemId))
			{
				if (dictionary[itemId] is TemplateDefinition && allowNonMemberTemplateCreation)
				{
					TemplateDefinition templateDefinition = dictionary[itemId] as TemplateDefinition;
					result = templateDefinition.IsMemberOnlyCreatable;
				}
				else
				{
					result = dictionary[itemId].IsMemberOnly;
				}
			}
			return result;
		}

		public static bool IsRewardMemberOnly(RewardCategory category, object definitionId)
		{
			try
			{
				switch (category)
				{
				case RewardCategory.equipmentInstances:
				case RewardCategory.equipmentTemplates:
					return IsMemberLockableItemMemberOnly<TemplateDefinition, int>(int.Parse(definitionId.ToString()), true);
				case RewardCategory.decorationInstances:
				case RewardCategory.decorationPurchaseRights:
					return IsMemberLockableItemMemberOnly<DecorationDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.structureInstances:
				case RewardCategory.structurePurchaseRights:
					return IsMemberLockableItemMemberOnly<StructureDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.lots:
					return IsMemberLockableItemMemberOnly<LotDefinition, string>(definitionId.ToString());
				case RewardCategory.lighting:
					return IsMemberLockableItemMemberOnly<LightingDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.musicTracks:
					return IsMemberLockableItemMemberOnly<MusicTrackDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.fabrics:
					return IsMemberLockableItemMemberOnly<FabricDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.decals:
					return IsMemberLockableItemMemberOnly<DecalDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.emotePacks:
					return IsMemberLockableItemMemberOnly<EmoteDefinition, string>(definitionId.ToString());
				case RewardCategory.sizzleClips:
					return IsMemberLockableItemMemberOnly<SizzleClipDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.tubes:
					return IsMemberLockableItemMemberOnly<TubeDefinition, int>(int.Parse(definitionId.ToString()));
				case RewardCategory.durables:
				case RewardCategory.partySupplies:
				{
					int itemId = int.Parse(definitionId.ToString());
					return IsMemberLockableItemMemberOnly<PropDefinition, int>(itemId);
				}
				case RewardCategory.consumables:
				{
					int consumableIdByServerName = GetConsumableIdByServerName(definitionId.ToString());
					return IsMemberLockableItemMemberOnly<PropDefinition, int>(consumableIdByServerName);
				}
				default:
					return true;
				}
			}
			catch (Exception)
			{
				return true;
			}
		}
	}
}
