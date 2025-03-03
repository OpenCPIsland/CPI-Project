// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Clamps the magnitude of Vector3 variable on the XZ Plane.")]
	public class Vector3ClampMagnitudeXZ : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The Vector3 variable to clamp.")]
        public FsmVector3 vector3Variable;

		[RequiredField]
        [Tooltip("Clamp to this max length.")]
        public FsmFloat maxLength;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

		public override void Reset()
		{
			vector3Variable = null;
			maxLength = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
            DoVector3ClampMagnitudeXZ();
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
            DoVector3ClampMagnitudeXZ();
		}
		
		private void DoVector3ClampMagnitudeXZ()
		{
            var xz = Vector2.ClampMagnitude(new Vector2(vector3Variable.Value.x, vector3Variable.Value.z), maxLength.Value);
            vector3Variable.Value = new Vector3(xz.x, vector3Variable.Value.y, xz.y);
		}
	}
}

