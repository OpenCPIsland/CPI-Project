using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DiscountableItemPriceTag : MonoBehaviour
	{
		public const int PRICETAG_INDEX_DEFAULT = 0;

		public const int PRICETAG_INDEX_SALE = 1;

		public GameObjectSelector PriceTagSelector;

		public Text CostText;

		public Text CostTextSaleOld;

		public Text CostTExtSaleNew;

		public void setItem<T>(T definition, int itemDefaultCost, MarketPlaceUtils.IsItemInSaleDelegate<T> isItemInSale)
		{
			int itemCost = MarketPlaceUtils.GetItemCost(definition, itemDefaultCost, isItemInSale);
			if (itemCost != itemDefaultCost)
			{
				showItemOnSale(itemCost, itemDefaultCost);
			}
			else
			{
				showItemDefaultPrice(itemDefaultCost);
			}
		}

		public void Hide()
		{
			PriceTagSelector.SelectedObject.SetActive(false);
		}

		public void Show()
		{
			PriceTagSelector.SelectedObject.SetActive(true);
		}

		private void showItemOnSale(int saleCost, int defaultCost)
		{
			PriceTagSelector.SelectGameObject(1);
			CostTExtSaleNew.text = saleCost.ToString();
			CostTextSaleOld.text = defaultCost.ToString();
		}

		private void showItemDefaultPrice(int cost)
		{
			PriceTagSelector.SelectGameObject(0);
			CostText.text = cost.ToString();
		}
	}
}
