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
	// TODO: fairly basic right now
	// should have more options and be more robust, e.g., other fingers.
	
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends an event when a swipe is detected.")]
	public class SwipeGestureEvent : FsmStateAction
	{
		[Tooltip("How far a touch has to travel to be considered a swipe. Uses normalized distance (e.g. 1 = 1 screen diagonal distance). Should generally be a very small number.")]
		public FsmFloat minSwipeDistance;
		
		[Tooltip("Event to send when swipe left detected.")]
		public FsmEvent swipeLeftEvent;
		[Tooltip("Event to send when swipe right detected.")]
		public FsmEvent swipeRightEvent;
		[Tooltip("Event to send when swipe up detected.")]
		public FsmEvent swipeUpEvent;
		[Tooltip("Event to send when swipe down detected.")]
		public FsmEvent swipeDownEvent;
		
		// TODO
/*		[UIHint(UIHint.Variable)]
		[Tooltip("Store the speed of the swipe.")]
		public FsmFloat getSpeed;
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance the swipe traveled.")]
		public FsmFloat getDistance;*/
		
		float screenDiagonalSize;
		float minSwipeDistancePixels;
		bool touchStarted;
		Vector2 touchStartPos;
		//float touchStartTime;
		
		public override void Reset()
		{
			minSwipeDistance = 0.1f;
			swipeLeftEvent = null;
			swipeRightEvent = null;
			swipeUpEvent = null;
			swipeDownEvent = null;
		}
		
		public override void OnEnter()
		{
			screenDiagonalSize = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
			minSwipeDistancePixels = minSwipeDistance.Value * screenDiagonalSize;
		}
		
		public override void OnUpdate()
		{
#if NEW_INPUT_SYSTEM_ONLY
            if (Touchscreen.current == null) return;

            var touchCount = Touchscreen.current.touches.Count;
            if (touchCount > 0)
            {
                var touch = Touchscreen.current.touches[0];
                var touchPosition = touch.position.ReadValue();
                var touchPhase = touch.phase.ReadValue().ToString();

                switch (touchPhase)
                {
                    case "Began":
                        touchStarted = true;
                        touchStartPos = touchPosition;
                        //touchStartTime = FsmTime.RealtimeSinceStartup;
                        break;

                    case "Ended":
                        if (touchStarted)
                        {
                            TestForSwipeGesture(touchPosition);
                            touchStarted = false;
                        }
                        break;

                    case "Canceled":
                        touchStarted = false;
                        break;
                }
            }
#else
            if (Input.touchCount > 0)
			{
				var touch = Input.touches[0];
				
				switch (touch.phase) 
				{
				case TouchPhase.Began:
					
					touchStarted = true;
					touchStartPos = touch.position;
					//touchStartTime = FsmTime.RealtimeSinceStartup;
					
					break;
					
				case TouchPhase.Ended:
					
					if (touchStarted)
					{
						TestForSwipeGesture(touch.position);
						touchStarted = false;
					}
					
					break;
					
				case TouchPhase.Canceled:
					
					touchStarted = false;
					
					break;
					
				case TouchPhase.Stationary:
					
/*					if (touchStarted)
					{
						// don't want idle time to count towards swipe
						
						touchStartPos = touch.position;
						touchStartTime = FsmTime.RealtimeSinceStartup;
					}*/
					
					break;

				case TouchPhase.Moved:
					
					break;
				}
			}
#endif
        }

        private void TestForSwipeGesture(Vector2 touchPosition)
		{
			// test min distance
			
			var lastPos = touchPosition;
			var distance = Vector2.Distance(lastPos, touchStartPos);
			
			if (distance > minSwipeDistancePixels)
			{
				float dy = lastPos.y - touchStartPos.y;
				float dx = lastPos.x - touchStartPos.x;
				
				float angle = Mathf.Rad2Deg * Mathf.Atan2(dx, dy);
				
				angle = (360 + angle - 45) % 360;

				Debug.Log (angle);
				
				if (angle < 90)
				{
					Fsm.Event(swipeRightEvent);
				}
				else if (angle < 180)
				{
					Fsm.Event(swipeDownEvent);
				}
				else if (angle < 270)
				{
					Fsm.Event(swipeLeftEvent);
				}
				else 
				{
					Fsm.Event(swipeUpEvent);
				}
			}
		}
			
	}
}