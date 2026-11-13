// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Multiplies a Vector3 variable by a Float.")]
	public class Vector3Multiply : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The vector3 variable to multiply.")]
        public FsmVector3 vector3Variable;

		[RequiredField]
        [Tooltip("The float to multiply each axis of the Vector3 variable by.")]
        public FsmFloat multiplyBy;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

		public override void Reset()
		{
			vector3Variable = null;
			multiplyBy = 1;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			vector3Variable.Value = vector3Variable.Value * multiplyBy.Value;
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
			vector3Variable.Value = vector3Variable.Value * multiplyBy.Value;
		}
	}
}

