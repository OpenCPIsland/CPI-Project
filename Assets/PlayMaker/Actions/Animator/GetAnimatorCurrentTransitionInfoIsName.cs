// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Check the active Transition name on a specified layer. Format is 'CURRENT_STATE -> NEXT_STATE'.")]
	public class GetAnimatorCurrentTransitionInfoIsName : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;
		
		[Tooltip("The name to check the transition against.")]
		public FsmString name;

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
			
			name = null;
			
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

            var _info = animator.GetAnimatorTransitionInfo(layerIndex.Value);
				
            if (_info.IsName(name.Value))
            {
                nameMatch.Value = true;
                Fsm.Event(nameMatchEvent);
            }
            else
            {
                nameMatch.Value = false;
                Fsm.Event(nameDoNotMatchEvent);
            }
        }
	}
}