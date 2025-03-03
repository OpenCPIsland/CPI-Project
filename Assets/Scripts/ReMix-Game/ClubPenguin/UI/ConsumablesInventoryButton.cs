using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ConsumablesInventoryButton : MonoBehaviour
	{
		public Image Icon;

		public Button SelectButton;

		public Text Title;

		public Text InventoryCount;

		public GameObject ShareableIcon;

		public NotificationBreadcrumb breadcrumb;

		private string consumableType;

		private int inventoryCount;

		private int consumableCost;

		public void OnDestroy()
		{
			SelectButton.onClick.RemoveListener(onSelected);
			Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).OnConsumableInventoryChanged -= setInventoryCount;
		}

		public void Init(PropDefinition def)
		{
			consumableType = def.GetNameOnServer();
			consumableCost = def.Cost;
			Title.text = Service.Get<Localizer>().GetTokenTranslation(def.Name);
			if (def.Shareable)
			{
				ShareableIcon.SetActive(true);
			}
			Content.LoadAsync(onIconLoaded, def.GetIconContentKey());
			SelectButton.onClick.AddListener(onSelected);
			setInventoryCount(Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).ConsumableInventory);
			Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).OnConsumableInventoryChanged += setInventoryCount;
			breadcrumb.SetBreadcrumbId(string.Format("Consumable_{0}", def.GetNameOnServer()));
		}

		private void onIconLoaded(string path, Sprite image)
		{
			Icon.sprite = image;
		}

		private void setInventoryCount(ConsumableInventory inventory)
		{
			int num = 0;
			if (inventory.inventoryMap != null && inventory.inventoryMap.ContainsKey(consumableType))
			{
				num = inventory.inventoryMap[consumableType].GetItemCount();
			}
			if (num > 0)
			{
				InventoryCount.text = num.ToString();
			}
			else
			{
				InventoryCount.text = "$" + consumableCost;
			}
			inventoryCount = num;
		}

		private void onSelected()
		{
			if (inventoryCount > 0)
			{
				equipItem(consumableType);
				jumpToControls();
				Service.Get<NotificationBreadcrumbController>().ResetBreadcrumbs(breadcrumb.BreadcrumbId);
			}
		}

		private void equipItem(string type)
		{
			Service.Get<PropService>().LocalPlayerRetrieveProp(type);
		}

		private void jumpToControls()
		{
			StateMachineContext component = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponent<StateMachineContext>();
			component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
		}
	}
}
