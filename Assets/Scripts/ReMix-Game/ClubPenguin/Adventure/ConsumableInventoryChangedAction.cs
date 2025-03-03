using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Props")]
	public class ConsumableInventoryChangedAction : FsmStateAction
	{
		public FsmEvent ChangedEvent;

		public bool WaitForChange = true;

		public override void OnEnter()
		{
			if (!Service.Get<CPDataEntityCollection>().LocalPlayerHandle.IsNull)
			{
				Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).OnConsumableInventoryChanged += onInventoryChanged;
			}
			if (!WaitForChange)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (!Service.Get<CPDataEntityCollection>().LocalPlayerHandle.IsNull)
			{
				Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).OnConsumableInventoryChanged -= onInventoryChanged;
			}
		}

		private void onInventoryChanged(ConsumableInventory inventory)
		{
			base.Fsm.Event(ChangedEvent);
			Finish();
		}
	}
}
