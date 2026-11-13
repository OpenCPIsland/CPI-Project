// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Transforms position from world space to a Game Object's local space. The opposite of TransformPoint.")]
	public class InverseTransformPoint : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The game object that defines local space.")]
        public FsmOwnerDefault gameObject;

		[RequiredField]
        [Tooltip("The world position vector.")]
        public FsmVector3 worldPosition;

		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the transformed vector in a Vector3 Variable.")]
        public FsmVector3 storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			worldPosition = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoInverseTransformPoint();
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
			DoInverseTransformPoint();
		}
		
		void DoInverseTransformPoint()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if(go == null) return;
			
			storeResult.Value = go.transform.InverseTransformPoint(worldPosition.Value);
		}
	}
}

