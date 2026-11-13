// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the value of a bool parameter")]
	public class SetAnimatorBool : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorBool)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;
		
		[Tooltip("The Bool value to assign to the animator parameter")]
		public FsmBool Value;

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
	
		public override void OnActionUpdate() 
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

            animator.SetBool(paramID, Value.Value) ;
		}

	}
}