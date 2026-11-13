using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ClubPenguin.Commerce
{
	public abstract class CommerceProcessor
	{
		public delegate void BillingEnabledResponseSend(bool success, CommerceError cError);

		public delegate void SkuInventoryResponseSend(List<SkuInfo> listSkus, CommerceError cError);

		public delegate void PlayerInventoryResponseSend(List<PurchaseInfo> playerInventoryList, CommerceError cError);

		public delegate void PurchaseResponseSend(PurchaseInfo Purchase, SkuInfo purSkuInfo, CommerceError cError);

		public delegate void UnexpectedPurchaseResponseSend(PurchaseInfo Purchase, SkuInfo purSkuInfo, CommerceError cError);

		public delegate void PurchaseRestoreResponseSend(List<PurchaseInfo> listPurchases, List<SkuInfo> listSkus, CommerceError cError);

		public delegate void PurchaseRestoreVerifiedResponseSend(bool success, CommerceError cError);

		public delegate void PurchaseVerifiedResponseSend(bool success, CommerceError cError);

		private bool billingSupported = false;

		private int billingUnsupportedReason = 0;

		protected bool isPurchaseInProgress = false;

		public ProductList productList = new ProductList();

		public BillingEnabledResponseSend BillingEnabledResponse;

		public SkuInventoryResponseSend SkuInventoryResponse;

		public PlayerInventoryResponseSend PlayerInventoryResponse;

		public PurchaseResponseSend PurchaseResponse;

		public UnexpectedPurchaseResponseSend UnexpectedPurchaseResponse;

		public PurchaseRestoreResponseSend PurchaseRestoreResponse;

		public PurchaseRestoreVerifiedResponseSend PurchaseRestoreVerifiedResponse;

		public PurchaseVerifiedResponseSend PurchaseVerifiedResponse;

		private Dictionary<string, CultureInfo> currencyCultures;

		public virtual void InitializeStore(string token = "")
		{
		}

		public bool IsPurchaseInProgress()
		{
			return isPurchaseInProgress;
		}

		public virtual void GetSKUDetails(string[] sku_array = null)
		{
		}

		public virtual void RestorePurchases()
		{
		}

		public virtual void ManageAccountInStore()
		{
		}

		public virtual void PurchaseProduct(string product)
		{
			PurchaseProduct(product, null);
		}

		public virtual void PurchaseProduct(string product, string payload)
		{
			CommerceError commerceError = null;
			CommerceLog("PurchaseProduct: Purchasing product " + product + ", with payload " + payload);
			if (!IsBillingSupported())
			{
				commerceError = new CommerceError(100, "Billing Not Supported");
				sendPurchaseResponse(commerceError);
				return;
			}
			Product productByKey = GetProductByKey(product);
			if (productByKey == null)
			{
				commerceError = new CommerceError(306, "Product key' " + product + "' Not Found");
				sendPurchaseResponse(commerceError);
				return;
			}
			if (!productByKey.IsPurchasable())
			{
				commerceError = new CommerceError(307, "Product key' " + product + "' Not Purchasable. No store information retrieved?");
				sendPurchaseResponse(commerceError);
				return;
			}
			if (payload != null && payload != "")
			{
				CommerceLog("PurchaseProduct: Triggering purchase product with payload");
				PurchaseProductFromStore(productByKey.GetStoreSKU(), payload);
			}
			else
			{
				CommerceLog("PurchaseProduct: Triggering purchase product without payload");
				PurchaseProductFromStore(productByKey.GetStoreSKU());
			}
			CommerceLog("PurchaseProduct: Finishing purchasing " + product);
		}

		public virtual void PurchaseProductFromStore(string product, string payload)
		{
		}

		public virtual void PurchaseProductFromStore(string product)
		{
			PurchaseProductFromStore(product, "");
		}

		public virtual void SetTestMode(int testMode)
		{
		}

		public virtual void DisableStore()
		{
		}

		public virtual void ChangeCommerceResourceURLs(string CommerceResourceURLsDefinitionName)
		{
		}

		public bool IsBillingSupported()
		{
			return billingSupported;
		}

		protected void setBillingSupported(bool supported)
		{
			billingSupported = supported;
		}

		protected int getBillingUnsupportedReason()
		{
			return billingUnsupportedReason;
		}

		protected void setBillingUnsupportedReason(int unsupported_reason)
		{
			billingUnsupportedReason = unsupported_reason;
		}

		protected void sendPurchaseResponse(CommerceError cError)
		{
			sendPurchaseResponse(null, null, cError);
		}

		protected void sendPurchaseResponse(PurchaseInfo pi, SkuInfo si, CommerceError cError = null)
		{
			if (PurchaseResponse != null)
			{
				CommerceLog("sendPurchaseResponse: not null");
				if (cError == null)
				{
					cError = new CommerceError();
				}
				PurchaseResponse(pi, si, cError);
			}
			else
			{
				CommerceLog("sendPurchaseResponse: listner is null and not pointing to anything");
			}
		}

		protected void sendUnexpctedPurchaseResponse(CommerceError cError)
		{
			sendUnexpectedPurchaseResponse(null, null, cError);
		}

		protected void sendUnexpectedPurchaseResponse(PurchaseInfo pi, SkuInfo si, CommerceError cError = null)
		{
			if (PurchaseResponse != null)
			{
				CommerceLog("sendUnexpectedPurchaseResponse: not null");
				if (cError == null)
				{
					cError = new CommerceError();
				}
				UnexpectedPurchaseResponse(pi, si, cError);
			}
			else
			{
				CommerceLog("sendUnexpectedPurchaseResponse: listner is null and not pointing to anything");
			}
		}

		protected void sendInventoryResponse(CommerceError cError)
		{
			sendInventoryResponse(null, cError);
		}

		protected void sendInventoryResponse(List<SkuInfo> sil, CommerceError cError = null)
		{
			if (SkuInventoryResponse != null)
			{
				CommerceLog("sendInventoryResponse: not null");
				if (cError == null)
				{
					cError = new CommerceError();
				}
				SkuInventoryResponse(sil, cError);
			}
			else
			{
				CommerceLog("sendInventoryResponse: is null");
			}
		}

		protected void sendPlayerInventoryResponse(CommerceError cError)
		{
			sendPlayerInventoryResponse(null, cError);
		}

		protected void sendPlayerInventoryResponse(List<PurchaseInfo> pil, CommerceError cError = null)
		{
			if (PlayerInventoryResponse != null)
			{
				CommerceLog("sendPlayerInventoryResponse: not null");
				if (cError == null)
				{
					cError = new CommerceError();
				}
				PlayerInventoryResponse(pil, cError);
			}
			else
			{
				CommerceLog("sendPlayerInventoryResponse: is null");
			}
		}

		protected void sendPurchaseRestoreResponse(CommerceError cError)
		{
			sendPurchaseRestoreResponse(null, null, cError);
		}

		protected void sendPurchaseRestoreResponse(List<PurchaseInfo> pil, List<SkuInfo> sil, CommerceError cError = null)
		{
			if (PurchaseRestoreResponse != null)
			{
				CommerceLog("sendPurchaseRestoreResponse: not null");
				if (cError == null)
				{
					cError = new CommerceError();
				}
				PurchaseRestoreResponse(pil, sil, cError);
			}
			else
			{
				CommerceLog("sendPurchaseRestoreResponse: is null");
			}
		}

		protected void sendPurchaseRestoreVerifiedResponse(CommerceError cError)
		{
			sendPurchaseRestoreVerifiedResponse(false, cError);
		}

		protected void sendPurchaseRestoreVerifiedResponse(bool success)
		{
			CommerceError cError = new CommerceError();
			sendPurchaseRestoreVerifiedResponse(success, cError);
		}

		protected void sendPurchaseRestoreVerifiedResponse(bool success, CommerceError cError)
		{
			if (cError == null)
			{
				CommerceLog("sendPurchaseRestoreVerfiedResponse: error is null");
				cError = new CommerceError();
			}
			PurchaseRestoreVerifiedResponse(success, cError);
		}

		protected void sendPurchaseVerifiedResponse(CommerceError cError)
		{
			sendPurchaseVerifiedResponse(false, cError);
		}

		protected void sendPurchaseVerifiedResponse(bool success)
		{
			CommerceError cError = new CommerceError();
			sendPurchaseVerifiedResponse(success, cError);
		}

		protected void sendPurchaseVerifiedResponse(bool success, CommerceError cError)
		{
			if (cError == null)
			{
				CommerceLog("sendPurchaseRestoreVerfiedResponse: error is null");
				cError = new CommerceError();
			}
			PurchaseRestoreVerifiedResponse(success, cError);
		}

		protected void sendBillingEnabledResponse(CommerceError cError)
		{
			sendBillingEnabledResponse(false, cError);
		}

		protected void sendBillingEnabledResponse(bool success)
		{
			CommerceError cError = new CommerceError();
			sendBillingEnabledResponse(success, cError);
		}

		protected void sendBillingEnabledResponse(bool success, CommerceError cError)
		{
			if (cError == null)
			{
				CommerceLog("sendBillingEnabledResponse: error is null");
				cError = new CommerceError();
			}
			BillingEnabledResponse(success, cError);
		}

		public virtual string GetStoreType()
		{
			return "Unknown";
		}

		public void AddProduct(string key = null, string gp_product = null, string apple_product = null, string csg_id = null, string duration = null, string trial_duration = null, bool is_recurring = true)
		{
			productList.AddProduct(key, gp_product, apple_product, csg_id, duration, trial_duration, is_recurring);
		}

		public void AddStoreInformationBySKU(List<SkuInfo> skuInfoList)
		{
			productList.AddStoreInformationBySKU(skuInfoList);
		}

		public Product GetProductByKey(string key)
		{
			return productList.GetProductByKey(key);
		}

		public void CommerceLog(string logInfo)
		{
		}

		public void CommerceErrorLog(string logInfo)
		{
			Log.LogError(this, logInfo);
		}

		public string GetCurrencyFormattedNumber(double amount, string currencyCode)
		{
			CultureInfo cultureInfoByCurrencySymbol = GetCultureInfoByCurrencySymbol(currencyCode);
			if (cultureInfoByCurrencySymbol != null)
			{
				return amount.ToString("C", cultureInfoByCurrencySymbol);
			}
			return amount.ToString("N2");
		}

		public string GetCurrencySymbol(string currencyCode)
		{
			CultureInfo cultureInfoByCurrencySymbol = GetCultureInfoByCurrencySymbol(currencyCode);
			if (cultureInfoByCurrencySymbol != null)
			{
				RegionInfo regionInfo = new RegionInfo(cultureInfoByCurrencySymbol.LCID);
				return regionInfo.CurrencySymbol;
			}
			return "";
		}

		public CultureInfo GetCultureInfoByCurrencySymbol(string currencyCode)
		{
			if (currencyCultures == null)
			{
				currencyCultures = new Dictionary<string, CultureInfo>();
			}
			if (currencyCode == null)
			{
				throw new ArgumentNullException("currencyCode");
			}
			if (currencyCultures.ContainsKey(currencyCode))
			{
				return currencyCultures[currencyCode];
			}
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
			CultureInfo cultureInfo = null;
			CultureInfo[] array = cultures;
			foreach (CultureInfo cultureInfo2 in array)
			{
				RegionInfo regionInfo = new RegionInfo(cultureInfo2.LCID);
				if (regionInfo.ISOCurrencySymbol.ToUpper() == currencyCode.ToUpper())
				{
					if (cultureInfo == null)
					{
						cultureInfo = cultureInfo2;
					}
					if (Service.Get<Localizer>().LanguageString.StartsWith(cultureInfo2.TwoLetterISOLanguageName))
					{
						cultureInfo = cultureInfo2;
					}
				}
			}
			if (cultureInfo != null)
			{
				currencyCultures.Add(currencyCode, cultureInfo);
				return cultureInfo;
			}
			return null;
		}
	}
}
