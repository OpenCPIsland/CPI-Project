// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Rotation of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetRotation : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Game Object.")]
        public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
        [Tooltip("Get the rotation as a Quaternion.")]
		public FsmQuaternion quaternion;

        [UIHint(UIHint.Variable)]
		[Title("Euler Angles")]
        [Tooltip("Get the rotation as Euler angles (rotation around each axis) and store in a Vector3 Variable.")]
		public FsmVector3 vector;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the angle around the X axis.")]
		public FsmFloat xAngle;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the angle around the Y axis.")]
		public FsmFloat yAngle;

        [UIHint(UIHint.Variable)]
        [Tooltip("Get the angle around the Z axis.")]
		public FsmFloat zAngle;

        [Tooltip("The coordinate space to get the rotation in.")]
		public Space space;

        [Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			quaternion = null;
			vector = null;
			xAngle = null;
			yAngle = null;
			zAngle = null;
			space = Space.World;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetRotation();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			DoGetRotation();
		}

		void DoGetRotation()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}

			if (space == Space.World)
			{
				quaternion.Value = go.transform.rotation;

				var rotation = go.transform.eulerAngles;
				
				vector.Value = rotation;
				xAngle.Value = rotation.x;
				yAngle.Value = rotation.y;
				zAngle.Value = rotation.z;
			}
			else
			{
				var rotation = go.transform.localEulerAngles;

				quaternion.Value = Quaternion.Euler(rotation);
				
				vector.Value = rotation;
				xAngle.Value = rotation.x;
				yAngle.Value = rotation.y;
				zAngle.Value = rotation.z;
			}
		}

#if UNITY_EDITOR

        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, Fsm, gameObject);
        }

#endif
	}
}