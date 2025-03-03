using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketplaceInventoryScreenController : MonoBehaviour
	{
		public GameObject ContentPanel;

		public Text TitleText;

		public Button BackButton;

		private Dictionary<string, Sprite> itemSprites;

		private Dictionary<string, MarketplaceInventoryItem> inventoryItems;

		private static PrefabContentKey inventoryItemContentKey = new PrefabContentKey("Prefabs/InventoryItemPrefab");

		private GameObject inventoryItemPrefab;

		private void Start()
		{
			inventoryItems = new Dictionary<string, MarketplaceInventoryItem>();
			Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).OnConsumableInventoryChanged += onInventoryChanged;
		}

		private void OnDestroy()
		{
			ConsumableInventoryData component = Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			if (component != null)
			{
				component.OnConsumableInventoryChanged -= onInventoryChanged;
			}
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
			TitleText.text = "My Inventory";
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		public void SetItemSprites(Dictionary<string, Sprite> sprites)
		{
			itemSprites = sprites;
		}

		public void LoadInventoryItems()
		{
			Content.LoadAsync(onInventoryItemPrefabLoaded, inventoryItemContentKey);
		}

		private void onInventoryItemPrefabLoaded(string path, GameObject prefab)
		{
			inventoryItemPrefab = prefab;
			ConsumableInventory consumableInventory = Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).ConsumableInventory;
			foreach (PropDefinition value in Service.Get<PropService>().Props.Values)
			{
				string nameOnServer = value.GetNameOnServer();
				if (value.PropType == PropDefinition.PropTypes.Consumable && !value.ServerAddedItem && !value.QuestOnly && value.GetIconContentKey() != null && !string.IsNullOrEmpty(value.GetIconContentKey().Key) && consumableInventory.inventoryMap.ContainsKey(nameOnServer) && consumableInventory.inventoryMap[nameOnServer].GetItemCount() > 0)
				{
					GameObject gameObject = Object.Instantiate(prefab);
					gameObject.transform.SetParent(ContentPanel.transform, false);
					gameObject.GetComponent<MarketplaceInventoryItem>().Init(itemSprites[value.GetIconContentKey().Key.ToLower()], value, consumableInventory.inventoryMap[nameOnServer].GetItemCount());
					inventoryItems[nameOnServer] = gameObject.GetComponent<MarketplaceInventoryItem>();
				}
			}
		}

		private void onInventoryChanged(ConsumableInventory inventory)
		{
			foreach (KeyValuePair<string, InventoryItemStock> item in inventory.inventoryMap)
			{
				if (inventoryItems.ContainsKey(item.Key))
				{
					if (item.Value.GetItemCount() > 0)
					{
						if (inventoryItems.ContainsKey(item.Key))
						{
							inventoryItems[item.Key].SetCount(item.Value.GetItemCount());
						}
					}
					else
					{
						Object.Destroy(inventoryItems[item.Key]);
						inventoryItems.Remove(item.Key);
					}
				}
				else if (item.Value.GetItemCount() > 0)
				{
					PropDefinition propDefinition = Service.Get<PropService>().Props[item.Key];
					GameObject gameObject = Object.Instantiate(inventoryItemPrefab);
					gameObject.transform.SetParent(ContentPanel.transform, false);
					gameObject.GetComponent<MarketplaceInventoryItem>().Init(itemSprites[propDefinition.GetIconContentKey().Key.ToLower()], propDefinition, item.Value.GetItemCount());
					inventoryItems[item.Key] = gameObject.GetComponent<MarketplaceInventoryItem>();
				}
			}
		}
	}
}
