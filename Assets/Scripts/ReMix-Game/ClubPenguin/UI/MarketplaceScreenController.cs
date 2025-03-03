using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using ClubPenguin.Marketplace;
using ClubPenguin.Progression;
using ClubPenguin.Props;
using ClubPenguin.Rewards;
using ClubPenguin.Tutorial;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketplaceScreenController : MonoBehaviour
	{
		public struct MarketplaceItemData
		{
			public readonly PropDefinition PropDefn;

			public readonly int UnlockLevel;

			public MarketplaceItemData(PropDefinition propDefn, int unlockLevel = -1)
			{
				PropDefn = propDefn;
				UnlockLevel = unlockLevel;
			}
		}

		private class MarketplaceListItemData
		{
			public readonly PropDefinition PropDefn;

			public readonly int UnlockLevel;

			public readonly bool IsSpecialItem;

			public MarketplaceItem MarketplaceItem;

			public MarketplaceListItemData(PropDefinition propDefn, int unlockLevel = -1, bool isSpecialItem = false)
			{
				PropDefn = propDefn;
				UnlockLevel = unlockLevel;
				IsSpecialItem = isSpecialItem;
			}
		}

		private class MarketplaceListItemDataComparer : IComparer<MarketplaceListItemData>
		{
			public int Compare(MarketplaceListItemData x, MarketplaceListItemData y)
			{
				int num = x.UnlockLevel.CompareTo(y.UnlockLevel);
				if (num == 0 && x.PropDefn != y.PropDefn && x.IsSpecialItem)
				{
					num = -1;
				}
				return num;
			}
		}

		private const string PLAYER_PREFS_PURCHASED_PARTY_GAME_KEY = "purchased_party_game";

		public Text HeaderText;

		public Text CoinsText;

		public GameObject ItemListHeader;

		public GameObject TopPanelContainer;

		public ScrollRect ContentScrollRect;

		public Transform ItemContainer;

		public Image BackgroundImage;

		public MarketplaceAd marketplaceAd;

		public DisneyStoreTrayAnimator TrayAnimator;

		public GameObject PopupButton;

		public GameObject LoadingModal;

		public Transform ConfirmationContainer;

		public TutorialDefinitionKey MarketPlaceTutorialDefinition;

		public TutorialDefinitionKey PartySuppliesTutorialDefinition;

		public TutorialDefinitionKey PartyGamesTutorialDefinition;

		private static PrefabContentKey itemContentKey = new PrefabContentKey("Prefabs/MarketplaceItemPrefab");

		private static PrefabContentKey confirmationPrefabKey = new PrefabContentKey("Prefabs/PartySupplyPurchase");

		private static PrefabContentKey partyGameConfirmationPrefabKey = new PrefabContentKey("Prefabs/PartyGamesPurchase");

		private static PrefabContentKey popupContentKey = new PrefabContentKey("Prefabs/MarketplacePopupBackgroundPrefab");

		private static SpriteContentKey itemOffContentKey = new SpriteContentKey("Images/Marketplace_Item_Off");

		private static SpriteContentKey itemOnContentKey = new SpriteContentKey("Images/Marketplace_Item_On");

		private static SpriteContentKey bgAssetContentKey = new SpriteContentKey("Images/Marketplace_*_BG");

		private Dictionary<string, Sprite> itemSprites;

		private List<MarketplaceListItemData> marketplaceItems;

		private MarketplaceDefinition definition = null;

		private List<string> inStockItems;

		private MarketplaceItemData currentAdItem;

		private Sprite currentAdBackground;

		private Sprite itemOnSprite;

		private Sprite itemOffSprite;

		private Color itemTextColor;

		private MarketplacePopupController popupController;

		private GameObject popupPrefab;

		private GameObject confirmation;

		private int numImagesLoaded = 0;

		private int totalImagesToLoad = 0;

		private bool doneCountingImages = false;

		private int numAssetsLoaded = 0;

		private int totalAssetsToLoad = 3;

		private bool hasOpened = false;

		private bool isClosing = false;

		private Animator screenAnimator;

		private bool screenAnimatorDisabled;

		private ProgressionService progressionService;

		private bool isMember = false;

		private MembershipData membershipData;

		private MembershipData MembershipData
		{
			get
			{
				if (membershipData == null)
				{
					CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
					if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out membershipData))
					{
						membershipData.MembershipDataUpdated += onMembershipDataUpdated;
					}
				}
				return membershipData;
			}
			set
			{
				if (membershipData == null && value != null)
				{
					membershipData = value;
					membershipData.MembershipDataUpdated += onMembershipDataUpdated;
				}
			}
		}

		private void onMembershipDataUpdated(MembershipData membershipData)
		{
			if (membershipData.IsMember && !isMember)
			{
				isMember = true;
				int count = marketplaceItems.Count;
				for (int i = 0; i < count; i++)
				{
					MarketplaceItem marketplaceItem = marketplaceItems[i].MarketplaceItem;
					marketplaceItem.checkLockedState();
					marketplaceItem.UpdateVisualStates();
				}
			}
		}

		private void Awake()
		{
			screenAnimatorDisabled = ((bool)screenAnimator && screenAnimator.enabled);
			isMember = MembershipData.IsMember;
			itemSprites = new Dictionary<string, Sprite>();
			marketplaceItems = new List<MarketplaceListItemData>();
			inStockItems = new List<string>();
			loadInStockItems();
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Marketplace));
		}

		private void Start()
		{
			progressionService = Service.Get<ProgressionService>();
			PopupButton.SetActive(false);
			screenAnimator = GetComponent<Animator>();
			LoadingModal.SetActive(false);
			Service.Get<EventDispatcher>().DispatchEvent(default(RewardEvents.SuppressLevelUpPopup));
		}

		public void Init(MarketplaceDefinition definition)
		{
			this.definition = definition;
			loadItemImages();
			loadAssets(definition);
			Content.LoadAsync(onPopupPrefabLoaded, popupContentKey);
			HeaderText.text = Service.Get<Localizer>().GetTokenTranslation(definition.DisplayName).ToUpper();
			if (!string.IsNullOrEmpty(definition.ItemListDisplayName))
			{
				Text componentInChildren = ItemListHeader.GetComponentInChildren<Text>();
				componentInChildren.text = Service.Get<Localizer>().GetTokenTranslation(definition.ItemListDisplayName);
				ItemListHeader.SetActive(true);
			}
			itemTextColor = ColorUtils.HexToColor(definition.TextColorHex);
			Service.Get<EventDispatcher>().DispatchEvent(new MarketplaceEvents.MarketplaceOpened(definition.Name));
			Service.Get<ICPSwrveService>().Action("view.market", Service.Get<Localizer>().GetTokenTranslation(definition.DisplayName));
		}

		public void OnDestroy()
		{
			if (popupController != null)
			{
				UnityEngine.Object.Destroy(popupController.gameObject);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Here));
			if (membershipData != null)
			{
				membershipData.MembershipDataUpdated -= onMembershipDataUpdated;
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(RewardEvents.SuppressLevelUpPopup));
			CoroutineRunner.StopAllForOwner(this);
		}

		public void Update()
		{
			if (!screenAnimatorDisabled && hasOpened && !isClosing && screenAnimator != null)
			{
				screenAnimator.enabled = false;
				screenAnimatorDisabled = true;
			}
		}

		public void SetPurchasedPartyGame()
		{
			PlayerPrefs.SetInt("purchased_party_game", 1);
		}

		private void loadInStockItems()
		{
			foreach (PropDefinition value in Service.Get<PropService>().Props.Values)
			{
				if (value.PropType == PropDefinition.PropTypes.Consumable && !value.QuestOnly && !value.ServerAddedItem)
				{
					inStockItems.Add(value.GetNameOnServer());
				}
			}
		}

		private void loadAssets(MarketplaceDefinition definition)
		{
			Content.LoadAsync(onBGLoaded, bgAssetContentKey, definition.NameInAssets);
			Content.LoadAsync(onItemOffLoaded, itemOffContentKey);
			Content.LoadAsync(onItemOnLoaded, itemOnContentKey);
		}

		private void onBGLoaded(string path, Sprite image)
		{
			BackgroundImage.sprite = image;
			onLoadedAsset();
		}

		private void onItemOffLoaded(string path, Sprite image)
		{
			itemOffSprite = image;
			onLoadedAsset();
		}

		private void onItemOnLoaded(string path, Sprite image)
		{
			itemOnSprite = image;
			onLoadedAsset();
		}

		private void onLoadedAsset()
		{
			numAssetsLoaded++;
			if (numAssetsLoaded == totalAssetsToLoad)
			{
				tryLoadItems();
			}
		}

		private void onPopupPrefabLoaded(string path, GameObject popup)
		{
			popupPrefab = popup;
		}

		private void loadItemImages()
		{
			foreach (PropDefinition value in Service.Get<PropService>().Props.Values)
			{
				if (value.PropType == PropDefinition.PropTypes.Consumable && !value.QuestOnly && value.GetIconContentKey() != null && !string.IsNullOrEmpty(value.GetIconContentKey().Key))
				{
					Content.LoadAsync(onItemIconLoaded, value.GetIconContentKey());
					totalImagesToLoad++;
				}
			}
			doneCountingImages = true;
		}

		private void onItemIconLoaded(string path, Sprite image)
		{
			itemSprites[path] = image;
			numImagesLoaded++;
			if (doneCountingImages && numImagesLoaded == totalImagesToLoad)
			{
				doneLoadingImages();
			}
		}

		private void doneLoadingImages()
		{
			tryLoadItems();
			showMarketplaceAd();
			if (!string.IsNullOrEmpty(definition.TopPanel.Key))
			{
				Content.LoadAsync(onTopPanelLoaded, definition.TopPanel);
			}
		}

		private void showMarketplaceAd()
		{
			CellPhoneSaleActivityDefinition validSaleForMarketplace = getValidSaleForMarketplace(definition);
			if (validSaleForMarketplace != null)
			{
				CoroutineRunner.Start(loadFlashSaleAd(validSaleForMarketplace), this, "loadFlashSaleAd");
				marketplaceAd.gameObject.SetActive(false);
			}
			else if (definition.AdItem != null)
			{
				marketplaceAd.gameObject.SetActive(true);
				CoroutineRunner.Start(loadAd(), this, "MarketplaceScreen.loadAd");
			}
			else
			{
				marketplaceAd.gameObject.SetActive(false);
			}
		}

		private CellPhoneSaleActivityDefinition getValidSaleForMarketplace(MarketplaceDefinition market)
		{
			Dictionary<int, CellPhoneSaleActivityDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, CellPhoneSaleActivityDefinition>>();
			foreach (CellPhoneSaleActivityDefinition value in dictionary.Values)
			{
				if (DateTimeUtils.DoesDateFallBetween(Service.Get<ContentSchedulerService>().ScheduledEventDate(), value.GetStartingDate().Date, value.GetEndingDate().Date))
				{
					for (int i = 0; i < value.MarketPlaceData.Length; i++)
					{
						if (value.MarketPlaceData[i].MarketPlace.Name == market.Name)
						{
							return value;
						}
					}
				}
			}
			return null;
		}

		private void onTopPanelLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.transform.SetParent(TopPanelContainer.transform, false);
			TopPanelContainer.SetActive(true);
		}

		private IEnumerator loadAd()
		{
			if (!(definition.AdItem == null) && definition.AdItem.PropType == PropDefinition.PropTypes.Consumable && !(definition.AdMarket == null))
			{
				currentAdItem = new MarketplaceItemData(unlockLevel: progressionService.GetUnlockLevelFromDefinition(definition.AdItem, ProgressionUnlockCategory.partySupplies), propDefn: definition.AdItem);
				AssetRequest<Sprite> backgroundSprite = Content.LoadAsync(bgAssetContentKey, definition.AdMarket.NameInAssets);
				yield return backgroundSprite;
				currentAdBackground = backgroundSprite.Asset;
				marketplaceAd.SetData(currentAdItem, definition, itemSprites[currentAdItem.PropDefn.GetIconContentKey().Key.ToLower()], currentAdBackground);
			}
		}

		private IEnumerator loadFlashSaleAd(CellPhoneSaleActivityDefinition sale)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(sale.MarketBannerKey);
			yield return request;
			GameObject saleGO = UnityEngine.Object.Instantiate(request.Asset, marketplaceAd.transform.parent, false);
			saleGO.transform.SetSiblingIndex(0);
			MarketPlaceSaleAd saleAd = saleGO.GetComponent<MarketPlaceSaleAd>();
			saleAd.setSaleData(sale);
		}

		private void tryLoadItems()
		{
			if (numImagesLoaded == totalImagesToLoad && numAssetsLoaded == totalAssetsToLoad)
			{
				Content.LoadAsync(onItemPrefabLoaded, itemContentKey);
			}
		}

		private bool checkPropDefnForSpecialItem(PropDefinition propDefnToCheck)
		{
			bool result = false;
			if (propDefnToCheck.HasSpecialMarket && definition.SpecialItems != null)
			{
				int num = definition.SpecialItems.Length;
				for (int i = 0; i < num; i++)
				{
					if (definition.SpecialItems[i].Id == propDefnToCheck.Id)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private void filterDefinitions()
		{
			marketplaceItems.Clear();
			Dictionary<string, PropDefinition>.ValueCollection values = Service.Get<PropService>().Props.Values;
			foreach (PropDefinition item in values)
			{
				if (item.PropType == PropDefinition.PropTypes.Consumable && !item.QuestOnly && !item.ServerAddedItem && item.GetIconContentKey() != null && !string.IsNullOrEmpty(item.GetIconContentKey().Key))
				{
					bool flag = checkPropDefnForSpecialItem(item);
					if (flag || (!item.HasSpecialMarket && definition.ShowDefaultItems))
					{
						int unlockLevelFromDefinition = progressionService.GetUnlockLevelFromDefinition(item, ProgressionUnlockCategory.partySupplies);
						marketplaceItems.Add(new MarketplaceListItemData(item, unlockLevelFromDefinition, flag));
					}
				}
			}
			marketplaceItems.Sort(new MarketplaceListItemDataComparer());
		}

		private void onItemPrefabLoaded(string path, GameObject prefab)
		{
			if (!(base.gameObject == null))
			{
				filterDefinitions();
				int count = marketplaceItems.Count;
				for (int i = 0; i < count; i++)
				{
					MarketplaceListItemData marketplaceListItemData = marketplaceItems[i];
					GameObject gameObject = UnityEngine.Object.Instantiate(prefab, ItemContainer);
					string nameOnServer = marketplaceListItemData.PropDefn.GetNameOnServer();
					bool isOutOfStock = !inStockItems.Contains(nameOnServer);
					marketplaceListItemData.MarketplaceItem = gameObject.GetComponent<MarketplaceItem>();
					marketplaceListItemData.MarketplaceItem.Init(itemSprites[marketplaceListItemData.PropDefn.GetIconContentKey().Key.ToLower()], marketplaceListItemData.PropDefn, isOutOfStock, marketplaceListItemData.IsSpecialItem, marketplaceListItemData.UnlockLevel, itemOffSprite, itemOnSprite, itemTextColor);
					int index = i;
					gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
					{
						onItemSelected(index);
					});
					gameObject.name = string.Format("{0}Button", nameOnServer);
				}
				CoroutineRunner.Start(showItemButtons(), this, "showItemButtons");
				Canvas.ForceUpdateCanvases();
				ContentScrollRect.verticalNormalizedPosition = 1f;
				if (Service.Get<TutorialManager>().IsTutorialAvailable(MarketPlaceTutorialDefinition.Id))
				{
					TutorialManager tutorialManager = Service.Get<TutorialManager>();
					tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Combine(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onMarketPlaceTutorialComplete));
					Service.Get<TutorialManager>().TryStartTutorial(MarketPlaceTutorialDefinition.Id);
				}
				else
				{
					tryStartPartyGamesTutorial();
				}
			}
		}

		private void onMarketPlaceTutorialComplete(TutorialDefinition definition)
		{
			TutorialManager tutorialManager = Service.Get<TutorialManager>();
			tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Remove(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onMarketPlaceTutorialComplete));
			tryStartPartyGamesTutorial();
		}

		private void tryStartPartyGamesTutorial()
		{
			if (PlayerPrefs.GetInt("purchased_party_game") != 1)
			{
				Service.Get<TutorialManager>().TryStartTutorial(PartyGamesTutorialDefinition.Id);
			}
		}

		private IEnumerator showItemButtons()
		{
			yield return new WaitForSeconds(0.2f);
			int itemCount = marketplaceItems.Count;
			for (int i = 0; i < itemCount; i++)
			{
				yield return new WaitForSeconds(0.03f);
				marketplaceItems[i].MarketplaceItem.GetComponent<Animator>().SetTrigger("Intro");
			}
		}

		private void onItemSelected(int itemIndex)
		{
			MarketplaceItem marketplaceItem = marketplaceItems[itemIndex].MarketplaceItem;
			if (marketplaceItem.IsMemberLocked)
			{
				Service.Get<GameStateController>().ShowAccountSystemMembership("Marketplace");
			}
			else if (marketplaceItem.IsLevelLocked)
			{
				if (popupController == null)
				{
					InitPopupController();
				}
				MarketplaceItemData itemData = new MarketplaceItemData(marketplaceItem.ItemDefinition, marketplaceItem.RequiredLevel);
				popupController.ShowItemPopup(itemData, itemSprites[marketplaceItem.ItemDefinition.GetIconContentKey().Key.ToLower()], marketplaceItem.LockStates);
			}
			else
			{
				PrefabContentKey prefabKey = (marketplaceItem.ItemDefinition.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby) ? partyGameConfirmationPrefabKey : confirmationPrefabKey;
				CoroutineRunner.Start(loadConfirmation(prefabKey, marketplaceItem), this, "LoadMarketplaceConfirmation");
			}
		}

		private IEnumerator loadConfirmation(PrefabContentKey prefabKey, MarketplaceItem item)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(prefabKey);
			yield return request;
			GameObject newConfirmation = UnityEngine.Object.Instantiate(request.Asset, ConfirmationContainer, false);
			newConfirmation.GetComponent<MarketplaceConfirmation>().SetItem(item.ItemDefinition, item.ItemIcon.texture, this, (RectTransform)item.transform, ContentScrollRect.transform as RectTransform);
			if (confirmation != null)
			{
				HideConfirmation();
			}
			confirmation = newConfirmation;
		}

		public void HideConfirmation()
		{
			if (confirmation != null)
			{
				UnityEngine.Object.Destroy(confirmation);
			}
		}

		private bool isNewTouch()
		{
			bool flag = false;
			return UnityEngine.Input.GetMouseButtonDown(0);
		}

		public void ShowLoadingModal()
		{
			LoadingModal.SetActive(true);
		}

		public void HideLoadingModal()
		{
			LoadingModal.SetActive(false);
		}

		private void InitPopupController()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(popupPrefab);
			popupController = gameObject.GetComponent<MarketplacePopupController>();
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(gameObject));
			popupController.ItemPopup.LearnMoreButtonPressed += LearnMoreButtonPressed;
		}

		public void CloseButtonPressed()
		{
			close();
		}

		private void close()
		{
			isClosing = true;
			HideConfirmation();
			if (!screenAnimator.enabled)
			{
				screenAnimator.enabled = true;
				screenAnimatorDisabled = false;
			}
			screenAnimator.SetTrigger("Close");
			Service.Get<EventDispatcher>().DispatchEvent(new MarketplaceEvents.MarketplaceClosed(definition.Name));
		}

		public void UseItemButtonPressed()
		{
			close();
		}

		public void KeepShoppingButtonPressed()
		{
			CoroutineRunner.Start(WaitForItemTween(), this, "WaitForItemTween");
		}

		private IEnumerator WaitForItemTween()
		{
			yield return new WaitForSeconds(TrayAnimator.TweenTime);
			if (Service.Get<TutorialManager>().IsTutorialAvailable(PartySuppliesTutorialDefinition.Id))
			{
				TutorialManager tutorialManager = Service.Get<TutorialManager>();
				tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Combine(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onPartySuppliesTutorialComplete));
				if (Service.Get<TutorialManager>().TryStartTutorial(PartySuppliesTutorialDefinition.Id))
				{
					TrayAnimator.TrayAnimator.speed = 0f;
					yield break;
				}
				TutorialManager tutorialManager2 = Service.Get<TutorialManager>();
				tutorialManager2.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Remove(tutorialManager2.TutorialCompleteAction, new Action<TutorialDefinition>(onPartySuppliesTutorialComplete));
				HideConfirmation();
			}
			else
			{
				HideConfirmation();
			}
		}

		private void onPartySuppliesTutorialComplete(TutorialDefinition definition)
		{
			TutorialManager tutorialManager = Service.Get<TutorialManager>();
			tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Remove(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onPartySuppliesTutorialComplete));
			TrayAnimator.TrayAnimator.speed = 1f;
			HideConfirmation();
		}

		public void LearnMoreButtonPressed()
		{
			Service.Get<GameStateController>().ShowAccountSystemMembership("party_supplies_market");
		}

		public void AdButtonPressed()
		{
			if (popupController == null)
			{
				InitPopupController();
			}
			popupController.ShowAdPopup(currentAdItem, definition.AdDescriptionText, itemSprites[currentAdItem.PropDefn.GetIconContentKey().Key.ToLower()], currentAdBackground);
		}

		public void MarketplaceScreenIntroComplete()
		{
			if (!hasOpened)
			{
				hasOpened = true;
			}
		}

		public void MarketplaceScreenOutroComplete()
		{
			if (hasOpened)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void onMarketplaceItemAnimationComplete()
		{
			HideConfirmation();
		}
	}
}
