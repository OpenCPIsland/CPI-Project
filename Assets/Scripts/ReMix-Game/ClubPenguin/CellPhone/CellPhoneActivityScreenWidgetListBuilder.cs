using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenWidgetListBuilder
	{
		private CellPhoneActivityScreenDefinition definition;

		private DateTime date;

		private CellPhoneActivityScreenDefinition.AccessedWidgets accessedWidgets;

		public CellPhoneActivityScreenWidgetListBuilder(DateTime date, CellPhoneActivityScreenDefinition definition)
		{
			this.definition = definition;
			this.date = date;
			accessedWidgets = CellPhoneActivityScreenDefinition.AccessedWidgets.GetAccessedWidgets();
		}

		public List<CellPhoneActivityDefinition> Build()
		{
			Dictionary<CellPhoneActivityDefinition, int> widgetToPriority = new Dictionary<CellPhoneActivityDefinition, int>();
			if (Service.Get<QuestService>().ActiveQuest != null)
			{
				addActiveQuestWidget(widgetToPriority, Service.Get<QuestService>().ActiveQuest);
			}
			else
			{
				addDailyChallengeWidget(widgetToPriority);
				addAvailableQuestWidget(widgetToPriority);
			}
			addDailySpinWidget(widgetToPriority);
			addEventWidget(date, widgetToPriority);
			addFeatureWidget(date, widgetToPriority);
			addStartingSoonWidget(date, widgetToPriority);
			addFlashSaleWidget(date, widgetToPriority);
			addProgressionWidget(widgetToPriority);
			addClaimableRewardWidgets(date, widgetToPriority);
			return addTitleWidgesToList(getWidgetSortedByPriority(widgetToPriority));
		}

		private List<CellPhoneActivityDefinition> getWidgetSortedByPriority(Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			List<CellPhoneActivityDefinition> list = new List<CellPhoneActivityDefinition>(widgetToPriority.Keys);
			list.Sort((CellPhoneActivityDefinition a, CellPhoneActivityDefinition b) => widgetToPriority[a].CompareTo(widgetToPriority[b]));
			return list;
		}

		private void addDailySpinWidget(Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.DailySpinPriority != ActivityScreenPriorities.Hidden && Service.Get<GameStateController>().IsFTUEComplete)
			{
				CellPhoneActivityDefinition cellPhoneActivityDefinition = ScriptableObject.CreateInstance<CellPhoneActivityDefinition>();
				cellPhoneActivityDefinition.WidgetPrefabKey = definition.DailySpinWidgetKey;
				widgetToPriority.Add(cellPhoneActivityDefinition, (int)definition.DailySpinPriority);
			}
		}

		private void addEventWidget(DateTime date, Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.EventPriority == ActivityScreenPriorities.Hidden)
			{
				return;
			}
			List<CellPhoneEventActivityDefinition> scheduledItemsForDate = getScheduledItemsForDate(definition.ScheduledEvents, date);
			if (scheduledItemsForDate.Count > 0)
			{
				scheduledItemsForDate.Sort((CellPhoneEventActivityDefinition a, CellPhoneEventActivityDefinition b) => a.WidgetPriority.CompareTo(b.WidgetPriority));
				scheduledItemsForDate.RemoveAll((CellPhoneEventActivityDefinition widget) => widget.IsHiddenAfterAccessed && hasWidgetBeenAccessed(widget));
				for (int i = 0; i < scheduledItemsForDate.Count; i++)
				{
					widgetToPriority.Add(scheduledItemsForDate[i], (int)getGlobalPriorityForItem(scheduledItemsForDate[i], definition.EventPriority));
				}
			}
		}

		private void addClaimableRewardWidgets(DateTime date, Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.ClaimableRewardPriority == ActivityScreenPriorities.Hidden)
			{
				return;
			}
			ClaimedRewardIdsData claimedRewardData = Service.Get<CPDataEntityCollection>().GetComponent<ClaimedRewardIdsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			if (claimedRewardData == null)
			{
				return;
			}
			List<CellPhoneClaimableRewardActivityDefinition> scheduledItemsForDate = getScheduledItemsForDate(definition.ScheduledClaimableRewardActivities, date);
			if (scheduledItemsForDate.Count > 0)
			{
				scheduledItemsForDate.Sort((CellPhoneClaimableRewardActivityDefinition a, CellPhoneClaimableRewardActivityDefinition b) => a.WidgetPriority.CompareTo(b.WidgetPriority));
				scheduledItemsForDate.RemoveAll((CellPhoneClaimableRewardActivityDefinition widget) => claimedRewardData.RewardIds.Contains(widget.ClaimableReward.Id));
				for (int i = 0; i < scheduledItemsForDate.Count; i++)
				{
					widgetToPriority.Add(scheduledItemsForDate[i], (int)getGlobalPriorityForItem(scheduledItemsForDate[i], definition.EventPriority));
				}
			}
		}

		private void addFlashSaleWidget(DateTime date, Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.SalesPriority != ActivityScreenPriorities.Hidden)
			{
				List<CellPhoneSaleActivityDefinition> scheduledItemsForDate = getScheduledItemsForDate(definition.ScheduledSales, date);
				for (int i = 0; i < scheduledItemsForDate.Count; i++)
				{
					widgetToPriority.Add(scheduledItemsForDate[i], (int)getGlobalPriorityForItem(scheduledItemsForDate[i], definition.FeaturePriority));
				}
			}
		}

		private void addFeatureWidget(DateTime date, Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.FeaturePriority == ActivityScreenPriorities.Hidden)
			{
				return;
			}
			List<CellPhoneFeatureActivityDefinition> scheduledItemsForDate = getScheduledItemsForDate(definition.ScheduledFeatures, date);
			if (scheduledItemsForDate.Count > 0)
			{
				scheduledItemsForDate.Sort((CellPhoneFeatureActivityDefinition a, CellPhoneFeatureActivityDefinition b) => a.WidgetPriority.CompareTo(b.WidgetPriority));
				scheduledItemsForDate.RemoveAll((CellPhoneFeatureActivityDefinition widget) => widget.IsHiddenAfterAccessed && hasWidgetBeenAccessed(widget));
				for (int i = 0; i < scheduledItemsForDate.Count; i++)
				{
					widgetToPriority.Add(scheduledItemsForDate[i], (int)getGlobalPriorityForItem(scheduledItemsForDate[i], definition.FeaturePriority));
				}
			}
		}

		private void addDailyChallengeWidget(Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.DailiesPriority != ActivityScreenPriorities.Hidden)
			{
				CellPhoneActivityDefinition cellPhoneActivityDefinition = ScriptableObject.CreateInstance<CellPhoneActivityDefinition>();
				cellPhoneActivityDefinition.WidgetPrefabKey = definition.DailyChallengeWidgetKey;
				widgetToPriority.Add(cellPhoneActivityDefinition, (int)definition.DailiesPriority);
			}
		}

		private void addAvailableQuestWidget(Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.AvailableQuestPriority != ActivityScreenPriorities.Hidden)
			{
				QuestDefinition availableQuest = getAvailableQuest();
				if (availableQuest != null)
				{
					CellPhoneQuestActivityDefinition cellPhoneQuestActivityDefinition = ScriptableObject.CreateInstance<CellPhoneQuestActivityDefinition>();
					cellPhoneQuestActivityDefinition.Quest = availableQuest;
					cellPhoneQuestActivityDefinition.WidgetPrefabKey = definition.AvailableQuestWidgetKey;
					widgetToPriority.Add(cellPhoneQuestActivityDefinition, (int)definition.AvailableQuestPriority);
				}
			}
		}

		private void addActiveQuestWidget(Dictionary<CellPhoneActivityDefinition, int> widgetToPriority, Quest quest)
		{
			if (definition.ActiveQuestPriority != ActivityScreenPriorities.Hidden)
			{
				CellPhoneQuestActivityDefinition cellPhoneQuestActivityDefinition = ScriptableObject.CreateInstance<CellPhoneQuestActivityDefinition>();
				cellPhoneQuestActivityDefinition.Quest = quest.Definition;
				cellPhoneQuestActivityDefinition.WidgetPrefabKey = definition.ActiveQuestWidgetKey;
				widgetToPriority.Add(cellPhoneQuestActivityDefinition, (int)definition.ActiveQuestPriority);
			}
		}

		private void addProgressionWidget(Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			ProgressionService progressionService = Service.Get<ProgressionService>();
			if (definition.ProgressionPriority != ActivityScreenPriorities.Hidden && progressionService.Level < progressionService.MaxUnlockLevel)
			{
				Mascot mascotClosestToNextLevel = getMascotClosestToNextLevel();
				if (mascotClosestToNextLevel != null && progressionService.MascotLevelPercent(mascotClosestToNextLevel.Name) >= definition.PercentToNextLevelToShowProgressionWidget)
				{
					string tipForMascot = getTipForMascot(mascotClosestToNextLevel.Definition);
					Reward rewardForProgressionLevel = getRewardForProgressionLevel(progressionService.Level + 1);
					CellPhoneProgressionActivityDefinition cellPhoneProgressionActivityDefinition = ScriptableObject.CreateInstance<CellPhoneProgressionActivityDefinition>();
					cellPhoneProgressionActivityDefinition.Mascot = mascotClosestToNextLevel;
					cellPhoneProgressionActivityDefinition.TipToken = tipForMascot;
					cellPhoneProgressionActivityDefinition.RewardItems = rewardForProgressionLevel;
					cellPhoneProgressionActivityDefinition.WidgetPrefabKey = definition.ProgressionWidgetKey;
					widgetToPriority.Add(cellPhoneProgressionActivityDefinition, (int)definition.ProgressionPriority);
				}
			}
		}

		private string getTipForMascot(MascotDefinition mascot)
		{
			string result = "";
			for (int i = 0; i < definition.TipsData.Count; i++)
			{
				if (definition.TipsData[i].Mascot == mascot)
				{
					if (definition.TipsData[i].Tips.Length > 0)
					{
						result = definition.TipsData[i].Tips[UnityEngine.Random.Range(0, definition.TipsData[i].Tips.Length)];
					}
					break;
				}
			}
			return result;
		}

		private Reward getRewardForProgressionLevel(int level)
		{
			Reward result = new Reward();
			for (int i = 0; i < definition.LevelData.Count; i++)
			{
				if (definition.LevelData[i].Level == level)
				{
					result = definition.LevelData[i].RewardItems.ToReward();
					break;
				}
			}
			return result;
		}

		private void addStartingSoonWidget(DateTime date, Dictionary<CellPhoneActivityDefinition, int> widgetToPriority)
		{
			if (definition.ActivityPriority != ActivityScreenPriorities.Hidden)
			{
				CellPhoneActivityDefinition cellPhoneActivityDefinition = ScriptableObject.CreateInstance<CellPhoneActivityDefinition>();
				cellPhoneActivityDefinition.WidgetPrefabKey = definition.RecurringActivityWidgetKey;
				widgetToPriority.Add(cellPhoneActivityDefinition, (int)definition.ActivityPriority);
			}
		}

		private List<T> getScheduledItemsForDate<T>(List<T> scheduledItems, DateTime date) where T : ICellPhoneScheduledActivityDefinition
		{
			List<T> list = new List<T>();
			foreach (T scheduledItem in scheduledItems)
			{
				if (isDateWithinScheduledItemAvailability(date, scheduledItem))
				{
					list.Add(scheduledItem);
				}
			}
			return list;
		}

		private bool isDateWithinScheduledItemAvailability(DateTime date, ICellPhoneScheduledActivityDefinition scheduledItem)
		{
			return DateTimeUtils.DoesDateFallBetween(date, scheduledItem.GetStartingDate().Date, scheduledItem.GetEndingDate().Date);
		}

		private ActivityScreenPriorities getGlobalPriorityForItem(CellPhoneActivityDefinition item, ActivityScreenPriorities globalPriorityForType)
		{
			return (item.WidgetGlobalPriorityOverride == ActivityScreenPriorities.None) ? globalPriorityForType : item.WidgetGlobalPriorityOverride;
		}

		private QuestDefinition getAvailableQuest()
		{
			QuestDefinition result = null;
			IEnumerable<Mascot> mascots = Service.Get<MascotService>().Mascots;
			foreach (Mascot item in mascots)
			{
				if (item.IsQuestGiver && item.HasAvailableQuests())
				{
					result = item.GetNextAvailableQuest();
					break;
				}
			}
			return result;
		}

		private Mascot getMascotClosestToNextLevel()
		{
			ProgressionService progressionService = Service.Get<ProgressionService>();
			Mascot mascot = null;
			foreach (Mascot mascot2 in Service.Get<MascotService>().Mascots)
			{
				if (mascot2.IsQuestGiver && (mascot == null || progressionService.MascotLevelPercent(mascot2.Definition.name) > progressionService.MascotLevelPercent(mascot.Definition.name)))
				{
					mascot = mascot2;
				}
			}
			return mascot;
		}

		private bool hasWidgetBeenAccessed(CellPhoneActivityDefinition widgetData)
		{
			return accessedWidgets != null && accessedWidgets.Widgets.Contains(widgetData.name);
		}

		private List<CellPhoneActivityDefinition> addTitleWidgesToList(List<CellPhoneActivityDefinition> widgetDatas)
		{
			List<CellPhoneActivityDefinition> list = new List<CellPhoneActivityDefinition>();
			for (int i = 0; i < widgetDatas.Count; i++)
			{
				if (widgetDatas[i] as CellPhoneFeatureActivityDefinition != null)
				{
					if (i == 0 || widgetDatas[i - 1] as CellPhoneFeatureActivityDefinition == null)
					{
						CellPhoneActivityDefinition cellPhoneActivityDefinition = ScriptableObject.CreateInstance<CellPhoneActivityDefinition>();
						cellPhoneActivityDefinition.WidgetPrefabKey = definition.FeatureTitleWidgetKey;
						list.Add(cellPhoneActivityDefinition);
					}
				}
				else if (widgetDatas[i] as CellPhoneEventActivityDefinition != null && (i == 0 || widgetDatas[i - 1] as CellPhoneEventActivityDefinition == null))
				{
					CellPhoneActivityDefinition cellPhoneActivityDefinition = ScriptableObject.CreateInstance<CellPhoneActivityDefinition>();
					cellPhoneActivityDefinition.WidgetPrefabKey = definition.EventTitleWidgetKey;
					list.Add(cellPhoneActivityDefinition);
				}
				list.Add(widgetDatas[i]);
			}
			return list;
		}
	}
}
