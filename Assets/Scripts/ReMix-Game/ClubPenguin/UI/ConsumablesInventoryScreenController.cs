using ClubPenguin.Breadcrumbs;
using ClubPenguin.Consumable;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Props;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ConsumablesInventoryScreenController : AbstractProgressionLockedItems<PropDefinition>
	{
		public GameObject EmptyInventoryMessage;

		public StaticBreadcrumbDefinitionKey ConsumableBreadcrumb;

		public StaticBreadcrumbDefinitionKey GearBreadcrumb;

		protected override void start()
		{
			removeBreadcrumbs();
		}

		protected override void createItem(PropDefinition itemDefinition, GameObject item, ItemGroup.LockedState lockedState)
		{
			item.GetComponent<GearInventoryButton>().Init(itemDefinition);
		}

		protected override PropDefinition[] getRewards(RewardDefinition rewardDefinition)
		{
			return AbstractStaticGameDataRewardDefinition<PropDefinition>.ToDefinitionArray(rewardDefinition.GetDefinitions<ConsumableRewardDefinition>());
		}

		protected override PropDefinition[] filterDefinitions(PropDefinition[] definitions)
		{
			List<PropDefinition> list = new List<PropDefinition>();
			if (definitions != null && definitions.Length > 0)
			{
				list.AddRange(definitions);
				list.RemoveAll((PropDefinition definition) => definition.PropType != PropDefinition.PropTypes.Consumable);
				list.RemoveAll((PropDefinition definition) => definition.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby);
				list.RemoveAll((PropDefinition definition) => definition.ServerAddedItem);
				ConsumableInventory consumableInventory = Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).ConsumableInventory;
				for (int num = list.Count - 1; num >= 0; num--)
				{
					if (consumableInventory.inventoryMap.ContainsKey(list[num].GetNameOnServer()))
					{
						if (consumableInventory.inventoryMap[list[num].GetNameOnServer()].GetItemCount() == 0)
						{
							list.RemoveAt(num);
						}
					}
					else
					{
						list.RemoveAt(num);
					}
				}
			}
			return list.ToArray();
		}

		protected override void elementsAdded()
		{
			bool flag = GetComponentInChildren<ScrollRect>().transform.Find("ScrollContent/ElementContainer").childCount > 0;
			EmptyInventoryMessage.SetActive(!flag);
		}

		private void removeBreadcrumbs()
		{
			if (UnlockCategory == ProgressionUnlockCategory.partySupplies)
			{
				Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(ConsumableBreadcrumb);
			}
			if (UnlockCategory == ProgressionUnlockCategory.durables)
			{
				Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(GearBreadcrumb);
			}
		}
	}
}
