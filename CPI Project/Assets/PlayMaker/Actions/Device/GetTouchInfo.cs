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
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets info on a touch event.")]
	public class GetTouchInfo : FsmStateAction
	{
		[Tooltip("Filter by a Finger ID. You can store a Finger ID in other Touch actions, e.g., Touch Event.")]
		public FsmInt fingerId;
		[Tooltip("If true, all screen coordinates are returned normalized (0-1), otherwise in pixels.")]
		public FsmBool normalize;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the position of the touch in a Vector3 Variable.")]
		public FsmVector3 storePosition;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the X position  in a Float Variable.")]
		public FsmFloat storeX;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Y position  in a Float Variable.")]
		public FsmFloat storeY;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the movement of the touch in a Vector3 Variable.")]
		public FsmVector3 storeDeltaPosition;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the X movement in a Float Variable.")]
        public FsmFloat storeDeltaX;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Y movement in a Float Variable.")]
        public FsmFloat storeDeltaY;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the time between touch events in a Float Variable.")]
		public FsmFloat storeDeltaTime;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the number of tap count of the touch (e.g. 2 = double tap).")]
		public FsmInt storeTapCount;

        [Tooltip("Repeat every frame.")]
		public bool everyFrame = true;

        private float screenWidth;
        private float screenHeight;
		
		public override void Reset()
		{
			fingerId = new FsmInt { UseVariable = true };
			normalize = true;
			storePosition = null;
			storeDeltaPosition = null;
			storeDeltaTime = null;
			storeTapCount = null;
			everyFrame = true;
		}
		
		public override void OnEnter()
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			DoGetTouchInfo();

			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoGetTouchInfo();
		}

        private void DoGetTouchInfo()
		{
#if NEW_INPUT_SYSTEM_ONLY
            if (Touchscreen.current == null) return;

            var touchCount = Touchscreen.current.touches.Count;
            if (touchCount > 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (fingerId.IsNone || touch.touchId.ReadValue() == fingerId.Value)
                    {
                        var touchPosition = touch.position.ReadValue();
                        float x = normalize.Value == false ? touchPosition.x : touchPosition.x / screenWidth;
                        float y = normalize.Value == false ? touchPosition.y : touchPosition.y / screenHeight;

                        if (!storePosition.IsNone)
                        {
                            storePosition.Value = new Vector3(x, y, 0);
                        }

                        storeX.Value = x;
                        storeY.Value = y;

                        var touchDeltaPosition = touch.delta.ReadValue();
                        float deltaX = normalize.Value == false ? touchDeltaPosition.x : touchDeltaPosition.x / screenWidth;
                        float deltaY = normalize.Value == false ? touchDeltaPosition.y : touchDeltaPosition.y / screenHeight;

                        if (!storeDeltaPosition.IsNone)
                        {
                            storeDeltaPosition.Value = new Vector3(deltaX, deltaY, 0);
                        }

                        storeDeltaX.Value = deltaX;
                        storeDeltaY.Value = deltaY;

                        // New Input System doesn't seem to have touch.deltaTime.
                        // Not sure if this is a good substitute...?
                        storeDeltaTime.Value = Time.deltaTime; 

                        storeTapCount.Value = touch.tapCount.ReadValue();
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
						float x = normalize.Value == false ? touch.position.x : touch.position.x / screenWidth;
						float y = normalize.Value == false ? touch.position.y : touch.position.y / screenHeight;
						
						if (!storePosition.IsNone)
						{
							storePosition.Value = new Vector3(x, y, 0);
						}
						
						storeX.Value = x;
						storeY.Value = y;

						float deltax =
 normalize.Value == false ? touch.deltaPosition.x : touch.deltaPosition.x / screenWidth;
						float deltay =
 normalize.Value == false ? touch.deltaPosition.y : touch.deltaPosition.y / screenHeight;
						
						if (!storeDeltaPosition.IsNone)
						{
							storeDeltaPosition.Value = new Vector3(deltax, deltay, 0);
						}

						storeDeltaX.Value = deltax;
						storeDeltaY.Value = deltay;
						
						storeDeltaTime.Value = touch.deltaTime;
						storeTapCount.Value = touch.tapCount;
					}
				}
			}
#endif
        }
    }
}