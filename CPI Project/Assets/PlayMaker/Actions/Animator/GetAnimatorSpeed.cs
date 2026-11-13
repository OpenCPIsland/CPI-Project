// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the playback speed of the Animator. 1 is normal playback speed")]
	public class GetAnimatorSpeed : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The playBack speed of the animator. 1 is normal playback speed")]
		public FsmFloat speed;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			base.Reset();

			gameObject = null;
			speed = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetPlaybackSpeed();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}
	
		public override void OnActionUpdate()
		{
			GetPlaybackSpeed();
		}

        private void GetPlaybackSpeed()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            speed.Value = animator.speed;
		}
	}
}