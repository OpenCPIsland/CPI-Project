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
	[Tooltip("Perform a Mouse Pick on a 2d scene and stores the results. Use Ray Distance to set how close the camera must be to pick the 2d object.")]
	public class MousePick2d : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
        [Tooltip("Store if a GameObject was picked in a Bool variable. True if a GameObject was picked, otherwise false.")]
		public FsmBool storeDidPickObject;
		
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the picked GameObject in a variable.")]
		public FsmGameObject storeGameObject;
		
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the picked point in a variable.")]
		public FsmVector2 storePoint;
		
        [UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;
		
        [Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;
		
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;
		
		public override void Reset()
		{
			storeDidPickObject = null;
			storeGameObject = null;
			storePoint = null;
			layerMask = new FsmInt[0];
			invertMask = false;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoMousePick2d();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnUpdate()
		{
			DoMousePick2d();
		}

        private void DoMousePick2d()
        {
#if NEW_INPUT_SYSTEM_ONLY
            if (Mouse.current == null) return;
            var mousePosition = Mouse.current.position.ReadValue();
#else
            var mousePosition = Input.mousePosition;
#endif

            var hitInfo = Physics2D.GetRayIntersection(
				Camera.main.ScreenPointToRay(mousePosition),
				Mathf.Infinity,
				ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));

			var didPick = hitInfo.collider != null;
			storeDidPickObject.Value = didPick;
			
			if (didPick)
			{
				storeGameObject.Value = hitInfo.collider.gameObject;
				storePoint.Value = hitInfo.point;
			}
			else
			{
				// TODO: not sure if this is the right strategy...
				storeGameObject.Value = null;
				storePoint.Value = Vector3.zero;
			}
			
		}
	}
}