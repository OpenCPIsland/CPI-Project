// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.
// Thanks to James Murchison for the original version of this script.

using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
    [ActionTarget(typeof(Camera),"storeGameObject")]
	[Tooltip("Gets the GameObject tagged MainCamera from the scene")]
	public class GetMainCamera : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Game Object tagged as MainCamera and in a Game Object Variable.")]
		public FsmGameObject storeGameObject;
		
		public override void Reset ()
		{
			storeGameObject = null;
		}
		
		public override void OnEnter ()
		{
			storeGameObject.Value = Camera.main != null ? Camera.main.gameObject : null;
			
			Finish();
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, storeGameObject);
	    }
#endif

	}
}