using ClubPenguin.Analytics;
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
	public class GearInventoryButton : MonoBehaviour
	{
		public Image Icon;

		public Button SelectButton;

		public Text Title;

		public NotificationBreadcrumb breadcrumb;

		public TutorialBreadcrumb TutorialBreadcrumb;

		public GameObject ItemCountIcon;

		public GameObject SelectedPanel;

		public PersistentBreadcrumbTypeDefinitionKey ConsumableBreadcrumbType;

		public PersistentBreadcrumbTypeDefinitionKey GearBreadcrumbType;

		public PersistentBreadcrumbTypeDefinitionKey PartyGameBreadcrumbType;

		private PropDefinition definition;

		public int PropId
		{
			get
			{
				return definition.Id;
			}
		}

		public void OnDestroy()
		{
			SelectButton.onClick.RemoveListener(onSelected);
		}

		public void Init(PropDefinition def)
		{
			definition = def;
			Title.text = Service.Get<Localizer>().GetTokenTranslation(def.Name);
			Content.LoadAsync(onIconLoaded, def.GetIconContentKey());
			SelectButton.onClick.AddListener(onSelected);
			if (def.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby)
			{
				breadcrumb.SetBreadcrumbId(PartyGameBreadcrumbType, definition.Id.ToString());
				TutorialBreadcrumb.SetBreadcrumbId(string.Format("PartyGame_{0}", definition.Id));
			}
			else if (def.PropType == PropDefinition.PropTypes.Consumable)
			{
				breadcrumb.SetBreadcrumbId(ConsumableBreadcrumbType, definition.Id.ToString());
				TutorialBreadcrumb.SetBreadcrumbId(string.Format("Consumable_{0}", definition.Id));
			}
			else
			{
				breadcrumb.SetBreadcrumbId(GearBreadcrumbType, definition.Id.ToString());
				TutorialBreadcrumb.SetBreadcrumbId(string.Format("Gear_{0}", definition.Id));
			}
			ConsumableInventory consumableInventory = Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).ConsumableInventory;
			if (consumableInventory.inventoryMap.ContainsKey(definition.GetNameOnServer()))
			{
				int itemCount = consumableInventory.inventoryMap[definition.GetNameOnServer()].GetItemCount();
				if (itemCount == 0)
				{
					ItemCountIcon.SetActive(false);
				}
				else
				{
					ItemCountIcon.SetActive(true);
					ItemCountIcon.GetComponentInChildren<Text>().text = itemCount.ToString();
				}
			}
			else
			{
				ItemCountIcon.SetActive(false);
			}
			if (def.PropType == PropDefinition.PropTypes.Durable)
			{
				showSelectedState();
			}
		}

		private void onIconLoaded(string path, Sprite image)
		{
			Icon.sprite = image;
			Icon.enabled = true;
		}

		private void onSelected()
		{
			if (definition.PropType == PropDefinition.PropTypes.Consumable)
			{
				Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(ConsumableBreadcrumbType, definition.Id.ToString());
			}
			else
			{
				Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(GearBreadcrumbType, definition.Id.ToString());
			}
			Service.Get<TutorialBreadcrumbController>().RemoveBreadcrumb(TutorialBreadcrumb.BreadcrumbId);
			equipItem(definition.GetNameOnServer());
			switchToJoystick();
			Service.Get<NotificationBreadcrumbController>().ResetBreadcrumbs(breadcrumb.BreadcrumbId);
			Service.Get<ICPSwrveService>().Action("game.gear", definition.name);
		}

		private void equipItem(string type)
		{
			Service.Get<PropService>().LocalPlayerRetrieveProp(type);
		}

		private void switchToJoystick()
		{
			StateMachineContext component = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponent<StateMachineContext>();
			component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
		}

		private void showSelectedState()
		{
			HeldObjectsData component = Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			if (component != null && component.HeldObject != null)
			{
				SelectedPanel.SetActive(definition.NameOnServer == component.HeldObject.ObjectId);
			}
		}
	}
}
