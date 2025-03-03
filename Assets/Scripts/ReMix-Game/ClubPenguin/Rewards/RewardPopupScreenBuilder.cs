using ClubPenguin.Adventure;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Rewards
{
	public static class RewardPopupScreenBuilder
	{
		public static DRewardPopupScreen[] BuildScreens(DRewardPopup popupData)
		{
			List<DRewardPopupScreen> list = new List<DRewardPopupScreen>();
			bool flag = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			list.Add(buildSplashScreen(popupData));
			list.AddRange(buildCustomScreens(popupData));
			if (popupData.RewardData != null)
			{
				list.AddRange(buildCustomEquipmentScreens(popupData, !flag));
				list.AddRange(buildItemScreens(popupData, !flag));
				list.AddRange(buildQuestsScreen(popupData, !flag));
				list.AddRange(buildXPScreen(popupData));
			}
			if (!flag)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].PreferredSortOrder = i;
				}
				list.Sort(compareScreenByNonMemberScreen);
			}
			return list.ToArray();
		}

		private static int compareScreenByNonMemberScreen(DRewardPopupScreen screen1, DRewardPopupScreen screen2)
		{
			if (screen1.IsRewardsAllNonMember && !(screen1 is DRewardPopupScreenCoinsXP) && !screen2.IsRewardsAllNonMember)
			{
				return -1;
			}
			if (screen2.IsRewardsAllNonMember && !(screen2 is DRewardPopupScreenCoinsXP) && !screen1.IsRewardsAllNonMember)
			{
				return 1;
			}
			return screen1.PreferredSortOrder.CompareTo(screen2.PreferredSortOrder);
		}

		private static List<DRewardPopupScreen> buildXPScreen(DRewardPopup popupData)
		{
			List<DRewardPopupScreen> list = new List<DRewardPopupScreen>();
			CoinReward rewardable;
			if ((popupData.RewardData.TryGetValue(out rewardable) && !rewardable.IsEmpty()) || popupData.XP > 0)
			{
				DRewardPopupScreenCoinsXP dRewardPopupScreenCoinsXP = new DRewardPopupScreenCoinsXP();
				dRewardPopupScreenCoinsXP.CoinCount = rewardable.Coins;
				dRewardPopupScreenCoinsXP.XPCount = popupData.XP;
				dRewardPopupScreenCoinsXP.mascotName = popupData.MascotName;
				dRewardPopupScreenCoinsXP.ShowXpAndCoinsUI = popupData.ShowXpAndCoinsUI;
				dRewardPopupScreenCoinsXP.IsRewardsAllNonMember = true;
				list.Add(dRewardPopupScreenCoinsXP);
			}
			return list;
		}

		private static List<DRewardPopupScreen> buildCustomEquipmentScreens(DRewardPopup popupData, bool checkForNonMemberScreens)
		{
			List<DRewardPopupScreen> list = new List<DRewardPopupScreen>();
			EquipmentInstanceReward rewardable;
			if (popupData.RewardData.TryGetValue(out rewardable) && !rewardable.IsEmpty())
			{
				DRewardPopupScreenItems dRewardPopupScreenItems = new DRewardPopupScreenItems();
				DReward[] array = new DReward[rewardable.EquipmentInstances.Count];
				bool isRewardsAllNonMember = false;
				for (int i = 0; i < rewardable.EquipmentInstances.Count; i++)
				{
					DReward dReward = new DReward();
					dReward.EquipmentRequest = rewardable.EquipmentInstances[i];
					array[i] = dReward;
				}
				if (checkForNonMemberScreens)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (RewardUtils.IsRewardMemberOnly(RewardCategory.equipmentInstances, array[j].EquipmentRequest.definitionId))
						{
							isRewardsAllNonMember = false;
							break;
						}
						isRewardsAllNonMember = true;
					}
				}
				dRewardPopupScreenItems.ItemCategory = RewardCategory.equipmentInstances;
				dRewardPopupScreenItems.Rewards = array;
				dRewardPopupScreenItems.IsRewardsAllNonMember = isRewardsAllNonMember;
				list.Add(dRewardPopupScreenItems);
			}
			return list;
		}

		private static List<DRewardPopupScreen> buildItemScreens(DRewardPopup popupData, bool checkForNonMemberScreens)
		{
			List<DRewardPopupScreen> list = new List<DRewardPopupScreen>();
			Type typeFromHandle = typeof(IList);
			Type typeFromHandle2 = typeof(int);
			Type typeFromHandle3 = typeof(IDictionary);
			foreach (IRewardable rewardDatum in popupData.RewardData)
			{
				if (!(rewardDatum is EquipmentInstanceReward) && !rewardDatum.IsEmpty() && Enum.IsDefined(typeof(RewardCategory), rewardDatum.RewardType))
				{
					RewardCategory rewardCategory = (RewardCategory)Enum.Parse(typeof(RewardCategory), rewardDatum.RewardType);
					Type type = rewardDatum.Reward.GetType();
					if (rewardCategory == RewardCategory.iglooSlots)
					{
						int num = (int)rewardDatum.Reward;
						if (num > 0)
						{
							DRewardPopupScreenItems dRewardPopupScreenItems = new DRewardPopupScreenItems();
							DReward[] rewards = new DReward[num];
							for (int i = 0; i < num; i++)
							{
								DReward dReward = new DReward();
								dReward.Category = rewardCategory;
							}
							dRewardPopupScreenItems.ItemCategory = rewardCategory;
							dRewardPopupScreenItems.Rewards = rewards;
							dRewardPopupScreenItems.RewardPopupType = popupData.PopupType;
							list.Add(dRewardPopupScreenItems);
						}
					}
					else if (typeFromHandle.IsAssignableFrom(type))
					{
						IList list2 = rewardDatum.Reward as IList;
						if (list2 != null && list2.Count > 0)
						{
							bool isRewardsAllNonMember = false;
							DRewardPopupScreenItems dRewardPopupScreenItems = buildItemScreenFromIList(rewardCategory, list2, popupData);
							if (checkForNonMemberScreens)
							{
								for (int i = 0; i < dRewardPopupScreenItems.Rewards.Length; i++)
								{
									if (RewardUtils.IsRewardMemberOnly(rewardCategory, dRewardPopupScreenItems.Rewards[i].UnlockID))
									{
										isRewardsAllNonMember = false;
										break;
									}
									isRewardsAllNonMember = true;
								}
							}
							dRewardPopupScreenItems.IsRewardsAllNonMember = isRewardsAllNonMember;
							list.Add(dRewardPopupScreenItems);
						}
					}
					else if (typeFromHandle3.IsAssignableFrom(type))
					{
						IDictionary dictionary = rewardDatum.Reward as IDictionary;
						if (dictionary != null && dictionary.Count > 0)
						{
							bool isRewardsAllNonMember = false;
							DRewardPopupScreenItems dRewardPopupScreenItems = buildItemScreenFromDictionary(rewardCategory, dictionary, popupData);
							if (checkForNonMemberScreens)
							{
								for (int i = 0; i < dRewardPopupScreenItems.Rewards.Length; i++)
								{
									if (RewardUtils.IsRewardMemberOnly(rewardCategory, dRewardPopupScreenItems.Rewards[i].UnlockID))
									{
										isRewardsAllNonMember = false;
										break;
									}
									isRewardsAllNonMember = true;
								}
							}
							dRewardPopupScreenItems.IsRewardsAllNonMember = isRewardsAllNonMember;
							list.Add(dRewardPopupScreenItems);
						}
					}
					else if (type.Equals(typeFromHandle2))
					{
						int num = (int)rewardDatum.Reward;
						if (num > 0)
						{
							list.Add(buildCountScreen(rewardCategory, num));
						}
					}
				}
			}
			list.Sort(delegate(DRewardPopupScreen p1, DRewardPopupScreen p2)
			{
				RewardCategory rewardCategory2 = (!(p1 is DRewardPopupScreenItems)) ? ((DRewardPopupScreenCount)p1).CountCategory : ((DRewardPopupScreenItems)p1).ItemCategory;
				RewardCategory rewardCategory3 = (!(p2 is DRewardPopupScreenItems)) ? ((DRewardPopupScreenCount)p2).CountCategory : ((DRewardPopupScreenItems)p2).ItemCategory;
				return rewardCategory2.CompareTo(rewardCategory3);
			});
			return list;
		}

		private static DRewardPopupScreenItems buildItemScreenFromIList(RewardCategory category, IList unlockedItems, DRewardPopup popupData)
		{
			DRewardPopupScreenItems dRewardPopupScreenItems = new DRewardPopupScreenItems();
			DReward[] array = new DReward[unlockedItems.Count];
			for (int i = 0; i < unlockedItems.Count; i++)
			{
				DReward dReward = new DReward();
				dReward.UnlockID = unlockedItems[i];
				array[i] = dReward;
				dReward.Category = category;
			}
			dRewardPopupScreenItems.ItemCategory = category;
			dRewardPopupScreenItems.Rewards = array;
			dRewardPopupScreenItems.RewardPopupType = popupData.PopupType;
			return dRewardPopupScreenItems;
		}

		private static DRewardPopupScreenItems buildItemScreenFromDictionary(RewardCategory category, IDictionary unlockedItems, DRewardPopup popupData)
		{
			DRewardPopupScreenItems dRewardPopupScreenItems = new DRewardPopupScreenItems();
			DReward[] array = new DReward[unlockedItems.Count];
			int num = 0;
			IDictionaryEnumerator enumerator = unlockedItems.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DReward dReward = new DReward();
				dReward.UnlockID = enumerator.Key;
				dReward.Data = enumerator.Value;
				dReward.Category = category;
				array[num] = dReward;
				num++;
			}
			dRewardPopupScreenItems.ItemCategory = category;
			dRewardPopupScreenItems.Rewards = array;
			dRewardPopupScreenItems.RewardPopupType = popupData.PopupType;
			return dRewardPopupScreenItems;
		}

		private static DRewardPopupScreenCount buildCountScreen(RewardCategory category, int unlockedCount)
		{
			DRewardPopupScreenCount dRewardPopupScreenCount = new DRewardPopupScreenCount();
			dRewardPopupScreenCount.CountCategory = category;
			dRewardPopupScreenCount.Count = unlockedCount;
			return dRewardPopupScreenCount;
		}

		private static DRewardPopupScreen buildSplashScreen(DRewardPopup popupData)
		{
			DRewardPopupScreenSplash dRewardPopupScreenSplash = new DRewardPopupScreenSplash();
			dRewardPopupScreenSplash.SplashScreenTextToken = popupData.SplashTitleToken;
			dRewardPopupScreenSplash.IsRewardsAllNonMember = true;
			switch (popupData.PopupType)
			{
			case DRewardPopup.RewardPopupType.levelUp:
				dRewardPopupScreenSplash.ScreenType = DRewardPopupScreen.RewardScreenPopupType.splash_levelup;
				break;
			case DRewardPopup.RewardPopupType.questComplete:
			case DRewardPopup.RewardPopupType.generic:
				dRewardPopupScreenSplash.ScreenType = DRewardPopupScreen.RewardScreenPopupType.splash;
				break;
			case DRewardPopup.RewardPopupType.replay:
				dRewardPopupScreenSplash.ScreenType = DRewardPopupScreen.RewardScreenPopupType.splash_replay;
				break;
			}
			return dRewardPopupScreenSplash;
		}

		private static List<DRewardPopupScreen> buildQuestsScreen(DRewardPopup popupData, bool checkForNonMemberScreens)
		{
			DRewardPopupScreenQuests dRewardPopupScreenQuests = new DRewardPopupScreenQuests();
			List<QuestDefinition> list = new List<QuestDefinition>();
			bool isRewardsAllNonMember = false;
			if (popupData.PopupType == DRewardPopup.RewardPopupType.levelUp)
			{
				MascotService mascotService = Service.Get<MascotService>();
				Mascot mascot = mascotService.GetMascot(popupData.MascotName);
				int num = Service.Get<ProgressionService>().MascotLevel(mascot.Name);
				if (popupData.MascotName == "AuntArctic" && num == 1)
				{
					foreach (Mascot mascot2 in mascotService.Mascots)
					{
						if (mascot2.IsQuestGiver && mascot2.Name != "AuntArctic")
						{
							for (int i = 0; i < mascot2.KnownQuests.Length; i++)
							{
								QuestDefinition questDefinition = mascot2.KnownQuests[i];
								if (questDefinition.QuestNumber == 1 && questDefinition.ChapterNumber == 1)
								{
									list.Add(questDefinition);
								}
								if (list.Count >= 2)
								{
									break;
								}
							}
						}
					}
				}
				if (list.Count < 2)
				{
					for (int i = 0; i < mascot.KnownQuests.Length; i++)
					{
						QuestDefinition questDefinition = mascot.KnownQuests[i];
						if (!questDefinition.Prototyped && questDefinition.LevelRequirement == num)
						{
							list.Add(questDefinition);
						}
						if (list.Count >= 2)
						{
							break;
						}
					}
				}
			}
			List<DRewardPopupScreen> list2 = new List<DRewardPopupScreen>();
			if (list.Count > 0)
			{
				dRewardPopupScreenQuests.quests = list.ToArray();
				if (checkForNonMemberScreens)
				{
					for (int i = 0; i < dRewardPopupScreenQuests.quests.Length; i++)
					{
						if (dRewardPopupScreenQuests.quests[i].isMemberOnly)
						{
							isRewardsAllNonMember = false;
							break;
						}
						isRewardsAllNonMember = true;
					}
				}
				dRewardPopupScreenQuests.IsRewardsAllNonMember = isRewardsAllNonMember;
				list2.Add(dRewardPopupScreenQuests);
			}
			return list2;
		}

		private static List<DRewardPopupScreen> buildCustomScreens(DRewardPopup popupData)
		{
			List<DRewardPopupScreen> list = new List<DRewardPopupScreen>();
			if (popupData.CustomScreenKeys != null)
			{
				for (int i = 0; i < popupData.CustomScreenKeys.Count; i++)
				{
					DRewardPopupScreenCustom item = new DRewardPopupScreenCustom(popupData.CustomScreenKeys[i]);
					list.Add(item);
				}
			}
			return list;
		}
	}
}
