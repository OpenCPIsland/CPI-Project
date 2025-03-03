using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogBuyPanel : MonoBehaviour
	{
		public CatalogShopBuyPanelState State;

		public Text ItemNameText;

		public Text CreatorNameText;

		public Text PurchaseCountText;

		public void Show()
		{
			base.gameObject.SetActive(true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		public void SetText(string itemName, string creatorName, string purchasedCount)
		{
			if (ItemNameText != null)
			{
				ItemNameText.text = itemName;
			}
			if (CreatorNameText != null)
			{
				CreatorNameText.text = creatorName;
			}
			if (PurchaseCountText != null)
			{
				PurchaseCountText.text = purchasedCount;
			}
		}
	}
}
