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
	[Tooltip("Gets the pressed state of a Key.")]
	public class GetKey : FsmStateAction
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
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store if the key is down (True) or up (False).")]
		public FsmBool storeResult;
		
		[Tooltip("Repeat every frame. Useful if you're waiting for a key press/release.")]
		public bool everyFrame;
		
		public override void Reset()
		{
#if NEW_INPUT_SYSTEM_ONLY
            key = Key.None;
#else
			key = KeyCode.None;
#endif
			storeResult = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoGetKey();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		

		public override void OnUpdate()
		{
			DoGetKey();
		}

        private void DoGetKey()
		{
#if NEW_INPUT_SYSTEM_ONLY
            storeResult.Value = Keyboard.current[key].isPressed;
#else
            storeResult.Value = Input.GetKey(key);
#endif
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, key.ToString(), ActionHelpers.GetValueLabel(storeResult));
        }
#endif

    }
}