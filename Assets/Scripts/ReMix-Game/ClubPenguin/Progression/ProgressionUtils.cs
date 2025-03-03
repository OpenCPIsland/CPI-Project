using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Progression
{
	public static class ProgressionUtils
	{
		public class ParsedProgression<T> where T : StaticGameDataDefinition, IMemberLocked
		{
			public readonly T Definition;

			public readonly int Level;

			public readonly string MascotName;

			public readonly bool LevelLocked;

			public readonly bool ProgressionLocked;

			public readonly bool MemberLocked;

			public ParsedProgression(T definition, int level, string mascotName, bool levelLocked, bool progressionLocked, bool memberLocked)
			{
				Definition = definition;
				Level = level;
				MascotName = mascotName;
				LevelLocked = levelLocked;
				ProgressionLocked = progressionLocked;
				MemberLocked = memberLocked;
			}
		}

		public static ProgressionUnlockCategory? RewardToProgressionCategory(string rewardCategory)
		{
			ProgressionUnlockCategory? result = null;
			if (Enum.IsDefined(typeof(ProgressionUnlockCategory), rewardCategory))
			{
				return Enum.Parse(typeof(ProgressionUnlockCategory), rewardCategory) as ProgressionUnlockCategory?;
			}
			return result;
		}

		public static List<ParsedProgression<TDefinition>> RetrieveProgressionLockedItems<TDefinition, TReward>(ProgressionUnlockCategory category, Func<List<TReward>, TDefinition[]> getRewards) where TDefinition : StaticGameDataDefinition, IMemberLocked where TReward : AbstractStaticGameDataRewardDefinition<TDefinition>
		{
			ProgressionService progressionService = Service.Get<ProgressionService>();
			TDefinition[] unlockedDefinitionsForCategory = progressionService.GetUnlockedDefinitionsForCategory<TDefinition>(category);
			HashSet<TDefinition> hashSet = new HashSet<TDefinition>(unlockedDefinitionsForCategory);
			int level = progressionService.Level;
			List<ParsedProgression<TDefinition>> list = new List<ParsedProgression<TDefinition>>();
			int lastUnlockedIndex = -1;
			bool isLocalPlayerMember = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			for (int i = 0; i <= progressionService.MaxUnlockLevel; i++)
			{
				TDefinition[] array = progressionService.GetUnlockedDefinitionsForLevel(i, category).Definitions as TDefinition[];
				if (array == null || array.Length <= 0)
				{
					continue;
				}
				bool flag = i > level;
				for (int j = 0; j < array.Length; j++)
				{
					if ((UnityEngine.Object)array[j] != (UnityEngine.Object)null)
					{
						ParsedProgression<TDefinition> parsedProgression = new ParsedProgression<TDefinition>(array[j], i, null, flag, false, array[j].IsMemberOnly);
						addParsedProgression(list, parsedProgression, flag, isLocalPlayerMember, ref lastUnlockedIndex);
						if (hashSet.Contains(array[j]))
						{
							hashSet.Remove(array[j]);
						}
					}
				}
			}
			QuestService questService = Service.Get<QuestService>();
			Dictionary<string, Mascot> questToMascotMap = questService.QuestToMascotMap;
			foreach (QuestDefinition knownQuest in questService.KnownQuests)
			{
				Mascot value;
				questToMascotMap.TryGetValue(knownQuest.name, out value);
				if (value != null)
				{
					string name = value.Name;
					questService.GetQuest(knownQuest);
					if (knownQuest.StartReward != null)
					{
						parseRewardDefinition(list, getRewards(knownQuest.StartReward.GetDefinitions<TReward>()), hashSet, progressionService, category, name, ref lastUnlockedIndex);
					}
					if (knownQuest.CompleteReward != null)
					{
						parseRewardDefinition(list, getRewards(knownQuest.CompleteReward.GetDefinitions<TReward>()), hashSet, progressionService, category, name, ref lastUnlockedIndex);
					}
					if (knownQuest.ObjectiveRewards != null)
					{
						for (int j = 0; j < knownQuest.ObjectiveRewards.Length; j++)
						{
							parseRewardDefinition(list, getRewards(knownQuest.ObjectiveRewards[j].GetDefinitions<TReward>()), hashSet, progressionService, category, name, ref lastUnlockedIndex);
						}
					}
				}
			}
			if (hashSet.Count > 0)
			{
				TDefinition[] array2 = new TDefinition[hashSet.Count];
				hashSet.CopyTo(array2);
				for (int j = 0; j < array2.Length; j++)
				{
					ParsedProgression<TDefinition> parsedProgression = new ParsedProgression<TDefinition>(array2[j], -1, null, false, false, array2[j].IsMemberOnly);
					lastUnlockedIndex++;
					list.Insert(lastUnlockedIndex, parsedProgression);
				}
			}
			return list;
		}

		private static void parseRewardDefinition<T>(List<ParsedProgression<T>> parsedProgressionList, T[] unlocks, HashSet<T> progressionUnlockedRewards, ProgressionService progressionService, ProgressionUnlockCategory category, string mascotName, ref int lastUnlockedIndex) where T : StaticGameDataDefinition, IMemberLocked
		{
			if (unlocks == null || unlocks.Length <= 0)
			{
				return;
			}
			bool flag = false;
			bool isLocalPlayerMember = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			for (int i = 0; i < unlocks.Length; i++)
			{
				if ((UnityEngine.Object)unlocks[i] != (UnityEngine.Object)null)
				{
					if (!progressionService.IsUnlocked(unlocks[i], category))
					{
						flag = true;
					}
					ParsedProgression<T> parsedProgression = new ParsedProgression<T>(unlocks[i], -1, mascotName, false, flag, unlocks[i].IsMemberOnly);
					addParsedProgression(parsedProgressionList, parsedProgression, flag, isLocalPlayerMember, ref lastUnlockedIndex);
					if (progressionUnlockedRewards.Contains(unlocks[i]))
					{
						progressionUnlockedRewards.Remove(unlocks[i]);
					}
				}
			}
		}

		private static void addParsedProgression<T>(List<ParsedProgression<T>> parsedProgressionList, ParsedProgression<T> parsedProgression, bool isProgressionLocked, bool isLocalPlayerMember, ref int lastUnlockedIndex) where T : StaticGameDataDefinition, IMemberLocked
		{
			if (isLocalPlayerMember)
			{
				if (isProgressionLocked)
				{
					parsedProgressionList.Add(parsedProgression);
					return;
				}
				lastUnlockedIndex++;
				parsedProgressionList.Insert(lastUnlockedIndex, parsedProgression);
			}
			else if (parsedProgression.MemberLocked)
			{
				parsedProgressionList.Add(parsedProgression);
			}
			else
			{
				lastUnlockedIndex++;
				parsedProgressionList.Insert(lastUnlockedIndex, parsedProgression);
			}
		}
	}
}
