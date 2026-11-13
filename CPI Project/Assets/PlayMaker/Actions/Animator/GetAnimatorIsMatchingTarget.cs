// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if automatic matching is active. Can also send events")]
	public class GetAnimatorIsMatchingTarget: FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results")]
		
		[UIHint(UIHint.Variable)]
		[Tooltip("True if automatic matching is active")]
		public FsmBool isMatchingActive;
		
		[Tooltip("Event send if automatic matching is active")]
		public FsmEvent matchingActivatedEvent;
		
		[Tooltip("Event send if automatic matching is not active")]
		public FsmEvent matchingDeactivedEvent;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			base.Reset();

			gameObject = null;
			isMatchingActive = null;
			matchingActivatedEvent = null;
			matchingDeactivedEvent = null;
		}
		
		public override void OnEnter()
		{
            DoCheckIsMatchingActive();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
        
		public override void OnActionUpdate() 
		{
			DoCheckIsMatchingActive();
        }

        private void DoCheckIsMatchingActive()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            var _isMatchingActive = animator.isMatchingTarget;
			isMatchingActive.Value = _isMatchingActive;

            Fsm.Event(_isMatchingActive ? matchingActivatedEvent : matchingDeactivedEvent);
        }
		
	}
}