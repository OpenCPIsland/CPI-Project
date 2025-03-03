using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Gui;
using ClubPenguin.SceneManipulation;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	[DisallowMultipleComponent]
	public class DecorationCategoryManager : MonoBehaviour
	{
		private const string ALL_ITEMS_COUNT_TOKEN = "Clothing.Category.AllCount";

		private const int START_CAPACITY_LIST = 16;

		public CategoryRefreshedEvent CategoryRefreshedEvent;

		[SerializeField]
		private GameObject recentDecorationsButton;

		[SerializeField]
		private GameObject inMyIglooButton;

		[SerializeField]
		private GameObject allCategoryButton;

		[SerializeField]
		[Tooltip("The category button prefab used for each instance of the category button")]
		private GameObject categoryButtonPrefab;

		[SerializeField]
		private ScrollRect scrollRect;

		[SerializeField]
		private HorizontalLayoutGroup layoutGroup;

		private Button recentButtonComponent;

		private Button inIglootButtonComponent;

		private Button allButtonComponent;

		private int categoryToSelectButtonIndex;

		private DataEventListener sceneLayoutListener;

		private SceneLayoutData sceneLayoutData;

		private DecorationInventoryService decorationInventoryService;

		private Dictionary<int, DecorationDefinition> dictionaryOfDecorationDefinitions;

		public List<KeyValuePair<DecorationDefinition, int>> DisplayList;

		private List<RectTransform> buttonGameObjectsList = new List<RectTransform>(16);

		private List<AbstractDecorationCategoryFilter> strategyFilterList = new List<AbstractDecorationCategoryFilter>(16);

		private List<DecorationCategoryDefinition> categories;

		private int standardButtonCount = 0;

		private bool isDestroyed = false;

		public AbstractDecorationCategoryFilter GetCurrentFilter()
		{
			return strategyFilterList[categoryToSelectButtonIndex];
		}

		private void Awake()
		{
			initStandardButtons();
			dictionaryOfDecorationDefinitions = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
			decorationInventoryService = Service.Get<DecorationInventoryService>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle handle = cPDataEntityCollection.FindEntityByName("ActiveSceneData");
			sceneLayoutListener = cPDataEntityCollection.Whenever<SceneLayoutData>(handle, onLayoutAdded, onLayoutRemoved);
		}

		private void OnDestroy()
		{
			if (!isDestroyed)
			{
				isDestroyed = true;
				if (sceneLayoutListener != null)
				{
					sceneLayoutListener.StopListening();
				}
				recentButtonComponent.onClick.RemoveAllListeners();
				inIglootButtonComponent.onClick.RemoveAllListeners();
				allButtonComponent.onClick.RemoveAllListeners();
			}
		}

		private void initialize()
		{
			if (isDestroyed)
			{
				return;
			}
			categories = new List<DecorationCategoryDefinition>(Service.Get<IGameData>().Get<Dictionary<int, DecorationCategoryDefinition>>().Values);
			categories.Sort((DecorationCategoryDefinition x, DecorationCategoryDefinition y) => x.SortOrder.CompareTo(y.SortOrder));
			initializeStrategies();
			createCategoryButtons();
			if (buttonGameObjectsList.Count > 0)
			{
				categoryToSelectButtonIndex = Service.Get<RecentDecorationsService>().SelectedCategoryIndex;
				if (categoryToSelectButtonIndex < 0 || categoryToSelectButtonIndex >= buttonGameObjectsList.Count)
				{
					categoryToSelectButtonIndex = standardButtonCount - 1;
				}
				selectButton(buttonGameObjectsList[categoryToSelectButtonIndex]);
				CoroutineRunner.Start(CenterOnButton(categoryToSelectButtonIndex), this, "waitForLayoutToCenterScrollView");
				SetAllItemCount();
				DisplayList = GetDefinitionsToDisplay();
			}
		}

		private void initializeStrategies()
		{
			strategyFilterList.Add(new RecentDecorationCategoryFilter(dictionaryOfDecorationDefinitions, decorationInventoryService));
			strategyFilterList.Add(new InMyIglooDecorationCategoryFilter(dictionaryOfDecorationDefinitions, decorationInventoryService, sceneLayoutData));
			strategyFilterList.Add(new AllMyIglooDecorationCategoryFilter(dictionaryOfDecorationDefinitions, decorationInventoryService));
			standardButtonCount = strategyFilterList.Count;
			foreach (DecorationCategoryDefinition category in categories)
			{
				strategyFilterList.Add(new SpecificDecorationCategoryFilter(dictionaryOfDecorationDefinitions, decorationInventoryService, category));
			}
		}

		private void createCategoryButtons()
		{
			int num = standardButtonCount;
			foreach (DecorationCategoryDefinition category in categories)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(categoryButtonPrefab);
				DecorationCategoryButton component = gameObject.GetComponent<DecorationCategoryButton>();
				component.Initialize(num++, category.DisplayName);
				component.ButtonClicked = (Action<int>)Delegate.Combine(component.ButtonClicked, new Action<int>(OnCategoryButtonClicked));
				gameObject.transform.SetParent(base.transform);
				buttonGameObjectsList.Add(gameObject.GetComponent<RectTransform>());
			}
		}

		private void OnCategoryButtonClicked(int buttonIndex)
		{
			RefreshDecorationsDisplayed(buttonIndex);
		}

		private void onLayoutAdded(SceneLayoutData value)
		{
			sceneLayoutData = value;
			initialize();
		}

		private void onLayoutRemoved(SceneLayoutData value)
		{
		}

		internal List<KeyValuePair<DecorationDefinition, int>> GetDefinitionsToDisplay()
		{
			AbstractDecorationCategoryFilter abstractDecorationCategoryFilter = strategyFilterList[categoryToSelectButtonIndex];
			return abstractDecorationCategoryFilter.GetDefinitionsToDisplay();
		}

		public void SetAllItemCount()
		{
			Text componentInChildren = allCategoryButton.GetComponentInChildren<Text>();
			componentInChildren.text = Service.Get<Localizer>().GetTokenTranslation("Clothing.Category.AllCount");
			componentInChildren.text = string.Format(componentInChildren.text, ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().GetAvailableDecorations().Count);
		}

		private void initStandardButtons()
		{
			recentButtonComponent = recentDecorationsButton.GetComponent<Button>();
			recentButtonComponent.onClick.AddListener(onRecentButton);
			recentDecorationsButton.SetActive(true);
			buttonGameObjectsList.Add(recentDecorationsButton.GetComponent<RectTransform>());
			inIglootButtonComponent = inMyIglooButton.GetComponent<Button>();
			inIglootButtonComponent.onClick.AddListener(onInIglooButton);
			inMyIglooButton.SetActive(true);
			buttonGameObjectsList.Add(inMyIglooButton.GetComponent<RectTransform>());
			allButtonComponent = allCategoryButton.GetComponent<Button>();
			allButtonComponent.onClick.AddListener(onAllButton);
			allCategoryButton.SetActive(true);
			buttonGameObjectsList.Add(allCategoryButton.GetComponent<RectTransform>());
		}

		private void onRecentButton()
		{
			RefreshDecorationsDisplayed(0);
		}

		private void onInIglooButton()
		{
			RefreshDecorationsDisplayed(1);
		}

		private void onAllButton()
		{
			RefreshDecorationsDisplayed(2);
		}

		public void RefreshDecorationsDisplayed()
		{
			RefreshDecorationsDisplayed(categoryToSelectButtonIndex);
		}

		private void RefreshDecorationsDisplayed(int buttonIndex)
		{
			categoryToSelectButtonIndex = (Service.Get<RecentDecorationsService>().SelectedCategoryIndex = buttonIndex);
			DisplayList = GetDefinitionsToDisplay();
			if (CategoryRefreshedEvent != null)
			{
				CategoryRefreshedEvent(categoryToSelectButtonIndex - standardButtonCount);
			}
		}

		private void selectButton(RectTransform buttonGO)
		{
			if (buttonGO != null)
			{
				TintToggleGroupButton component = buttonGO.GetComponent<TintToggleGroupButton>();
				if (component != null)
				{
					component.OnClick();
				}
				TintToggleGroupButton_Text component2 = buttonGO.GetComponent<TintToggleGroupButton_Text>();
				if (component2 != null)
				{
					component2.OnClick();
				}
			}
		}

		public void ShowAll()
		{
			categoryToSelectButtonIndex = 2;
		}

		private IEnumerator CenterOnButton(int buttonIndex)
		{
			yield return new WaitForEndOfFrame();
			scrollRect.CenterOnElement(buttonIndex, buttonGameObjectsList.ToArray(), new Vector2(layoutGroup.spacing, 0f));
		}

		public int GetCurrentCategoryId()
		{
			return categoryToSelectButtonIndex - standardButtonCount;
		}
	}
}
