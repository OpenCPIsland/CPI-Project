using ClubPenguin.Analytics;
using ClubPenguin.Commerce;
using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class MembershipService : IBaseNetworkErrorHandler
	{
		public const string ERROR_PURCHASE_CANCELLED_BY_USER = "Membership.Purchase.Error.ERROR_PURCHASE_CANCELLED_BY_USER";

		private CommerceService cs;

		private bool productsLoaded = false;

		private bool productsLoading = false;

		private bool csSetupInprogress = false;

		private CommerceError productsLoadedError;

		private MembershipPlans membershipPlans;

		private Product productPurchaseInProcess;

		private bool membershipGrantInProcess;

		public bool LoginViaMembership;

		public bool LoginViaRestore;

		public string AccountFunnelName;

		public string MembershipFunnelName;

		private ScheduledEventDateDefinition supported;

		private static DevCacheableType<string> overrideSimCountryCode;

		private static DevCacheableType<string> overrideSimCarrierName;

		public bool IsPurchaseInProgress
		{
			get
			{
				return productPurchaseInProcess != null;
			}
		}

		[Tweakable("Session.SimCountryCode", Description = "Set a SIM country code for testing carrier billing")]
		public static string OverrideSimCountryCode
		{
			get
			{
				return overrideSimCountryCode.Value;
			}
			set
			{
				overrideSimCountryCode.SetValue(value);
			}
		}

		[Tweakable("Session.SimCarrierName", Description = "Set a SIM carrier name for testing carrier billing")]
		public static string OverrideSimCarrierName
		{
			get
			{
				return overrideSimCarrierName.Value;
			}
			set
			{
				overrideSimCarrierName.Value = value;
			}
		}

		public event System.Action OnPurchaseSuccess;

		public event Action<ApplicationService.Error, bool> OnPurchaseFailed;

		public event System.Action OnRestoreSuccess;

		public event Action<ApplicationService.Error> OnRestoreFailed;

		public MembershipService(ScheduledEventDateDefinition supported)
		{
			this.supported = supported;
			cs = Service.Get<CommerceService>();
			if (IsPurchaseFunnelAvailable() && cs != null)
			{
				Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.PurchaseVerified>(PurchaseWasVerified);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.PurchaseVerifiedError>(PurchaseWasVerifiedError);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.UnexpectedPurchase>(UnexpectedPurchaseEvent);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.ProductsLoaded>(ProducstWereLoadedVerified);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.ProductsLoadedError>(ProducstWereLoadedError);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.BillingEnabled>(BillingEnabled);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.BillingEnabledError>(BillingEnabledError);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.RestoreVerified>(RestoreWasVerified);
				Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.RestoreVerifiedError>(RestoreWasVerifiedError);
				Service.Get<EventDispatcher>().AddListener<NetworkErrors.InvalidSubscriptionError>(onInvalidSubscriptionError);
				Service.Get<EventDispatcher>().AddListener<IAPServiceErrors.MembershipGrantError>(onMembershipGrantError);
				if (cs.CommerceProcessor.IsBillingSupported())
				{
					LoadProducts();
				}
			}
			overrideSimCountryCode = new DevCacheableType<string>("cp.OverrideSimCountryCode", "");
			overrideSimCarrierName = new DevCacheableType<string>("cp.OverrideSimCarrierName", "");
			MembershipFunnelName = "membership_offer";
		}

		public bool IsPurchaseFunnelAvailable()
		{
			return Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(supported) || supported == null;
		}

		public AccountFlowData GetAccountFlowData()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			AccountFlowData component;
			if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component = cPDataEntityCollection.AddComponent<AccountFlowData>(cPDataEntityCollection.LocalPlayerHandle);
				component.Initialize();
			}
			return component;
		}

		public void RemoveAccountFlowData()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			cPDataEntityCollection.RemoveComponent<AccountFlowData>(cPDataEntityCollection.LocalPlayerHandle);
		}

		public void TriggerPurchase(string SKU)
		{
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionPausedEvent>(onSessionPause);
			productPurchaseInProcess = GetProduct(SKU);
			cs.TriggerPurchase(SKU);
		}

		private bool onSessionPause(SessionEvents.SessionPausedEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionPausedEvent>(onSessionPause);
			Service.Get<ICPSwrveService>().Action("membership_session", "paused");
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
			Service.Get<EventDispatcher>().AddListener<SessionErrorEvents.NoSessionOnResumeError>(onSessionResumeFailed);
			return false;
		}

		private bool onSessionResumed(SessionEvents.SessionResumedEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
			Service.Get<ICPSwrveService>().Action("membership_session", "resumed");
			return false;
		}

		private bool onSessionResumeFailed(SessionErrorEvents.NoSessionOnResumeError evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionErrorEvents.NoSessionOnResumeError>(onSessionResumeFailed);
			Service.Get<ICPSwrveService>().Action("membership_session", "resume_failed");
			return false;
		}

		public void LoadProducts()
		{
			if (cs != null && !productsLoaded && !productsLoading)
			{
				productsLoading = true;
				cs.LoadProductInformation();
			}
		}

		public bool ProducstWereLoadedVerified(CommerceServiceEvents.ProductsLoaded pl)
		{
			if (pl.Success)
			{
				if (Service.Get<MixLoginCreateService>().RegistrationAgeBand != null)
				{
					membershipPlans = new MembershipPlans(Service.Get<MixLoginCreateService>().RegistrationAgeBand.CountryCode);
				}
				Service.Get<MixLoginCreateService>().OnRegistrationConfigUpdated += onRegistrationConfigUpdated;
			}
			productsLoaded = pl.Success;
			productsLoading = false;
			return false;
		}

		private void onRegistrationConfigUpdated(IRegistrationConfiguration registrationConfig)
		{
			membershipPlans = new MembershipPlans(Service.Get<MixLoginCreateService>().RegistrationAgeBand.CountryCode);
		}

		public bool ProducstWereLoadedError(CommerceServiceErrors.ProductsLoadedError ple)
		{
			productsLoaded = false;
			productsLoading = false;
			productsLoadedError = ple.Error;
			return false;
		}

		public bool BillingEnabledError(CommerceServiceErrors.BillingEnabledError ple)
		{
			productsLoaded = false;
			productsLoading = false;
			csSetupInprogress = false;
			productsLoadedError = ple.Error;
			return false;
		}

		public bool BillingEnabled(CommerceServiceEvents.BillingEnabled be)
		{
			csSetupInprogress = false;
			LoadProducts();
			return false;
		}

		public bool HasErrors()
		{
			return productsLoadedError != null;
		}

		public bool ProductsReady()
		{
			if (cs != null && !productsLoaded && !productsLoading && !csSetupInprogress && !HasErrors())
			{
				if (cs.CommerceProcessor.IsBillingSupported())
				{
					LoadProducts();
				}
				else
				{
					csSetupInprogress = true;
					cs.Setup();
				}
			}
			return productsLoaded;
		}

		public void SetMembershipGrantInProcess(bool inProcess)
		{
			membershipGrantInProcess = inProcess;
		}

		public string GetProductsLoadedErrorMsg()
		{
			return getCommerceErrorMsg(productsLoadedError.GetErrorNo());
		}

		public Product GetProduct(string productSKU)
		{
			Product productByKey = cs.GetProductByKey(productSKU);
			if (productByKey != null && productByKey.IsPurchasable())
			{
				return productByKey;
			}
			return null;
		}

		public Product GetFirstTimeProduct()
		{
			return GetProduct(membershipPlans.DefaultFirstTimeSKU);
		}

		public Product GetResubscribingProduct()
		{
			Product product = GetProduct(membershipPlans.DefaultResubSKU);
			if (product == null && cs.MatchCurrentVendor("CSG"))
			{
				product = GetFirstTimeProduct();
			}
			return product;
		}

		public List<Product> GetFirstTimeProductsToOffer()
		{
			List<Product> list = new List<Product>();
			foreach (string toOfferFirstTimeSKU in membershipPlans.ToOfferFirstTimeSKUs)
			{
				Product product = GetProduct(toOfferFirstTimeSKU);
				if (product != null)
				{
					list.Add(product);
				}
			}
			return list;
		}

		public List<Product> GetResubscribingProductsToOffer()
		{
			List<Product> list = new List<Product>();
			foreach (string toOfferResubSKU in membershipPlans.ToOfferResubSKUs)
			{
				Product product = GetProduct(toOfferResubSKU);
				if (product != null)
				{
					list.Add(product);
				}
			}
			if (list.Count != membershipPlans.ToOfferResubSKUs.Count && cs.MatchCurrentVendor("CSG"))
			{
				list = GetFirstTimeProductsToOffer();
			}
			return list;
		}

		public List<Product> GetFirstTimeProductsAll()
		{
			List<Product> list = new List<Product>();
			foreach (string allFirstTimeSKU in membershipPlans.AllFirstTimeSKUs)
			{
				list.Add(GetProduct(allFirstTimeSKU));
			}
			return list;
		}

		public List<Product> GetResubscribingProductsAll()
		{
			List<Product> list = new List<Product>();
			foreach (string allResubSKU in membershipPlans.AllResubSKUs)
			{
				list.Add(GetProduct(allResubSKU));
			}
			return list;
		}

		public void TriggerRestorePlayerPurchases()
		{
			cs.TriggerRestorePlayerPurchases();
		}

		public bool PurchaseWasVerified(CommerceServiceEvents.PurchaseVerified pv)
		{
			if (pv.Success || (cs.CommerceProcessor is CommerceProcessorMock && (cs.CommerceProcessor as CommerceProcessorMock).PurchaseShouldBeSucessful))
			{
				if (productPurchaseInProcess != null)
				{
					applyMembership(pv.Data);
					productPurchaseInProcess = null;
				}
				else if (membershipGrantInProcess)
				{
					applyMembership(pv.Data);
					membershipGrantInProcess = false;
				}
				if (this.OnPurchaseSuccess != null)
				{
					this.OnPurchaseSuccess();
				}
			}
			else
			{
				ApplicationService.Error arg = (cs.getCurrentVendor() == "Mock") ? new ApplicationService.Error("Membership.Purchase.Error", "Membership.Purchase.Error.InEditor") : (cs.IsPurchaseInProgress() ? new ApplicationService.Error("Account.MembershipRightsTimeout.Title", "Account.MembershipRightsTimeout.Body") : new ApplicationService.Error("Membership.Purchase.Error", "Membership.Purchase.Error.UnknownInPurchaseSuccess"));
				if (this.OnPurchaseFailed != null)
				{
					this.OnPurchaseFailed(arg, true);
				}
			}
			return false;
		}

		public bool PurchaseWasVerifiedError(CommerceServiceErrors.PurchaseVerifiedError pve)
		{
			string commerceErrorMsg = getCommerceErrorMsg(pve.Error.GetErrorNo());
			if (this.OnPurchaseFailed != null)
			{
				ApplicationService.Error arg = new ApplicationService.Error("Membership.Purchase.Error", commerceErrorMsg);
				this.OnPurchaseFailed(arg, false);
			}
			Service.Get<ICPSwrveService>().Error("purchase_error", commerceErrorMsg, pve.Error.GetSkuToLookup(), SceneManager.GetActiveScene().name);
			return false;
		}

		public bool UnexpectedPurchaseEvent(CommerceServiceEvents.UnexpectedPurchase up)
		{
			if (Service.Get<SessionManager>().HasSession)
			{
				cs.VerifyUnexpectedPurchase(up.PI);
			}
			return false;
		}

		public bool RestoreWasVerified(CommerceServiceEvents.RestoreVerified rv)
		{
			if (rv.Success)
			{
				applyMembership(rv.Data);
				if (this.OnRestoreSuccess != null)
				{
					this.OnRestoreSuccess();
				}
				Service.Get<ICPSwrveService>().Action("restore_purchase", "success");
			}
			else
			{
				string format = "Membership.Purchase.Restore.NothingToRestore";
				ApplicationService.Error obj = new ApplicationService.Error("Membership.Purchase.RestoreMessageTitle", format);
				this.OnRestoreFailed(obj);
				Service.Get<ICPSwrveService>().Action("restore_purchase", "nothing_to_restore");
			}
			return false;
		}

		public bool RestoreWasVerifiedError(CommerceServiceErrors.RestoreVerifiedError rve)
		{
			string commerceErrorMsg = getCommerceErrorMsg(rve.Error.GetErrorNo());
			if (this.OnRestoreFailed != null)
			{
				ApplicationService.Error obj = new ApplicationService.Error("Membership.Purchase.RestoreMessageTitle", commerceErrorMsg);
				this.OnRestoreFailed(obj);
			}
			Service.Get<ICPSwrveService>().Action("restore_purchase", "error");
			Service.Get<ICPSwrveService>().Error("restore_error", commerceErrorMsg, rve.Error.GetSkuToLookup(), SceneManager.GetActiveScene().name);
			return false;
		}

		private void applyMembership(MembershipRightsRefresh data)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection == null || cPDataEntityCollection.LocalPlayerHandle.IsNull)
			{
				throw new Exception("Unable to resolve data entity collection");
			}
			MembershipData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component.MembershipExpireDate = data.expireDate;
				component.IsMember = true;
				component.MembershipType = MembershipType.Member;
				SubscriptionData component2;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component2))
				{
					component2.SubscriptionVendor = data.vendor;
					component2.SubscriptionProductId = data.productId;
					Service.Get<INetworkServicesManager>().PlayerStateService.GetLocalPlayerData(this);
					Service.Get<ICPSwrveService>().Action("game.new_member");
					return;
				}
				throw new MissingReferenceException("No subscription data found for local player");
			}
			throw new MissingReferenceException("No membership data found for local player");
		}

		private bool onInvalidSubscriptionError(NetworkErrors.InvalidSubscriptionError evt)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			MembershipData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component.IsMember = false;
				component.MembershipType = MembershipType.None;
				return false;
			}
			throw new MissingReferenceException("No membership data found for local player");
		}

		public string GetCurrentMembershipStatus()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection == null || cPDataEntityCollection.LocalPlayerHandle.IsNull)
			{
				Log.LogError(this, "Unable to resolve data entity collection");
				return "unknown";
			}
			MembershipData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				if (component.IsMember && component.MembershipType == MembershipType.Member)
				{
					return "member";
				}
				if (component.MembershipExpireDate > 0)
				{
					return "lapsed";
				}
				return "new";
			}
			return "unknown";
		}

		private string getCommerceErrorMsg(int errorNumber)
		{
			string text = "";
			switch (errorNumber)
			{
			case 100:
				return "Membership.Purchase.Error.ERROR_BILLING_NOT_SUPPORTED";
			case 300:
				return "Membership.Purchase.Error.UnknownInPurchaseSuccess";
			case 301:
				return "Membership.Purchase.Error.ERROR_PURCHASE_FAILURE_ALREADY_PURCHASED";
			case 303:
				return "Membership.Purchase.Error.ERROR_PURCHASE_FAILURE_NO_SKU_INFO_DO_NOT_LOOKUP";
			case 304:
				return "Membership.Purchase.Error.ERROR_PURCHASE_CANCELLED_BY_USER";
			case 305:
				return "Membership.Purchase.Error.ERROR_PURCHASE_DEFERRED";
			case 306:
				return "Membership.Purchase.Error.ERROR_PURCHASE_FAILURE_PRODUCT_NOT_FOUND";
			case 307:
				return "Membership.Purchase.Error.ERROR_PURCHASE_FAILURE_PRODUCT_NOT_PURCHASABLE";
			case 308:
				return "Membership.Purchase.Error.ERROR_PURCHASE_FAILURE_COULD_NOT_VERIFY";
			case 309:
				return "Membership.Purchase.Error.ERROR_PURCHASE_MAINTENANCE_MODE";
			case 400:
				return "Membership.Purchase.Error.ERROR_PURCHASE_RESTORE_FAILURE_GENERAL";
			case 401:
				return "Membership.Purchase.Error.ERROR_PURCHASE_RESTORE_NO_PURCHASES";
			case 402:
				return "Membership.Purchase.Error.ERROR_PURCHASE_FAILURE_COULD_NOT_VERIFY";
			case 200:
				return "Membership.ProductsLoaded.Error.ERROR_INVENTORY_FAILURE_GENERAL";
			case 201:
				return "Membership.ProductsLoaded.Error.ERROR_INVENTORY_FAILURE_NO_ITEMS_RETURNED";
			case 202:
				return "Membership.ProductsLoaded.Error.ERROR_INVENTORY_FAILURE_PRODUCTS_NOT_RETRIEVED";
			default:
				return string.Format(Service.Get<Localizer>().GetTokenTranslation("Membership.Purchase.Error.Unknown"), errorNumber);
			}
		}

		private bool onMembershipGrantError(IAPServiceErrors.MembershipGrantError evt)
		{
			SetMembershipGrantInProcess(false);
			return false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.PurchaseVerified>(PurchaseWasVerified);
			Service.Get<EventDispatcher>().RemoveListener<CommerceServiceErrors.PurchaseVerifiedError>(PurchaseWasVerifiedError);
			Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.UnexpectedPurchase>(UnexpectedPurchaseEvent);
			Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.ProductsLoaded>(ProducstWereLoadedVerified);
			Service.Get<EventDispatcher>().RemoveListener<CommerceServiceErrors.ProductsLoadedError>(ProducstWereLoadedError);
			Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.RestoreVerified>(RestoreWasVerified);
			Service.Get<EventDispatcher>().RemoveListener<CommerceServiceErrors.RestoreVerifiedError>(RestoreWasVerifiedError);
			Service.Get<EventDispatcher>().RemoveListener<IAPServiceErrors.MembershipGrantError>(onMembershipGrantError);
			Service.Get<MixLoginCreateService>().OnRegistrationConfigUpdated -= onRegistrationConfigUpdated;
		}

		public void onRequestTimeOut()
		{
		}

		public void onGeneralNetworkError()
		{
		}
	}
}
