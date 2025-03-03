using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class MarketplaceEventController : MonoBehaviour
	{
		public MarketplaceEventRowItem ExistingRowItem;

		public GameObject ItemPanelContainer;

		public Material ItemIconImageMaterial;

		public Color ItemIconBgColor;

		public Color ItemIconPenguinColor;

		public GameObject ItemsBlocker;

		public string EventNameForBI;

		public ClaimableRewardDefinition[] EventItems;

		private static PrefabContentKey eventItemButtonContentKey = new PrefabContentKey("Prefabs/MarketplaceEventItemButton");

		private void Start()
		{
			createItems();
		}

		private void createItems()
		{
			bool flag = true;
			for (int i = 0; i < EventItems.Length; i++)
			{
				MarketplaceEventItem item = new MarketplaceEventItem();
				item.EventItemDefinition = EventItems[i];
				if (ExistingRowItem == null)
				{
					Content.LoadAsync(delegate(string path, GameObject prefab)
					{
						onEventItemButtonPrefabLoaded(prefab, item);
					}, eventItemButtonContentKey);
				}
				else
				{
					ExistingRowItem.SetItem(item, ItemIconImageMaterial, ItemIconBgColor, ItemIconPenguinColor, EventNameForBI);
				}
				if (flag && !item.IsAvailable())
				{
					flag = false;
				}
			}
			if (ItemsBlocker != null)
			{
				ItemsBlocker.SetActive(!flag);
			}
		}

		private void onEventItemButtonPrefabLoaded(GameObject prefab, MarketplaceEventItem item)
		{
			GameObject gameObject = Object.Instantiate(prefab, ItemPanelContainer.transform, false);
			gameObject.GetComponent<MarketplaceEventRowItem>().SetItem(item, ItemIconImageMaterial, ItemIconBgColor, ItemIconPenguinColor, EventNameForBI);
		}
	}
}
