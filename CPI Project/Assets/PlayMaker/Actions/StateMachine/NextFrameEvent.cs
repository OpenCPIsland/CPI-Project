// (c) Copyright HutongGames, LLC. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
	public class NextFrameEvent : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Event to send.")]
		public FsmEvent sendEvent;

		public override void Reset()
		{
			sendEvent = null;
		}

		public override void OnEnter()
		{
		}

		public override void OnUpdate()
		{
			Finish();

			Fsm.Event(sendEvent);
		}


#if UNITY_EDITOR
        public override string AutoName()
        {
            return "NextFrameEvent: " + (sendEvent != null ? sendEvent.Name : "[none]");
        }
#endif
	}
}