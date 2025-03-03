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
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets the number of Touches.")]
	public class GetTouchCount : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the current number of touches in an Int Variable.")]
		public FsmInt storeCount;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;
		
		public override void Reset()
		{
			storeCount = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
            DoGetTouchCount();

            if (!everyFrame)
            {
                Finish();
            }
		}
		
		public override void OnUpdate()
		{
			DoGetTouchCount();
		}

        private void DoGetTouchCount()
		{

#if NEW_INPUT_SYSTEM_ONLY
            storeCount.Value = Touchscreen.current != null ? Touchscreen.current.touches.Count : 0;
#else
            storeCount.Value = Input.touchCount;
#endif
        }
    }
}