using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Gui;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner
{
	public class CategoryManager : MonoBehaviour
	{
		private const string ALL_ITEMS_COUNT_TOKEN = "Clothing.Category.AllCount";

		private const string ALL_ITEMS_TOKEN = "Clothing.Category.All";

		[SerializeField]
		private GameObject allCategoryButton;

		[SerializeField]
		private GameObject equippedCategoryButton;

		[SerializeField]
		private GameObject hiddenCategoryButton;

		[SerializeField]
		private GameObject categoryButtonPrefab;

		private Button allButton;

		private Button equippedButton;

		private Button hiddenButton;

		private CategoryManagerEventProxy eventProxy;

		private Dictionary<string, CategoryButton> categoryButtons = new Dictionary<string, CategoryButton>();

		private int categoryToSelectButtonIndex;

		private void Awake()
		{
			allCategoryButton.SetActive(false);
			if (equippedCategoryButton != null)
			{
				equippedCategoryButton.SetActive(false);
			}
			if (hiddenCategoryButton != null)
			{
				hiddenCategoryButton.SetActive(false);
			}
			allCategoryButton.GetComponentInChildren<Text>().text = Service.Get<Localizer>().GetTokenTranslation("Clothing.Category.All");
		}

		public void SetEventProxy(CategoryManagerEventProxy eventProxy)
		{
			this.eventProxy = eventProxy;
		}

		public void SetAllItemCount(int itemCount)
		{
			Text componentInChildren = allCategoryButton.GetComponentInChildren<Text>();
			componentInChildren.text = Service.Get<Localizer>().GetTokenTranslation("Clothing.Category.AllCount");
			componentInChildren.text = string.Format(componentInChildren.text, itemCount);
		}

		public void EnableEquippedButton(bool isEnabledd)
		{
			equippedButton.enabled = isEnabledd;
			if (isEnabledd)
			{
				equippedButton.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				equippedButton.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 0.5f);
			}
		}

		public void EnableHiddenButton(bool isEnabledd)
		{
			hiddenButton.enabled = isEnabledd;
			if (isEnabledd)
			{
				hiddenButton.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				hiddenButton.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 0.5f);
			}
		}

		public void ShowHiddenButton(bool isVisible)
		{
			hiddenButton.gameObject.SetActive(isVisible);
		}

		public void InitButtons(List<EquipmentCategoryDefinitionContentKey> categoryKeys, CategoryManagerEventProxy eventProxy, string categoryToSelect = "")
		{
			this.eventProxy = eventProxy;
			initAllButton();
			for (int i = 0; i < categoryKeys.Count; i++)
			{
				createCategoryButton(categoryKeys[i], false, categoryToSelect);
			}
			if (string.IsNullOrEmpty(categoryToSelect))
			{
				selectButton(allButton.gameObject);
				return;
			}
			HorizontalLayoutCenterOnElement component = GetComponent<HorizontalLayoutCenterOnElement>();
			if (component != null)
			{
				component.CenterOnElement<Button>(categoryToSelectButtonIndex);
			}
		}

		public void InitAndDisableButtons(List<EquipmentCategoryDefinitionContentKey> categoryKeys, CategoryManagerEventProxy eventProxy, List<string> enabledKeys)
		{
			this.eventProxy = eventProxy;
			initAllButton();
			bool flag = false;
			for (int i = 0; i < categoryKeys.Count; i++)
			{
				flag = true;
				if (enabledKeys.Contains(categoryKeys[i].Key))
				{
					flag = false;
				}
				createCategoryButton(categoryKeys[i], flag);
			}
			selectButton(allButton.gameObject);
		}

		public void InitAndDisableButtons(InventoryData inventoryData, CategoryManagerEventProxy eventProxy)
		{
			this.eventProxy = eventProxy;
			initAllButton();
			initEquippedButton();
			initHiddenButton();
			bool flag = false;
			int num = -1;
			for (int i = 0; i < inventoryData.CategoryKeys.Count; i++)
			{
				num = inventoryData.GetEquipmentIdsForCategory(inventoryData.CategoryKeys[i].Key).Length;
				flag = (num == 0);
				createCategoryButton(inventoryData.CategoryKeys[i], flag);
			}
			selectButton(allButton.gameObject);
		}

		public void SetCategoryButtonDisabledState(string key, bool isDisabled)
		{
			if (categoryButtons.ContainsKey(key))
			{
				CategoryButton categoryButton = categoryButtons[key];
				categoryButton.GetComponent<Button>().interactable = !isDisabled;
				if (isDisabled)
				{
					categoryButton.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 0.5f);
				}
				else
				{
					categoryButton.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 1f);
				}
			}
		}

		public void SetAllButtonsDisabledState(bool isDisabled)
		{
			foreach (string key in categoryButtons.Keys)
			{
				SetCategoryButtonDisabledState(key, isDisabled);
			}
		}

		public void ClickAllButton()
		{
			onAllButton();
			selectButton(allButton.gameObject);
		}

		public void SelectAllButton()
		{
			selectButton(allButton.gameObject);
		}

		private void initAllButton()
		{
			allButton = allCategoryButton.GetComponent<Button>();
			allButton.onClick.AddListener(onAllButton);
			allCategoryButton.SetActive(true);
		}

		private void initEquippedButton()
		{
			if (equippedCategoryButton != null)
			{
				equippedButton = equippedCategoryButton.GetComponent<Button>();
				equippedButton.onClick.AddListener(onEquippedButton);
				equippedCategoryButton.SetActive(true);
			}
		}

		private void initHiddenButton()
		{
			if (hiddenCategoryButton != null)
			{
				hiddenButton = hiddenCategoryButton.GetComponent<Button>();
				hiddenButton.onClick.AddListener(onHiddenButton);
				hiddenCategoryButton.SetActive(true);
			}
		}

		private void createCategoryButton(EquipmentCategoryDefinitionContentKey categoryDataKey, bool isDisabled = false, string categoryToSelect = "")
		{
			GameObject gameObject = Object.Instantiate(categoryButtonPrefab);
			gameObject.transform.SetParent(base.transform, false);
			CategoryButton component = gameObject.GetComponent<CategoryButton>();
			component.Init(categoryDataKey, eventProxy);
			categoryButtons.Add(categoryDataKey.Key, component);
			SetCategoryButtonDisabledState(categoryDataKey.Key, isDisabled);
			if (!string.IsNullOrEmpty(categoryToSelect) && categoryDataKey.Key == categoryToSelect)
			{
				gameObject.GetComponent<TintToggleGroupButton>().OnClick();
				categoryToSelectButtonIndex = categoryButtons.Count;
			}
			else
			{
				gameObject.GetComponent<TintToggleGroupButton>().SetTint(false);
			}
		}

		private void onAllButton()
		{
			eventProxy.OnAllButton();
		}

		private void onEquippedButton()
		{
			eventProxy.OnEquippedButton();
		}

		private void onHiddenButton()
		{
			eventProxy.OnHiddenButton();
		}

		private void selectButton(GameObject buttonGO)
		{
			buttonGO.GetComponent<TintToggleGroupButton>().OnClick();
			buttonGO.GetComponent<TintToggleGroupButton_Text>().OnClick();
		}

		private void OnDestroy()
		{
			categoryButtons = null;
			if (allButton != null)
			{
				allButton.onClick.RemoveListener(onAllButton);
			}
			if (equippedButton != null)
			{
				equippedButton.onClick.RemoveListener(onEquippedButton);
			}
			if (hiddenButton != null)
			{
				hiddenButton.onClick.RemoveListener(onHiddenButton);
			}
		}
	}
}
