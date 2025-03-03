using ClubPenguin.Avatar;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Core;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin.UI
{
	public static class DisneyStoreUtils
	{
		public const string DISNEY_STORE_SHOW_TUTORIAL_KEY = "DisneyStoreShowTutorial";

		public static bool IsItemOwned(DisneyStoreItemData item)
		{
			List<DReward> rewards = item.GetRewards();
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].Category == RewardCategory.equipmentInstances)
				{
					if (!InventoryUtils.IsCustomEquipmentOwned(rewards[i].EquipmentRequest))
					{
						return false;
					}
				}
				else if (Enum.IsDefined(typeof(ProgressionUnlockCategory), rewards[i].Category.ToString()))
				{
					ProgressionUnlockCategory category = (ProgressionUnlockCategory)Enum.Parse(typeof(ProgressionUnlockCategory), rewards[i].Category.ToString());
					if (!Service.Get<ProgressionService>().IsUnlocked(rewards[i].UnlockID, category))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool IsIglooReward(DisneyStoreItemData item)
		{
			bool result = false;
			List<DReward> rewards = item.GetRewards();
			for (int i = 0; i < rewards.Count; i++)
			{
				result = IsIglooReward(rewards[i].Category);
			}
			return result;
		}

		public static bool IsIglooReward(RewardCategory rewardCategory)
		{
			bool result = false;
			switch (rewardCategory)
			{
			case RewardCategory.lots:
			case RewardCategory.structureInstances:
			case RewardCategory.structurePurchaseRights:
			case RewardCategory.decorationInstances:
			case RewardCategory.decorationPurchaseRights:
			case RewardCategory.lighting:
			case RewardCategory.musicTracks:
			case RewardCategory.iglooSlots:
				result = true;
				break;
			}
			return result;
		}

		public static bool IsItemMultiPurchase(DisneyStoreItemData item)
		{
			List<DReward> rewards = item.GetRewards();
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].Category == RewardCategory.consumables || rewards[i].Category == RewardCategory.decorationInstances)
				{
					flag = true;
					continue;
				}
				flag2 = true;
				break;
			}
			return flag && !flag2;
		}

		public static RewardCategory GetItemRewardCategory(DisneyStoreItemData item)
		{
			List<DReward> rewards = item.GetRewards();
			RewardCategory result = RewardCategory.equipmentInstances;
			if (rewards.Count > 0)
			{
				result = rewards[0].Category;
			}
			return result;
		}

		public static bool DoesItemContainEquipmentInstance(DisneyStoreItemData item)
		{
			List<DReward> rewards = item.GetRewards();
			bool result = false;
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].Category == RewardCategory.equipmentInstances)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static DCustomEquipment? GetEquipmentFromDreward(DReward reward)
		{
			InventoryData component = Service.Get<CPDataEntityCollection>().GetComponent<InventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			if (component != null && component.Inventory != null)
			{
				Dictionary<long, InventoryIconModel<DCustomEquipment>>.Enumerator enumerator = component.Inventory.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InventoryIconModel<DCustomEquipment> value = enumerator.Current.Value;
					if (InventoryUtils.IsEquipmentEqual(value.Data, reward.EquipmentRequest))
					{
						return value.Data;
					}
				}
			}
			return null;
		}
	}
}
