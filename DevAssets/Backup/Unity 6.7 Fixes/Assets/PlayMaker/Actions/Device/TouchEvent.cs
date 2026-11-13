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
// We can't do this since the enum is defined in a different order
//using TouchPhase = UnityEngine.InputSystem.TouchPhase;
#else
using UnityEngine;
#endif

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends events based on Touch Phases. Optionally filter by a fingerID.")]
	public class TouchEvent : FsmStateAction
	{
        [Tooltip("An optional Finger Id to filter by. For example, if you detected a Touch Began and stored the FingerId, you could look for the Ended event for that Finger Id.")]
        public FsmInt fingerId;
        [Tooltip("The phase you're interested in detecting (Began, Moved, Stationary, Ended, Cancelled).")]
        public UnityEngine.TouchPhase touchPhase;
        [Tooltip("The event to send when the Touch Phase is detected.")]
        public FsmEvent sendEvent;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Finger Id associated with the touch event for later use.")]
        public FsmInt storeFingerId;
		
		public override void Reset()
		{
			fingerId = new FsmInt { UseVariable = true } ;
			storeFingerId = null;
		}

		public override void OnUpdate()
		{
#if NEW_INPUT_SYSTEM_ONLY
            if (Touchscreen.current == null) return;

            var touchCount = Touchscreen.current.touches.Count;
            if (touchCount > 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    var touchId = touch.touchId.ReadValue();

                    if (fingerId.IsNone || touchId == fingerId.Value)
                    {
                        if (touch.phase.ReadValue().ToString() == touchPhase.ToString())
                        {
                            storeFingerId.Value = touchId;
                            Fsm.Event(sendEvent);
                        }
                    }
                }
            }
#else
            if (Input.touchCount > 0)
			{
				foreach (var touch in Input.touches)
				{

					if (fingerId.IsNone || touch.fingerId == fingerId.Value)
					{
						if (touch.phase == touchPhase)
						{
							storeFingerId.Value = touch.fingerId;
							Fsm.Event(sendEvent);
						}
					}
				}
			}
#endif
		}
	}
}