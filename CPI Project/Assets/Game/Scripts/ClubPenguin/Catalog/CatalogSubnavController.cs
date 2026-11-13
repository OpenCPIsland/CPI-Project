using ClubPenguin.Analytics;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Core;
using ClubPenguin.Gui;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Catalog
{
	public class CatalogSubnavController : ACatalogController
	{
		public const string SUB_NAV_ALL_CATEGORY = "All";

		public GameObject SubNavGameObject;

		public CategoryManager CategoryManager;

		public GameObject Recent;

		private EventDispatcher eventBus;

		private CatalogShopNavEnum navCategory;

		private string subNavCategory;

		private bool isNavInited = false;

		private void Awake()
		{
			eventBus = CatalogContext.EventBus;
			eventBus.AddListener<CatalogUIEvents.SubNavAllButtonClickedEvent>(onAllButtonClicked);
			eventBus.AddListener<CatalogUIEvents.SubNavCategoryButtonClickedEvent>(onButtonClicked);
			eventBus.AddListener<CatalogUIEvents.SetShopNav>(onSetShopNav);
			eventBus.AddListener<CatalogUIEvents.SelectCategoryButtonAll>(onSelectCategoryButtonAll);
		}

		private void Destory()
		{
			eventBus.RemoveListener<CatalogUIEvents.SubNavAllButtonClickedEvent>(onAllButtonClicked);
			eventBus.RemoveListener<CatalogUIEvents.SubNavCategoryButtonClickedEvent>(onButtonClicked);
			eventBus.RemoveListener<CatalogUIEvents.SetShopNav>(onSetShopNav);
			eventBus.RemoveListener<CatalogUIEvents.SelectCategoryButtonAll>(onSelectCategoryButtonAll);
		}

		private bool onSelectCategoryButtonAll(CatalogUIEvents.SelectCategoryButtonAll evt)
		{
			SetRecent();
			CategoryManager.ClickAllButton();
			return false;
		}

		private bool onSetShopNav(CatalogUIEvents.SetShopNav evt)
		{
			navCategory = CatalogShopNavEnum.RECENT;
			subNavCategory = "All";
			setCategories(evt.EnabledKeys);
			return false;
		}

		public void SetRecent()
		{
			navCategory = CatalogShopNavEnum.RECENT;
			Recent.GetComponent<TintToggleGroupButton>().OnClick();
			Recent.GetComponent<TintToggleGroupButton_Text>().OnClick();
		}

		public void OnCategoryButtonClick()
		{
			navCategory = CatalogShopNavEnum.RECENT;
			SetNavFilter();
		}

		public void OnRecentButtonClick()
		{
			SubNavGameObject.SetActive(true);
			navCategory = CatalogShopNavEnum.RECENT;
			SetNavFilter();
		}

		public void OnPopularButtonClick()
		{
			SubNavGameObject.SetActive(false);
			navCategory = CatalogShopNavEnum.POPULAR;
			SetNavFilter();
		}

		public void OnFriendsButtonClick()
		{
			SubNavGameObject.SetActive(false);
			navCategory = CatalogShopNavEnum.FRIENDS;
			SetNavFilter();
		}

		private void setCategories(List<string> enabledCategoriesList)
		{
			CategoryManager.SetAllButtonsDisabledState(true);
			if (!isNavInited)
			{
				DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
				if (!localPlayerHandle.IsNull)
				{
					InventoryData component = Service.Get<CPDataEntityCollection>().GetComponent<InventoryData>(localPlayerHandle);
					if (component != null)
					{
						CategoryManager.InitAndDisableButtons(component.CategoryKeys, new CatalogCategoryManagerEventProxy(), enabledCategoriesList);
					}
				}
				isNavInited = true;
			}
			else
			{
				foreach (string enabledCategories in enabledCategoriesList)
				{
					CategoryManager.SetCategoryButtonDisabledState(enabledCategories, false);
				}
			}
			CategoryManager.SelectAllButton();
		}

		private bool onAllButtonClicked(CatalogUIEvents.SubNavAllButtonClickedEvent evt)
		{
			subNavCategory = "All";
			SetNavFilter();
			return false;
		}

		private bool onButtonClicked(CatalogUIEvents.SubNavCategoryButtonClickedEvent evt)
		{
			subNavCategory = evt.Category;
			SetNavFilter();
			return false;
		}

		private void SetNavFilter()
		{
			Service.Get<ICPSwrveService>().Action("clothing_catalog_challenge", "filter", navCategory.ToString());
			CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.SetCatalogShopNavFilter(navCategory, subNavCategory));
		}
	}
}
