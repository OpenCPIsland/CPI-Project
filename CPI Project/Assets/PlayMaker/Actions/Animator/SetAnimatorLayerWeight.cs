// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the layer's current weight")]
	public class SetAnimatorLayerWeight: ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
        public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;
		
		[RequiredField]
		[Tooltip("Sets the layer's current weight")]
		public FsmFloat layerWeight;
		
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

        public override void Reset()
		{
			gameObject = null;
			layerIndex = null;
			layerWeight= null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoLayerWeight();
			
			if (!everyFrame) 
			{
				Finish();
			}
		}
	
		public override void OnUpdate()
		{
			DoLayerWeight();
		}


        private void DoLayerWeight()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                cachedComponent.SetLayerWeight(layerIndex.Value, layerWeight.Value);
            }
        }
		
	}
}