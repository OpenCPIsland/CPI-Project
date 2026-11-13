// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Check the current State name on a specified layer, this is more than the layer name, it holds the current state as well.")]
	public class GetAnimatorCurrentStateInfoIsName : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;
		
		[Tooltip("The name to check the layer against.")]
		public FsmString name;
		
		[ActionSection("Results")]

		[UIHint(UIHint.Variable)]
		[Tooltip("True if name matches")]
		public FsmBool isMatching;

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
			
			nameMatchEvent = null;
			nameDoNotMatchEvent = null;
			
			everyFrame = false;
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

            var info = animator.GetCurrentAnimatorStateInfo(layerIndex.Value);

            if (!isMatching.IsNone)
            {
                isMatching.Value = info.IsName(name.Value);
            }

            Fsm.Event(info.IsName(name.Value) ? nameMatchEvent : nameDoNotMatchEvent);
        }
	}
}