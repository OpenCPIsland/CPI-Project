// (c) Copyright HutongGames, LLC. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event in LateUpdate, after the Update loop.")]
	public class LateUpdateEvent : FsmStateAction
	{
		[RequiredField]
        [Tooltip("Event to send in LateUpdate.")]
		public FsmEvent sendEvent;

        public override void Reset()
		{
			sendEvent = null;
		}

        public override void OnPreprocess()
        {
            Fsm.HandleLateUpdate = true;
        }

		public override void OnEnter()
		{
		}

		public override void OnLateUpdate()
		{
			Finish();

			Fsm.Event(sendEvent);
		}


#if UNITY_EDITOR
        public override string AutoName()
        {
            return "LateUpdate Event: " + (sendEvent != null ? sendEvent.Name : "[none]");
        }
#endif
	}
}