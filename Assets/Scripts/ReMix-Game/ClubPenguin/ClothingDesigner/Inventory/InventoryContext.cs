using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Breadcrumbs;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class InventoryContext : MonoBehaviour
	{
		public StaticBreadcrumbDefinitionKey Breadcrumb;

		private static EventDispatcher eventBus;

		[SerializeField]
		private EquipmentListController equipmentListController;

		private InventoryController inventoryController;

		public static EventDispatcher EventBus
		{
			get
			{
				if (eventBus == null)
				{
					eventBus = new EventDispatcher();
				}
				return eventBus;
			}
		}

		public void Init()
		{
			inventoryController = new InventoryController(Breadcrumb);
			inventoryController.Init(equipmentListController);
			Service.Get<ICPSwrveService>().StartTimer("inventory_time", "my_style.inventory");
		}

		private void OnDestroy()
		{
			Service.Get<ICPSwrveService>().EndTimer("inventory_time");
		}

		[Invokable("Avatar.ClearOutfit", Description = "Remove the currently equipped penguin outfit.")]
		[PublicTweak]
		public static void ClearPenguinOutfit()
		{
			new SaveOutfitToWearCMD(new long[0], new DCustomEquipment[0]).Execute();
		}
	}
}
