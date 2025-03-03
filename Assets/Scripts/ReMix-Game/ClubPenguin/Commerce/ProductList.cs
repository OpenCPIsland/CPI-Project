using System;
using System.Collections.Generic;

namespace ClubPenguin.Commerce
{
	public class ProductList
	{
		private List<Product> productInformation = new List<Product>();

		public void AddProduct(string key = null, string gp_product = null, string apple_product = null, string csg_id = null, string duration = null, string trial_duration = null, bool is_recurring = true)
		{
			Product product = productInformation.Find((Product x) => x.shared_key == key);
			if (product == null)
			{
				productInformation.Add(new Product(key, gp_product, apple_product, csg_id, duration, trial_duration, is_recurring));
			}
		}

		public void AddStoreInformationByKey(string KeyCode, SkuInfo skuInfo)
		{
			Product product = productInformation.Find((Product x) => x.shared_key == KeyCode);
			if (product != null)
			{
				product.AddStoreInformation(skuInfo);
			}
		}

		public void AddStoreInformationBySKU(List<SkuInfo> skuInfoList)
		{
			foreach (Product item in productInformation)
			{
				if (item.NeedsSkuInformation())
				{
					SkuInfo skuFromList = SkuInfo.GetSkuFromList(skuInfoList, item.GetStoreSKU());
					if (skuFromList != null)
					{
						item.AddStoreInformation(skuFromList);
					}
				}
			}
		}

		public string[] GetSkusForLookup(bool getall = false)
		{
			List<string> list = new List<string>();
			foreach (Product item in productInformation)
			{
				if (item.NeedsSkuInformation() || getall)
				{
					list.Add(item.GetStoreSKU());
				}
			}
			return list.ToArray();
		}

		public Product GetProductByKey(string key)
		{
			Product product = productInformation.Find((Product x) => x.shared_key.ToLower() == key.ToLower());
			if (product == null)
			{
				return null;
			}
			return product;
		}

		public Product GetProductByDuration(string sku_duration, string trial_duration)
		{
			Product product = null;
			product = ((!string.IsNullOrEmpty(trial_duration)) ? productInformation.Find((Product x) => x.sku_duration == sku_duration && x.sku_trial_duration == trial_duration) : productInformation.Find((Product x) => x.sku_duration == sku_duration && string.IsNullOrEmpty(x.sku_trial_duration)));
			if (product == null)
			{
				return null;
			}
			return product;
		}

		public Product GetProductBySku(string sku)
		{
			return GetProductByVendorSku("csg", sku);
		}

		public Product GetProductByVendorSku(string vendor, string sku)
		{
			if (string.IsNullOrEmpty(sku))
			{
				throw new ArgumentException("SKU cannot be null or empty");
			}
			Product product = null;
			if ("apple".Equals(vendor))
			{
				product = productInformation.Find((Product x) => x.apple_store_sku == sku);
			}
			else if ("google".Equals(vendor))
			{
				product = productInformation.Find((Product x) => x.gp_store_sku == sku);
			}
			else if ("csg".Equals(vendor))
			{
				product = productInformation.Find((Product x) => x.csg_id == sku);
				if (product == null)
				{
					product = productInformation.Find((Product x) => !string.IsNullOrEmpty(x.csg_external_reference) && x.csg_external_reference.ToLower() == sku.ToLower());
				}
			}
			return product;
		}

		public List<Product> GetAllProducts()
		{
			return productInformation;
		}

		public void ClearStoreDetailsAllProducts()
		{
			foreach (Product item in productInformation)
			{
				item.ClearStoreInformation();
			}
		}

		public override string ToString()
		{
			return string.Format("<ProductList> Number Of Products: {0}", productInformation.Count);
		}
	}
}
