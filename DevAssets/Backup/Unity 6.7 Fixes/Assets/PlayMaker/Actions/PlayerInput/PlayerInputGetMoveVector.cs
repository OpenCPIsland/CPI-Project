// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
	[Tooltip("Gets a world direction Vector from an InputAction in a PlayerInput component. " +
             "Typically used for a third person controller with Relative To set to the camera. " +
             "This works like the Get Axis Vector action for the old Input System.")]
	public class PlayerInputGetMoveVector : PlayerInputUpdateActionBase
    {
        public enum AxisPlane
        {
            XZ,
            XY,
            YZ
        }

        [RequiredField]
        [Tooltip("Sets the world axis the input maps to. The remaining axis will be set to zero.")]
        public AxisPlane mapToPlane;

        [Tooltip("Calculate a vector relative to this game object. Typically the camera.")]
        public FsmGameObject relativeTo;

        [Tooltip("Normally axis values are in the range -1 to 1. Use the multiplier to make this range bigger. " +
                 "\nE.g., A multiplier of 100 returns values from -100 to 100.\nTypically this represents the maximum movement speed.")]
        public FsmFloat multiplier;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the magnitude of the vector. Useful if you want to measure the strength of the input and react accordingly. " +
                 "Hint: Use {{Float Compare}}.")]
        public FsmFloat storeMagnitude;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the move vector in a Vector3 variable.")]
        public FsmVector3 storeMoveVector;

        public override void Reset()
		{
			base.Reset();

            multiplier = 1.0f;
            mapToPlane = AxisPlane.XZ;
            relativeTo = null;
            storeMagnitude = null;
            storeMoveVector = null;
        }

        // Note we're updating by polling instead of using callbacks.
        // This is because this action is in a stack with other actions
        // updated in order, from top to bottom.
        // If this action was updated out of sync with the other actions,
        // it could lead to unexpected behaviour that would be hard to debug.

        protected override void Execute()
		{
            if (action == null) return;

            var v2 = action.ReadValue<Vector2>();

            var forward = new Vector3();
            var right = new Vector3();

            if (relativeTo.Value == null)
            {
                switch (mapToPlane)
                {
                    case AxisPlane.XZ:
                        forward = Vector3.forward;
                        right = Vector3.right;
                        break;

                    case AxisPlane.XY:
                        forward = Vector3.up;
                        right = Vector3.right;
                        break;

                    case AxisPlane.YZ:
                        forward = Vector3.up;
                        right = Vector3.forward;
                        break;
                }
            }
            else
            {
                var transform = relativeTo.Value.transform;

                switch (mapToPlane)
                {
                    case AxisPlane.XZ:
                        forward = transform.TransformDirection(Vector3.forward);
                        forward.y = 0;
                        forward = forward.normalized;
                        right = new Vector3(forward.z, 0, -forward.x);
                        break;

                    case AxisPlane.XY:
                    case AxisPlane.YZ:
                        // NOTE: in relative mode XY ans YZ are the same!
                        forward = Vector3.up;
                        forward.z = 0;
                        forward = forward.normalized;
                        right = transform.TransformDirection(Vector3.right);
                        break;
                }

                // Right vector relative to the object
                // Always orthogonal to the forward vector

            }

            // calculate resulting direction vector

            var direction = v2.x * right + v2.y * forward;
            direction *= multiplier.Value;

            storeMoveVector.Value = direction;

            if (!storeMagnitude.IsNone)
            {
                storeMagnitude.Value = direction.magnitude;
            }

        }
    }
}

#endif