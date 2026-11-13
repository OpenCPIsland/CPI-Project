using Fabric;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class FabricSetSwitchAction : Action
	{
		public string EventName;

		public string SwitchValue;

		public GameObject TheObject;

		protected override void CopyTo(Action _destAction)
		{
			FabricSetSwitchAction fabricSetSwitchAction = _destAction as FabricSetSwitchAction;
			fabricSetSwitchAction.EventName = EventName;
			fabricSetSwitchAction.SwitchValue = SwitchValue;
			fabricSetSwitchAction.TheObject = TheObject;
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			EventManager.Instance.PostEvent(EventName, EventAction.SetSwitch, SwitchValue, TheObject);
			Completed();
		}
	}
}
