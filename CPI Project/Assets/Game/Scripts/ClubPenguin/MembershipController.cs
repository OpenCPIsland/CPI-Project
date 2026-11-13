using ClubPenguin.Analytics;
using ClubPenguin.Commerce;
using ClubPenguin.Mix;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class MembershipController : MonoBehaviour
	{
		private const string ARE_YOU_SURE_TRIAL_PROMPT_ID = "MembershipAreYouSureTrialPrompt";

		private const string ARE_YOU_SURE_NO_TRIAL_PROMPT_ID = "MembershipAreYouSureNoTrialPrompt";

		private StateMachine rootStateMachine;

		private MembershipService membershipService;

		private string sku;

		[Header("Prompts")]
		public PrefabContentKey MembershipPromptPrefabContentKey;

		[Header("Events")]
		public string offerContinueEvent;

		public string loginEvent;

		public string termsContinueEvent;

		public string thanksContinueEvent;

		public string backToStartEvent;

		[Header("SKUS")]
		public string firstTimeSKU;

		public string reSubscribingSKU;

		[HideInInspector]
		public AccountFlowData accountFlowData;

		public static string CARRIER_BILLING_SUPPORTED_ASSET = "carrier_billing_supported";

		private JsonData carrierBillingSupported;

		public Product CurrentProduct
		{
			get;
			set;
		}

		public event Action<Product, List<Product>> OnProductsReady;

		public event System.Action OnPurchaseFailed;

		public event System.Action OnPurchaseRetried;

		public void Start()
		{
			rootStateMachine = GetComponent<StateMachine>();
			membershipService = Service.Get<MembershipService>();
		}

		public void OnDestroy()
		{
			this.OnProductsReady = null;
			this.OnPurchaseFailed = null;
			this.OnPurchaseRetried = null;
		}

		public void MembershipOfferContinueClick()
		{
			accountFlowData = membershipService.GetAccountFlowData();
			accountFlowData.SkipMembership = true;
			rootStateMachine.SendEvent(offerContinueEvent);
		}

		public void MembershipLoginNeeded()
		{
			LayoutMappings componentInParent = GetComponentInParent<LayoutMappings>();
			if (componentInParent != null)
			{
				componentInParent.SetLayoutType("OneIdFullscreen", "center");
			}
			Service.Get<MixLoginCreateService>().LogoutLastSession();
			Service.Get<RememberMeService>().ResetCurrentUsername();
			membershipService.LoginViaMembership = true;
			rootStateMachine.SendEvent(loginEvent);
		}

		public void MembershipTermsContinueClick(string sku)
		{
			this.sku = sku;
			membershipService.OnPurchaseFailed += PurchaseFailed;
			membershipService.OnPurchaseSuccess += PurchaseSuccess;
			membershipService.TriggerPurchase(sku);
		}

		public void MembershipThanksContinueClick()
		{
			rootStateMachine.SendEvent(thanksContinueEvent);
		}

		public void MembershipBackToStart()
		{
			rootStateMachine.SendEvent(backToStartEvent);
		}

		public void ShowProductLoadedError()
		{
			DPrompt data = new DPrompt("Membership.Purchase.Error", membershipService.GetProductsLoadedErrorMsg());
			Content.LoadAsync(delegate(string path, GameObject prefab)
			{
				onMembershipPromptLoaded(data, false, prefab);
			}, MembershipPromptPrefabContentKey);
		}

		private void onMembershipPromptLoaded(DPrompt data, bool closeAfterError, GameObject membershipPromptPrefab)
		{
			PromptController component = membershipPromptPrefab.GetComponent<PromptController>();
			if (closeAfterError)
			{
				Service.Get<PromptManager>().ShowPrompt(data, onErrorClosed, component);
			}
			else
			{
				Service.Get<PromptManager>().ShowPrompt(data, null, component);
			}
		}

		public void GetProduct(bool hasTrialAvailable)
		{
			StartCoroutine(prepareProduct(hasTrialAvailable));
		}

		private IEnumerator prepareProduct(bool hasTrialAvailable)
		{
			if (membershipService.HasErrors())
			{
				ShowProductLoadedError();
			}
			else if (membershipService.ProductsReady())
			{
				Product product;
				List<Product> arg;
				if (!hasTrialAvailable)
				{
					product = membershipService.GetResubscribingProduct();
					arg = membershipService.GetResubscribingProductsToOffer();
				}
				else
				{
					product = membershipService.GetFirstTimeProduct();
					arg = membershipService.GetFirstTimeProductsToOffer();
				}
				if (product != null)
				{
					if (this.OnProductsReady != null)
					{
						this.OnProductsReady(product, arg);
					}
				}
				else
				{
					Log.LogError(this, "Product was null. Ensure that the bundle identifier is correct. hasTrialAvailable: " + hasTrialAvailable);
				}
			}
			else
			{
				yield return null;
			}
		}

		public void PurchaseSuccess()
		{
			membershipService.OnPurchaseFailed -= PurchaseFailed;
			membershipService.OnPurchaseSuccess -= PurchaseSuccess;
			rootStateMachine.SendEvent(termsContinueEvent);
		}

		public void PurchaseFailed(ApplicationService.Error error, bool closeAfterError)
		{
			membershipService.OnPurchaseFailed -= PurchaseFailed;
			membershipService.OnPurchaseSuccess -= PurchaseSuccess;
			if (error.Message == "Membership.Purchase.Error.ERROR_PURCHASE_CANCELLED_BY_USER")
			{
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "03a", "are_you_sure_view");
				string promptId = (CurrentProduct == null) ? "MembershipAreYouSureNoTrialPrompt" : (CurrentProduct.IsTrial() ? "MembershipAreYouSureTrialPrompt" : "MembershipAreYouSureNoTrialPrompt");
				Service.Get<PromptManager>().ShowPrompt(promptId, onAreYouSurePromptButtonClicked);
			}
			else
			{
				DPrompt data = new DPrompt(error.Type, error.Message);
				Content.LoadAsync(delegate(string path, GameObject prefab)
				{
					onMembershipPromptLoaded(data, closeAfterError, prefab);
				}, MembershipPromptPrefabContentKey);
			}
			if (this.OnPurchaseFailed != null)
			{
				this.OnPurchaseFailed();
			}
		}

		private void onAreYouSurePromptButtonClicked(DPrompt.ButtonFlags buttonFlags)
		{
			if (buttonFlags == DPrompt.ButtonFlags.CANCEL)
			{
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "03c", "are_you_sure_cancel");
				if (!string.IsNullOrEmpty(sku))
				{
					MembershipTermsContinueClick(sku);
					if (this.OnPurchaseRetried != null)
					{
						this.OnPurchaseRetried();
					}
				}
				else
				{
					Log.LogError(this, "sku was null or empty, cannot continue");
				}
			}
			else
			{
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "03b", "are_you_sure_yes");
			}
		}

		private void onErrorClosed(DPrompt.ButtonFlags buttonFlags)
		{
			rootStateMachine.SendEvent(thanksContinueEvent);
		}

		public string GetLegalText(string price = "", string planTitle = "")
		{
			bool flag = !string.IsNullOrEmpty(price);
			string token;
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				token = (flag ? "Membership.MembershipOffer.Legal" : "Membership.MembershipOffer.Legal.noPrice");
				break;
			case RuntimePlatform.IPhonePlayer:
				token = (flag ? "Membership.MembershipOffer.Legal.iOS" : "Membership.MembershipOffer.Legal.iOS.noPrice");
				break;
			default:
				token = (flag ? "Membership.MembershipOffer.Legal.Disney" : "Membership.MembershipOffer.Legal.Disney.noPrice");
				break;
			}
			if (flag)
			{
				return string.Format(Service.Get<Localizer>().GetTokenTranslation(token), price, planTitle);
			}
			return Service.Get<Localizer>().GetTokenTranslation(token);
		}

		public string GetShortLegalText()
		{
			string token;
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				token = "Account.NewMemberPanel.Body.Android";
				break;
			case RuntimePlatform.IPhonePlayer:
				token = "Account.NewMemberPanel.Body.iOS";
				break;
			default:
				token = "Account.NewMemberPanel.Body.iOS";
				break;
			}
			return Service.Get<Localizer>().GetTokenTranslation(token);
		}

		private void initializeCarrierBillingSupported()
		{
			carrierBillingSupported = Content.LoadImmediate(new TypedAssetContentKey<JsonData>(CARRIER_BILLING_SUPPORTED_ASSET));
			if (carrierBillingSupported != null && carrierBillingSupported.Count > 0)
			{
			}
		}

		public bool IsCarrierBillingAvailable()
		{
			if (carrierBillingSupported == null)
			{
				initializeCarrierBillingSupported();
			}
			string prop_name = "skip";
			if (carrierBillingSupported.Contains(prop_name))
			{
				Dictionary<string, string> deviceInfo = Service.Get<ICPSwrveService>().GetDeviceInfo();
				string value = MembershipService.OverrideSimCountryCode;
				if (string.IsNullOrEmpty(value))
				{
					deviceInfo.TryGetValue("swrve.sim_operator.iso_country_code", out value);
				}
				string value2 = MembershipService.OverrideSimCarrierName;
				if (string.IsNullOrEmpty(value2))
				{
					deviceInfo.TryGetValue("swrve.sim_operator.name", out value2);
				}
				if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2) && carrierBillingSupported[prop_name].Contains(value.ToUpper()))
				{
					JsonData jsonData = carrierBillingSupported[prop_name][value.ToUpper()];
					foreach (object item in (IEnumerable)jsonData)
					{
						if (item.ToString().Trim().ToUpper() == value2.Trim().ToUpper())
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
