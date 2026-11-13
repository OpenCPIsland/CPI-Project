// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Create a dynamic transition between the current state and the destination state. " +
	         "Both states have to be on the same layer. " +
	         "Note: You cannot change the current state on a synchronized layer, " +
	         "you need to change it on the referenced layer.")]
	public class AnimatorCrossFade : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
        public FsmOwnerDefault gameObject;
		
		[Tooltip("The name of the state that will be played.")]
		public FsmString stateName;

		[Tooltip("The duration of the transition. Value is in source state normalized time.")]
		public FsmFloat transitionDuration;

		[Tooltip("Layer index containing the destination state. Leave to none to ignore")]
		public FsmInt layer;
		
		[Tooltip("Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
		public FsmFloat normalizedTime;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			gameObject = null;
			stateName = null;
			transitionDuration = 1f;
			layer = new FsmInt(){UseVariable=true};
			normalizedTime = new FsmFloat(){UseVariable=true};
		}
		
		public override void OnEnter()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            if (animator != null)
			{
				var _layer = layer.IsNone ? -1 : layer.Value;
				
				var _normalizedTime = normalizedTime.IsNone?Mathf.NegativeInfinity:normalizedTime.Value;
				
				animator.CrossFade(stateName.Value,transitionDuration.Value,_layer,_normalizedTime);
			}
			
			Finish();
			
		}
		
#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, stateName, transitionDuration);
	    }
#endif
		
	}
}