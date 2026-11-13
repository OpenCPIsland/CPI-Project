using ClubPenguin.Actions;

namespace ClubPenguin.Adventure
{
	public class SendFSMEvent : Action
	{
		public string Event;

		protected override void OnEnable()
		{
			if (Owner.CompareTag("Player"))
			{
				PlayMakerFSM component = CustomTarget.GetComponent<PlayMakerFSM>();
				component.SendEvent(Event);
			}
		}

		protected override void CopyTo(Action _dest)
		{
			SendFSMEvent sendFSMEvent = _dest as SendFSMEvent;
			sendFSMEvent.Event = Event;
			base.CopyTo(_dest);
		}

		protected override void Update()
		{
			Completed();
		}
	}
}
