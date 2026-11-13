// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the value of an int parameter.")]
	public class GetAnimatorInt : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorInt)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;
        
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The int value of the animator parameter")]
		public FsmInt result;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        private string cachedParameter;
        private int paramID;

        public override void Reset()
		{
			base.Reset();
			gameObject = null;
			parameter = null;
			result = null;
		}

        // Code that runs on entering the state.
		public override void OnEnter()
		{
            GetParameter();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}	
		
		public override void OnActionUpdate() 
		{
			GetParameter();
		}

        private void GetParameter()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            if (cachedParameter != parameter.Value)
            {
                cachedParameter = parameter.Value;
                paramID = Animator.StringToHash(parameter.Value);
            }

            result.Value = animator.GetInteger(paramID);
        }

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoNameGetProperty(this, parameter, result);
	    }
#endif

	}
}