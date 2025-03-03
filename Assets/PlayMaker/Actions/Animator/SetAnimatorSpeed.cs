// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
	public class SetAnimatorSpeed: ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
        public FsmOwnerDefault gameObject;
		
		[Tooltip("The playback speed.")]
		public FsmFloat speed;
		
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

        public override void Reset()
		{
			gameObject = null;
			speed= null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
            DoPlaybackSpeed();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}
	
		public override void OnUpdate()
		{
			DoPlaybackSpeed();
		}
        
		void DoPlaybackSpeed()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                cachedComponent.speed = speed.Value;
            }
		}
	}
}