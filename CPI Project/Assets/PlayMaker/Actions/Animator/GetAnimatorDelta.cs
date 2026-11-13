// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the avatar delta position and rotation for the last evaluated frame.")]
	public class GetAnimatorDelta: FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar delta position for the last evaluated frame")]
		public FsmVector3 deltaPosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar delta position for the last evaluated frame")]
		public FsmQuaternion deltaRotation;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			base.Reset();
			gameObject = null;
			deltaPosition= null;
			deltaRotation = null;
		}
		
		public override void OnEnter()
		{
            DoGetDeltaPosition();

            if (!everyFrame)
            {
                Finish();
            }
        }
	
		public override void OnActionUpdate() 
		{
			DoGetDeltaPosition();
		}

        private void DoGetDeltaPosition()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            deltaPosition.Value = animator.deltaPosition;
			deltaRotation.Value = animator.deltaRotation;
		}

	}
}