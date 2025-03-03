// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the value of ApplyRootMotion of an avatar. If true, root is controlled by animations")]
	public class GetAnimatorApplyRootMotion : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[ActionSection("Results")]
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Is the rootMotionapplied. If true, root is controlled by animations")]
		public FsmBool rootMotionApplied;
		
		[Tooltip("Event send if the root motion is applied")]
		public FsmEvent rootMotionIsAppliedEvent;
		
		[Tooltip("Event send if the root motion is not applied")]
		public FsmEvent rootMotionIsNotAppliedEvent;

        public override void Reset()
		{
			gameObject = null;
			rootMotionApplied = null;
			rootMotionIsAppliedEvent = null;
			rootMotionIsNotAppliedEvent = null;
		}
        
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                var _applyRootMotion = cachedComponent.applyRootMotion;
                rootMotionApplied.Value = _applyRootMotion;
                Fsm.Event(_applyRootMotion ? rootMotionIsAppliedEvent : rootMotionIsNotAppliedEvent);
            }

            Finish();
        }
    }
}