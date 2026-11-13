using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Props")]
	public class CheckConsumableInventoryAction : FsmStateAction
	{
		public PropDefinition PropDefinition;

		public FsmInt OUT_ItemCount;

		public override void OnEnter()
		{
			if (!Service.Get<CPDataEntityCollection>().LocalPlayerHandle.IsNull)
			{
				ConsumableInventory consumableInventory = Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).ConsumableInventory;
				string nameOnServer = PropDefinition.GetNameOnServer();
				if (consumableInventory.inventoryMap.ContainsKey(nameOnServer))
				{
					OUT_ItemCount.Value = consumableInventory.inventoryMap[nameOnServer].GetItemCount();
				}
			}
			Finish();
		}
	}
}
