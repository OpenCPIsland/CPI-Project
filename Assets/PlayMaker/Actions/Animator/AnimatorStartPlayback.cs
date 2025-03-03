// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the animator in playback mode.")]
	public class AnimatorStartPlayback : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
        public FsmOwnerDefault gameObject;
		
		public override void Reset()
		{
			gameObject = null;
		}
		
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                cachedComponent.StartPlayback();
            }
			
			Finish();
        }
	}
}