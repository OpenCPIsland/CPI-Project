// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the value of an integer parameter")]
	public class SetAnimatorInt : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorInt)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;
		
		[Tooltip("The Int value to assign to the animator parameter")]
		public FsmInt Value;

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
			Value = null;
		}

		public override void OnEnter()
		{
            SetParameter();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}
	
		public override void OnActionUpdate ()
		{
			SetParameter();
		}

        private void SetParameter()
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

            animator.SetInteger(paramID,Value.Value) ;			
		}
	}
}