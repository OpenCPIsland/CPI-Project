// (c) Copyright HutongGames, LLC. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends events when a GUI Texture or GUI Text is touched. Optionally filter by a fingerID.")]

    #if UNITY_2019_3_OR_NEWER
    [Obsolete("GUIElement is part of the legacy UI system removed in 2019.3")]
    #elif UNITY_2017_2_OR_NEWER
	#pragma warning disable 618  
	[Obsolete("GUIElement is part of the legacy UI system and will be removed in a future release")]
	#endif
	public class TouchGUIEvent : FsmStateAction
	{
		[RequiredField]
#if !UNITY_2019_3_OR_NEWER
		[CheckForComponent(typeof(GUIElement))]
#else
        [ActionSection("Obsolete. Use Unity UI instead.")]
#endif
		[Tooltip("The Game Object that owns the GUI Texture or GUI Text.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Only detect touches that match this fingerID, or set to None.")]
		public FsmInt fingerId;

		[ActionSection("Events")]

		[Tooltip("Event to send on touch began.")]
		public FsmEvent touchBegan;

		[Tooltip("Event to send on touch moved.")]
		public FsmEvent touchMoved;

		[Tooltip("Event to send on stationary touch.")]
		public FsmEvent touchStationary;

		[Tooltip("Event to send on touch ended.")]
		public FsmEvent touchEnded;

		[Tooltip("Event to send on touch cancel.")]
		public FsmEvent touchCanceled;

		[Tooltip("Event to send if not touching (finger down but not over the GUI element)")]
		public FsmEvent notTouching;

		[ActionSection("Store Results")]

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the fingerId of the touch.")]
		public FsmInt storeFingerId;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen position where the GUI element was touched.")]
		public FsmVector3 storeHitPoint;

		[Tooltip("Normalize the hit point screen coordinates (0-1).")]
		public FsmBool normalizeHitPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the offset position of the hit.")]
		public FsmVector3 storeOffset;

		[Tooltip("How to measure the offset.")]
		public OffsetOptions relativeTo;

		public enum OffsetOptions
		{
			TopLeft,
			Center,
			TouchStart
		}

		[Tooltip("Normalize the offset.")]
		public FsmBool normalizeOffset;

		[ActionSection("")] 
		
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// private work variables

		private Vector3 touchStartPos;

#if !UNITY_2019_3_OR_NEWER
		private GUIElement guiElement;
#endif

		public override void Reset()
		{
			gameObject = null;
			fingerId = new FsmInt { UseVariable = true };

			touchBegan = null;
			touchMoved = null;
			touchStationary = null;
			touchEnded = null;
			touchCanceled = null;

			storeFingerId = null;
			storeHitPoint = null;
			normalizeHitPoint = false;
			storeOffset = null;
			relativeTo = OffsetOptions.Center;
			normalizeOffset = true;

			everyFrame = true;
		}

		public override void  OnEnter()
		{
#if !UNITY_2019_3_OR_NEWER
			DoTouchGUIEvent();

            if (!everyFrame)
			{
				Finish();
			}
#else
            Finish();
#endif
		}

#if !UNITY_2019_3_OR_NEWER
		public override void  OnUpdate()
		{
 			 DoTouchGUIEvent();
		}

		void DoTouchGUIEvent()
		{
			if (Input.touchCount > 0)
			{
				var go = Fsm.GetOwnerDefaultTarget(gameObject);
				if (go == null)
				{
					return;
				}

				guiElement = go.GetComponent<GUITexture>() ?? (GUIElement) go.GetComponent<GUIText>();

				if (guiElement == null)
				{
					return;
				}

				foreach (var touch in Input.touches)
				{
					DoTouch(touch);
				}
			}
		}

		void DoTouch(Touch touch)
		{
			// Filter by finger ID

			if (fingerId.IsNone || touch.fingerId == fingerId.Value)
			{
				// Get the screen position of the touch

				Vector3 touchPos = touch.position;

				// Is touchPos inside the guiElement's rect

				if (guiElement.HitTest(touchPos))
				{
					// First touch?

					if (touch.phase == TouchPhase.Began)
					{
						touchStartPos = touchPos;
					}

					// Store results

					storeFingerId.Value = touch.fingerId;

					if (normalizeHitPoint.Value)
					{
						touchPos.x /= Screen.width;
						touchPos.y /= Screen.height;
					}
					
					storeHitPoint.Value = touchPos;

					// Store touch offset

					DoTouchOffset(touchPos);

					// Send Events

					switch (touch.phase)
					{
						case TouchPhase.Began:
							Fsm.Event(touchBegan);
							return;

						case TouchPhase.Moved:
							Fsm.Event(touchMoved);
							return;

						case TouchPhase.Stationary:
							Fsm.Event(touchStationary);
							return;

						case TouchPhase.Ended:
							Fsm.Event(touchEnded);
							return;

						case TouchPhase.Canceled:
							Fsm.Event(touchCanceled);
							return;
					}
				}
				else
				{
					Fsm.Event(notTouching);
				}
			}
		}

		void DoTouchOffset(Vector3 touchPos)
		{
			if (storeOffset.IsNone)
			{
				return;
			}

			var guiRect = guiElement.GetScreenRect();
			var offset = new Vector3();

			switch (relativeTo)
			{
				case OffsetOptions.TopLeft:

					offset.x = touchPos.x - guiRect.x;
					offset.y = touchPos.y - guiRect.y;
					
					break;
				
				case OffsetOptions.Center:

					var center = new Vector3(guiRect.x + guiRect.width * 0.5f, guiRect.y + guiRect.height * 0.5f, 0);
					offset = touchPos - center;
					
					break;
				
				case OffsetOptions.TouchStart:

					offset = touchPos - touchStartPos;
					
					break;
			}

			if (normalizeOffset.Value)
			{
				offset.x /= guiRect.width;
				offset.y /= guiRect.height;
			}

			storeOffset.Value = offset;
		}

#endif
	}
}