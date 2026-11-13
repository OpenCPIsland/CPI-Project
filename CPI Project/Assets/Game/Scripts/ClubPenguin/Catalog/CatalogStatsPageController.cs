using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogStatsPageController : ACatalogController
	{
		public GameObject ShopItemRow;

		public GameObject ShopItem;

		public GameObject ScrollHeader;

		public GameObject ShopScrollerPrefab;

		public AvatarRenderTextureComponent AvatarRenderTextureComponent;

		public Text TotalSold;

		public Text TotalSubmitted;

		public Text BestSellerSold;

		private CatalogShopItemScroller shopScroller;

		private CatalogStats stats;

		private AvatarDetailsData avatarDetailsData;

		private bool isStatsLoading = false;

		private EventChannel eventChannel;

		private EventChannel catalogEventChannel;

		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<CatalogServiceProxyEvents.StatsReponse>(onStatsResponse);
			eventChannel.AddListener<CatalogServiceEvents.UserStatsErrorEvent>(onUserStatsErrorEvent);
			catalogEventChannel = new EventChannel(CatalogContext.EventBus);
			catalogEventChannel.AddListener<CatalogUIEvents.ShowStatsPage>(onShowStatsPage);
			catalogEventChannel.AddListener<CatalogUIEvents.HideCatalog>(onHideCatalog);
			catalogEventChannel.AddListener<CatalogUIEvents.BackButtonClicked>(onBackButtonClicked);
			AvatarDetailsData component;
			if (!Service.Get<CPDataEntityCollection>().LocalPlayerHandle.IsNull && Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component))
			{
				avatarDetailsData = component;
			}
			else
			{
				Log.LogError(this, "Unable to get local player avatar details data.");
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			eventChannel.RemoveAllListeners();
			catalogEventChannel.RemoveAllListeners();
			DestoryScroller();
		}

		private void OnEnable()
		{
			Service.Get<BackButtonController>().Add(onAndroidBackButtonClicked);
		}

		private void OnDisable()
		{
			Service.Get<BackButtonController>().Remove(onAndroidBackButtonClicked);
		}

		private void CreateScroller()
		{
			if (shopScroller == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(ShopScrollerPrefab);
				shopScroller = gameObject.GetComponent<CatalogShopItemScroller>();
				shopScroller.IndexOffset = 1;
				gameObject.transform.SetParent(base.transform, false);
				gameObject.transform.SetAsLastSibling();
				GameObject[] scrollerPrefabs = new GameObject[2]
				{
					ShopItemRow,
					ScrollHeader
				};
				shopScroller.SetScrollerPrefabs(scrollerPrefabs);
			}
		}

		private void DestoryScroller()
		{
			if (shopScroller != null)
			{
				shopScroller.transform.SetParent(null);
				UnityEngine.Object.Destroy(shopScroller.gameObject);
			}
			shopScroller = null;
		}

		private void onAndroidBackButtonClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.InvokeBackButtonClick));
		}

		private bool onBackButtonClicked(CatalogUIEvents.BackButtonClicked evt)
		{
			CoroutineRunner.StopAllForOwner(this);
			isStatsLoading = false;
			DestoryScroller();
			return false;
		}

		private bool onHideCatalog(CatalogUIEvents.HideCatalog evt)
		{
			CoroutineRunner.StopAllForOwner(this);
			isStatsLoading = false;
			DestoryScroller();
			return false;
		}

		private bool onShowStatsPage(CatalogUIEvents.ShowStatsPage evt)
		{
			CoroutineRunner.Start(WaitForUi(), this, "WaitForUi");
			return false;
		}

		private IEnumerator WaitForUi()
		{
			CreateScroller();
			isStatsLoading = true;
			yield return new WaitForSeconds(0.1f);
			Service.Get<CatalogServiceProxy>().GetPlayerCatalogStats();
		}

		private bool onUserStatsErrorEvent(CatalogServiceEvents.UserStatsErrorEvent evt)
		{
			isStatsLoading = false;
			Service.Get<PromptManager>().ShowPrompt("CatalogItemRetrievalErrorPrompt", null);
			return false;
		}

		private bool onStatsResponse(CatalogServiceProxyEvents.StatsReponse evt)
		{
			if (isStatsLoading)
			{
				isStatsLoading = false;
				if (Model.State == CatalogState.StatsView)
				{
					shopScroller.ClearScroller();
					stats = evt.Stats;
					if (TotalSold != null)
					{
						TotalSold.text = stats.TotalItemsSold.ToString();
					}
					if (BestSellerSold != null)
					{
						try
						{
							BestSellerSold.text = stats.StatsItem.numberSold.ToString();
							if (stats.TotalItemsSold < stats.StatsItem.numberSold)
							{
								BestSellerSold.text = stats.TotalItemsSold.ToString();
							}
						}
						catch (Exception ex)
						{
							Log.LogErrorFormatted(this, "An error occured when setting best seller sold text. Message: {0}", ex.Message);
						}
					}
					if (TotalSubmitted != null)
					{
						TotalSubmitted.text = stats.TotalItemsPurchased.ToString();
					}
					if (stats.StatsData != null)
					{
						DCustomEquipment[] outfit;
						if (stats.StatsItem.equipment.parts == null || stats.StatsItem.equipment.parts.Length == 0)
						{
							outfit = new DCustomEquipment[0];
						}
						else
						{
							DCustomEquipment dCustomEquipment = CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(stats.StatsItem.equipment);
							outfit = new DCustomEquipment[1]
							{
								dCustomEquipment
							};
						}
						AvatarDetailsData avatarDetailsData = new AvatarDetailsData();
						avatarDetailsData.Init(outfit);
						if (this.avatarDetailsData != null)
						{
							avatarDetailsData.BodyColor = this.avatarDetailsData.BodyColor;
						}
						AvatarRenderTextureComponent.RenderAvatar(avatarDetailsData);
						shopScroller.items = stats.StatsData;
						shopScroller.filteredItems = stats.StatsData;
						shopScroller.GenerateScrollData(stats.StatsData);
						for (int i = 0; i < shopScroller.numShopRows; i++)
						{
							shopScroller.Scroller.AddElement(1);
						}
						shopScroller.isShopScrollInitialized = true;
						shopScroller.lastNumShopRows = shopScroller.numShopRows;
					}
				}
			}
			return false;
		}
	}
}
