// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets Random Rotation for a Game Object. Uncheck an axis to keep the current rotation around that axis.")]
	public class SetRandomRotation : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Game Object to randomly rotate.")]
        public FsmOwnerDefault gameObject;
		[RequiredField]
        [Tooltip("Use X axis.")]
        public FsmBool x;
		[RequiredField]
        [Tooltip("Use Y axis.")]
        public FsmBool y;
		[RequiredField]
        [Tooltip("Use Z axis.")]
        public FsmBool z;

		public override void Reset()
		{
			gameObject = null;
			x = true;
			y = true;
			z = true;
		}

		public override void OnEnter()
		{
			DoRandomRotation();
			
			Finish();		
		}

		void DoRandomRotation()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;
			
			Vector3 rotation = go.transform.localEulerAngles;

			float xAngle = rotation.x;
			float yAngle = rotation.y;
			float zAngle = rotation.z;
			
			if (x.Value) xAngle = Random.Range(0,360);
			if (y.Value) yAngle = Random.Range(0,360);
			if (z.Value) zAngle = Random.Range(0,360);
				
			go.transform.localEulerAngles = new Vector3(xAngle, yAngle, zAngle);
		}


	}
}