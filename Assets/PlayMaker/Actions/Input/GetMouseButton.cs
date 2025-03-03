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
	[Tooltip("Gets the pressed state of the specified Mouse Button and stores it in a Bool Variable. See Unity Input Manager doc.")]
	public class GetMouseButton : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The mouse button to test.")]
		public MouseButton button;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the pressed state in a Bool Variable.")]
		public FsmBool storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

		public override void Reset()
		{
			button = MouseButton.Left;
			storeResult = null;
            everyFrame = true;
        }

        public override void OnEnter()
        {
            DoGetMouseButton();

            if (!everyFrame)
            {
                Finish();
            }
        }

		public override void OnUpdate()
		{
			DoGetMouseButton();
		}

        private void DoGetMouseButton()
        {
#if NEW_INPUT_SYSTEM_ONLY

            var buttonDown = false;

            switch (button)
            {
                case MouseButton.Left:
                    buttonDown = Mouse.current.leftButton.isPressed;
                    break;
                case MouseButton.Right:
                    buttonDown = Mouse.current.rightButton.isPressed;
                    break;
                case MouseButton.Middle:
                    buttonDown = Mouse.current.middleButton.isPressed;
                    break;
            }

            storeResult.Value = buttonDown;
#else
            storeResult.Value = Input.GetMouseButton((int)button);
#endif
        }
	}
}

