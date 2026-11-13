using ClubPenguin.Chat;
using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatEmotePanel : AbstractProgressionLockedItems<EmoteDefinition>
	{
		internal const string RecentEmoteKey = "RecentEmoteKey";

		public List<EmoteGroupSpacingRule> SpacingRules;

		private RecentEmotesService recentEmoteService;

		private EmoteDefinition[] recentEmotesArray;

		private List<EmoteDefinition> tempRecentEmotesList;

		private EmoteDefinition _recentEmoteIcon;

		private EmoteDefinition recentEmoteIcon
		{
			get
			{
				if (_recentEmoteIcon == null)
				{
					_recentEmoteIcon = ScriptableObject.CreateInstance<EmoteDefinition>();
					_recentEmoteIcon.Id = "RecentEmoteKey";
				}
				return _recentEmoteIcon;
			}
		}

		private void Awake()
		{
			recentEmoteService = Service.Get<RecentEmotesService>();
			recentEmotesArray = new EmoteDefinition[recentEmoteService.RecentEmotesMaxCount + 1];
		}

		protected override void start()
		{
			setScrollBar();
			Service.Get<EventDispatcher>().AddListener<RecentEmotesService.RecentEmotesUpdated>(onRecentEmotesUpdated);
		}

		protected override void onDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<RecentEmotesService.RecentEmotesUpdated>(onRecentEmotesUpdated);
		}

		private bool onRecentEmotesUpdated(RecentEmotesService.RecentEmotesUpdated evt)
		{
			int count = tempRecentEmotesList.Count;
			List<EmoteDefinition> emoteDefinitionsList = recentEmoteService.GetEmoteDefinitionsList();
			for (int num = count - 1; num >= 0; num--)
			{
				if (!emoteDefinitionsList.Contains(tempRecentEmotesList[num]))
				{
					tempRecentEmotesList.RemoveAt(num);
				}
			}
			int count2 = emoteDefinitionsList.Count;
			for (int num = 0; num < count2; num++)
			{
				EmoteDefinition item = emoteDefinitionsList[num];
				if (!tempRecentEmotesList.Contains(item))
				{
					tempRecentEmotesList.Insert(0, item);
				}
			}
			recentEmotesArray[0] = recentEmoteIcon;
			tempRecentEmotesList.CopyTo(recentEmotesArray, 1);
			base.refreshGroupItems(0, recentEmotesArray, ItemGroup.LockedState.Unlocked);
			return false;
		}

		protected override void parseItemGroups()
		{
			ItemGroup.LockedState lockedState = ItemGroup.LockedState.CustomLocked;
			List<EmoteDefinition> emoteDefinitionsList = recentEmoteService.GetEmoteDefinitionsList();
			if (emoteDefinitionsList.Count > 0)
			{
				recentEmotesArray[0] = recentEmoteIcon;
				emoteDefinitionsList.CopyTo(recentEmotesArray, 1);
				lockedState = ItemGroup.LockedState.Unlocked;
			}
			tempRecentEmotesList = emoteDefinitionsList;
			createItemGroup(recentEmotesArray, lockedState, -1, null, null, true);
			base.parseItemGroups();
		}

		protected override void createItem(EmoteDefinition itemDefinition, GameObject item, ItemGroup.LockedState lockedState)
		{
			ChatEmoteItem component = item.GetComponent<ChatEmoteItem>();
			component.SetEmote(itemDefinition);
		}

		protected override Dictionary<int, ItemGroupSpacingRule<EmoteDefinition>> getSpacingRulesMap()
		{
			Dictionary<int, ItemGroupSpacingRule<EmoteDefinition>> dictionary = new Dictionary<int, ItemGroupSpacingRule<EmoteDefinition>>();
			if (SpacingRules != null)
			{
				for (int i = 0; i < SpacingRules.Count; i++)
				{
					dictionary.Add(SpacingRules[i].Count, SpacingRules[i]);
				}
			}
			return dictionary;
		}

		protected override EmoteDefinition[] getRewards(RewardDefinition rewardDefinition)
		{
			return AbstractStaticGameDataRewardDefinition<EmoteDefinition>.ToDefinitionArray(rewardDefinition.GetDefinitions<EmoteRewardDefinition>());
		}

		private void setScrollBar()
		{
			ChatScreenPanel componentInParent = GetComponentInParent<ChatScreenPanel>();
			if (componentInParent != null)
			{
				ScrollRect componentInChildren = GetComponentInChildren<ScrollRect>();
				Scrollbar scrollbar2 = componentInChildren.horizontalScrollbar = componentInParent.GetComponentsInChildren<Scrollbar>(true)[0];
				componentInChildren.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
				ScrollerAccessibilitySettings[] componentsInChildren = componentInParent.GetComponentsInChildren<ScrollerAccessibilitySettings>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Scrollbar = scrollbar2;
				}
			}
		}

		protected override void parseUnlockedItems()
		{
			Dictionary<EmoteDefinition.ECategory, List<EmoteDefinition>> dictionary = new Dictionary<EmoteDefinition.ECategory, List<EmoteDefinition>>(default(EmoteDefinition.MyECategoryComparer));
			List<EmoteDefinition> value = null;
			int count = unlockedItemsList.Count;
			for (int i = 0; i < count; i++)
			{
				EmoteDefinition emoteDefinition = unlockedItemsList[i];
				if (dictionary.TryGetValue(emoteDefinition.Category, out value))
				{
					value.Add(emoteDefinition);
					continue;
				}
				value = new List<EmoteDefinition>(10);
				value.Add(emoteDefinition);
				dictionary.Add(emoteDefinition.Category, value);
			}
			bool combineWithPreviousGroup = false;
			Array values = Enum.GetValues(typeof(EmoteDefinition.ECategory));
			foreach (object item in values)
			{
				if (dictionary.TryGetValue((EmoteDefinition.ECategory)item, out value))
				{
					createItemGroupGrouping(value, combineWithPreviousGroup);
					combineWithPreviousGroup = true;
				}
			}
		}
	}
}
