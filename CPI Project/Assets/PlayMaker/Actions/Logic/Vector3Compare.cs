// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Vector3 variables.")]
	public class Vector3Compare : FsmStateAction
	{
		[RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The first Vector3 variable.")]
		public FsmVector3 vector1;

		[RequiredField]
        [Tooltip("The second Vector3 variable.")]
		public FsmVector3 vector2;

		[RequiredField]
        [Tooltip("Tolerance for the Equal test (almost equal).")]
		public FsmFloat tolerance;

		[Tooltip("Event sent if Rect 1 equals Rect 2 (within Tolerance)")]
		public FsmEvent equal;

	    [Tooltip("Event sent if Rect 1 does not equal Rect 2 (within Tolerance)")]
	    public FsmEvent notEqual;
		
        [Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
        public bool everyFrame;

		public override void Reset()
		{
		    vector1 = null;
		    vector2 = null;   
			tolerance = 0f;
			equal = null;
		    notEqual = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCompare();
			
			if (!everyFrame)
			{
			    Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCompare();
		}

	    private void DoCompare()
		{
		    Fsm.Event(Vector3.Distance(vector1.Value, vector2.Value) > tolerance.Value ? notEqual : equal);
		}

		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(equal) &&
				FsmEvent.IsNullOrEmpty(notEqual))
				return "Action sends no events!";
			return "";
		}

        
#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, vector1, vector2);
	    }
#endif
	}
}