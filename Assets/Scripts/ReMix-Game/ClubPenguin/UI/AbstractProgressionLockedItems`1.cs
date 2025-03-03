using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.DisneyStore;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(HorizontalScrollingLayoutElementPool))]
	public abstract class AbstractProgressionLockedItems<T> : MonoBehaviour where T : StaticGameDataDefinition, IMemberLocked
	{
		protected class ItemGroup
		{
			public enum LockedState
			{
				Unlocked,
				MemberLocked,
				LevelLocked,
				MascotLocked,
				CustomLocked
			}

			public T[] Items;

			public LockedState State;

			public int Level;

			public SpriteContentKey MascotIconContentKey;

			public string LocalizedThemeName;

			public SpriteContentKey ThemeIconContentKey;

			public GroupPosition GroupPosition;

			public bool IsCombined;

			public BGImage BGImage;

			public bool AddSpacingBefore
			{
				get
				{
					return BGImage == BGImage.Default || BGImage == BGImage.Start;
				}
			}

			public ItemGroup(T[] items, LockedState lockedState)
			{
				State = lockedState;
				Items = items;
				Level = -1;
				MascotIconContentKey = null;
				LocalizedThemeName = null;
				ThemeIconContentKey = null;
				BGImage = BGImage.Default;
				GroupPosition = GroupPosition.None;
			}
		}

		public RectTransform ScrollContentParentTransform;

		public ProgressionUnlockCategory UnlockCategory;

		public PrefabContentKey ItemContentKey;

		public PrefabContentKey LockedItemsContentKey;

		public float LockedSpacing;

		public bool ShowUnlockedBGs = true;

		[Header("Unlocked Items Settings")]
		public int UnlockedItemsGroupCount;

		public bool GroupAllUnlockedItems;

		public int UnlockedArrayIncrement = 50;

		public float GroupSpacing = 0f;

		public float CombinedGroupSpacing = 0f;

		private GameObject itemPrefab;

		private ProgressionService progressionService;

		private QuestService questService;

		private HashSet<T> progressionUnlockedRewards;

		private List<ItemGroup> itemGroups;

		private int lastUnlockedIndex = -1;

		private HorizontalScrollingLayoutElementPool layoutElementPool;

		private GameObjectPool itemPool;

		private bool isLayoutElementPoolReady;

		private Localizer localizer;

		private Dictionary<int, ItemGroupSpacingRule<T>> spacingRulesMap;

		private Vector2 zeroSpacing;

		private Vector2 groupSpacing;

		private Vector2 combinedGroupSpacing;

		protected List<T> unlockedItemsList;

		private PrefabContentKey gameObjectPoolContentKey = new PrefabContentKey("Pooling/GameObjectPool");

		private void Start()
		{
			unlockedItemsList = new List<T>(UnlockedArrayIncrement);
			progressionService = Service.Get<ProgressionService>();
			questService = Service.Get<QuestService>();
			itemGroups = new List<ItemGroup>();
			localizer = Service.Get<Localizer>();
			layoutElementPool = GetComponent<HorizontalScrollingLayoutElementPool>();
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool = layoutElementPool;
			horizontalScrollingLayoutElementPool.OnPoolReady = (System.Action)Delegate.Combine(horizontalScrollingLayoutElementPool.OnPoolReady, new System.Action(onPoolReady));
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool2 = layoutElementPool;
			horizontalScrollingLayoutElementPool2.OnElementShown = (Action<int, GameObject>)Delegate.Combine(horizontalScrollingLayoutElementPool2.OnElementShown, new Action<int, GameObject>(onElementShown));
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool3 = layoutElementPool;
			horizontalScrollingLayoutElementPool3.OnElementHidden = (Action<int, GameObject>)Delegate.Combine(horizontalScrollingLayoutElementPool3.OnElementHidden, new Action<int, GameObject>(onElementHidden));
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool4 = layoutElementPool;
			horizontalScrollingLayoutElementPool4.OnElementRefreshed = (Action<int, GameObject>)Delegate.Combine(horizontalScrollingLayoutElementPool4.OnElementRefreshed, new Action<int, GameObject>(onElementRefreshed));
			zeroSpacing = Vector2.zero;
			groupSpacing = new Vector2(GroupSpacing, 0f);
			combinedGroupSpacing = new Vector2(CombinedGroupSpacing, 0f);
			spacingRulesMap = getSpacingRulesMap();
			parseItemGroups();
			Content.LoadAsync(onItemLoaded, ItemContentKey);
			Content.LoadAsync(onLockedItemsLoaded, LockedItemsContentKey);
			Content.LoadAsync(onGameObjectPoolLoaded, gameObjectPoolContentKey);
			start();
		}

		private void OnDestroy()
		{
			onDestroy();
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool = layoutElementPool;
			horizontalScrollingLayoutElementPool.OnPoolReady = (System.Action)Delegate.Remove(horizontalScrollingLayoutElementPool.OnPoolReady, new System.Action(onPoolReady));
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool2 = layoutElementPool;
			horizontalScrollingLayoutElementPool2.OnElementShown = (Action<int, GameObject>)Delegate.Remove(horizontalScrollingLayoutElementPool2.OnElementShown, new Action<int, GameObject>(onElementShown));
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool3 = layoutElementPool;
			horizontalScrollingLayoutElementPool3.OnElementHidden = (Action<int, GameObject>)Delegate.Remove(horizontalScrollingLayoutElementPool3.OnElementHidden, new Action<int, GameObject>(onElementHidden));
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool4 = layoutElementPool;
			horizontalScrollingLayoutElementPool4.OnElementRefreshed = (Action<int, GameObject>)Delegate.Remove(horizontalScrollingLayoutElementPool4.OnElementRefreshed, new Action<int, GameObject>(onElementRefreshed));
			unlockedItemsList.Clear();
		}

		protected virtual void start()
		{
		}

		protected virtual void onDestroy()
		{
		}

		protected virtual Dictionary<int, ItemGroupSpacingRule<T>> getSpacingRulesMap()
		{
			return new Dictionary<int, ItemGroupSpacingRule<T>>();
		}

		private void onItemLoaded(string path, GameObject itemPrefab)
		{
			this.itemPrefab = itemPrefab;
			if (itemPool != null)
			{
				initializeItemPool();
			}
		}

		private void onLockedItemsLoaded(string path, GameObject lockedItemsPrefab)
		{
			layoutElementPool.SetPrefabToInstance(lockedItemsPrefab);
		}

		private void onGameObjectPoolLoaded(string path, GameObject gameObjectPoolPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gameObjectPoolPrefab);
			gameObject.transform.SetParent(base.transform, false);
			itemPool = gameObject.GetComponent<GameObjectPool>();
			if (itemPrefab != null)
			{
				initializeItemPool();
			}
		}

		private void initializeItemPool()
		{
			itemPool.PrefabToInstance = itemPrefab;
			itemPool.enabled = true;
			if (isLayoutElementPoolReady)
			{
				addElements();
			}
		}

		private void onPoolReady()
		{
			HorizontalScrollingLayoutElementPool horizontalScrollingLayoutElementPool = layoutElementPool;
			horizontalScrollingLayoutElementPool.OnPoolReady = (System.Action)Delegate.Remove(horizontalScrollingLayoutElementPool.OnPoolReady, new System.Action(onPoolReady));
			isLayoutElementPoolReady = true;
			if (itemPool.enabled)
			{
				addElements();
			}
		}

		private void addElements()
		{
			for (int i = 0; i < itemGroups.Count; i++)
			{
				ItemGroup itemGroup = itemGroups[i];
				if (itemGroup.AddSpacingBefore)
				{
					layoutElementPool.AddSpacing(LockedSpacing);
				}
				Vector2 additionalPadding = zeroSpacing;
				if (itemGroup.GroupPosition != 0 && itemGroup.BGImage != BGImage.End)
				{
					additionalPadding = ((itemGroup.GroupPosition != GroupPosition.End && itemGroup.GroupPosition != GroupPosition.StartEnd) ? groupSpacing : combinedGroupSpacing);
				}
				layoutElementPool.AddElement(itemGroup.Items.Length, 0, itemGroup.GroupPosition != GroupPosition.None, additionalPadding);
			}
			elementsAdded();
		}

		protected virtual void elementsAdded()
		{
		}

		protected virtual void parseItemGroups()
		{
			T[] collection = filterDefinitions(progressionService.GetUnlockedDefinitionsForCategory<T>(UnlockCategory));
			progressionUnlockedRewards = new HashSet<T>(collection);
			bool flag = isLocalPlayerMember();
			for (int i = 0; i <= progressionService.MaxUnlockLevel; i++)
			{
				ProgressionService.UnlockDefinition unlockedDefinitionsForLevel = progressionService.GetUnlockedDefinitionsForLevel(i, UnlockCategory);
				T[] array = filterDefinitions(unlockedDefinitionsForLevel.Definitions as T[]);
				List<T> list = new List<T>();
				List<T> list2 = new List<T>();
				List<T> list3 = new List<T>();
				if (array == null || array.Length <= 0)
				{
					continue;
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].IsMemberOnly && !flag)
					{
						list.Add(array[j]);
					}
					else if (i > progressionService.Level)
					{
						list2.Add(array[j]);
					}
					else
					{
						list3.Add(array[j]);
					}
				}
				if (list3.Count > 0)
				{
					addItems(list3.ToArray(), ItemGroup.LockedState.Unlocked, i, null, unlockedDefinitionsForLevel.ThemeDefinition);
				}
				if (list2.Count > 0)
				{
					addItems(list2.ToArray(), ItemGroup.LockedState.LevelLocked, i, null, unlockedDefinitionsForLevel.ThemeDefinition);
				}
				if (list.Count > 0)
				{
					addItems(list.ToArray(), ItemGroup.LockedState.MemberLocked, i, null, unlockedDefinitionsForLevel.ThemeDefinition);
				}
				removeUnlocksFromRewardSet(array);
			}
			Dictionary<string, Mascot> questToMascotMap = questService.QuestToMascotMap;
			foreach (QuestDefinition knownQuest in questService.KnownQuests)
			{
				Mascot value;
				questToMascotMap.TryGetValue(knownQuest.name, out value);
				if (value != null)
				{
					if (knownQuest.StartReward != null)
					{
						CPRewardDefinition cPRewardDefinition = knownQuest.StartReward as CPRewardDefinition;
						parseRewardDefinition(getRewards(knownQuest.StartReward), flag, value.Definition.ProgressionLockedIconContentKey, cPRewardDefinition.ThemeDefinition);
					}
					if (knownQuest.CompleteReward != null)
					{
						CPRewardDefinition cPRewardDefinition = knownQuest.CompleteReward as CPRewardDefinition;
						parseRewardDefinition(getRewards(knownQuest.CompleteReward), flag, value.Definition.ProgressionLockedIconContentKey, cPRewardDefinition.ThemeDefinition);
					}
					if (knownQuest.ObjectiveRewards != null)
					{
						for (int i = 0; i < knownQuest.ObjectiveRewards.Length; i++)
						{
							CPRewardDefinition cPRewardDefinition = knownQuest.ObjectiveRewards[i] as CPRewardDefinition;
							parseRewardDefinition(getRewards(knownQuest.ObjectiveRewards[i]), flag, value.Definition.ProgressionLockedIconContentKey, cPRewardDefinition.ThemeDefinition);
						}
					}
				}
			}
			parseDisneyShopRewards();
			parseClaimableRewards();
			if (progressionUnlockedRewards.Count > 0)
			{
				if (flag)
				{
					T[] array2 = new T[progressionUnlockedRewards.Count];
					progressionUnlockedRewards.CopyTo(array2);
					addItems(array2, ItemGroup.LockedState.Unlocked, -1);
				}
				else
				{
					List<T> list4 = new List<T>();
					List<T> list5 = new List<T>();
					foreach (T progressionUnlockedReward in progressionUnlockedRewards)
					{
						T current2 = progressionUnlockedReward;
						if (current2.IsMemberOnly)
						{
							list4.Add(current2);
						}
						else
						{
							list5.Add(current2);
						}
					}
					if (list5.Count > 0)
					{
						T[] array2 = new T[progressionUnlockedRewards.Count];
						list5.CopyTo(array2);
						addItems(array2, ItemGroup.LockedState.Unlocked, -1);
					}
					if (list4.Count > 0)
					{
						T[] array2 = new T[progressionUnlockedRewards.Count];
						list4.CopyTo(array2);
						addItems(array2, ItemGroup.LockedState.MemberLocked, -1);
					}
				}
			}
			parseUnlockedItems();
		}

		protected void parseDisneyShopRewards()
		{
			Dictionary<int, DisneyStoreFranchiseDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DisneyStoreFranchiseDefinition>>();
			Dictionary<int, DisneyStoreFranchiseDefinition>.Enumerator enumerator = dictionary.GetEnumerator();
			bool isMember = isLocalPlayerMember();
			while (enumerator.MoveNext())
			{
				DisneyStoreFranchiseDefinition value = enumerator.Current.Value;
				for (int i = 0; i < value.Items.Count; i++)
				{
					DisneyStoreItemData disneyStoreItemData = new DisneyStoreItemData(value.Items[i]);
					if (disneyStoreItemData.Definition != null)
					{
						if (DisneyStoreUtils.IsItemOwned(disneyStoreItemData))
						{
							parseRewardDefinition(getRewards(value.Items[i].Reward), isMember, new SpriteContentKey("Images/ProgressionIcons/Quests_ProgressionLock_Membership"), value.Items[i].ThemeDefinition);
						}
					}
					else
					{
						Log.LogError(this, string.Format("Franchise contains null item: {0}", value.name));
					}
				}
			}
		}

		protected void parseClaimableRewards()
		{
			ClaimedRewardIdsData component = Service.Get<CPDataEntityCollection>().GetComponent<ClaimedRewardIdsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			List<int> rewardIds = component.RewardIds;
			Dictionary<int, ClaimableRewardDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, ClaimableRewardDefinition>>();
			bool isMember = isLocalPlayerMember();
			for (int i = 0; i < rewardIds.Count; i++)
			{
				ClaimableRewardDefinition value;
				if (dictionary.TryGetValue(rewardIds[i], out value))
				{
					CPRewardDefinition cPRewardDefinition = value.Reward as CPRewardDefinition;
					parseRewardDefinition(getRewards(value.Reward), isMember, new SpriteContentKey("Images/ProgressionIcons/Quests_ProgressionLock_Membership"), cPRewardDefinition.ThemeDefinition);
				}
			}
		}

		protected virtual T[] filterDefinitions(T[] definitions)
		{
			return definitions;
		}

		protected abstract T[] getRewards(RewardDefinition rewardDefinition);

		private void parseRewardDefinition(T[] unlocks, bool isMember, SpriteContentKey mascotIconContentKey, RewardThemeDefinition themeDefinition)
		{
			T[] array = filterDefinitions(unlocks);
			if (array == null || array.Length <= 0)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (!progressionService.IsUnlocked(array[i], UnlockCategory))
				{
					flag = true;
				}
				if (!isMember && array[i].IsMemberOnly)
				{
					flag2 = true;
					break;
				}
			}
			ItemGroup.LockedState lockedState = flag2 ? ItemGroup.LockedState.MemberLocked : (flag ? ItemGroup.LockedState.MascotLocked : ItemGroup.LockedState.Unlocked);
			addItems(array, lockedState, -1, mascotIconContentKey, themeDefinition);
			removeUnlocksFromRewardSet(array);
		}

		private void removeUnlocksFromRewardSet(T[] unlocks)
		{
			for (int i = 0; i < unlocks.Length; i++)
			{
				if (progressionUnlockedRewards.Contains(unlocks[i]))
				{
					progressionUnlockedRewards.Remove(unlocks[i]);
				}
			}
		}

		protected virtual void parseUnlockedItems()
		{
			createItemGroupGrouping(unlockedItemsList);
		}

		protected void createItemGroupGrouping(List<T> items, bool combineWithPreviousGroup = false)
		{
			int num = 0;
			int count = items.Count;
			if (count > 0)
			{
				bool flag = false;
				ItemGroup itemGroup = null;
				if (combineWithPreviousGroup && lastUnlockedIndex >= 0)
				{
					ItemGroup itemGroup2 = itemGroups[lastUnlockedIndex];
					itemGroup2.BGImage = ((itemGroup2.BGImage == BGImage.Default) ? BGImage.Start : BGImage.Middle);
					flag = true;
				}
				while (num < count)
				{
					int num2 = Math.Min(UnlockedItemsGroupCount, count - num);
					T[] items2 = items.GetRange(num, num2).ToArray();
					itemGroup = new ItemGroup(items2, ItemGroup.LockedState.Unlocked);
					itemGroup.IsCombined = combineWithPreviousGroup;
					itemGroup.BGImage = ((num == 0 && !flag) ? BGImage.Start : BGImage.Middle);
					itemGroup.GroupPosition = ((num == 0) ? GroupPosition.Start : GroupPosition.Middle);
					lastUnlockedIndex++;
					num += num2;
					itemGroups.Insert(lastUnlockedIndex, itemGroup);
				}
				itemGroup.BGImage = ((itemGroup.BGImage != BGImage.Start) ? BGImage.End : BGImage.Default);
				itemGroup.GroupPosition = ((itemGroup.GroupPosition == GroupPosition.Start) ? GroupPosition.StartEnd : GroupPosition.End);
			}
		}

		private void addItems(T[] items, ItemGroup.LockedState lockedState, int level, SpriteContentKey mascotIconContentKey = null, RewardThemeDefinition themeDefinition = null)
		{
			if (!GroupAllUnlockedItems || lockedState != 0)
			{
				createItemGroup(items, lockedState, level, mascotIconContentKey, themeDefinition);
			}
			else
			{
				unlockedItemsList.AddRange(items);
			}
		}

		protected void createItemGroup(T[] items, ItemGroup.LockedState lockedState, int level, SpriteContentKey mascotIconContentKey, RewardThemeDefinition themeDefinition, bool displayBeforeUnlocks = false)
		{
			if (spacingRulesMap.ContainsKey(items.Length))
			{
				items = spacingRulesMap[items.Length].CreateSpacing(items);
			}
			ItemGroup itemGroup = new ItemGroup(items, lockedState);
			setUpItemGroup(itemGroup, level, mascotIconContentKey, themeDefinition);
			if (lockedState != 0)
			{
				if (displayBeforeUnlocks)
				{
					lastUnlockedIndex++;
					itemGroups.Insert(0, itemGroup);
				}
				else
				{
					itemGroups.Add(itemGroup);
				}
			}
			else
			{
				lastUnlockedIndex++;
				itemGroups.Insert(lastUnlockedIndex, itemGroup);
			}
		}

		private void setUpItemGroup(ItemGroup itemGroup, int level, SpriteContentKey mascotIconContentKey = null, RewardThemeDefinition themeDefinition = null)
		{
			if (level > -1)
			{
				itemGroup.Level = level;
			}
			if (mascotIconContentKey != null)
			{
				if (!string.IsNullOrEmpty(mascotIconContentKey.Key))
				{
					itemGroup.MascotIconContentKey = mascotIconContentKey;
				}
				else
				{
					Log.LogError(this, "Mascot icon content key was null");
				}
			}
			if (themeDefinition != null)
			{
				if (!string.IsNullOrEmpty(themeDefinition.ShortThemeToken))
				{
					itemGroup.LocalizedThemeName = localizer.GetTokenTranslation(themeDefinition.ShortThemeToken);
				}
				else if (!string.IsNullOrEmpty(themeDefinition.LongThemeToken))
				{
					itemGroup.LocalizedThemeName = localizer.GetTokenTranslation(themeDefinition.LongThemeToken);
				}
				if (themeDefinition.ThemeIconContentKey != null && !string.IsNullOrEmpty(themeDefinition.ThemeIconContentKey.Key))
				{
					itemGroup.ThemeIconContentKey = themeDefinition.ThemeIconContentKey;
				}
			}
		}

		private void onElementShown(int index, GameObject element)
		{
			LockedItemGroup component = element.GetComponent<LockedItemGroup>();
			component.ShowUnlockedBG = ShowUnlockedBGs;
			ItemGroup itemGroup = itemGroups[index];
			switch (itemGroup.State)
			{
			case ItemGroup.LockedState.LevelLocked:
				component.GoToLevelLockState(itemGroup.Level);
				break;
			case ItemGroup.LockedState.MascotLocked:
				component.GoToMascotLockState(itemGroup.MascotIconContentKey);
				break;
			case ItemGroup.LockedState.MemberLocked:
				component.GoToMemberLockState();
				break;
			case ItemGroup.LockedState.Unlocked:
				component.GoToUnlockedState();
				break;
			case ItemGroup.LockedState.CustomLocked:
				component.GoToCustomLockState();
				break;
			}
			component.SetThemeState(itemGroup.LocalizedThemeName, itemGroup.ThemeIconContentKey);
			component.SetBackgroundImage(itemGroup.BGImage);
			component.SetChildAlignment((itemGroup.GroupPosition == GroupPosition.None) ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft);
			for (int i = 0; i < itemGroup.Items.Length; i++)
			{
				spawnAndCreateItem(itemGroup.Items[i], component.ItemsContainer, itemGroup.State);
			}
			modifyShownElement(index, element);
		}

		protected virtual void modifyShownElement(int index, GameObject element)
		{
		}

		private void onElementHidden(int index, GameObject element)
		{
			LockedItemGroup component = element.GetComponent<LockedItemGroup>();
			for (int num = component.ItemsContainer.childCount - 1; num >= 0; num--)
			{
				Transform child = component.ItemsContainer.GetChild(num);
				itemPool.Unspawn(child.gameObject);
			}
		}

		protected virtual void onElementRefreshed(int index, GameObject element)
		{
			onElementHidden(index, element);
			onElementShown(index, element);
		}

		private bool isLocalPlayerMember()
		{
			return Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
		}

		protected void refreshGroupItems(int index, T[] items, ItemGroup.LockedState lockedState)
		{
			ItemGroup itemGroup = itemGroups[index];
			itemGroup.Items = items;
			itemGroup.State = lockedState;
			layoutElementPool.RefreshElement(index, itemGroup.Items.Length);
		}

		private void spawnAndCreateItem(T itemDefinition, Transform parent, ItemGroup.LockedState lockedState)
		{
			if ((UnityEngine.Object)itemDefinition != (UnityEngine.Object)null)
			{
				GameObject gameObject = itemPool.Spawn();
				RectTransform rectTransform = gameObject.transform as RectTransform;
				rectTransform.SetParent(parent, false);
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.zero;
				rectTransform.localScale = Vector3.one;
				createItem(itemDefinition, gameObject, lockedState);
			}
		}

		protected abstract void createItem(T itemDefinition, GameObject item, ItemGroup.LockedState lockedState);
	}
}
