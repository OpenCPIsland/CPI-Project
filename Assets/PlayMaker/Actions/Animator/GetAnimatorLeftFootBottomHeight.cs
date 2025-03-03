// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Get the left foot bottom height.")]
	public class GetAnimatorLeftFootBottomHeight : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Result")]

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("the left foot bottom height.")]
		public FsmFloat leftFootHeight;
		
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

        public override void Reset()
		{
			gameObject = null;
			leftFootHeight = null;
			everyFrame = false;
		}

        public override void OnPreprocess()
        {
            Fsm.HandleLateUpdate = true;
        }

		public override void OnEnter()
		{
			GetLeftFootBottomHeight();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}
		
		public override void OnLateUpdate()
		{
			GetLeftFootBottomHeight();
		}
		
		private void GetLeftFootBottomHeight()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                leftFootHeight.Value = cachedComponent.leftFeetBottomHeight;
            }
        }
	}
}