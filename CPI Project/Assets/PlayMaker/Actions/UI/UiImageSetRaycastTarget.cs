// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the raycast target of a UI Image component.")]
	public class UiImageSetRaycastTarget : ComponentAction<UnityEngine.UI.Image>
	{
		[RequiredField]
		[CheckForComponent(typeof(UnityEngine.UI.Image))]
		[Tooltip("The GameObject with the Image UI component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The raycast target value to be set")]
		public FsmBool raycastTarget;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

	    private bool originalBool;

		public override void Reset()
		{
			gameObject = null;
            raycastTarget = null;
            resetOnExit = false;
		}
		
		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
                originalBool = cachedComponent.raycastTarget;
                DoSetRaycastTarget();
            }
			Finish();
		}

	    private void DoSetRaycastTarget()
		{
            cachedComponent.raycastTarget = raycastTarget.Value;
		}

		public override void OnExit()
		{
			if (resetOnExit.Value)
			{
                cachedComponent.raycastTarget = originalBool;
			}
		}
		
	}
}