using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Commerce;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class MembershipTermsContentController : MonoBehaviour
	{
		private enum ButtonState
		{
			disabled,
			enabled,
			loading
		}

		[Header("Confirm Purchase")]
		public Button ConfirmButton;

		public Text ConfirmButtonText;

		public GameObject ConfirmLoadingImage;

		public Button DisabledConfirmButton;

		[Header("Penguin Details")]
		public Text PenguinName;

		public Button ChangePenguin;

		public GameObject ChangePenguinContainer;

		[Header("Terms")]
		public ScrollRect TermsScrollRect;

		public Text Terms;

		public Text LegalText;

		[Header("Carrier Billing")]
		public bool showCarrierBilling;

		public GameObject CarrierBillingPanel;

		[Header("Product Details")]
		public Text CostText;

		public Text DurationText;

		public Text CostTitleText;

		public Text CostTrialText;

		public Text CurrencyCode;

		[Header("Products To Offer")]
		public Button ChangeProduct;

		public GameObject ChangeProductPopup;

		public Transform OfferButtonContainer;

		public Button AcceptProductChoice;

		public SubscriptionOptionButton OfferButtonWithRecurringPrefab;

		public SubscriptionOptionButton OfferButtonWithoutRecurringPrefab;

		[Header("Panels")]
		public GameObject Pending;

		public GameObject Ready;

		public GameObject MembershipInProcess;

		public PrefabContentKey WebViewPrefab;

		private MembershipController membershipController;

		private AvatarRenderTextureComponent avatarRenderer;

		private Product product;

		private ButtonState currentButtonState;

		private List<SubscriptionOptionButton> offerButtons;

		private BackgroundButtonsController backgroundButtons;

		private void OnEnable()
		{
			ConfirmButton.onClick.AddListener(onConfirmClicked);
			SetButtonInteraction(ButtonState.disabled);
			ChangePenguin.onClick.AddListener(onChangePenguin);
			ChangeProduct.onClick.AddListener(onChangeProduct);
			AcceptProductChoice.onClick.AddListener(onAcceptProductChoice);
			Service.Get<EventDispatcher>().AddListener<IAPServiceEvents.PCPurchaseSuccess>(onProviderPurchaseSuccess);
			Pending.SetActive(true);
			Ready.SetActive(false);
			MembershipInProcess.SetActive(false);
		}

		private void Start()
		{
			membershipController = GetComponentInParent<MembershipController>();
			membershipController.OnPurchaseRetried += onPurchaseRetried;
			backgroundButtons = GetComponentInParent<BackgroundButtonsController>();
			showCarrierBilling = membershipController.IsCarrierBillingAvailable();
			string message = showCarrierBilling ? "with_carrier_billing_info" : "regular";
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "03", "membership_terms", message);
			PenguinName.text = Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayName;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (!(SceneManager.GetActiveScene().name == "Boot"))
			{
				avatarRenderer = GetComponentInChildren<AvatarRenderTextureComponent>();
				AvatarAnimationFrame avatarFrame = new AvatarAnimationFrame("Base Layer.Interactions.PassPortPoses_CelebrateAnimation", 0f);
				AvatarDetailsData component;
				if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component))
				{
					avatarRenderer.RenderAvatar(component, avatarFrame);
				}
				else
				{
					avatarRenderer.RenderAvatar(new DCustomEquipment[0], avatarFrame);
				}
			}
			bool hasTrialAvailable = true;
			ProfileData component2;
			MembershipData component3;
			if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component2) && cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component3))
			{
				if (component2.IsFirstTimePlayer || SceneManager.GetActiveScene().name != Service.Get<GameStateController>().SceneConfig.HomeSceneName)
				{
					ChangePenguinContainer.SetActive(false);
				}
				else
				{
					ChangePenguinContainer.SetActive(true);
				}
				hasTrialAvailable = component3.MembershipTrialAvailable;
			}
			membershipController.OnProductsReady += onProductsReady;
			membershipController.GetProduct(hasTrialAvailable);
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Accessibility.Popup.Title.MembershipTerms");
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
			}
		}

		private void onProductsReady(Product productToShow, List<Product> productsToOffer)
		{
			membershipController.OnProductsReady -= onProductsReady;
			showProduct(productToShow);
			showOptions(productsToOffer);
			TermsScrollRect.onValueChanged.AddListener(onContentScrolled);
			Pending.SetActive(false);
			Ready.SetActive(true);
			MembershipInProcess.SetActive(false);
		}

		private void showProduct(Product productToShow)
		{
			product = productToShow;
			membershipController.CurrentProduct = productToShow;
			string text = "";
			string text2 = "";
			string text3 = "";
			string text4 = "";
			bool flag = true;
			string formatToken = "Membership.MembershipTerms.LegalText";
			string text5 = "Membership.MembershipTerms.TermsTitle." + product.sku_duration;
			text3 = Service.Get<Localizer>().GetTokenTranslation(text5);
			if (product.IsTrial())
			{
				text = Service.Get<Localizer>().GetTokenTranslationFormatted(formatToken, text5, product.price, "Membership.MembershipTerms.ConfirmButtonFreeTrial");
				text2 = Service.Get<Localizer>().GetTokenTranslation("Membership.MembershipTerms.ConfirmButtonFreeTrial");
				text4 = Service.Get<Localizer>().GetTokenTranslation("Membership.MembershipTerms.TrialText");
				flag = true;
			}
			else
			{
				text = Service.Get<Localizer>().GetTokenTranslationFormatted(formatToken, text5, product.price, "Membership.MembershipTerms.ConfirmButtonFreeTrial");
				text2 = Service.Get<Localizer>().GetTokenTranslation("Membership.MembershipTerms.ConfirmButtonRegular");
				text4 = "";
				flag = false;
			}
			Terms.text = text;
			ConfirmButtonText.text = text2;
			CurrencyCode.text = product.currencyCode;
			LegalText.gameObject.SetActive(flag);
			LegalText.text = membershipController.GetLegalText(product.price, text3);
			CarrierBillingPanel.SetActive(showCarrierBilling);
			CostTitleText.text = text3;
			CostTrialText.text = text4;
			CostText.text = product.price;
			DurationText.text = "/" + text3;
		}

		private void showOptions(List<Product> productsToOffer)
		{
			if (productsToOffer.Count > 1)
			{
				ChangeProduct.gameObject.SetActive(true);
				foreach (Transform item in OfferButtonContainer.transform)
				{
					Object.Destroy(item.gameObject);
				}
				offerButtons = new List<SubscriptionOptionButton>();
				foreach (Product item2 in productsToOffer)
				{
					GameObject gameObject = (!item2.IsRecurring()) ? Object.Instantiate(OfferButtonWithoutRecurringPrefab.gameObject) : Object.Instantiate(OfferButtonWithRecurringPrefab.gameObject);
					gameObject.transform.SetParent(OfferButtonContainer, false);
					SubscriptionOptionButton component = gameObject.GetComponent<SubscriptionOptionButton>();
					component.SetProduct(item2, item2 == product);
					offerButtons.Add(component);
				}
			}
			else
			{
				ChangeProduct.gameObject.SetActive(false);
			}
		}

		private void onChangeProduct()
		{
			ChangeProductPopup.SetActive(true);
			GameObject[] enableButtons = backgroundButtons.EnableButtons;
			foreach (GameObject gameObject in enableButtons)
			{
				gameObject.SetActive(false);
			}
			Service.Get<BackButtonController>().Add(onAcceptProductChoice);
		}

		private void onAcceptProductChoice()
		{
			Service.Get<BackButtonController>().Remove(onAcceptProductChoice);
			GameObject[] enableButtons = backgroundButtons.EnableButtons;
			foreach (GameObject gameObject in enableButtons)
			{
				gameObject.SetActive(true);
			}
			foreach (SubscriptionOptionButton offerButton in offerButtons)
			{
				if (offerButton.IsSelected)
				{
					showProduct(offerButton.GetProduct());
					break;
				}
			}
			ChangeProductPopup.SetActive(false);
		}

		private void onConfirmClicked()
		{
			SetButtonInteraction(ButtonState.loading);
			membershipController.OnPurchaseFailed += onPurchaseFailed;
			membershipController.MembershipTermsContinueClick(product.shared_key);
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "04", "iap");
			Service.Get<ICPSwrveService>().NavigationAction("membership_buttons.TermsConfirm");
		}

		private bool onProviderPurchaseSuccess(IAPServiceEvents.PCPurchaseSuccess evt)
		{
			Pending.SetActive(false);
			Ready.SetActive(false);
			MembershipInProcess.SetActive(true);
			return false;
		}

		private void onPurchaseFailed()
		{
			membershipController.OnPurchaseFailed -= onPurchaseFailed;
			SetButtonInteraction(ButtonState.enabled);
		}

		private void onPurchaseRetried()
		{
			SetButtonInteraction(ButtonState.loading);
			membershipController.OnPurchaseFailed += onPurchaseFailed;
		}

		private void onChangePenguin()
		{
			Service.Get<SessionManager>().Logout();
			membershipController.MembershipLoginNeeded();
			Service.Get<ICPSwrveService>().NavigationAction("membership_buttons.ChangePenguin");
		}

		private void onContentScrolled(Vector2 scroll_pos)
		{
			if (scroll_pos.y <= 0.05f && currentButtonState == ButtonState.disabled)
			{
				SetButtonInteraction(ButtonState.enabled);
				Service.Get<ICPSwrveService>().NavigationAction("membership_buttons.TermsConfirmEnabled");
			}
		}

		private void SetButtonInteraction(ButtonState state)
		{
			currentButtonState = state;
			switch (state)
			{
			case ButtonState.disabled:
				ConfirmButton.interactable = false;
				ConfirmButtonText.gameObject.SetActive(true);
				ConfirmLoadingImage.SetActive(false);
				DisabledConfirmButton.gameObject.SetActive(true);
				break;
			case ButtonState.enabled:
				ConfirmButton.interactable = true;
				ConfirmButtonText.gameObject.SetActive(true);
				ConfirmLoadingImage.SetActive(false);
				DisabledConfirmButton.gameObject.SetActive(false);
				break;
			case ButtonState.loading:
				ConfirmButton.interactable = false;
				ConfirmButtonText.gameObject.SetActive(false);
				ConfirmLoadingImage.SetActive(true);
				DisabledConfirmButton.gameObject.SetActive(false);
				break;
			}
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<IAPServiceEvents.PCPurchaseSuccess>(onProviderPurchaseSuccess);
			if (membershipController != null)
			{
				membershipController.CurrentProduct = null;
				membershipController.OnPurchaseRetried -= onPurchaseRetried;
			}
		}
	}
}
