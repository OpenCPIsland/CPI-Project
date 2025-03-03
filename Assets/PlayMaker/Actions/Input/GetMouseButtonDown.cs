// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
// This action was developed for the old input manager, so we will use it if its available. 
// If only the new input system is available we will try to use that instead,
// but there might be subtle differences in the behaviour in the new system!

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

#if NEW_INPUT_SYSTEM_ONLY
using UnityEngine.InputSystem;
#else
using UnityEngine;
#endif

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when the specified Mouse Button is pressed. Optionally store the button state in a bool variable.")]
	public class GetMouseButtonDown : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The mouse button to test.")]
		public MouseButton button;

        [Tooltip("Event to send if the mouse button is down.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the button state in a Bool Variable.")]
        public FsmBool storeResult;

        [Tooltip("Uncheck to run when entering the state.")]
	    public bool inUpdateOnly;
		
		public override void Reset()
		{
			button = MouseButton.Left;
			sendEvent = null;
			storeResult = null;
		    inUpdateOnly = true;
		}

        public override void OnEnter()
        {
            if (!inUpdateOnly)
            {
                DoGetMouseButtonDown();
            }
        }

        public override void OnUpdate()
        {
            DoGetMouseButtonDown();
        }

        private void DoGetMouseButtonDown()
		{
#if NEW_INPUT_SYSTEM_ONLY
            var buttonDown = false;
            switch (button)
            {
                case MouseButton.Left:
                    buttonDown = Mouse.current.leftButton.wasPressedThisFrame;
                    break;
                case MouseButton.Right:
                    buttonDown = Mouse.current.rightButton.wasPressedThisFrame;
                    break;
                case MouseButton.Middle:
                    buttonDown = Mouse.current.middleButton.wasPressedThisFrame;
                    break;
            }
#else
            var buttonDown = Input.GetMouseButtonDown((int)button);
#endif
            storeResult.Value = buttonDown;

            if (buttonDown)
			{
			    Fsm.Event(sendEvent);
			}
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return string.Format("GetMouseButtonDown: {0} {1}",
                sendEvent != null ? sendEvent.Name : "",
                storeResult.IsNone ? "" : storeResult.Name);
        }
#endif
    }
}