using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Commerce
{
	[RequireComponent(typeof(CommerceProcessorInit))]
	public class CommerceService
	{
		private CommerceProcessor commerceProcessor;

		public bool look_up_skus = true;

		public bool trigger_purchase = false;

		public bool trigger_restore = true;

		public bool trigger_purchase_once = false;

		private bool listnersSetUp = false;

		private float productsReloadTime = 0f;

		private float productsReloadAfterSecs = 86400f;

		public BiUserId biUserId;

		private string gpkey = "SMqM6mnPDGbVNDdphwgJ";

		private string gpToken = "BxgdB2cGAiEVElcVFHYQCAkwEzoBNx1+eygkEhESNBEfESIgOUchGBwINwNlOAITFXUGGh90IiI6IiElBh5IBVIAKAIRKzgbAygAOTscE34eJyshUjo4AhATMA4AdypCJjEAfgkgNzhVKzQgJnUOHC8RXUE+MQAzNwsVOVc6ODsgFFsgLRAIGCcxNXkAfjsVYygaGCYTLC4UPiJAOxwtHBwVIwV7OCAiCQMkYCoCIiYKIhQ8Bg4FP1UlDSoQPQYvHy82MzEcEwIJGjcGZTsCAiYDIyEZEw9DPhwiPAYnGTVnXSgAHSooECp0EBENGQMjARo7C3glAgQdPls5GHcyAwoiDwIEGxk+eBcsJhItOD0cAj1DCzI1PB83Jwt6XTgmERIwZQV1IjUyIy05BCcdP2NcJAAhKigbGhMLAg0bCyExBQEDYCo4BRMsMDkqdy0IPRo9GDAjGRxnOTQ9EgI0DBgDFDQKRV4wMBkrI3g6Gj0WKTQfHxwyGzEiEx4CNyMZeCoeHyd0DmcDEi0CPSM1AzELJ395KhYBESkvLCsQNgI+Py0dABszBmw1JB0hAiQjFy9cRCYwMTwBITsGZVwgChYPEhoBAAAGJRoPHwoZOyNTOF8lFxAsOi8vEDkKIjUECSMWeWMHOycIdChmH3YqQSNGCzkCGyg1VSUoPicsCSAcEiI6OjIhGAIYOHA=";

		public CommerceProcessor CommerceProcessor
		{
			get
			{
				return commerceProcessor;
			}
		}

		public void Setup()
		{
			biUserId = new BiUserId();
			if (commerceProcessor == null)
			{
				CommerceProcessorInit commerceProcessorInit = new CommerceProcessorInit();
				commerceProcessor = commerceProcessorInit.GetCommerceProcessor(1);
			}
			if (commerceProcessor != null)
			{
				setUpListners();
				commerceProcessor.InitializeStore(ObsString(gpToken, gpkey, false));
			}
			else
			{
				Log.LogError(this, "start: commerceProcessor is NULL - Purchasing will not work");
			}
		}

		private void handleBillingEnabled(bool success, CommerceError cError)
		{
			if (cError.HasError())
			{
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.BillingEnabledError(cError));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceEvents.BillingEnabled(success));
			}
		}

		[Invokable("Commerce.LoadProductInformation", Description = "Get the IAP products from static game data")]
		public void LoadProductInformation(bool forceLookup = false)
		{
			if (lookUpProducts() || forceLookup)
			{
				try
				{
					Dictionary<string, ProductDefinition> products = Service.Get<GameData>().Get<Dictionary<string, ProductDefinition>>();
					addProductsToProcessor(products);
				}
				catch (Exception ex)
				{
					Log.LogErrorFormatted(this, "Unable to load Products Game Data: {0} ", ex.ToString());
					getProductsFromServiceError(default(IAPServiceErrors.ProductsLoadedError));
				}
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceEvents.ProductsLoaded(true));
			}
		}

		private bool getProductsFromServiceError(IAPServiceErrors.ProductsLoadedError serviceError)
		{
			CommerceError error = new CommerceError(202, "Could not retrieve products list");
			Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.ProductsLoadedError(error));
			return false;
		}

		private void addProductsToProcessor(Dictionary<string, ProductDefinition> products)
		{
			foreach (KeyValuePair<string, ProductDefinition> product in products)
			{
				commerceProcessor.AddProduct(product.Value.key, product.Value.gp_sku, product.Value.apple_sku, product.Value.csg_id, product.Value.duration, product.Value.trial, product.Value.is_recurring);
			}
			commerceProcessor.GetSKUDetails();
		}

		private void handleProductsLoaded(List<SkuInfo> listSkus, CommerceError cError)
		{
			if (cError.HasError())
			{
				string text = "";
				text = "handleProductsLoaded: Error Found: Number " + cError.GetErrorNo() + ", Desc " + cError.GetErrorDesc();
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.ProductsLoadedError(cError));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceEvents.ProductsLoaded(true));
				productsReloadTime = Time.time + productsReloadAfterSecs;
			}
		}

		public Product GetProductByKey(string key)
		{
			return commerceProcessor.productList.GetProductByKey(key);
		}

		public Product GetProductBySku(string sku)
		{
			return commerceProcessor.productList.GetProductBySku(sku);
		}

		public Product GetProductByVendorSku(string vendor, string sku)
		{
			return commerceProcessor.productList.GetProductByVendorSku(vendor, sku);
		}

		public List<Product> GetAllProducts()
		{
			return commerceProcessor.productList.GetAllProducts();
		}

		private bool lookUpProducts()
		{
			return productsReloadTime <= Time.time;
		}

		public void TriggerManageAccount(string vendorString)
		{
			if (!string.IsNullOrEmpty(vendorString))
			{
				switch (vendorString.ToLower())
				{
				case "apple":
					Application.OpenURL("https://appleid.apple.com/#!&page=signin");
					break;
				case "google":
					Application.OpenURL("https://play.google.com/store/account");
					break;
				default:
					commerceProcessor.ManageAccountInStore();
					break;
				}
			}
		}

		public bool IsPurchaseInProgress()
		{
			return commerceProcessor.IsPurchaseInProgress();
		}

		[Invokable("Commerce.TriggerPurchase", Description = "Trigger a purchase for a product")]
		public void TriggerPurchase(string product_name)
		{
			commerceProcessor.PurchaseProduct(product_name);
		}

		private void handlePurchaseRetrieval(PurchaseInfo PI, SkuInfo purSkuInfo, CommerceError cError)
		{
			string text = "";
			if (cError.HasError())
			{
				text = "handlePurchaseRetrieval: Error Found: Number " + cError.GetErrorNo() + ", Desc " + cError.GetErrorDesc() + ", skuToLookup " + cError.GetSkuToLookup();
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.PurchaseVerifiedError(cError));
			}
			else
			{
				text = "handlePurchaseRetrieval: Purchase Success so far purchaseinfo object\n" + PI.ToString();
				verifyPurchaseWithService(PI);
			}
		}

		private void handleUnexpectedPurchaseRetrieval(PurchaseInfo PI, SkuInfo purSkuInfo, CommerceError cError)
		{
			string text = "";
			if (!cError.HasError())
			{
				text = "handleUnexpectedPurchaseRetrieval: Purchase Success so far purchaseinfo object\n" + PI.ToString();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceEvents.UnexpectedPurchase(PI, purSkuInfo));
		}

		private void verifyPurchaseWithService(PurchaseInfo purchase)
		{
			PurchaseRequest purchaseRequest = new PurchaseRequest(purchase.token, purchase.sku, purchase.signature, purchase.originalJson, commerceProcessor.GetStoreType(), biUserId);
			Service.Get<INetworkServicesManager>().IAPService.Purchase(purchaseRequest);
		}

		public void VerifyUnexpectedPurchase(PurchaseInfo purchase)
		{
			verifyPurchaseWithService(purchase);
		}

		private bool verifyPurchaseWithServiceError(IAPServiceErrors.PurchaseError serviceError)
		{
			CommerceError error = new CommerceError(308, "Could not verify purchase");
			Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.PurchaseVerifiedError(error));
			return false;
		}

		private bool verifyPurchaseWithServiceSuccess(IAPServiceEvents.PurchaseReturned evt)
		{
			MembershipRightsRefresh data = (evt.Purchase.rights != null) ? evt.Purchase.rights.Data : default(MembershipRightsRefresh);
			Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceEvents.PurchaseVerified(evt.Purchase.success, data));
			return false;
		}

		[Invokable("Commerce.TriggerRestorePlayerPurchases", Description = "Trigger A restore for a user")]
		public void TriggerRestorePlayerPurchases()
		{
			commerceProcessor.RestorePurchases();
		}

		private void handleRestorePlayerPurchases(List<PurchaseInfo> playerInventory, List<SkuInfo> skuInfos, CommerceError cError)
		{
			string text = "";
			if (cError.HasError())
			{
				text = "RestorePlayerPurchases: Error Found: Number " + cError.GetErrorNo() + ", Desc " + cError.GetErrorDesc();
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.RestoreVerifiedError(cError));
				return;
			}
			object obj = text;
			text = string.Concat(obj, "Total Purchases to Restore: ", playerInventory.Count, "\n");
			foreach (PurchaseInfo item in playerInventory)
			{
			}
			verifyPurchaseRestoreWithService(playerInventory, skuInfos);
		}

		private void verifyPurchaseRestoreWithService(List<PurchaseInfo> purchases, List<SkuInfo> skus)
		{
			if (purchases.Count == 0)
			{
				CommerceError error = new CommerceError(401, "No Purchases to Restore");
				Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.RestoreVerifiedError(error));
			}
			else
			{
				List<PurchaseRequest> list = new List<PurchaseRequest>();
				foreach (PurchaseInfo purchase in purchases)
				{
					list.Add(new PurchaseRequest(purchase.token, purchase.sku, purchase.signature, purchase.originalJson, commerceProcessor.GetStoreType(), biUserId));
				}
				Service.Get<INetworkServicesManager>().IAPService.CheckRestore(list);
			}
		}

		private bool verifyPurchaseRestoreWithServiceError(IAPServiceErrors.CheckRestoreError serviceError)
		{
			CommerceError error = new CommerceError(403, "Could not verify restored purchase");
			Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceErrors.RestoreVerifiedError(error));
			return false;
		}

		private bool verifyPurchaseRestoreWithServiceSuccess(IAPServiceEvents.CheckRestoreReturned evt)
		{
			MembershipRightsRefresh data = (evt.Purchase.rights != null) ? evt.Purchase.rights.Data : default(MembershipRightsRefresh);
			Service.Get<EventDispatcher>().DispatchEvent(new CommerceServiceEvents.RestoreVerified(evt.Purchase.success, data));
			return false;
		}

		private static string ObsString(string str, string key, bool encode = true)
		{
			str = ((!encode) ? Base64Decode(str) : Base64Encode(str));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				stringBuilder.Append((char)(str[i] ^ key[i % key.Length]));
			}
			if (!encode)
			{
				return Base64Decode(stringBuilder.ToString());
			}
			return Base64Encode(stringBuilder.ToString());
		}

		public void setBiUserId(string biUserId)
		{
			this.biUserId.biUserId = biUserId;
		}

		public bool MatchCurrentVendor(string vendorString)
		{
			return commerceProcessor.GetStoreType().ToUpper().Contains(vendorString.ToUpper());
		}

		public string getCurrentVendor()
		{
			return commerceProcessor.GetStoreType();
		}

		public CultureInfo GetCultureInfoByCurrencySymbol(string currencyCode)
		{
			return commerceProcessor.GetCultureInfoByCurrencySymbol(currencyCode);
		}

		private static string Base64Encode(string plainText)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(bytes);
		}

		private static string Base64Decode(string base64EncodedData)
		{
			byte[] bytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(bytes);
		}

		private void setUpListners()
		{
			if (!listnersSetUp)
			{
				CommerceProcessor obj = commerceProcessor;
				obj.BillingEnabledResponse = (CommerceProcessor.BillingEnabledResponseSend)Delegate.Combine(obj.BillingEnabledResponse, new CommerceProcessor.BillingEnabledResponseSend(handleBillingEnabled));
				CommerceProcessor obj2 = commerceProcessor;
				obj2.PurchaseResponse = (CommerceProcessor.PurchaseResponseSend)Delegate.Combine(obj2.PurchaseResponse, new CommerceProcessor.PurchaseResponseSend(handlePurchaseRetrieval));
				CommerceProcessor obj3 = commerceProcessor;
				obj3.UnexpectedPurchaseResponse = (CommerceProcessor.UnexpectedPurchaseResponseSend)Delegate.Combine(obj3.UnexpectedPurchaseResponse, new CommerceProcessor.UnexpectedPurchaseResponseSend(handleUnexpectedPurchaseRetrieval));
				CommerceProcessor obj4 = commerceProcessor;
				obj4.SkuInventoryResponse = (CommerceProcessor.SkuInventoryResponseSend)Delegate.Combine(obj4.SkuInventoryResponse, new CommerceProcessor.SkuInventoryResponseSend(handleProductsLoaded));
				CommerceProcessor obj5 = commerceProcessor;
				obj5.PurchaseRestoreResponse = (CommerceProcessor.PurchaseRestoreResponseSend)Delegate.Combine(obj5.PurchaseRestoreResponse, new CommerceProcessor.PurchaseRestoreResponseSend(handleRestorePlayerPurchases));
				Service.Get<EventDispatcher>().AddListener<IAPServiceEvents.CheckRestoreReturned>(verifyPurchaseRestoreWithServiceSuccess);
				Service.Get<EventDispatcher>().AddListener<IAPServiceErrors.CheckRestoreError>(verifyPurchaseRestoreWithServiceError);
				Service.Get<EventDispatcher>().AddListener<IAPServiceEvents.PurchaseReturned>(verifyPurchaseWithServiceSuccess, EventDispatcher.Priority.LAST);
				Service.Get<EventDispatcher>().AddListener<IAPServiceErrors.PurchaseError>(verifyPurchaseWithServiceError);
				listnersSetUp = true;
			}
		}
	}
}
