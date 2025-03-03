// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Float")]
	public class EaseFloat : EaseFsmAction
	{
		[RequiredField]
        [Tooltip("The float value to ease from.")]
		public FsmFloat fromValue;

        [RequiredField]
        [Tooltip("The float value to ease to.")]
        public FsmFloat toValue;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Float Variable.")]
        public FsmFloat floatVariable;
		
		private bool finishInNextStep = false;
		
		public override void Reset (){
			base.Reset();
			floatVariable = null;
			fromValue = null;
			toValue = null;
			finishInNextStep = false;
		}
		                   
		
		public override void OnEnter ()
		{
			base.OnEnter();
			fromFloats = new float[1];
			fromFloats[0] = fromValue.Value;
			toFloats = new float[1];
			toFloats[0] = toValue.Value;
			resultFloats = new float[1];
			finishInNextStep = false;
            floatVariable.Value = reverse.IsNone ? fromValue.Value : reverse.Value ? toValue.Value : fromValue.Value;
		}
		
		public override void OnExit (){
			base.OnExit();
		}
			
		public override void OnUpdate(){
			base.OnUpdate();
			if(!floatVariable.IsNone && isRunning){
				floatVariable.Value = resultFloats[0];
			}
			
			if(finishInNextStep){
				Finish();
				if(finishEvent != null)	Fsm.Event(finishEvent);
			}
			
			if(finishAction && !finishInNextStep){
				if(!floatVariable.IsNone){
					floatVariable.Value = reverse.IsNone ? toValue.Value : reverse.Value ? fromValue.Value : toValue.Value; 
				}
				finishInNextStep = true;
			}
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameRange(this, fromValue, toValue) + " " + easeType;
	    }
#endif
	}
}
