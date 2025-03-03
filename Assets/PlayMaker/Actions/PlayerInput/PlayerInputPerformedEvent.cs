// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
	[Tooltip("Sends an Event when an InputAction in a PlayerInput component is Performed.")]
	public class PlayerInputPerformedEvent: PlayerInputActionBase
	{
        [Tooltip("The event to send on Input Performed")]
        public FsmEvent sendEvent;

        public override void Reset()
        {
            base.Reset();
            sendEvent = null;
        }

        protected override void OnPerformed(InputAction.CallbackContext ctx)
        {
            Fsm.Event(sendEvent);
        }
    }
}

#endif
