// (c) Copyright HutongGames. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the value of a Game Object Variable.")]
	public class SetGameObject : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The GameObject Variable to set.")]
		public FsmGameObject variable;

        // Note: NOT a required field since can set to null
        [Tooltip("Set the variable value. NOTE: leave empty to set to null.")]
		public FsmGameObject gameObject;

	    [Tooltip("Repeat every frame.")]
	    public bool everyFrame;

		public override void Reset()
		{
			variable = null;
			gameObject = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			variable.Value = gameObject.Value;
			
			if (!everyFrame)
			{
				Finish();		
			}
		}

		public override void OnUpdate()
		{
			variable.Value = gameObject.Value;
		}
                
#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameSetVar("SetGameObject", variable, gameObject);
	    }
#endif
	}
}