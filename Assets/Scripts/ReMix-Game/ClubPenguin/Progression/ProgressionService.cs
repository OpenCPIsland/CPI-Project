using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Consumable;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Rewards;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ClubPenguin.Progression
{
	public class ProgressionService
	{
		public struct UnlockDefinition
		{
			public RewardThemeDefinition ThemeDefinition;

			public StaticGameDataDefinition[] Definitions;

			public UnlockDefinition(RewardThemeDefinition themeDefinition, StaticGameDataDefinition[] definitions)
			{
				ThemeDefinition = themeDefinition;
				Definitions = definitions;
			}
		}

		private const string QUEST_COMPLETE_POPUP_CONTENT_KEY = "Prefabs/Quest/Popups/QuestCompletePopup";

		private Dictionary<string, int[]> mascotLevelXPData;

		private ShowRewardPopup pendingLevelUpPopup;

		private Dictionary<ProgressionUnlockCategory, string[]>[] unlockItems;

		private Dictionary<ProgressionUnlockCategory, UnlockDefinition>[] unlockDefinitions;

		private Dictionary<ProgressionUnlockCategory, int>[] unlockCounts;

		private Dictionary<ProgressionUnlockCategory, HashSet<string>> rewardItems;

		private Dictionary<ProgressionUnlockCategory, HashSet<StaticGameDataDefinition>> rewardDefinitions;

		private Dictionary<ProgressionUnlockCategory, int> rewardCounts;

		private FieldInfo[] playerAssetsFieldInfo;

		private Dictionary<string, long> myMascotXP;

		private int maxUnlockLevel;

		public ShowRewardPopup PendingLevelUpPopup
		{
			get
			{
				return pendingLevelUpPopup;
			}
			set
			{
				pendingLevelUpPopup = value;
			}
		}

		public int Level
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<string, long> item in myMascotXP)
				{
					num += MascotLevel(item.Key);
				}
				return num;
			}
		}

		public int MaxUnlockLevel
		{
			get
			{
				return maxUnlockLevel;
			}
		}

		public ProgressionService(Manifest mascotLevelXPManifest)
		{
			parseUnlockDefinitions(Service.Get<GameData>().Get<Dictionary<int, ProgressionUnlockDefinition>>());
			parseMascotLevelXPManifest(mascotLevelXPManifest);
			myMascotXP = new Dictionary<string, long>();
			rewardItems = new Dictionary<ProgressionUnlockCategory, HashSet<string>>();
			rewardDefinitions = new Dictionary<ProgressionUnlockCategory, HashSet<StaticGameDataDefinition>>();
			rewardCounts = new Dictionary<ProgressionUnlockCategory, int>();
			playerAssetsFieldInfo = typeof(PlayerAssets).GetFields();
			addListeners();
		}

		public bool IsUnlocked(string unlockID, ProgressionUnlockCategory category)
		{
			bool flag = false;
			for (int i = 0; i <= Level; i++)
			{
				if (Array.IndexOf(unlockItems[i][category], unlockID) != -1)
				{
					flag = true;
					break;
				}
			}
			if (!flag && rewardItems[category].Contains(unlockID))
			{
				flag = true;
			}
			return flag;
		}

		public bool IsUnlocked(object definitionID, ProgressionUnlockCategory category)
		{
			for (int i = 0; i <= Level; i++)
			{
				if (unlockDefinitions.Length <= i || !unlockDefinitions[i].ContainsKey(category) || unlockDefinitions[i][category].Definitions.Length <= 0)
				{
					continue;
				}
				StaticGameDataDefinition[] definitions = unlockDefinitions[i][category].Definitions;
				FieldInfo attributedField = StaticGameDataDefinitionIdAttribute.GetAttributedField(definitions[0].GetType());
				for (int j = 0; j < definitions.Length; j++)
				{
					if (attributedField.GetValue(definitions[j]).Equals(definitionID))
					{
						return true;
					}
				}
			}
			if (rewardDefinitions.ContainsKey(category) && rewardDefinitions[category].Count > 0)
			{
				HashSet<StaticGameDataDefinition> hashSet = rewardDefinitions[category];
				HashSet<StaticGameDataDefinition>.Enumerator enumerator = hashSet.GetEnumerator();
				enumerator.MoveNext();
				FieldInfo attributedField = StaticGameDataDefinitionIdAttribute.GetAttributedField(enumerator.Current.GetType());
				if (attributedField.GetValue(enumerator.Current).Equals(definitionID))
				{
					return true;
				}
				while (enumerator.MoveNext())
				{
					if (attributedField.GetValue(enumerator.Current).Equals(definitionID))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsUnlocked(int count, ProgressionUnlockCategory category)
		{
			for (int i = 0; i <= Level; i++)
			{
				if (unlockCounts[i].ContainsKey(category))
				{
					count -= unlockCounts[i][category];
				}
			}
			if (rewardCounts.ContainsKey(category))
			{
				count -= rewardCounts[category];
			}
			return count <= 0;
		}

		public bool IsUnlocked(StaticGameDataDefinition definition, ProgressionUnlockCategory category)
		{
			bool flag = false;
			for (int i = 0; i <= Level; i++)
			{
				if (unlockDefinitions.Length > i && unlockDefinitions[i].ContainsKey(category) && Array.IndexOf(unlockDefinitions[i][category].Definitions, definition) != -1)
				{
					flag = true;
					break;
				}
			}
			if (rewardDefinitions.ContainsKey(category))
			{
				if (!flag && rewardDefinitions[category].Contains(definition))
				{
					flag = true;
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Category {0} was not found in Reward Definitions.", category);
			}
			return flag;
		}

		public int GetUnlockLevelFromDefinition(StaticGameDataDefinition definition, ProgressionUnlockCategory category)
		{
			int result = -1;
			for (int i = 0; i <= MaxUnlockLevel; i++)
			{
				if (unlockDefinitions.Length > i && unlockDefinitions[i].ContainsKey(category) && Array.IndexOf(unlockDefinitions[i][category].Definitions, definition) != -1)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public Dictionary<ProgressionUnlockCategory, string[]> GetUnlockedIDsForLevel(int level)
		{
			level = normalizeLevel(level);
			return unlockItems[level];
		}

		public Dictionary<ProgressionUnlockCategory, int> GetUnlockedCountsForLevel(int level)
		{
			level = normalizeLevel(level);
			return unlockCounts[level];
		}

		public Dictionary<ProgressionUnlockCategory, UnlockDefinition> GetUnlockedDefinitionsForLevel(int level)
		{
			level = normalizeLevel(level);
			return unlockDefinitions[level];
		}

		public string[] GetUnlockedIDsForLevel(int level, ProgressionUnlockCategory category)
		{
			level = normalizeLevel(level);
			if (unlockItems[level].ContainsKey(category))
			{
				return unlockItems[level][category];
			}
			return new string[0];
		}

		public int GetUnlockedCountsForLevel(int level, ProgressionUnlockCategory category)
		{
			level = normalizeLevel(level);
			if (unlockCounts[level].ContainsKey(category))
			{
				return unlockCounts[level][category];
			}
			return 0;
		}

		public UnlockDefinition GetUnlockedDefinitionsForLevel(int level, ProgressionUnlockCategory category)
		{
			level = normalizeLevel(level);
			if (unlockDefinitions.Length > level && unlockDefinitions[level].ContainsKey(category))
			{
				return unlockDefinitions[level][category];
			}
			return default(UnlockDefinition);
		}

		public string[] GetUnlockedIDsForCategory(ProgressionUnlockCategory category)
		{
			List<string> list = new List<string>();
			for (int i = 0; i <= Level; i++)
			{
				if (unlockItems[i].ContainsKey(category))
				{
					list.AddRange(unlockItems[i][category]);
				}
			}
			if (rewardItems.ContainsKey(category))
			{
				list.AddRange(rewardItems[category]);
			}
			return list.ToArray();
		}

		public int GetUnlockedCountsForCategory(ProgressionUnlockCategory category)
		{
			int num = 0;
			for (int i = 0; i <= Level; i++)
			{
				if (unlockCounts[i].ContainsKey(category))
				{
					num += unlockCounts[i][category];
				}
			}
			if (rewardCounts.ContainsKey(category))
			{
				num += rewardCounts[category];
			}
			return num;
		}

		public T[] GetUnlockedDefinitionsForCategory<T>(ProgressionUnlockCategory category) where T : StaticGameDataDefinition
		{
			List<T> list = new List<T>();
			int level = Level;
			for (int i = 0; i <= level; i++)
			{
				if (unlockDefinitions.Length > i && unlockDefinitions[i].ContainsKey(category))
				{
					list.AddRange(unlockDefinitions[i][category].Definitions as T[]);
				}
			}
			if (rewardDefinitions.ContainsKey(category))
			{
				foreach (T item in rewardDefinitions[category])
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public float MascotLevelPercent(string mascotName, int xpOffset = 0)
		{
			long mascotXP = GetMascotXP(mascotName);
			long xp = addXp(mascotName, xpOffset, mascotXP);
			return GetMascotLevelPercentFromXP(xp);
		}

		public int MascotLevelXpDelta(string mascot, long mascotXP)
		{
			long num = addXp(mascot, 1, mascotXP);
			return (int)(num - mascotXP);
		}

		public int MascotLevel(string mascotName, int xpOffset = 0)
		{
			long mascotXP = GetMascotXP(mascotName);
			long xp = addXp(mascotName, xpOffset, mascotXP);
			return GetMascotLevelFromXP(xp);
		}

		public bool IsMascotMaxLevel(string mascotName, long mascotXP)
		{
			int[] value;
			if (mascotLevelXPData.TryGetValue(mascotName, out value))
			{
				int mascotLevelFromXP = GetMascotLevelFromXP(mascotXP);
				return value.Length - 1 <= mascotLevelFromXP;
			}
			return false;
		}

		public bool IsMascotMaxLevel(string mascotName)
		{
			long mascotXP = GetMascotXP(mascotName);
			return IsMascotMaxLevel(mascotName, mascotXP);
		}

		public static int GetMascotLevelFromXP(long xp)
		{
			return (int)(xp / 1000000);
		}

		public static float GetMascotLevelPercentFromXP(long xp)
		{
			return (float)(xp % 1000000) / 1000000f;
		}

		private int normalizeLevel(int level)
		{
			if (level < 0)
			{
				return 0;
			}
			return Math.Min(level, Math.Max(unlockItems.Length, unlockCounts.Length) - 1);
		}

		public long GetMascotXP(string mascotName, int xpOffset = 0)
		{
			long currentMascotXp = 0L;
			if (myMascotXP.ContainsKey(mascotName))
			{
				currentMascotXp = myMascotXP[mascotName];
			}
			return addXp(mascotName, xpOffset, currentMascotXp);
		}

		public long addXp(string mascot, int newXp, long currentMascotXp)
		{
			int mascotLevelFromXP = GetMascotLevelFromXP(currentMascotXp);
			int mascotXpFloor = getMascotXpFloor(mascot, mascotLevelFromXP);
			int mascotXpFloor2 = getMascotXpFloor(mascot, mascotLevelFromXP + 1);
			if (mascotXpFloor >= mascotXpFloor2)
			{
				return currentMascotXp;
			}
			int num = mascotXpFloor2 - mascotXpFloor;
			long num2 = (long)newXp * 1000000L / num;
			long num3 = currentMascotXp + num2;
			if (GetMascotLevelFromXP(num3) > GetMascotLevelFromXP(currentMascotXp))
			{
				long num4 = 1000000 - currentMascotXp % 1000000;
				int newXp2 = newXp - (int)(num4 * num / 1000000);
				num3 = (long)(mascotLevelFromXP + 1) * 1000000L;
				return addXp(mascot, newXp2, num3);
			}
			return num3;
		}

		private int getMascotXpFloor(string mascot, int currentLevel)
		{
			if (!mascotLevelXPData.ContainsKey(mascot))
			{
				return 0;
			}
			if (mascotLevelXPData[mascot].Length <= currentLevel)
			{
				return 0;
			}
			return mascotLevelXPData[mascot][currentLevel];
		}

		private void addListeners()
		{
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.MyAssetsReceived>(onMyRewardAssetsReceived);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.RewardsEarned>(onRewardsEarned);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.MyRewardEarned>(onMyRewardEarned);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimedReward>(onClaimedReward);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			myMascotXP.Clear();
			rewardItems.Clear();
			rewardCounts.Clear();
			rewardDefinitions.Clear();
			return false;
		}

		private bool onMyRewardAssetsReceived(RewardServiceEvents.MyAssetsReceived evt)
		{
			if (evt.Assets.mascotXP != null)
			{
				foreach (KeyValuePair<string, long> item in evt.Assets.mascotXP)
				{
					myMascotXP[item.Key] = item.Value;
				}
			}
			addRewardItems(evt.Assets);
			return false;
		}

		private bool onRewardsEarned(RewardServiceEvents.RewardsEarned evt)
		{
			long localPlayerSessionId = Service.Get<CPDataEntityCollection>().LocalPlayerSessionId;
			if (evt.RewardedUsers.rewards.ContainsKey(localPlayerSessionId))
			{
				addReward(evt.RewardedUsers.rewards[localPlayerSessionId], evt.RewardedUsers.source.ToString(), evt.RewardedUsers.sourceId);
			}
			return false;
		}

		private bool onMyRewardEarned(RewardServiceEvents.MyRewardEarned evt)
		{
			RewardSource source = evt.Source;
			if (source == RewardSource.QUEST_COMPLETED)
			{
				bool flag = true;
				Quest quest = Service.Get<QuestService>().GetQuest(evt.SourceId);
				if (quest == null)
				{
					Log.LogError(this, "QuestService returned null quest for name : " + evt.SourceId);
				}
				else if (quest.Definition.IsRewardPopupSupressed)
				{
					flag = false;
				}
				if (flag)
				{
					ShowRewardPopup showRewardPopup = new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.questComplete, evt.Reward).setRewardSource(evt.SourceId).setRewardPopupPrefabOverride(quest.Definition.RewardPopupPrefabOverride).Build();
					showRewardPopup.Execute();
				}
				addReward(evt.Reward, evt.Source.ToString(), evt.SourceId, false);
			}
			else
			{
				addReward(evt.Reward, evt.Source.ToString(), evt.SourceId, evt.ShowReward);
			}
			return false;
		}

		private bool onClaimedReward(RewardServiceEvents.ClaimedReward evt)
		{
			addReward(evt.Reward, "", "claimed_reward", false);
			return false;
		}

		private void addReward(Reward reward, string source, string source_id, bool showReward = true)
		{
			addRewardXp(reward, source, source_id, showReward);
			addRewardCoins(reward, showReward);
			addRewardItems(reward);
			addRewardConsumableInstances(reward);
			CoinReward rewardable;
			if (reward.TryGetValue(out rewardable) && !rewardable.IsEmpty())
			{
				Service.Get<ICPSwrveService>().CoinsGiven(rewardable.Coins, source, source_id);
			}
		}

		private void addRewardXp(Reward reward, string source, string source_id, bool showReward = true)
		{
			MascotXPReward rewardable;
			if (reward.TryGetValue(out rewardable))
			{
				foreach (KeyValuePair<string, int> item in rewardable.XP)
				{
					long num = 0L;
					if (myMascotXP.ContainsKey(item.Key))
					{
						num = myMascotXP[item.Key];
					}
					myMascotXP[item.Key] = addXp(item.Key, item.Value, num);
					if (source_id == "fishing")
					{
						showReward = false;
					}
					Service.Get<EventDispatcher>().DispatchEvent(new RewardEvents.AddXP(item.Key, num, myMascotXP[item.Key], item.Value, showReward));
					int num2 = (int)(num / 1000000);
					string tier = string.Format("{0}_{1}", source, item.Key);
					Service.Get<ICPSwrveService>().Action("earn_xp", tier, num2.ToString(), item.Value.ToString());
					if (num2 != myMascotXP[item.Key] / 1000000)
					{
						dispatchLevelUp(item.Key, (int)(num / 1000000));
						string tier2 = Level.ToString();
						string tier3 = string.Format("{0}:{1}", source, source_id);
						string tier4 = item.Key + "_" + (int)(myMascotXP[item.Key] / 1000000);
						Service.Get<ICPSwrveService>().Action("game.level_up", tier2, tier3, tier4);
					}
				}
			}
		}

		private void addRewardConsumableInstances(Reward reward)
		{
			ConsumableInstanceReward rewardable;
			ConsumableInventoryData component;
			if (reward.TryGetValue(out rewardable) && !rewardable.IsEmpty() && Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component))
			{
				foreach (KeyValuePair<string, int> consumable in rewardable.Consumables)
				{
					component.AddConsumable(consumable.Key, consumable.Value);
				}
			}
		}

		private void addRewardItems(Reward reward)
		{
			foreach (IRewardable item in reward)
			{
				addRewardItem(item.RewardType, item.Reward);
			}
		}

		private void addRewardItems(PlayerAssets assets)
		{
			for (int i = 0; i < playerAssetsFieldInfo.Length; i++)
			{
				addRewardItem(playerAssetsFieldInfo[i].Name, playerAssetsFieldInfo[i].GetValue(assets));
			}
		}

		private void addRewardCoins(Reward reward, bool showReward)
		{
			CoinReward rewardable;
			if (reward.TryGetValue(out rewardable) && !rewardable.IsEmpty())
			{
				Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(rewardable.Coins, false, showReward);
			}
		}

		private void addRewardItem(string identifier, object item)
		{
			if (item == null)
			{
				return;
			}
			ProgressionUnlockCategory? progressionUnlockCategory = ProgressionUtils.RewardToProgressionCategory(identifier);
			if (!progressionUnlockCategory.HasValue)
			{
				return;
			}
			ProgressionUnlockCategory value = progressionUnlockCategory.Value;
			if (typeof(IList).IsAssignableFrom(item.GetType()))
			{
				IList list = (IList)item;
				if (list == null)
				{
					return;
				}
				FieldInfo field = typeof(ProgressionUnlockDefinition).GetField(identifier);
				if (field.FieldType.IsArray)
				{
					Type elementType = field.FieldType.GetElementType();
					FieldInfo attributedField = StaticGameDataDefinitionIdAttribute.GetAttributedField(elementType);
					if (attributedField == null)
					{
						Log.LogErrorFormatted(this, "The type {0} did not contain a field with the StaticGameDataDefinitionId attribute", elementType.Name);
						return;
					}
					Type fieldType = attributedField.FieldType;
					Type type = typeof(Dictionary<, >).MakeGenericType(fieldType, elementType);
					IDictionary dictionary = (IDictionary)Service.Get<GameData>().Get(type);
					List<StaticGameDataDefinition> list2 = new List<StaticGameDataDefinition>();
					for (int i = 0; i < list.Count; i++)
					{
						StaticGameDataDefinition staticGameDataDefinition = dictionary[list[i]] as StaticGameDataDefinition;
						if (staticGameDataDefinition != null)
						{
							list2.Add(staticGameDataDefinition);
						}
					}
					if (rewardDefinitions.ContainsKey(value))
					{
						rewardDefinitions[value].UnionWith(list2);
					}
					else
					{
						rewardDefinitions[value] = new HashSet<StaticGameDataDefinition>(list2);
					}
				}
				else if (rewardItems.ContainsKey(value))
				{
					rewardItems[value].UnionWith((List<string>)list);
				}
				else
				{
					rewardItems[value] = new HashSet<string>((List<string>)list);
				}
			}
			else
			{
				if (item.GetType() != typeof(int))
				{
					return;
				}
				int num = (int)item;
				if (num > 0)
				{
					if (rewardCounts.ContainsKey(value))
					{
						rewardCounts[value] += num;
					}
					else
					{
						rewardCounts[value] = num;
					}
				}
			}
		}

		private void dispatchLevelUp(string mascotName, int previousLevel)
		{
			for (int i = previousLevel + 1; i <= MascotLevel(mascotName); i++)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ProgressionEvents.LevelUp(mascotName, i, Level - (MascotLevel(mascotName) - i)));
			}
		}

		private void parseUnlockDefinitions(Dictionary<int, ProgressionUnlockDefinition> definitions)
		{
			unlockItems = new Dictionary<ProgressionUnlockCategory, string[]>[definitions.Count];
			unlockDefinitions = new Dictionary<ProgressionUnlockCategory, UnlockDefinition>[definitions.Count];
			unlockCounts = new Dictionary<ProgressionUnlockCategory, int>[definitions.Count];
			foreach (ProgressionUnlockDefinition value2 in definitions.Values)
			{
				Dictionary<ProgressionUnlockCategory, string[]> dictionary = new Dictionary<ProgressionUnlockCategory, string[]>();
				Dictionary<ProgressionUnlockCategory, UnlockDefinition> dictionary2 = new Dictionary<ProgressionUnlockCategory, UnlockDefinition>();
				Dictionary<ProgressionUnlockCategory, int> dictionary3 = new Dictionary<ProgressionUnlockCategory, int>();
				foreach (ProgressionUnlockCategory value3 in Enum.GetValues(typeof(ProgressionUnlockCategory)))
				{
					FieldInfo field = typeof(ProgressionUnlockDefinition).GetField(value3.ToString());
					if (field == null)
					{
						Log.LogErrorFormatted(this, "Category {0} was not found in Progression Unlock Definitions.", value3);
					}
					else if (field.FieldType == typeof(List<string>))
					{
						List<string> list = (List<string>)field.GetValue(value2);
						if (list != null && list.Count > 0)
						{
							dictionary.Add(value3, list.ToArray());
						}
					}
					else if (typeof(StaticGameDataDefinition).IsAssignableFrom(field.FieldType.GetElementType()))
					{
						StaticGameDataDefinition[] array = (StaticGameDataDefinition[])field.GetValue(value2);
						if (array != null && array.Length > 0)
						{
							UnlockDefinition value = new UnlockDefinition(value2.ThemeDefinition, array);
							dictionary2.Add(value3, value);
						}
					}
					else if (field.FieldType == typeof(int))
					{
						int num = (int)field.GetValue(value2);
						if (num > 0)
						{
							dictionary3.Add(value3, num);
						}
					}
				}
				int level = value2.Level;
				if (level > maxUnlockLevel)
				{
					maxUnlockLevel = level;
				}
				unlockItems[level] = dictionary;
				unlockCounts[level] = dictionary3;
				unlockDefinitions[level] = dictionary2;
			}
		}

		private void parseMascotLevelXPManifest(Manifest mascotLevelXPManifest)
		{
			mascotLevelXPData = new Dictionary<string, int[]>();
			for (int i = 0; i < mascotLevelXPManifest.Assets.Length; i++)
			{
				ProgressionMascotLevelXPDefinition progressionMascotLevelXPDefinition = mascotLevelXPManifest.Assets[i] as ProgressionMascotLevelXPDefinition;
				mascotLevelXPData.Add(progressionMascotLevelXPDefinition.Mascot.name, progressionMascotLevelXPDefinition.Levels.ToArray());
			}
		}
	}
}
