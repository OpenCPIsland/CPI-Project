using ClubPenguin.Analytics;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogShopController : ACatalogController
	{
		public const string CATALOG_ITEM_RETRIEVAL_ERROR_PROMPT_ID = "CatalogItemRetrievalErrorPrompt";

		public VerticalScrollingLayoutElementPool Scroller;

		public GameObject ShopRowItem;

		public GameObject ClothingPurchase;

		public GameObject NoSubmissions;

		public CatalogSubmissionCompleteController SubmissionCompletePanel;

		public GameObject Loader;

		public GameObject CoinReward;

		public GameObject ShopScrollerPrefab;

		public Image SubNavScrollerBackgroundImage;

		private EventChannel catalogEventChannel;

		private EventChannel eventChannel;

		private CurrentThemeData currentTheme;

		private string subNavCategory;

		private CatalogShopNavEnum navCategory;

		private bool isSubNavSetForCurrentList = false;

		private bool isNavInited = false;

		private CatalogShopItemScroller shopScroller;

		private long clothingCatalogItemId = -1L;

		private bool isShowItemOnRefresh = false;

		private Color[] tints;

		private void Awake()
		{
			catalogEventChannel = new EventChannel(CatalogContext.EventBus);
			catalogEventChannel.AddListener<CatalogUIEvents.ShowItemsForThemeEvent>(onShowItemsForThemeEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.SetCatalogShopNavFilter>(onSetCatalogShopNavFilter);
			catalogEventChannel.AddListener<CatalogUIEvents.ViewInCatalog>(onViewInCatalog);
			catalogEventChannel.AddListener<CatalogUIEvents.HideCatalog>(onHideCatalog);
			catalogEventChannel.AddListener<CatalogUIEvents.BackButtonClicked>(onBackButtonClicked);
			catalogEventChannel.AddListener<CatalogUIEvents.BuyPanelPurchaseButtonClickedEvent>(onPurchaseClickedEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.BuyPanelWearItButtonClickedEvent>(onWearItClickedEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.ShopItemClickedEvent>(onShopItemClickedEvent);
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<CatalogServiceProxyEvents.ShopItemsReponse>(onShopItemsResponse);
			eventChannel.AddListener<CatalogServiceEvents.CatalogItemsByRecentErrorEvent>(onCatalogItemsByRecentErrorEvent);
			eventChannel.AddListener<CatalogServiceEvents.CatalogItemsByCategoryErrorEvent>(onCatalogItemsByCategoryErrorEvent);
			eventChannel.AddListener<CatalogServiceEvents.CatalogItemsByFriendsErrorEvent>(onCatalogItemsByFriendsErrorEvent);
			eventChannel.AddListener<CatalogServiceEvents.CatalogItemsByPopularityErrorEvent>(onCatalogItemsByPopularityErrorEvent);
			tints = new Color[2]
			{
				Color.red,
				Color.grey
			};
		}

		private void OnEnable()
		{
			Service.Get<BackButtonController>().Add(onAndroidBackButtonClicked);
		}

		private void OnDisable()
		{
			Service.Get<BackButtonController>().Remove(onAndroidBackButtonClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			catalogEventChannel.RemoveAllListeners();
			eventChannel.RemoveAllListeners();
			DestoryScroller();
		}

		private void CreateScroller()
		{
			if (shopScroller == null)
			{
				DestoryScroller();
			}
			GameObject gameObject = Object.Instantiate(ShopScrollerPrefab);
			shopScroller = gameObject.GetComponent<CatalogShopItemScroller>();
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.SetAsFirstSibling();
			GameObject[] scrollerPrefabs = new GameObject[1]
			{
				ShopRowItem
			};
			shopScroller.SetScrollerPrefabs(scrollerPrefabs);
		}

		private void DestoryScroller()
		{
			if (shopScroller != null)
			{
				shopScroller.transform.SetParent(null);
				Object.Destroy(shopScroller.gameObject);
			}
			shopScroller = null;
		}

		private void onAndroidBackButtonClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.InvokeBackButtonClick));
		}

		private bool onBackButtonClicked(CatalogUIEvents.BackButtonClicked evt)
		{
			DestoryScroller();
			SetLoaderVisiblility(false);
			return false;
		}

		private bool onPurchaseClickedEvent(CatalogUIEvents.BuyPanelPurchaseButtonClickedEvent evt)
		{
			CatalogThemeDefinition themeByScheduelId = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(currentTheme.scheduledThemeChallengeId);
			TemplateDefinition templateDefinition = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>().Values.ToList().First((TemplateDefinition x) => x.Id == evt.ItemData.equipment.definitionId);
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(templateDefinition.Name);
			string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslation(themeByScheduelId.Title);
			int level = Service.Get<ProgressionService>().Level;
			long cost = evt.ItemData.cost;
			Service.Get<ICPSwrveService>().PurchaseClothing(tokenTranslation, (int)cost, 1, level);
			bool flag = navCategory == CatalogShopNavEnum.POPULAR;
			Service.Get<ICPSwrveService>().Action("clothing_catalog_item", "purchase", tokenTranslation2, tokenTranslation, flag.ToString());
			return false;
		}

		private bool onWearItClickedEvent(CatalogUIEvents.BuyPanelWearItButtonClickedEvent evt)
		{
			CatalogThemeDefinition themeByScheduelId = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(currentTheme.scheduledThemeChallengeId);
			TemplateDefinition templateDefinition = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>().Values.ToList().First((TemplateDefinition x) => x.Id == evt.ItemData.equipment.definitionId);
			bool flag = navCategory == CatalogShopNavEnum.POPULAR;
			Service.Get<ICPSwrveService>().Action("clothing_catalog_item", "wear_it", themeByScheduelId.Title, templateDefinition.Name, flag.ToString());
			return false;
		}

		private bool onShopItemClickedEvent(CatalogUIEvents.ShopItemClickedEvent evt)
		{
			CatalogThemeDefinition themeByScheduelId = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(currentTheme.scheduledThemeChallengeId);
			TemplateDefinition templateDefinition = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>().Values.ToList().First((TemplateDefinition x) => x.Id == evt.ItemData.equipment.definitionId);
			Service.Get<ICPSwrveService>().Action("clothing_catalog_item", "more_details", themeByScheduelId.Title, templateDefinition.Name, evt.IsAlreadyOwned.ToString());
			return false;
		}

		private bool onHideCatalog(CatalogUIEvents.HideCatalog evt)
		{
			DestoryScroller();
			return false;
		}

		private bool onViewInCatalog(CatalogUIEvents.ViewInCatalog evt)
		{
			isShowItemOnRefresh = !ShowSubmittedItem();
			SubmissionCompletePanel.gameObject.SetActive(false);
			return false;
		}

		private bool ShowSubmittedItem()
		{
			bool result = false;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < shopScroller.items.Count; i++)
			{
				CatalogItemData catalogItemData = shopScroller.items[i];
				if (catalogItemData.clothingCatalogItemId == clothingCatalogItemId)
				{
					result = true;
					num2++;
					if (num2 > shopScroller.CatalogShopItemsPerRow)
					{
						num++;
						num2 = 1;
					}
					shopScroller.ShowShopItemPanel(num, num2, true, false);
					break;
				}
			}
			return result;
		}

		private bool onCatalogItemsByRecentErrorEvent(CatalogServiceEvents.CatalogItemsByRecentErrorEvent evt)
		{
			OnRetreiveItemsError();
			return false;
		}

		private bool onCatalogItemsByCategoryErrorEvent(CatalogServiceEvents.CatalogItemsByCategoryErrorEvent evt)
		{
			OnRetreiveItemsError();
			return false;
		}

		private bool onCatalogItemsByFriendsErrorEvent(CatalogServiceEvents.CatalogItemsByFriendsErrorEvent evt)
		{
			OnRetreiveItemsError();
			return false;
		}

		private bool onCatalogItemsByPopularityErrorEvent(CatalogServiceEvents.CatalogItemsByPopularityErrorEvent evt)
		{
			OnRetreiveItemsError();
			return false;
		}

		private void OnRetreiveItemsError()
		{
			Service.Get<PromptManager>().ShowPrompt("CatalogItemRetrievalErrorPrompt", null);
		}

		private bool onShowItemsForThemeEvent(CatalogUIEvents.ShowItemsForThemeEvent evt)
		{
			CreateScroller();
			shopScroller.Scroller.Initilize();
			base.gameObject.SetActive(true);
			CoroutineRunner.Start(WaitForUIToUpdate(evt), this, "WaitForUIToUpdate");
			return false;
		}

		private IEnumerator WaitForUIToUpdate(CatalogUIEvents.ShowItemsForThemeEvent evt)
		{
			yield return new WaitForSeconds(0.1f);
			tints = Service.Get<CatalogServiceProxy>().themeColors.GetColorsByIndex(Service.Get<CatalogServiceProxy>().CurrentThemeIndex);
			clothingCatalogItemId = evt.ClothingCatalogItemId;
			if (clothingCatalogItemId != -1)
			{
				SubmissionCompletePanel.gameObject.SetActive(true);
				SubmissionCompletePanel.EquipSubmittedItem();
				CoroutineRunner.Start(PlayCoinsAnimation(), this, "PlayCoinsAnimation");
				Service.Get<CatalogServiceProxy>().cache.ClearCache();
			}
			shopScroller.ClearData();
			isSubNavSetForCurrentList = false;
			Model.State = CatalogState.ItemsView;
			subNavCategory = "All";
			currentTheme = evt.Theme;
			if (!isNavInited)
			{
				Service.Get<CatalogServiceProxy>().GetCatalogItemsByRecent(currentTheme.scheduledThemeChallengeId);
			}
			else
			{
				CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.SelectCategoryButtonAll));
			}
		}

		private IEnumerator PlayCoinsAnimation()
		{
			CoinReward.SetActive(true);
			yield return new WaitForSeconds(2f);
			CoinReward.SetActive(false);
		}

		private void TintScrollBar(Color tint)
		{
		}

		private bool onShopItemsResponse(CatalogServiceProxyEvents.ShopItemsReponse evt)
		{
			if (Model.State == CatalogState.ItemsView)
			{
				SetLoaderVisiblility(false);
				TintScrollBar(tints[1]);
				shopScroller.SetItemTintColors(tints[0], tints[1]);
				shopScroller.items = evt.Items;
				populateTheme();
				if (!isSubNavSetForCurrentList)
				{
					isNavInited = true;
					CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.SetShopNav(CalculateEnabledButtons()));
					isSubNavSetForCurrentList = true;
				}
				if (isShowItemOnRefresh)
				{
					isShowItemOnRefresh = false;
					ShowSubmittedItem();
				}
			}
			return false;
		}

		private void SetLoaderVisiblility(bool isVisible)
		{
			if (Loader != null)
			{
				Loader.SetActive(isVisible);
			}
		}

		private bool onSetCatalogShopNavFilter(CatalogUIEvents.SetCatalogShopNavFilter evt)
		{
			if (shopScroller.buyPanel != null)
			{
				shopScroller.CloseBuyPanel();
			}
			shopScroller.ClearScroller();
			SetLoaderVisiblility(true);
			navCategory = evt.NavCategory;
			subNavCategory = evt.SubNavCategory;
			if (navCategory != 0 || subNavCategory == "All")
			{
				switch (navCategory)
				{
				case CatalogShopNavEnum.RECENT:
					Service.Get<CatalogServiceProxy>().GetCatalogItemsByRecent(currentTheme.scheduledThemeChallengeId);
					break;
				case CatalogShopNavEnum.POPULAR:
					Service.Get<CatalogServiceProxy>().GetCatalogItemsByPopularity(currentTheme.scheduledThemeChallengeId);
					break;
				case CatalogShopNavEnum.FRIENDS:
					Service.Get<CatalogServiceProxy>().GetCatalogItemsByFriends(currentTheme.scheduledThemeChallengeId);
					break;
				}
			}
			else
			{
				string[] array = subNavCategory.Split('/');
				string category = array[array.Length - 1];
				Service.Get<CatalogServiceProxy>().GetCatalogItemsByCategory(category, currentTheme.scheduledThemeChallengeId);
			}
			return false;
		}

		private List<string> CalculateEnabledButtons()
		{
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			List<string> list = new List<string>();
			for (int i = 0; i < shopScroller.items.Count; i++)
			{
				int definitionId = shopScroller.items[i].equipment.definitionId;
				TemplateDefinition templateDefinition = dictionary.Values.ToList().First((TemplateDefinition x) => x.Id == definitionId);
				if (templateDefinition != null && list.IndexOf(templateDefinition.CategoryKey.Key) < 0)
				{
					list.Add(templateDefinition.CategoryKey.Key);
				}
			}
			return list;
		}

		private void populateTheme()
		{
			shopScroller.ClearScroller();
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			shopScroller.filteredItems = new List<CatalogItemData>();
			if (subNavCategory != "All" && navCategory == CatalogShopNavEnum.RECENT)
			{
				for (int i = 0; i < shopScroller.items.Count; i++)
				{
					int definitionId = shopScroller.items[i].equipment.definitionId;
					TemplateDefinition templateDefinition = dictionary.Values.ToList().First((TemplateDefinition x) => x.Id == definitionId);
					if (templateDefinition != null && templateDefinition.CategoryKey.Key.Equals(subNavCategory))
					{
						shopScroller.filteredItems.Add(shopScroller.items[i]);
					}
				}
			}
			else
			{
				shopScroller.filteredItems = shopScroller.items;
			}
			shopScroller.GenerateScrollData(shopScroller.filteredItems);
			shopScroller.lastNumShopRows = shopScroller.numShopRows;
			if (shopScroller.items == null || shopScroller.items.Count == 0)
			{
				Service.Get<ICPSwrveService>().Action("clothing_catalog_challenge", "no_submissions");
				NoSubmissions.SetActive(true);
				return;
			}
			NoSubmissions.SetActive(false);
			shopScroller.isShopScrollInitialized = true;
			for (int i = 0; i < shopScroller.numShopRows; i++)
			{
				shopScroller.Scroller.AddElement(1);
			}
		}
	}
}
