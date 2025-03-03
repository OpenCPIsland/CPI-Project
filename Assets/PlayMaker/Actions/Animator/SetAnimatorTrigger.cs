// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets a trigger parameter to active. Triggers are parameters that act mostly like booleans, but get reset to inactive when they are used in a transition.")]
	public class SetAnimatorTrigger : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.AnimatorTrigger)]
		[Tooltip("The trigger name")]
		public FsmString trigger;
        
		public override void Reset()
		{
			gameObject = null;
			trigger = null;
		}
		
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                cachedComponent.SetTrigger(trigger.Value);
            }

			Finish();
		}
    }
}