// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
	public class GetAnimatorFeetPivotActive : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The feet pivot Blending. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
		public FsmFloat feetPivotActive;
		
		
		public override void Reset()
		{
			gameObject = null;
			feetPivotActive = null;
		}
		
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                feetPivotActive.Value = cachedComponent.feetPivotActive;
            }
			
			Finish();
		}
	}
}