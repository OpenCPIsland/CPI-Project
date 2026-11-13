// (c) Copyright HutongGames. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Sets the value of a Color Variable.")]
	public class SetColorValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The Color Variable to set.")]
		public FsmColor colorVariable;

		[RequiredField]
        [Tooltip("The color to set the variable to.")]
        public FsmColor color;

	    [Tooltip("Repeat every frame.")]
	    public bool everyFrame;

		public override void Reset()
		{
			colorVariable = null;
			color = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetColorValue();
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
			DoSetColorValue();
		}
		
		void DoSetColorValue()
		{
			if (colorVariable != null)
				colorVariable.Value = color.Value;
		}
        
#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameSetVar("SetColor", colorVariable, color);
	    }
#endif
	}
}