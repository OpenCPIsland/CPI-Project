using System.Collections.Generic;

namespace ClubPenguin.Commerce
{
	public class CommerceProcessorMock : CommerceProcessor
	{
		private int testMode = 1;

		private List<SkuInfo> _retrievedSkuInfo = new List<SkuInfo>();

		public bool PurchaseShouldBeSucessful
		{
			get
			{
				return testMode == 1;
			}
		}

		public override void InitializeStore(string token = "")
		{
			CommerceLog("initializeStore: mock store");
			if (testMode > 0)
			{
				setBillingSupported(true);
				sendBillingEnabledResponse(true);
			}
		}

		public override void GetSKUDetails(string[] sku_array = null)
		{
			CommerceLog("getSKUDetails: Mock Getting Product Details From Product List");
			CommerceError commerceError = null;
			if (sku_array == null)
			{
				sku_array = productList.GetSkusForLookup();
			}
			switch (testMode)
			{
			case 0:
				commerceError = new CommerceError(100, "Mock Object general failure");
				sendInventoryResponse(commerceError);
				break;
			case 1:
			case 3:
			case 4:
			{
				CommerceLog("Starting to generate fake sku list");
				string[] array = sku_array;
				foreach (string text in array)
				{
					SkuInfo item = new SkuInfo(text + " title", "$1.99", "inapp", "Product 1 Desc", text, "USD", "$", true);
					CommerceLog("Adding sku to list: " + text);
					_retrievedSkuInfo.Add(item);
				}
				CommerceLog("Calling Inventory Response");
				productList.AddStoreInformationBySKU(_retrievedSkuInfo);
				sendInventoryResponse(_retrievedSkuInfo);
				break;
			}
			case 2:
				commerceError = new CommerceError(200, "Mock Object general failure");
				sendInventoryResponse(commerceError);
				break;
			default:
				CommerceLog("Mock Object: getSkuDetails does not cover this case");
				break;
			}
		}

		public override void PurchaseProductFromStore(string product)
		{
			isPurchaseInProgress = true;
			CommerceLog("Mock PurchaseProduct: Purchasing sku " + product);
			CommerceError commerceError = null;
			PurchaseInfo purchaseInfo = null;
			SkuInfo skuInfo = null;
			switch (testMode)
			{
			case 0:
				commerceError = new CommerceError(100, "Mock Object general failure");
				sendPurchaseResponse(commerceError);
				break;
			case 1:
				purchaseInfo = new PurchaseInfo("1001", product, 1413836611000L, "product1_token", "Purchased");
				skuInfo = new SkuInfo("Product 1", "$1.99", "Non-Consumable", "Product 1 Desc", product, "USD", "$", true);
				sendPurchaseResponse(purchaseInfo, skuInfo);
				break;
			case 2:
				commerceError = new CommerceError(300, "Mock Object general failure");
				sendPurchaseResponse(commerceError);
				break;
			case 3:
				commerceError = new CommerceError(305, "Apple Ask To Buy");
				sendPurchaseResponse(commerceError);
				break;
			case 4:
				purchaseInfo = new PurchaseInfo("1001", product, 1413836611000L, "product1_token", "Purchased");
				skuInfo = new SkuInfo("Product 1", "$1.99", "Non-Consumable", "Product 1 Desc", product, "USD", "$", true);
				sendUnexpectedPurchaseResponse(purchaseInfo, skuInfo);
				break;
			default:
				CommerceLog("Mock Object: PurchaseProduct does not cover this case");
				break;
			}
			isPurchaseInProgress = false;
		}

		public override void RestorePurchases()
		{
			CommerceLog("Mock RestorePurchases: Started");
			CommerceError commerceError = null;
			SkuInfo item = new SkuInfo("Product 1", "$1.99", "Non-Consumable", "Product 1 Desc", "com.cp.product1", "USD", "$", true);
			List<SkuInfo> list = new List<SkuInfo>();
			list.Add(item);
			PurchaseInfo item2 = new PurchaseInfo("1001", "com.cp.product1", 1413836611000L, "product1_token", "Purchased");
			List<PurchaseInfo> list2 = new List<PurchaseInfo>();
			list2.Add(item2);
			switch (testMode)
			{
			case 0:
				commerceError = new CommerceError(100, "Mock Object general failure");
				sendPurchaseRestoreVerifiedResponse(commerceError);
				break;
			case 1:
			case 3:
			case 4:
				sendPurchaseRestoreResponse(list2, list);
				break;
			case 2:
				commerceError = new CommerceError(400, "Mock Object general failure");
				sendPurchaseRestoreResponse(commerceError);
				break;
			default:
				CommerceLog("Mock Object: getSkuDetails does not cover this case");
				break;
			}
		}

		public override void SetTestMode(int testMode)
		{
			this.testMode = testMode;
		}

		public override string GetStoreType()
		{
			return "Mock";
		}
	}
}
