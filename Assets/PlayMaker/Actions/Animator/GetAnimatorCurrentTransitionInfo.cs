// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the current transition information on a specified layer. Only valid when during a transition.")]
	public class GetAnimatorCurrentTransitionInfo : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;
		
		[ActionSection("Results")]
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The unique name of the Transition")]
		public FsmString name;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The unique name of the Transition")]
		public FsmInt nameHash;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The user-specified name of the Transition")]
		public FsmInt userNameHash;

		[UIHint(UIHint.Variable)]
		[Tooltip("Normalized time of the Transition")]
		public FsmFloat normalizedTime;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			base.Reset();

			gameObject = null;
			layerIndex = null;
			
			name = null;
			nameHash = null;
			userNameHash = null;
			normalizedTime = null;
			
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
            GetTransitionInfo();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}
		
		public override void OnActionUpdate()
		{
            GetTransitionInfo();
		}

        private void GetTransitionInfo()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            var _info = animator.GetAnimatorTransitionInfo(layerIndex.Value);

            if (!name.IsNone)
            {
                name.Value = animator.GetLayerName(layerIndex.Value);	
            }

            if (!nameHash.IsNone)
            {
                nameHash.Value = _info.nameHash;
            }

            if (!userNameHash.IsNone)
            {
                userNameHash.Value = _info.userNameHash;
            }

            if (!normalizedTime.IsNone)
            {
                normalizedTime.Value = _info.normalizedTime;
            }
        }
			
	}
}