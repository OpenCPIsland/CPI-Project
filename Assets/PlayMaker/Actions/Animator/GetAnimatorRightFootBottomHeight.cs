// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Get the right foot bottom height.")]
	public class GetAnimatorRightFootBottomHeight : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[ActionSection("Result")]
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The right foot bottom height.")]
		public FsmFloat rightFootHeight;

		[Tooltip("Repeat every frame during LateUpdate. Useful when value is subject to change over time.")]
		public bool everyFrame;

		public override void Reset()
		{
			base.Reset();

			gameObject = null;
			rightFootHeight = null;
			everyFrame = false;
		}

        public override void OnPreprocess()
        {
            Fsm.HandleLateUpdate = true;
        }

		public override void OnEnter()
		{
            GetRightFootBottomHeight();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}
		
		public override void OnLateUpdate()
		{
			GetRightFootBottomHeight();
		}

        private void GetRightFootBottomHeight()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                rightFootHeight.Value = cachedComponent.rightFeetBottomHeight;
            }
        }
	}
}