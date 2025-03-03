// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Transforms a Position from a Game Object's local space to world space.")]
	public class TransformPoint : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Game Object that defines local space.")]
        public FsmOwnerDefault gameObject;
		[RequiredField]
        [Tooltip("A local position vector.")]
        public FsmVector3 localPosition;
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the transformed position, now in world space, in a Vector3 Variable.")]
        public FsmVector3 storeResult;
        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			localPosition = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoTransformPoint();
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
			DoTransformPoint();
		}
		
		void DoTransformPoint()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if(go == null) return;
			
			storeResult.Value = go.transform.TransformPoint(localPosition.Value);
		}
	}
}

