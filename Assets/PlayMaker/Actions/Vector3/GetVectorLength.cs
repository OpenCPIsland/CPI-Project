// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Get Vector3 Length.")]
	public class GetVectorLength : FsmStateAction
	{
        [Tooltip("The Vector3")]
		public FsmVector3 vector3;

        [RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the length (magnitude) of the Vector3 value in a float variable.")]
		public FsmFloat storeLength;
		
        [Tooltip("Repeat every frame")]
        public bool everyFrame;
        		
		public override void Reset()
		{
			vector3 = null;
			storeLength = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoVectorLength();
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoVectorLength();
		}
		
		void DoVectorLength()
		{
			if (vector3 == null) return;
			if (storeLength == null) return;
			storeLength.Value = vector3.Value.magnitude;
		}
	}
}