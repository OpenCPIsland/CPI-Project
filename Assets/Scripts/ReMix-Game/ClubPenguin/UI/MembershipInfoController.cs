using ClubPenguin.Commerce;
using ClubPenguin.ContentGates;
using ClubPenguin.Core;
using DevonLocalization;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MembershipInfoController : MonoBehaviour
	{
		private const string LEGAL_TEXT_TOKEN = "Settings.MembershipInfo.Legal1";

		private const string LEGAL_TEXT_ADDITIONAL_TOKEN = "Settings.Membershipinfo.Legal1.Additional";

		private const string RENEW_FINISHED_TOKEN = "Settings.MembershipInfo.FinishedTitle";

		public LocalizedText PriceTitleText;

		public GameObject PriceTitleSpinner;

		public LocalizedText RenewTitleText;

		public Text PriceText;

		public Text RenewDateText;

		public Text PriceLegalText;

		public Button ManageSubscriptionButton;

		public GameObject RenewalPanel;

		public PromptDefinitionKey MembershipDifferentPlatform;

		private ParentGate gate;

		private MembershipData membershipData;

		private SubscriptionData subscriptionData;

		private CommerceProcessorCSG csgProcessor;

		private void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			string text = "";
			string text2 = "";
			bool flag = false;
			bool productIsRecurring = true;
			bool flag2 = false;
			PriceTitleText.gameObject.SetActive(false);
			PriceTitleSpinner.gameObject.SetActive(true);
			gate = new ParentGate();
			gate.OnContinue += onGatePassed;
			RenewalPanel.SetActive(true);
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out subscriptionData))
			{
				string subscriptionProductId = subscriptionData.SubscriptionProductId;
				if (!string.IsNullOrEmpty(subscriptionProductId))
				{
					Product productBySku = Service.Get<CommerceService>().GetProductBySku(subscriptionProductId);
					if (productBySku != null)
					{
						flag = true;
						productIsRecurring = productBySku.IsRecurring();
						text = string.Format("{0} {1}", productBySku.price, productBySku.currencyCode);
						PriceTitleText.token = "Membership.MembershipTerms.TermsTitle." + productBySku.sku_duration;
						PriceTitleText.UpdateToken();
						if (!subscriptionData.SubscriptionRecurring)
						{
							RenewTitleText.token = "Settings.MembershipInfo.FinishedTitle";
							RenewTitleText.UpdateToken();
						}
						PriceTitleText.gameObject.SetActive(true);
						PriceTitleSpinner.gameObject.SetActive(false);
					}
					else
					{
						productBySku = Service.Get<CommerceService>().GetProductByVendorSku(subscriptionData.SubscriptionVendor, subscriptionProductId);
						if (productBySku != null)
						{
							flag = true;
							productIsRecurring = productBySku.IsRecurring();
							PriceTitleText.token = "Membership.MembershipTerms.TermsTitle." + productBySku.sku_duration;
							PriceTitleText.UpdateToken();
							if (!subscriptionData.SubscriptionRecurring)
							{
								RenewTitleText.token = "Settings.MembershipInfo.FinishedTitle";
								RenewTitleText.UpdateToken();
							}
							PriceTitleText.gameObject.SetActive(true);
							PriceTitleSpinner.gameObject.SetActive(false);
						}
						else if (!Service.Get<CommerceService>().MatchCurrentVendor(subscriptionData.SubscriptionVendor) && subscriptionData.SubscriptionVendor.ToLower() == "csg")
						{
							csgProcessor = new CommerceProcessorCSG();
							if (csgProcessor != null)
							{
								csgProcessor.InitializeStore();
								List<Product> allProducts = Service.Get<CommerceService>().GetAllProducts();
								foreach (Product item in allProducts)
								{
									csgProcessor.AddProduct(item.shared_key, item.gp_store_sku, item.apple_store_sku, item.csg_id, item.sku_duration, item.sku_trial_duration);
								}
								CommerceProcessorCSG commerceProcessorCSG = csgProcessor;
								commerceProcessorCSG.SkuInventoryResponse = (CommerceProcessor.SkuInventoryResponseSend)Delegate.Combine(commerceProcessorCSG.SkuInventoryResponse, new CommerceProcessor.SkuInventoryResponseSend(handleCSGDataLoaded));
								csgProcessor.GetSKUDetails();
								flag2 = true;
							}
						}
					}
				}
			}
			else
			{
				Log.LogError(this, "Could not find SubscriptionData on the local player");
			}
			if (!flag && !flag2)
			{
				PriceTitleText.token = "Settings.MembershipInfo.MonthlyCostUnavailableTitle";
				PriceTitleText.UpdateToken();
				PriceText.gameObject.SetActive(false);
				RenewalPanel.SetActive(false);
				PriceTitleText.gameObject.SetActive(true);
				PriceTitleSpinner.gameObject.SetActive(false);
			}
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out membershipData))
			{
				text2 = membershipData.MembershipExpireDateTime.ToString("d");
			}
			else
			{
				Log.LogError(this, "Could not find MembershipData on the local player");
			}
			PriceText.text = text;
			RenewDateText.text = text2;
			PriceLegalText.text = getLegalText(productIsRecurring, subscriptionData.SubscriptionRecurring, text);
			ManageSubscriptionButton.onClick.AddListener(onClicked);
		}

		private void handleCSGDataLoaded(List<SkuInfo> listSkus, CommerceError cError)
		{
			if (!cError.HasError())
			{
				Product productByVendorSku = csgProcessor.productList.GetProductByVendorSku("csg", subscriptionData.SubscriptionProductId);
				if (productByVendorSku != null)
				{
					PriceText.text = string.Format("{0} {1}", productByVendorSku.price, productByVendorSku.currencyCode);
					PriceTitleText.token = "Membership.MembershipTerms.TermsTitle." + productByVendorSku.sku_duration;
					PriceTitleText.UpdateToken();
					if (!subscriptionData.SubscriptionRecurring)
					{
						RenewTitleText.token = "Settings.MembershipInfo.FinishedTitle";
						RenewTitleText.UpdateToken();
					}
					PriceLegalText.text = getLegalText(productByVendorSku.IsRecurring(), subscriptionData.SubscriptionRecurring, productByVendorSku.price);
					PriceText.gameObject.SetActive(true);
					RenewalPanel.SetActive(true);
					PriceTitleText.gameObject.SetActive(true);
					PriceTitleSpinner.gameObject.SetActive(false);
					return;
				}
			}
			PriceTitleText.token = "Settings.MembershipInfo.MonthlyCostUnavailableTitle";
			PriceTitleText.UpdateToken();
			PriceText.gameObject.SetActive(false);
			RenewalPanel.SetActive(false);
			PriceTitleText.gameObject.SetActive(true);
			PriceTitleSpinner.gameObject.SetActive(false);
		}

		private string getLegalText(bool productIsRecurring, bool subscriptionIsRecurring, string price = "")
		{
			if (!productIsRecurring || !subscriptionIsRecurring)
			{
				return "";
			}
			string text = Service.Get<Localizer>().GetTokenTranslation("Settings.MembershipInfo.Legal1");
			if (!string.IsNullOrEmpty(price))
			{
				text = text + " " + string.Format(Service.Get<Localizer>().GetTokenTranslation("Settings.Membershipinfo.Legal1.Additional"), PriceText.text);
			}
			return text;
		}

		private void onClicked()
		{
			if (Service.Get<CommerceService>().MatchCurrentVendor(subscriptionData.SubscriptionVendor))
			{
				gate.Show(base.transform);
			}
			else
			{
				Service.Get<PromptManager>().ShowPrompt(MembershipDifferentPlatform.Id, null);
			}
		}

		private void onGatePassed()
		{
			Service.Get<CommerceService>().TriggerManageAccount(subscriptionData.SubscriptionVendor);
		}

		private void OnDestroy()
		{
			if (gate != null)
			{
				gate.OnContinue -= onGatePassed;
			}
		}
	}
}
