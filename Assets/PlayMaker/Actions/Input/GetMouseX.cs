// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
// This action was developed for the old input manager, so we will use it if its available. 
// If only the new input system is available we will try to use that instead,
// but there might be subtle differences in the behaviour in the new system!

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
	[Tooltip("Gets the X Position of the mouse and stores it in a Float Variable.")]
	public class GetMouseX : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store in a float variable.")]
		public FsmFloat storeResult;

        [Tooltip("Normalized coordinates are in the range 0 to 1 (0 = left, 1 = right). Otherwise the coordinate is in pixels. " +
                 "Normalized coordinates are useful for resolution independent functions.")]
        public bool normalize;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
		{
			storeResult = null;
			normalize = true;
            everyFrame = true;
        }

		public override void OnEnter()
		{
			DoGetMouseX();

            if (!everyFrame)
            {
                Finish();
            }
        }

		public override void OnUpdate()
		{
			DoGetMouseX();
		}

        private void DoGetMouseX()
		{
			if (storeResult != null)
			{

#if NEW_INPUT_SYSTEM_ONLY

                if (Mouse.current == null) return;

                var xPos = Mouse.current.position.ReadValue().x;
#else
                var xPos = Input.mousePosition.x;
#endif
                if (normalize)
                {
                    xPos /= Screen.width;
                }

                storeResult.Value = xPos;
            }
        }

#if UNITY_EDITOR

        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, storeResult);
        }

#endif
    }
}

