// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
// This action was developed for the old input manager, so we will use it if its available. 
// If only the new input system is available we will try to use that instead,
// but there might be subtle differences in the behaviour in the new system!

// NOTE: The new Input System uses a new Key enum instead of KeyCode
// So you will need to re-enter the key code if updating to the new Input System

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using UnityEngine;

#if NEW_INPUT_SYSTEM_ONLY
using UnityEngine.InputSystem;
#endif

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends an Event when a Key is released.")]
	public class GetKeyUp : FsmStateAction
	{
#if NEW_INPUT_SYSTEM_ONLY
        [RequiredField]
        [Tooltip("The key to detect.")]
        public Key key;
#else
		[RequiredField]
        [Tooltip("The key to detect.")]
		public KeyCode key;
#endif

        [Tooltip("The Event to send when the key is released.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Bool Variable. True if released, otherwise False.")]
		public FsmBool storeResult;
		
		public override void Reset()
		{
			sendEvent = null;
#if NEW_INPUT_SYSTEM_ONLY
            key = Key.None;
#else
			key = KeyCode.None;
#endif
			storeResult = null;
		}

		public override void OnUpdate()
		{
#if NEW_INPUT_SYSTEM_ONLY
            var keyUp = Keyboard.current[key].wasReleasedThisFrame;
#else
            var keyUp = Input.GetKeyUp(key);
#endif

            storeResult.Value = keyUp;

            if (keyUp)
            {
                Fsm.Event(sendEvent);
            }

		}

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, key.ToString(), sendEvent != null ? sendEvent.Name : "");
        }
#endif
    }
}