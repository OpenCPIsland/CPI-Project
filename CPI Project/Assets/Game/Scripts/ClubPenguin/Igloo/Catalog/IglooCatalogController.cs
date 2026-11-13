using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Gui;
using ClubPenguin.Igloo.UI;
using ClubPenguin.Progression;
using ClubPenguin.SceneManipulation;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.Catalog
{
	public class IglooCatalogController : MonoBehaviour
	{
		private const int CATEGORY_SUBFILTER_STANDARD_BUTTON_COUNT = 2;

		private const int THEME_SUBFILTER_STANDARD_BUTTON_COUNT = 1;

		private const int CATEGORY_SUBFILTER_INDEX_ALL = -1;

		private const int CATEGORY_SUBFILTER_INDEX_STRUCTURES = -2;

		private int defaultFilter = -1;

		private readonly PrefabContentKey IglooCatalogItemPrefabKey = new PrefabContentKey("UI/IglooCatalog/IglooCatalogItemButton");

		private readonly PrefabContentKey LoadingPrefabKey = new PrefabContentKey("UI/IglooCatalog/LoadingModal");

		private readonly PrefabContentKey PurchaseConfirmationPrefabKey = new PrefabContentKey("UI/IglooCatalog/IglooCatalogPurchase");

		private readonly PrefabContentKey SubfilterButtonPrefabKey = new PrefabContentKey("UI/IglooCatalog/SubfilterButton");

		[LocalizationToken]
		public string CategoryTitleToken;

		[LocalizationToken]
		public string CategoryAllButtonToken;

		[LocalizationToken]
		public string CategoryStructuresButtonToken;

		[LocalizationToken]
		public string ThemeTitleToken;

		[LocalizationToken]
		public string ThemeAllButtonToken;

		public VerticalGridPooledScrollRect PooledScrollRect;

		[SerializeField]
		private HorizontalLayoutGroup subfilterLayoutGroup;

		[SerializeField]
		private ScrollRect subfilterScrollRect;

		[SerializeField]
		private ScrollRect catalogScrollRect;

		[SerializeField]
		private Transform subfilterButtonParent;

		[SerializeField]
		private Transform itemContainer;

		[SerializeField]
		private Color iconBackgroundColor;

		[SerializeField]
		private GroupDefinition decorationExclusionGroup;

		[SerializeField]
		private GroupDefinitionKey iglooThemeParentGroup;

		[SerializeField]
		private DecorationCategoryButton themeFilterButton;

		[SerializeField]
		private DecorationCategoryButton categoryFilterButton;

		[SerializeField]
		private GameObject loadPanel;

		private Vector2 previousScrollPosition;

		private List<GameObject> themeSubfilterButtons = new List<GameObject>();

		private List<GameObject> categorySubfilterButtons = new List<GameObject>();

		private Dictionary<int, DecorationDefinition> decorationDefinitions;

		private Dictionary<int, StructureDefinition> structureDefinitions;

		private List<StaticGameDataDefinition> filteredDefinitions;

		private GameObject itemPrefab;

		private GameObject confirmation;

		private GameObject loadingModal;

		private GameObject subfilterButtonPrefab;

		private List<DecorationCategoryDefinition> categories;

		private List<GroupDefinition> themes;

		private Dictionary<int, ProgressionUtils.ParsedProgression<StructureDefinition>> structureProgressionStatus;

		private Dictionary<int, ProgressionUtils.ParsedProgression<DecorationDefinition>> decorationProgressionStatus;

		private bool waitingToHideLoadingModal = false;

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			catalogScrollRect.onValueChanged.RemoveListener(onScrollRectValueChanged);
			PooledScrollRect.ObjectAdded -= onPooledObjectAdded;
			loadPanel.SetActive(true);
		}

		private void Start()
		{
			Dictionary<int, DecorationDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DecorationDefinition>>();
			decorationDefinitions = new Dictionary<int, DecorationDefinition>(dictionary);
			Dictionary<int, StructureDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, StructureDefinition>>();
			structureDefinitions = new Dictionary<int, StructureDefinition>(dictionary2);
			filterExcludedDecorations();
			themeFilterButton.Initialize(0, ThemeTitleToken);
			DecorationCategoryButton decorationCategoryButton = themeFilterButton;
			decorationCategoryButton.ButtonClicked = (Action<int>)Delegate.Combine(decorationCategoryButton.ButtonClicked, new Action<int>(onThemeFilterButtonClicked));
			categoryFilterButton.Initialize(1, CategoryTitleToken);
			DecorationCategoryButton decorationCategoryButton2 = categoryFilterButton;
			decorationCategoryButton2.ButtonClicked = (Action<int>)Delegate.Combine(decorationCategoryButton2.ButtonClicked, new Action<int>(onCategoryFilterButtonClicked));
			categoryFilterButton.GetComponent<TintToggleGroupButton>().OnClick();
			categories = new List<DecorationCategoryDefinition>(Service.Get<IGameData>().Get<Dictionary<int, DecorationCategoryDefinition>>().Values);
			categories.Sort((DecorationCategoryDefinition x, DecorationCategoryDefinition y) => x.SortOrder.CompareTo(y.SortOrder));
			themes = GroupDefinitionHelper.GetGroups<DecorationDefinition>(iglooThemeParentGroup);
			themes.Sort();
			Service.Get<NotificationBreadcrumbController>().ResetBreadcrumbs("DecorationCatalog");
			List<ProgressionUtils.ParsedProgression<StructureDefinition>> list = ProgressionUtils.RetrieveProgressionLockedItems<StructureDefinition, StructureRewardDefinition>(ProgressionUnlockCategory.structurePurchaseRights, AbstractStaticGameDataRewardDefinition<StructureDefinition>.ToDefinitionArray);
			structureProgressionStatus = new Dictionary<int, ProgressionUtils.ParsedProgression<StructureDefinition>>(list.Count);
			foreach (ProgressionUtils.ParsedProgression<StructureDefinition> item in list)
			{
				structureProgressionStatus.Add(item.Definition.Id, item);
			}
			List<ProgressionUtils.ParsedProgression<DecorationDefinition>> list2 = ProgressionUtils.RetrieveProgressionLockedItems<DecorationDefinition, DecorationRewardDefinition>(ProgressionUnlockCategory.decorationPurchaseRights, AbstractStaticGameDataRewardDefinition<DecorationDefinition>.ToDefinitionArray);
			decorationProgressionStatus = new Dictionary<int, ProgressionUtils.ParsedProgression<DecorationDefinition>>(list2.Count);
			foreach (ProgressionUtils.ParsedProgression<DecorationDefinition> item2 in list2)
			{
				decorationProgressionStatus.Add(item2.Definition.Id, item2);
			}
			loadSubfilterButtons();
			Content.LoadAsync(onItemPrefabLoaded, IglooCatalogItemPrefabKey);
		}

		private float getIntoAnimationLength()
		{
			float result = 1f;
			Animator component = GetComponent<Animator>();
			if (component != null)
			{
				AnimationClip[] animationClips = component.runtimeAnimatorController.animationClips;
				AnimationClip[] array = animationClips;
				foreach (AnimationClip animationClip in array)
				{
					if (animationClip.name == "DisneyShopIntro")
					{
						result = animationClip.length;
					}
				}
			}
			return result;
		}

		public void OnCloseClicked()
		{
			if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>() && ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ObjectManipulationInputController != null)
			{
				ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ObjectManipulationInputController.SkipOneFrame = true;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void loadSubfilterButtons()
		{
			Content.LoadAsync(onSubfilterButtonPrefabLoaded, SubfilterButtonPrefabKey);
		}

		private void onSubfilterButtonPrefabLoaded(string path, GameObject buttonPrefab)
		{
			subfilterButtonPrefab = buttonPrefab;
			CoroutineRunner.Start(createThemeSubfilterButtons(), this, "createIglooCatalogThemeFilterButtons");
			CoroutineRunner.Start(createCategorySubfilterButtons(), this, "createIglooCatalogCategoryFilterButtons");
		}

		private void onThemeFilterButtonClicked(int val)
		{
			toggleThemeSubfilterButtons(true);
			toggleCategorySubfilterButtons(false);
			themeSubfilterButtons[0].GetComponent<TintToggleGroupButton>().OnClick();
			themeSubfilterButtons[0].GetComponent<DecorationCategoryButton>().OnButtonClicked();
			subfilterScrollRect.horizontalNormalizedPosition = 0f;
		}

		private void onCategoryFilterButtonClicked(int val)
		{
			toggleThemeSubfilterButtons(false);
			toggleCategorySubfilterButtons(true);
			categorySubfilterButtons[0].GetComponent<TintToggleGroupButton>().OnClick();
			categorySubfilterButtons[0].GetComponent<DecorationCategoryButton>().OnButtonClicked();
			subfilterScrollRect.horizontalNormalizedPosition = 0f;
		}

		private void filterExcludedDecorations()
		{
			if (decorationExclusionGroup != null)
			{
				List<StaticGameDataDefinitionKey> keys = GroupDefinitionHelper.GetKeys<DecorationDefinition>(decorationExclusionGroup);
				int result;
				foreach (StaticGameDataDefinitionKey item in keys)
				{
					if (int.TryParse(item.Id, out result) && decorationDefinitions.ContainsKey(result))
					{
						decorationDefinitions.Remove(result);
					}
				}
				keys = GroupDefinitionHelper.GetKeys<StructureDefinition>(decorationExclusionGroup);
				foreach (StaticGameDataDefinitionKey item2 in keys)
				{
					if (int.TryParse(item2.Id, out result) && structureDefinitions.ContainsKey(result))
					{
						structureDefinitions.Remove(result);
					}
				}
			}
		}

		private IEnumerator createThemeSubfilterButtons()
		{
			createThemeSubfilterButton(0, ThemeAllButtonToken);
			for (int i = 0; i < themes.Count; i++)
			{
				createThemeSubfilterButton(i + 1, themes[i].DisplayName);
				yield return null;
			}
			toggleThemeSubfilterButtons(false);
		}

		private void createThemeSubfilterButton(int index, string buttonText)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(subfilterButtonPrefab);
			gameObject.transform.SetParent(subfilterButtonParent);
			themeSubfilterButtons.Add(gameObject);
			DecorationCategoryButton component = gameObject.GetComponent<DecorationCategoryButton>();
			component.Initialize(index, buttonText);
			component.ButtonClicked = (Action<int>)Delegate.Combine(component.ButtonClicked, new Action<int>(onThemeSubfilterButtonClicked));
		}

		private void onThemeSubfilterButtonClicked(int buttonIndex)
		{
			string text = "Undefined";
			if (buttonIndex == 0)
			{
				text = "All";
				refilterCatalog();
			}
			else
			{
				int index = buttonIndex - 1;
				text = themes[index].DisplayName;
				refilterCatalog(themes[index]);
			}
		}

		private void toggleThemeSubfilterButtons(bool show)
		{
			for (int i = 0; i < themeSubfilterButtons.Count; i++)
			{
				themeSubfilterButtons[i].SetActive(show);
			}
		}

		private IEnumerator createCategorySubfilterButtons()
		{
			createCategorySubfilterButton(0, CategoryAllButtonToken);
			createCategorySubfilterButton(1, CategoryStructuresButtonToken);
			for (int i = 0; i < categories.Count; i++)
			{
				createCategorySubfilterButton(i + 2, categories[i].DisplayName);
				yield return null;
			}
			toggleCategorySubfilterButtons(true);
			int buttonIndex = (defaultFilter == -2) ? 1 : ((defaultFilter != -1) ? (defaultFilter + 2) : 0);
			categorySubfilterButtons[buttonIndex].GetComponent<TintToggleGroupButton>().OnClick();
			RectTransform[] buttonTransforms = new RectTransform[categorySubfilterButtons.Count];
			for (int j = 0; j < buttonTransforms.Length; j++)
			{
				buttonTransforms[j] = categorySubfilterButtons[j].GetComponent<RectTransform>();
			}
			subfilterScrollRect.CenterOnElement(buttonIndex, buttonTransforms, new Vector2(subfilterLayoutGroup.spacing, 0f));
		}

		private void createCategorySubfilterButton(int index, string buttonText)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(subfilterButtonPrefab);
			gameObject.transform.SetParent(subfilterButtonParent);
			categorySubfilterButtons.Add(gameObject);
			DecorationCategoryButton component = gameObject.GetComponent<DecorationCategoryButton>();
			component.Initialize(index, buttonText);
			component.ButtonClicked = (Action<int>)Delegate.Combine(component.ButtonClicked, new Action<int>(onCategorySubfilterButtonClicked));
		}

		private void onCategorySubfilterButtonClicked(int buttonIndex)
		{
			string text = "Undefined";
			switch (buttonIndex)
			{
			case 0:
				text = "All";
				refilterCatalog();
				break;
			case 1:
				text = "Structures";
				refilterCatalog(-2);
				break;
			default:
			{
				int index = buttonIndex - 2;
				text = categories[index].DisplayName;
				refilterCatalog(categories[index].Id);
				break;
			}
			}
		}

		private void toggleCategorySubfilterButtons(bool show)
		{
			for (int i = 0; i < categorySubfilterButtons.Count; i++)
			{
				categorySubfilterButtons[i].SetActive(show);
			}
		}

		private void onItemPrefabLoaded(string path, GameObject itemPrefab)
		{
			this.itemPrefab = itemPrefab;
			CoroutineRunner.Start(setupPooledScrollRect(), this, "Setup PooledScroll Rect for Igloo Catalog");
		}

		private IEnumerator setupPooledScrollRect()
		{
			catalogScrollRect.onValueChanged.AddListener(onScrollRectValueChanged);
			PooledScrollRect.ObjectAdded += onPooledObjectAdded;
			if (defaultFilter == -1 || defaultFilter == -2)
			{
				refilterCatalog(defaultFilter);
			}
			else
			{
				refilterCatalog(categories[defaultFilter].Id);
			}
			yield return new WaitForSeconds(getIntoAnimationLength());
			PooledScrollRect.Init(filteredDefinitions.Count, itemPrefab);
			while (!PooledScrollRect.IsInitialized)
			{
				yield return null;
			}
			loadPanel.SetActive(false);
		}

		private void onPooledObjectAdded(RectTransform item, int index)
		{
			if (index >= filteredDefinitions.Count)
			{
				return;
			}
			StaticGameDataDefinition staticGameDataDefinition = filteredDefinitions[index];
			IglooCatalogItem component = item.GetComponent<IglooCatalogItem>();
			IglooCatalogItemData iglooCatalogItemData = null;
			if (staticGameDataDefinition.GetType() == typeof(DecorationDefinition))
			{
				DecorationDefinition decorationDefinition = (DecorationDefinition)staticGameDataDefinition;
				ProgressionUtils.ParsedProgression<DecorationDefinition> value;
				if (!decorationProgressionStatus.TryGetValue(decorationDefinition.Id, out value))
				{
					value = new ProgressionUtils.ParsedProgression<DecorationDefinition>(decorationDefinition, -1, null, false, false, decorationDefinition.IsMemberOnly);
				}
				iglooCatalogItemData = new IglooCatalogItemData(decorationDefinition, value);
				component.SetItem(iglooCatalogItemData, this);
				try
				{
					Content.LoadAsync(component.SetImageFromTexture2D, decorationDefinition.Icon);
				}
				catch (Exception ex)
				{
					Log.LogException(this, ex);
				}
			}
			else if (staticGameDataDefinition.GetType() == typeof(StructureDefinition))
			{
				StructureDefinition structureDefinition = (StructureDefinition)staticGameDataDefinition;
				ProgressionUtils.ParsedProgression<StructureDefinition> value2;
				if (!structureProgressionStatus.TryGetValue(structureDefinition.Id, out value2))
				{
					value2 = new ProgressionUtils.ParsedProgression<StructureDefinition>(structureDefinition, -1, null, false, false, structureDefinition.IsMemberOnly);
				}
				iglooCatalogItemData = new IglooCatalogItemData(structureDefinition, value2);
				component.SetItem(iglooCatalogItemData, this);
				try
				{
					Content.LoadAsync(component.SetImageFromTexture2D, structureDefinition.Icon);
				}
				catch (Exception ex)
				{
					Log.LogException(this, ex);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Definition was not a Decoration or a Structure!");
			}
		}

		private void onScrollRectValueChanged(Vector2 value)
		{
			if (value != previousScrollPosition)
			{
				previousScrollPosition = value;
				hideConfirmation();
			}
		}

		public void SetDefaultFilterToStructures()
		{
			defaultFilter = -2;
		}

		public void SetDefaultFilterToAll()
		{
			defaultFilter = -1;
		}

		public void SetDefaultFilterToCategoryId(int categoryId)
		{
			defaultFilter = categoryId;
		}

		private void refilterCatalog(int categoryId = -1)
		{
			resetCatalog();
			switch (categoryId)
			{
			case -1:
				foreach (DecorationDefinition value in decorationDefinitions.Values)
				{
					filteredDefinitions.Add(value);
				}
				foreach (StructureDefinition value2 in structureDefinitions.Values)
				{
					filteredDefinitions.Add(value2);
				}
				break;
			case -2:
				foreach (StructureDefinition value3 in structureDefinitions.Values)
				{
					filteredDefinitions.Add(value3);
				}
				break;
			default:
				filteredDefinitions = filterDecorationListByCategory(categoryId);
				break;
			}
			filteredDefinitions = sortDisplayedList(filteredDefinitions);
			if (PooledScrollRect.IsInitialized)
			{
				PooledScrollRect.RefreshList(filteredDefinitions.Count);
			}
		}

		private void refilterCatalog(GroupDefinition themeDefinition)
		{
			resetCatalog();
			filteredDefinitions.AddRange(filterDecorationListByTheme(themeDefinition));
			filteredDefinitions.AddRange(filterStructureListByTheme(themeDefinition));
			filteredDefinitions = sortDisplayedList(filteredDefinitions);
			PooledScrollRect.RefreshList(filteredDefinitions.Count);
		}

		private List<StaticGameDataDefinition> sortDisplayedList(List<StaticGameDataDefinition> listToSort)
		{
			List<StaticGameDataDefinition> list = new List<StaticGameDataDefinition>();
			List<StaticGameDataDefinition> list2 = new List<StaticGameDataDefinition>();
			SortedDictionary<int, List<StaticGameDataDefinition>> sortedDictionary = new SortedDictionary<int, List<StaticGameDataDefinition>>();
			SortedDictionary<string, List<StaticGameDataDefinition>> sortedDictionary2 = new SortedDictionary<string, List<StaticGameDataDefinition>>();
			for (int i = 0; i < listToSort.Count; i++)
			{
				StaticGameDataDefinition staticGameDataDefinition = listToSort[i];
				if (listToSort[i].GetType() == typeof(DecorationDefinition))
				{
					int id = (staticGameDataDefinition as DecorationDefinition).Id;
					ProgressionUtils.ParsedProgression<DecorationDefinition> value;
					if (decorationProgressionStatus.TryGetValue(id, out value))
					{
						if (value.MemberLocked && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
						{
							list2.Add(staticGameDataDefinition);
						}
						else if (value.LevelLocked)
						{
							if (!sortedDictionary.ContainsKey(value.Level))
							{
								List<StaticGameDataDefinition> value2 = new List<StaticGameDataDefinition>();
								sortedDictionary.Add(value.Level, value2);
							}
							sortedDictionary[value.Level].Add(staticGameDataDefinition);
						}
						else if (value.ProgressionLocked)
						{
							if (!sortedDictionary2.ContainsKey(value.MascotName))
							{
								List<StaticGameDataDefinition> value2 = new List<StaticGameDataDefinition>();
								sortedDictionary2.Add(value.MascotName, value2);
							}
							sortedDictionary2[value.MascotName].Add(staticGameDataDefinition);
						}
						else
						{
							list.Add(staticGameDataDefinition);
						}
					}
					else if ((staticGameDataDefinition as DecorationDefinition).IsMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
					{
						list2.Add(staticGameDataDefinition);
					}
					else
					{
						list.Add(staticGameDataDefinition);
					}
				}
				else
				{
					if (listToSort[i].GetType() != typeof(StructureDefinition))
					{
						continue;
					}
					int id = (listToSort[i] as StructureDefinition).Id;
					ProgressionUtils.ParsedProgression<StructureDefinition> value3;
					if (structureProgressionStatus.TryGetValue(id, out value3))
					{
						if (value3.MemberLocked && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
						{
							list2.Add(staticGameDataDefinition);
						}
						else if (value3.LevelLocked)
						{
							if (!sortedDictionary.ContainsKey(value3.Level))
							{
								List<StaticGameDataDefinition> value2 = new List<StaticGameDataDefinition>();
								sortedDictionary.Add(value3.Level, value2);
							}
							sortedDictionary[value3.Level].Add(staticGameDataDefinition);
						}
						else if (value3.ProgressionLocked)
						{
							if (!sortedDictionary2.ContainsKey(value3.MascotName))
							{
								List<StaticGameDataDefinition> value2 = new List<StaticGameDataDefinition>();
								sortedDictionary2.Add(value3.MascotName, value2);
							}
							sortedDictionary2[value3.MascotName].Add(staticGameDataDefinition);
						}
						else
						{
							list.Add(staticGameDataDefinition);
						}
					}
					else if ((listToSort[i] as StructureDefinition).IsMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
					{
						list2.Add(staticGameDataDefinition);
					}
					else
					{
						list.Add(staticGameDataDefinition);
					}
				}
			}
			List<StaticGameDataDefinition> list3 = new List<StaticGameDataDefinition>();
			foreach (KeyValuePair<int, List<StaticGameDataDefinition>> item in sortedDictionary)
			{
				list3.AddRange(item.Value);
			}
			list.AddRange(list3);
			List<StaticGameDataDefinition> list4 = new List<StaticGameDataDefinition>();
			foreach (KeyValuePair<string, List<StaticGameDataDefinition>> item2 in sortedDictionary2)
			{
				list4.AddRange(item2.Value);
			}
			list.AddRange(list4);
			list.AddRange(list2);
			return list;
		}

		private void resetCatalog()
		{
			hideConfirmation();
			catalogScrollRect.verticalNormalizedPosition = 1f;
			filteredDefinitions = new List<StaticGameDataDefinition>();
		}

		private List<StaticGameDataDefinition> filterDecorationListByCategory(int categoryId)
		{
			List<StaticGameDataDefinition> list = new List<StaticGameDataDefinition>();
			foreach (DecorationDefinition value in decorationDefinitions.Values)
			{
				if (value.CategoryKey.Id == categoryId)
				{
					list.Add(value);
				}
			}
			return list;
		}

		private List<StaticGameDataDefinition> filterDecorationListByTheme(GroupDefinition themeDefinition)
		{
			List<StaticGameDataDefinition> list = new List<StaticGameDataDefinition>();
			foreach (StaticGameDataDefinitionKey key in GroupDefinitionHelper.GetKeys<DecorationDefinition>(themeDefinition))
			{
				int result;
				if (int.TryParse(key.Id, out result) && decorationDefinitions.ContainsKey(result))
				{
					list.Add(decorationDefinitions[result]);
				}
			}
			return list;
		}

		private List<StaticGameDataDefinition> filterStructureListByTheme(GroupDefinition themeDefinition)
		{
			List<StaticGameDataDefinition> list = new List<StaticGameDataDefinition>();
			foreach (StaticGameDataDefinitionKey key in GroupDefinitionHelper.GetKeys<StructureDefinition>(themeDefinition))
			{
				int result;
				if (int.TryParse(key.Id, out result) && structureDefinitions.ContainsKey(result))
				{
					list.Add(structureDefinitions[result]);
				}
			}
			return list;
		}

		public void ShowConfirmation(IglooCatalogItemData item, Sprite icon, IglooCatalogItem catalogItem)
		{
			CoroutineRunner.Start(loadConfirmation(PurchaseConfirmationPrefabKey, item, icon, catalogItem), this, "");
		}

		private IEnumerator loadConfirmation(PrefabContentKey prefabKey, IglooCatalogItemData item, Sprite icon, IglooCatalogItem catalogItem)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(prefabKey);
			yield return request;
			GameObject newConfirmation = UnityEngine.Object.Instantiate(request.Asset, base.transform, false);
			newConfirmation.GetComponent<IglooCatalogPurchaseConfirmation>().SetItem(item, icon, this, catalogItem, (RectTransform)catalogScrollRect.transform);
			hideConfirmation();
			confirmation = newConfirmation;
		}

		private void hideConfirmation()
		{
			if (confirmation != null)
			{
				UnityEngine.Object.Destroy(confirmation);
			}
		}

		public void ShowLoadingModal()
		{
			waitingToHideLoadingModal = false;
			if (loadingModal == null)
			{
				Content.LoadAsync(onLoadingModalLoadComplete, LoadingPrefabKey);
			}
		}

		private void onLoadingModalLoadComplete(string Path, GameObject loadingModalPrefab)
		{
			loadingModal = UnityEngine.Object.Instantiate(loadingModalPrefab, base.transform, false);
			if (waitingToHideLoadingModal)
			{
				HideLoadingModal();
			}
		}

		public void HideLoadingModal()
		{
			if (loadingModal != null)
			{
				UnityEngine.Object.Destroy(loadingModal);
				loadingModal = null;
				waitingToHideLoadingModal = false;
			}
			else
			{
				waitingToHideLoadingModal = true;
			}
		}
	}
}
