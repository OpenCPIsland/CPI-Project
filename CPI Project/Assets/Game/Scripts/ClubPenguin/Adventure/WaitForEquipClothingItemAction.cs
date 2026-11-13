using ClubPenguin.ClothingDesigner.Inventory;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class WaitForEquipClothingItemAction : FsmStateAction
	{
		public override void OnEnter()
		{
			InventoryContext.EventBus.AddListener<InventoryUIEvents.EquipEquipment>(onEquipItem);
		}

		public override void OnExit()
		{
			InventoryContext.EventBus.RemoveListener<InventoryUIEvents.EquipEquipment>(onEquipItem);
		}

		private bool onEquipItem(InventoryUIEvents.EquipEquipment evt)
		{
			Finish();
			return false;
		}
	}
}
