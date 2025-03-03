using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Environment;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Commerce
{
	public class CommerceProcessorCSG : CommerceProcessor
	{
		private enum DurationCode
		{
			H = 0,
			D = 7,
			W = 1,
			M = 2,
			S = 4,
			Y = 5,
			Q = 6
		}

		private List<SkuInfo> retrievedSkuInfo = null;

		private bool areListenersEnabled = false;

		private bool isSkuLookupInProgress = false;

		private bool skuLookupPending = false;

		private bool isSessionRefreshInProcess;

		private WebViewPurchaseController webViewPurchaseController;

		private CSGConfig csgConfig;

		private PurchaseInfo currentPurchaseInfo;

		private SkuInfo currentPurchaseSkuInfo;

		private int currentCheckWaitTime = 5;

		private bool hasShownError = false;

		private string productInProcess;

		private void enableListners()
		{
			if (!areListenersEnabled)
			{
				CommerceLog("Enabling listners");
				EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
				eventDispatcher.AddListener<SessionEvents.SessionLogoutEvent>(onLogout);
				eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
				Localizer localizer = Service.Get<Localizer>();
				localizer.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Combine(localizer.TokensUpdated, new Localizer.TokensUpdatedDelegate(onLanguageChanged));
				eventDispatcher.AddListener<IAPServiceEvents.SessionStarted>(onSessionStarted);
				eventDispatcher.AddListener<IAPServiceErrors.SessionExpired>(onSessionExpired);
				eventDispatcher.AddListener<IAPServiceErrors.SessionStartError>(onSessionStartError);
				eventDispatcher.AddListener<IAPServiceEvents.PCProductDetailsReturned>(onProductListReceivedEvent);
				eventDispatcher.AddListener<IAPServiceErrors.ProductsDetailsLoadedError>(onProductListRequestFailedEvent);
				eventDispatcher.AddListener<IAPServiceEvents.PurchaseReturned>(onPurchaseVerifyReturned, EventDispatcher.Priority.FIRST);
				eventDispatcher.AddListener<IAPServiceEvents.PCPurchaseSuccess>(onPurchaseSuccessfulEvent);
				eventDispatcher.AddListener<IAPServiceErrors.PCPurchaseError>(onPurchaseFailedEvent);
				eventDispatcher.AddListener<IAPServiceEvents.PCPurchaseCancelled>(onPurchaseCancelledEvent);
				areListenersEnabled = true;
			}
		}

		private void disableListners()
		{
			CommerceLog("Disabling listners");
			setBillingSupported(false);
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.RemoveListener<SessionEvents.SessionLogoutEvent>(onLogout);
			eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			Localizer localizer = Service.Get<Localizer>();
			localizer.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Remove(localizer.TokensUpdated, new Localizer.TokensUpdatedDelegate(onLanguageChanged));
			eventDispatcher.RemoveListener<IAPServiceEvents.SessionStarted>(onSessionStarted);
			eventDispatcher.RemoveListener<IAPServiceErrors.SessionExpired>(onSessionExpired);
			eventDispatcher.RemoveListener<IAPServiceErrors.SessionStartError>(onSessionStartError);
			eventDispatcher.RemoveListener<IAPServiceEvents.PCProductDetailsReturned>(onProductListReceivedEvent);
			eventDispatcher.RemoveListener<IAPServiceErrors.ProductsDetailsLoadedError>(onProductListRequestFailedEvent);
			eventDispatcher.RemoveListener<IAPServiceEvents.PurchaseReturned>(onPurchaseVerifyReturned);
			eventDispatcher.RemoveListener<IAPServiceEvents.PCPurchaseSuccess>(onPurchaseSuccessfulEvent);
			eventDispatcher.RemoveListener<IAPServiceErrors.PCPurchaseError>(onPurchaseFailedEvent);
			eventDispatcher.RemoveListener<IAPServiceEvents.PCPurchaseCancelled>(onPurchaseCancelledEvent);
			areListenersEnabled = false;
		}

		public override void InitializeStore(string token = "")
		{
			CommerceLog("<color='magenta'>initializeStore:</color> with token" + token);
			enableListners();
			Configurator configurator = Service.Get<Configurator>();
			IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem("CSGConfig");
			csgConfig = default(CSGConfig);
			csgConfig.SupportUrl = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.SettingsURLs.cphelp");
			csgConfig.Language = LocalizationLanguage.GetCultureLanguageString(Service.Get<Localizer>().Language);
			Dictionary<string, CommerceResourceURLsDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, CommerceResourceURLsDefinition>>();
			Disney.Kelowna.Common.Environment.Environment developmentEnvironment = Disney.Kelowna.Common.Environment.Environment.PRODUCTION;
			NetworkServicesConfig networkServicesConfig = NetworkController.GenerateNetworkServiceConfig(developmentEnvironment);
			CommerceResourceURLsDefinition value;
			if (dictionary.TryGetValue(networkServicesConfig.CommerceResourceURLsDefinition, out value))
			{
				csgConfig.BaseUrl = value.BaseURL;
				csgConfig.JavascriptUrls = value.JavascriptURLs;
				csgConfig.CSSUrls = value.CSSURLs;
			}
			string key = "windows";
			IDictionary<string, object> dictionary2 = (IDictionary<string, object>)dictionaryForSystem["PROD"];
			IDictionary<string, object> dictionary3 = (IDictionary<string, object>)dictionary2[key];
			IDictionary<string, object> dictionary4 = (IDictionary<string, object>)dictionary2["base"];
			csgConfig.DeviceType = dictionary3["DeviceType"].ToString();
			csgConfig.DistributionChannelId = dictionary3["DistributionChannelId"].ToString();
			csgConfig.PaypalMerchantId = dictionary4["PaypalMerchantId"].ToString();
			csgConfig.PaypalEnvironment = dictionary4["PaypalEnvironment"].ToString();
			csgConfig.SystemId = dictionary4["SystemId"].ToString();
			csgConfig.ServicesDomain = dictionary4["ServicesDomain"].ToString();
			csgConfig.MetadataDomain = dictionary4["MetadataDomain"].ToString();
		}

		public override void ChangeCommerceResourceURLs(string CommerceResourceURLsDefinitionName)
		{
			CommerceLog("ChangeCommerceResourceURLs to use CommerceResourceURLsDefinitionName=" + CommerceResourceURLsDefinitionName);
			Dictionary<string, CommerceResourceURLsDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, CommerceResourceURLsDefinition>>();
			CommerceResourceURLsDefinition value;
			if (dictionary.TryGetValue(CommerceResourceURLsDefinitionName, out value))
			{
				csgConfig.BaseUrl = value.BaseURL;
				csgConfig.JavascriptUrls = value.JavascriptURLs;
				csgConfig.CSSUrls = value.CSSURLs;
			}
		}

		private void initWebViewPurchaseController()
		{
			if (webViewPurchaseController == null)
			{
				webViewPurchaseController = new WebViewPurchaseController();
			}
		}

		private void onLanguageChanged()
		{
			csgConfig.Language = LocalizationLanguage.GetCultureLanguageString(Service.Get<Localizer>().Language);
		}

		private bool onLogout(SessionEvents.SessionLogoutEvent session)
		{
			CommerceLog("Logging out so removing session: " + csgConfig.SessionID);
			productList.ClearStoreDetailsAllProducts();
			csgConfig.SessionID = null;
			csgConfig.SessionSummary = null;
			setBillingSupported(false);
			retrievedSkuInfo = null;
			isSkuLookupInProgress = false;
			skuLookupPending = false;
			isPurchaseInProgress = false;
			return false;
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			if (productList.GetAllProducts().Count > 0)
			{
				skuLookupPending = true;
			}
			StartSession();
			return false;
		}

		private void StartSession(bool isRefresh = false)
		{
			CommerceLog("StartSession");
			isSessionRefreshInProcess = isRefresh;
			PCSessionStartRequest pcSessionRequest = default(PCSessionStartRequest);
			pcSessionRequest.DistributionChannelId = csgConfig.DistributionChannelId;
			pcSessionRequest.DeviceType = csgConfig.DeviceType;
			pcSessionRequest.Language = csgConfig.Language;
			Service.Get<INetworkServicesManager>().IAPService.StartPCSession(pcSessionRequest);
		}

		public bool onSessionStarted(IAPServiceEvents.SessionStarted session)
		{
			CommerceLog("onSessionStarted sessionID: " + session.SessionID);
			csgConfig.SessionID = session.SessionID;
			csgConfig.SessionSummary = session.SessionSummary;
			if (!string.IsNullOrEmpty(csgConfig.SessionID))
			{
				sendBillingEnabledResponse(true);
				setBillingSupported(true);
				if (skuLookupPending)
				{
					GetSKUDetails();
				}
				if (isSessionRefreshInProcess)
				{
					initWebViewPurchaseController();
					if (isPurchaseInProgress)
					{
						webViewPurchaseController.ReloadPurchaseFlow(csgConfig, productInProcess);
					}
					else
					{
						webViewPurchaseController.ReloadManageAccountFlow(csgConfig);
					}
				}
			}
			else
			{
				CommerceError cError = new CommerceError(100, "Unable to start CSG session");
				sendBillingEnabledResponse(false, cError);
			}
			return false;
		}

		public bool onSessionStartError(IAPServiceErrors.SessionStartError sessionError)
		{
			CommerceLog("onSessionStartError");
			CommerceError cError = new CommerceError(100, "Unable to start CSG session");
			sendBillingEnabledResponse(false, cError);
			return false;
		}

		public bool onSessionExpired(IAPServiceErrors.SessionExpired sessionError)
		{
			StartSession(true);
			return false;
		}

		private bool onProductListReceivedEvent(IAPServiceEvents.PCProductDetailsReturned productDetailsReturned)
		{
			List<CSGInfo> list = new List<CSGInfo>();
			List<CSGInfo> list2 = new List<CSGInfo>();
			csgConfig.ProductId = productDetailsReturned.ProductDetails.Context.ProductContext.ProductId;
			List<PCPricePlan> pricingPlans = productDetailsReturned.ProductDetails.Product.PricingPlans;
			List<PCOrderablePricePlan> orderablePricingPlans = productDetailsReturned.ProductDetails.Context.ProductContext.OrderablePricingPlans;
			if (orderablePricingPlans == null || pricingPlans == null)
			{
				CommerceError commerceError = null;
				commerceError = new CommerceError(201, "Inventory Query Failed");
				sendInventoryResponse(commerceError);
				return false;
			}
			CommerceLog(string.Format("PCProductDetailsReturned. total price plans: {0}", orderablePricingPlans.Count));
			retrievedSkuInfo = new List<SkuInfo>();
			Dictionary<long, PCPricePlan> dictionary = new Dictionary<long, PCPricePlan>();
			Dictionary<long, PCOrderablePricePlan> dictionary2 = new Dictionary<long, PCOrderablePricePlan>();
			foreach (PCPricePlan item2 in pricingPlans)
			{
				dictionary.Add(item2.Id, item2);
			}
			foreach (PCOrderablePricePlan item3 in orderablePricingPlans)
			{
				dictionary2.Add(item3.Id, item3);
			}
			foreach (PCPricePlan item4 in pricingPlans)
			{
				CommerceLog(string.Format("Processing availablePlan: Id={0}, Amount={1}, Currency={2}, Subscription={3}, BillingCycle={4}, BillingIterations={5}, CycleName={6}", item4.Id, item4.ChargeAmount, item4.Currency, item4.Subscription, item4.SubscriptionBillingCycle, item4.SubscriptionBillingCycleIterations, item4.SubscriptionBillingCycleName));
				string text = "";
				PCPricePlan current;
				PCPricePlan pCPricePlan = current = item4;
				CommerceLog(string.Format("Setting Main plan: Id={0}, Amount={1}, Currency={2}, Subscription={3}, BillingCycle={4}, BillingIterations={5}, CycleName={6}", current.Id, current.ChargeAmount, current.Currency, current.Subscription, current.SubscriptionBillingCycle, current.SubscriptionBillingCycleIterations, current.SubscriptionBillingCycleName));
				if (current.AdditionalProperties != null)
				{
					foreach (AdditionalPricePlanProperties additionalProperty in current.AdditionalProperties)
					{
						if (additionalProperty.Name == "FALLBACK_PLAN")
						{
							CommerceLog("Found FALLBACK_PLAN in AdditionalProperties");
							long result;
							if (long.TryParse(additionalProperty.Values[0], out result))
							{
								CommerceLog("Found fallback_id=" + result);
								if (dictionary.ContainsKey(result))
								{
									pCPricePlan = dictionary[result];
									CommerceLog(string.Format("Setting Fallback plan: Id={0}, Amount={1}, Currency={2}, Subscription={3}, BillingCycle={4}, BillingIterations={5}, CycleName={6}", pCPricePlan.Id, pCPricePlan.ChargeAmount, pCPricePlan.Currency, pCPricePlan.Subscription, pCPricePlan.SubscriptionBillingCycle, pCPricePlan.SubscriptionBillingCycleIterations, pCPricePlan.SubscriptionBillingCycleName));
								}
							}
						}
					}
				}
				if (current.ExternalReferences != null)
				{
					foreach (ExternalReference externalReference in current.ExternalReferences)
					{
						if (externalReference.ExternalId == "CPI_PricingPlan")
						{
							text = externalReference.Value.ToString();
							CommerceLog("CSG CPI_PricingPlan=" + text);
						}
					}
				}
				string trialDuration = "";
				string text2 = "";
				text2 = getDuration(pCPricePlan.SubscriptionBillingCycleIterations.ToString(), (DurationCode)pCPricePlan.SubscriptionBillingCycle);
				if (pCPricePlan.Id != current.Id)
				{
					CommerceLog("Plan has a fallback plan therefore has Trial");
					trialDuration = getDuration(current.SubscriptionBillingCycleIterations.ToString(), (DurationCode)current.SubscriptionBillingCycle);
				}
				bool flag = dictionary2.ContainsKey(current.Id);
				CommerceLog("Is this plan (" + current.Id + ") purchaseable? " + flag);
				SkuInfo skuData = new SkuInfo(pCPricePlan.DisplayName, GetCurrencyFormattedNumber(pCPricePlan.ChargeAmount, pCPricePlan.Currency), "", pCPricePlan.Description, current.Id.ToString(), pCPricePlan.Currency, GetCurrencySymbol(pCPricePlan.Currency), flag);
				CSGInfo item = new CSGInfo(current.Id.ToString(), text, text2, trialDuration, skuData);
				if (flag)
				{
					list.Add(item);
				}
				else
				{
					list2.Add(item);
				}
				CommerceLog("Adding sku to list: " + current.Id);
			}
			CommerceLog("<color=magenta>Calling Inventory Response</color>");
			foreach (CSGInfo item5 in list)
			{
				CommerceLog("Processing orderable plan=" + item5.PlanId);
				Product productByDuration = productList.GetProductByDuration(item5.SkuDuration, item5.TrialDuration);
				if (productByDuration != null && productByDuration.NeedsSkuInformation())
				{
					productByDuration.SetCSGData(item5.PlanId, item5.ExternalReference);
					productByDuration.AddStoreInformation(item5.SkuData);
					retrievedSkuInfo.Add(item5.SkuData);
					CommerceLog("Updated product for plan=" + item5.PlanId + " product=" + productByDuration);
				}
			}
			foreach (CSGInfo item6 in list2)
			{
				CommerceLog("Processing non orderable plan=" + item6.PlanId);
				Product productByDuration = productList.GetProductByDuration(item6.SkuDuration, item6.TrialDuration);
				if (productByDuration != null && productByDuration.NeedsSkuInformation())
				{
					productByDuration.SetCSGData(item6.PlanId, item6.ExternalReference);
					productByDuration.AddStoreInformation(item6.SkuData);
					retrievedSkuInfo.Add(item6.SkuData);
					CommerceLog("Updated product for plan=" + item6.PlanId + " product=" + productByDuration);
				}
			}
			sendInventoryResponse(retrievedSkuInfo);
			CommerceLog("Finished getting Price Plan Info");
			return false;
		}

		private string getDuration(string Iterations, DurationCode Cycle)
		{
			string text = Iterations + Cycle;
			if (text == "1Y")
			{
				return "12M";
			}
			if (text == "1Q")
			{
				return "3M";
			}
			return text;
		}

		private bool onProductListRequestFailedEvent(IAPServiceErrors.ProductsDetailsLoadedError pdle)
		{
			isSkuLookupInProgress = false;
			CommerceError commerceError = null;
			commerceError = new CommerceError(202, "Inventory Query Failed");
			sendInventoryResponse(commerceError);
			return false;
		}

		public override void PurchaseProductFromStore(string product, string payload)
		{
			CommerceLog("PurchaseProductFromStore is called with product " + product);
			if (!isPurchaseInProgress)
			{
				initWebViewPurchaseController();
				webViewPurchaseController.ShowPurchaseFlow(csgConfig, product);
				isPurchaseInProgress = true;
				productInProcess = product;
			}
			else
			{
				CommerceLog("PurchaseProduct: Purchase already in progress, skipping");
			}
		}

		private bool onPurchaseFailedEvent(IAPServiceErrors.PCPurchaseError error)
		{
			CommerceLog("purchaseFailedEvent: " + error.PurchaseError.ToJson());
			isPurchaseInProgress = false;
			JsonData jsonData = error.PurchaseError.Contains("error") ? error.PurchaseError["error"] : null;
			CommerceError commerceError = null;
			if (jsonData != null && jsonData.Contains("Code") && jsonData.Contains("Message"))
			{
				string text = jsonData["Code"].ToString();
				int num;
				switch (text)
				{
				case "1034":
					commerceError = new CommerceError(301, (string)jsonData["Message"]);
					break;
				default:
					num = ((!(text == "25")) ? 1 : 0);
					goto IL_0100;
				case "22":
				case "23":
				case "24":
					{
						num = 0;
						goto IL_0100;
					}
					IL_0100:
					commerceError = ((num != 0) ? new CommerceError(300, (string)jsonData["Message"]) : new CommerceError(309, (string)jsonData["Message"]));
					break;
				}
			}
			else
			{
				commerceError = new CommerceError(300, error.PurchaseError.ToJson());
			}
			sendPurchaseResponse(commerceError);
			CommerceLog("purchaseFailedEvent: complete");
			return false;
		}

		private bool onPurchaseCancelledEvent(IAPServiceEvents.PCPurchaseCancelled error)
		{
			CommerceLog("purchaseCancelledEvent");
			isPurchaseInProgress = false;
			CommerceError cError = new CommerceError(304, "Purchase Canceled By User");
			sendPurchaseResponse(cError);
			CommerceLog("purchaseCancelledEvent: complete");
			return false;
		}

		private bool onPurchaseSuccessfulEvent(IAPServiceEvents.PCPurchaseSuccess transaction)
		{
			CommerceLog("purchaseSuccessfulEvent: " + transaction.PurchaseDetails.ToJson());
			currentPurchaseInfo = new PurchaseInfo(transaction.PurchaseDetails);
			currentPurchaseSkuInfo = SkuInfo.GetSkuFromList(retrievedSkuInfo, currentPurchaseInfo.sku);
			sendPurchaseResponse(currentPurchaseInfo, currentPurchaseSkuInfo);
			return false;
		}

		private bool onPurchaseVerifyReturned(IAPServiceEvents.PurchaseReturned result)
		{
			if (result.Purchase.success)
			{
				isPurchaseInProgress = false;
				return false;
			}
			CommerceLog("purchaseVerifyFailed: currentCheckWaitTime=" + currentCheckWaitTime + " seconds");
			CoroutineRunner.Start(startNextPurchaseVerify(), this, "startNextPurchaseVerify");
			if (currentCheckWaitTime > 30 && !hasShownError)
			{
				return false;
			}
			return true;
		}

		private IEnumerator startNextPurchaseVerify()
		{
			yield return new WaitForSeconds(currentCheckWaitTime);
			currentCheckWaitTime *= 2;
			sendPurchaseResponse(currentPurchaseInfo, currentPurchaseSkuInfo);
		}

		public override void GetSKUDetails(string[] sku_array = null)
		{
			if (string.IsNullOrEmpty(csgConfig.SessionID))
			{
				CommerceLog("getSKUDetails: StartSession before getting SKU Details");
				StartSession();
				skuLookupPending = true;
				return;
			}
			skuLookupPending = false;
			CommerceLog("getSKUDetails: Getting Product Details");
			if (!isSkuLookupInProgress)
			{
				PCGetProductDetailsRequest pcGetProductDetailsRequest = default(PCGetProductDetailsRequest);
				pcGetProductDetailsRequest.DistributionChannelId = csgConfig.DistributionChannelId;
				pcGetProductDetailsRequest.DeviceType = csgConfig.DeviceType;
				pcGetProductDetailsRequest.SessionId = csgConfig.SessionID;
				pcGetProductDetailsRequest.Language = csgConfig.Language;
				Service.Get<INetworkServicesManager>().IAPService.GetPCProductDetails(pcGetProductDetailsRequest);
				isSkuLookupInProgress = true;
			}
			else
			{
				CommerceLog("getSKUDetails: sku lookup already in progress, will not resubmit");
			}
			CommerceLog("getSKUDetails: Finishing triggerning get product information");
		}

		public override void ManageAccountInStore()
		{
			CommerceLog("ManageAccountInStore is called");
			initWebViewPurchaseController();
			webViewPurchaseController.ShowManageAccountFlow(csgConfig);
		}

		public override string GetStoreType()
		{
			return "CSG";
		}

		~CommerceProcessorCSG()
		{
			disableListners();
		}
	}
}
