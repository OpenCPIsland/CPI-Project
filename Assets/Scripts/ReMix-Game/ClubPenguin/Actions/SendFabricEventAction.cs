using Fabric;

namespace ClubPenguin.Actions
{
	public class SendFabricEventAction : Action
	{
		public string EventName;

		public EventAction EventAction = EventAction.PlaySound;

		protected override void CopyTo(Action _destAction)
		{
			SendFabricEventAction sendFabricEventAction = _destAction as SendFabricEventAction;
			sendFabricEventAction.EventName = EventName;
			sendFabricEventAction.EventAction = EventAction;
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			EventManager.Instance.PostEvent(EventName, EventAction, GetTarget());
			Completed();
		}
	}
}
