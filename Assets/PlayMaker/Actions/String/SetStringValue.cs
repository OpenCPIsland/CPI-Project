// (c) Copyright HutongGames. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Sets the value of a String Variable.")]
	public class SetStringValue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The String Variable to set.")]
        public FsmString stringVariable;

        [UIHint(UIHint.TextArea)]
        [Tooltip("The value to set the variable to.")]
        public FsmString stringValue;
		
	    [Tooltip("Repeat every frame.")]
	    public bool everyFrame;

		public override void Reset()
		{
			stringVariable = null;
			stringValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetStringValue();
			
			if (!everyFrame)
				Finish();
		}

		public override void OnUpdate()
		{
			DoSetStringValue();
		}
		
		void DoSetStringValue()
		{
			if (stringVariable == null) return;
			if (stringValue == null) return;
			
			stringVariable.Value = stringValue.Value;
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameSetVar("SetString", stringVariable, stringValue);
	    }
#endif
		
	}
}