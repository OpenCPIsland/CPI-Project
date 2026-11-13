using Disney.Kelowna.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Catalog
{
	public class CatalogShopRowItem : MonoBehaviour
	{
		public Transform Layout;

		public CatalogShopItem ShopItem;

		private List<CatalogShopItem> shopItems = new List<CatalogShopItem>();

		private IEnumerator DestroyGameObject(GameObject go)
		{
			yield return new WaitForSeconds(0.1f);
			Object.Destroy(go);
		}

		public List<CatalogShopItem> GetShopItems()
		{
			return shopItems;
		}

		public void ClearShopItems()
		{
			foreach (CatalogShopItem shopItem in shopItems)
			{
				shopItem.gameObject.transform.SetParent(null);
				CoroutineRunner.Start(DestroyGameObject(shopItem.gameObject), this, "DestroyGameObject");
			}
			shopItems = new List<CatalogShopItem>();
		}

		public CatalogShopItem AddShopItem(int scrollIndex, int rowIndex, bool isHideCreatorName = false)
		{
			CatalogShopItem catalogShopItem = Object.Instantiate(ShopItem);
			catalogShopItem.GetComponent<CatalogShopItem>().SetRowIndex(rowIndex);
			catalogShopItem.GetComponent<CatalogShopItem>().SetScrollIndex(scrollIndex);
			catalogShopItem.transform.SetParent(Layout.transform, false);
			if (isHideCreatorName)
			{
				catalogShopItem.DisableName();
			}
			shopItems.Add(catalogShopItem);
			return catalogShopItem;
		}
	}
}
