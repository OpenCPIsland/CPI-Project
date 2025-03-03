// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Check the active Transition user-specified name on a specified layer.")]
	public class GetAnimatorCurrentTransitionInfoIsUserName : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;
		
		[Tooltip("The user-specified name to check the transition against.")]
		public FsmString userName;

		[ActionSection("Results")]

		[UIHint(UIHint.Variable)]
		[Tooltip("True if name matches")]
		public FsmBool nameMatch;
		
		[Tooltip("Event send if name matches")]
		public FsmEvent nameMatchEvent;
		
		[Tooltip("Event send if name doesn't match")]
		public FsmEvent nameDoNotMatchEvent;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			base.Reset();

			gameObject = null;
			layerIndex = null;
			
			userName = null;
			
			nameMatch = null;
			nameMatchEvent = null;
			nameDoNotMatchEvent = null;

		}
		
		public override void OnEnter()
		{
            IsName();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnActionUpdate()
		{
			IsName();
		}

        private void IsName()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            var info = animator.GetAnimatorTransitionInfo(layerIndex.Value);

            var isMatch = info.IsUserName(userName.Value);

            if (!nameMatch.IsNone)
            {
                nameMatch.Value = isMatch;
            }

            Fsm.Event(isMatch ? nameMatchEvent : nameDoNotMatchEvent);
        }
	}
}