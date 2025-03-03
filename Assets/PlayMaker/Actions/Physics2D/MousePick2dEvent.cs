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
	[Tooltip("Sends Events based on mouse interactions with a 2d Game Object: MouseOver, MouseDown, MouseUp, MouseOff.")]
	public class MousePick2dEvent : FsmStateAction
	{
		[CheckForComponent(typeof(Collider2D))]
		[Tooltip("The GameObject with a Collider2D attached.")]
		public FsmOwnerDefault GameObject;
		
		[Tooltip("Event to send when the mouse is over the GameObject.")]
		public FsmEvent mouseOver;
		
		[Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
		public FsmEvent mouseDown;
		
		[Tooltip("Event to send when the mouse is released while over the GameObject.")]
		public FsmEvent mouseUp;
		
		[Tooltip("Event to send when the mouse moves off the GameObject.")]
		public FsmEvent mouseOff;
		
		[Tooltip("Pick only from these layers.")]
		[UIHint(UIHint.Layer)]
		public FsmInt[] layerMask;
		
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;
		
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
		
		public override void Reset()
		{
			GameObject = null;
			mouseOver = null;
			mouseDown = null;
			mouseUp = null;
			mouseOff = null;
			layerMask = new FsmInt[0];
			invertMask = false;
			everyFrame = true;
		}
		
		public override void OnEnter()
		{
            DoMousePickEvent();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoMousePickEvent();
		}

        private void DoMousePickEvent()
		{
			// Do the raycast
			
			var isMouseOver = DoRaycast();
			
			// Send events based on the raycast and mouse buttons
			
			if (isMouseOver)
			{
#if NEW_INPUT_SYSTEM_ONLY
                if (Mouse.current == null) return;
                if (mouseDown != null && Mouse.current.leftButton.wasPressedThisFrame)
#else
                if (mouseDown != null && Input.GetMouseButtonDown(0))
#endif
                {
                    Fsm.Event(mouseDown);
				}
				
				if (mouseOver != null)
				{
					Fsm.Event(mouseOver);
				}

#if NEW_INPUT_SYSTEM_ONLY
                if (mouseUp != null && Mouse.current.leftButton.wasReleasedThisFrame)
#else
				if (mouseUp != null && Input.GetMouseButtonUp(0))
#endif
                {
                    Fsm.Event(mouseUp);
				}
			}
			else
			{
				if (mouseOff != null)
				{
					Fsm.Event(mouseOff);
				}
			}
		}

        private bool DoRaycast()
		{
			var testObject = GameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : GameObject.GameObject.Value;
			
			// ActionHelpers uses a cache to try and minimize Raycasts
            var hitInfo = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(ActionHelpers.GetMousePosition()),
                Mathf.Infinity, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));

			// Store mouse pick info so it can be seen by Get Raycast Hit Info action
			Fsm.RecordLastRaycastHit2DInfo(Fsm,hitInfo);

			if (hitInfo.transform != null)
			{
				if (hitInfo.transform.gameObject == testObject)
				{
					return true;
				}
			}
			return false;
		}
	}
}