using System.Collections.Generic;

namespace ClubPenguin.Commerce
{
	public class SkuInfo
	{
		public string title
		{
			get;
			private set;
		}

		public string price
		{
			get;
			private set;
		}

		public string type
		{
			get;
			private set;
		}

		public string description
		{
			get;
			private set;
		}

		public string sku
		{
			get;
			private set;
		}

		public string currencyCode
		{
			get;
			private set;
		}

		public string currencySymbol
		{
			get;
			private set;
		}

		public bool purchasable
		{
			get;
			private set;
		}

		public static SkuInfo GetSkuFromList(List<SkuInfo> items, string sku_name)
		{
			if (items != null && items.Count > 0)
			{
				foreach (SkuInfo item in items)
				{
					if (item.sku == sku_name)
					{
						return item;
					}
				}
			}
			return null;
		}

		public SkuInfo(string i_title, string i_price, string i_type, string i_description, string i_sku, string i_currencyCode, string i_currencySymbol, bool i_purchasable)
		{
			title = i_title;
			price = i_price;
			type = i_type;
			description = i_description;
			sku = i_sku;
			currencyCode = i_currencyCode;
			currencySymbol = i_currencySymbol;
			purchasable = i_purchasable;
		}

		public override string ToString()
		{
			return string.Format("<SkuInfo> title: {0}, price: {1}, type: {2}, description: {3}, productId: {4}", title, price, type, description, sku);
		}
	}
}
