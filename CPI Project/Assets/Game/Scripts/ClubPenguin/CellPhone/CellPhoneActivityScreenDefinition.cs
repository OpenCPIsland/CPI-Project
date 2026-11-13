using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenDefinition : StaticGameDataDefinition
	{
		[Serializable]
		public class ProgressionTipsData
		{
			public MascotDefinition Mascot;

			public string[] Tips;
		}

		[Serializable]
		public class ProgressionLevelData
		{
			public int Level;

			public RewardDefinition RewardItems;
		}

		[Serializable]
		public class AccessedWidgets
		{
			public List<string> Widgets;

			public AccessedWidgets()
			{
				Widgets = new List<string>();
			}

			private static string GetPlatformKey(string key)
			{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
				if (Application.isEditor)
				{
					return "Editor_" + key;
				}
#endif
				return key;
			}

			public static AccessedWidgets GetAccessedWidgets()
			{
				if (PlayerPrefs.HasKey(GetPlatformKey("CellPhone_Accessed_Widgets")))
				{
					string @string = PlayerPrefs.GetString(GetPlatformKey("CellPhone_Accessed_Widgets"));
					try
					{
						return Service.Get<JsonService>().Deserialize<AccessedWidgets>(@string);
					}
					catch
					{
						return new AccessedWidgets();
					}
				}
				return new AccessedWidgets();
			}

			public static void SaveAccessedWidgets(AccessedWidgets widgets)
			{
				string value = Service.Get<JsonService>().Serialize(widgets);
				PlayerPrefs.SetString(GetPlatformKey("CellPhone_Accessed_Widgets"), value);
			}
		}

		public const string AVAILABLE_QUEST_PREFAB_KEY = "Prefabs/CellPhoneActivityScreen/Widgets/AvailableQuest/AvailableQuest_{0}";

		public const string ACCESSED_WIDGETS_PLAYER_PREFS_KEY = "CellPhone_Accessed_Widgets";

		public ActivityScreenPriorities DailySpinPriority = ActivityScreenPriorities.First;

		public ActivityScreenPriorities ActiveQuestPriority = ActivityScreenPriorities.Third;

		public ActivityScreenPriorities EventPriority = ActivityScreenPriorities.Fourth;

		public ActivityScreenPriorities FeaturePriority = ActivityScreenPriorities.Fifth;

		public ActivityScreenPriorities DailiesPriority = ActivityScreenPriorities.Sixth;

		public ActivityScreenPriorities AvailableQuestPriority = ActivityScreenPriorities.Seventh;

		public ActivityScreenPriorities ActivityPriority = ActivityScreenPriorities.Eighth;

		public ActivityScreenPriorities SalesPriority = ActivityScreenPriorities.Ninth;

		public ActivityScreenPriorities ProgressionPriority = ActivityScreenPriorities.Tenth;

		public ActivityScreenPriorities ClaimableRewardPriority = ActivityScreenPriorities.Eleventh;

		public PrefabContentKey ActiveQuestWidgetKey;

		public PrefabContentKey DailyChallengeWidgetKey;

		public PrefabContentKey ProgressionWidgetKey;

		public PrefabContentKey AvailableQuestWidgetKey;

		public PrefabContentKey EventTitleWidgetKey;

		public PrefabContentKey FeatureTitleWidgetKey;

		public PrefabContentKey RecurringActivityWidgetKey;

		public PrefabContentKey DailySpinWidgetKey;

		public float PercentToNextLevelToShowProgressionWidget = 0.7f;

		public List<ProgressionTipsData> TipsData;

		public List<ProgressionLevelData> LevelData;

		public List<CellPhoneEventActivityDefinition> ScheduledEvents;

		public List<CellPhoneFeatureActivityDefinition> ScheduledFeatures;

		public List<CellPhoneRecurringLocationActivityDefinition> ScheduledRecurringActivities;

		public List<CellPhoneClaimableRewardActivityDefinition> ScheduledClaimableRewardActivities;

		public List<CellPhoneSaleActivityDefinition> ScheduledSales;
	}
}
