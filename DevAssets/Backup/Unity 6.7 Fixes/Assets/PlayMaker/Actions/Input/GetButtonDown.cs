// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// The new Input System optionally supports the legacy input manager 
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using System;

#if !NEW_INPUT_SYSTEM_ONLY
using UnityEngine;
#endif

namespace HutongGames.PlayMaker.Actions
{
#if NEW_INPUT_SYSTEM_ONLY
    [Obsolete("This action is not supported in the new Input System. " +
              "Use PlayerInputGetButtonValues or GamepadGetButtonValues instead.")]
#endif
    [ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Button is pressed.")]
	public class GetButtonDown : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The name of the button. Defined in the Unity Input Manager.")]
		public FsmString buttonName;

        [Tooltip("Event to send if the button is pressed.")]
		public FsmEvent sendEvent;

        [Tooltip("Set to True if the button is pressed, otherwise False.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;
		
		public override void Reset()
		{
			buttonName = "Fire1";
			sendEvent = null;
			storeResult = null;
		}

		public override void OnUpdate()
		{
#if !NEW_INPUT_SYSTEM_ONLY
            var buttonDown = Input.GetButtonDown(buttonName.Value);

            storeResult.Value = buttonDown;

            if (buttonDown)
			{
			    Fsm.Event(sendEvent);
			}
#endif
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, buttonName) + " " + (sendEvent != null ? sendEvent.Name : "");
        }

#if NEW_INPUT_SYSTEM_ONLY

        public override string ErrorCheck()
        {
            return "This action is not supported in the new Input System. " +
                   "Use PlayerInputGetButtonValues or GamepadGetButtonValues instead.";
        }
#endif

#endif
    }
}