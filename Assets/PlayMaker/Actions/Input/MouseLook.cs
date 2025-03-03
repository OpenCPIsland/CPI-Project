// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
// This action was developed for the old input manager, so we will use it if its available. 
// If only the new input system is available we will try to use that instead,
// but there might be subtle differences in the behaviour in the new system!

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using UnityEngine;

#if NEW_INPUT_SYSTEM_ONLY
using UnityEngine.InputSystem;
#endif

namespace HutongGames.PlayMaker.Actions
{
	/// <summary>
	/// Action version of Unity's builtin MouseLook behaviour.
	/// TODO: Expose invert Y option.
	/// </summary>
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Rotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
	public class MouseLook : ComponentAction<Transform>
	{
		public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }

		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The axes to rotate around.")]
		public RotationAxes axes = RotationAxes.MouseXAndY;

		[RequiredField]
		[Tooltip("Sensitivity of movement in X direction.")]
		public FsmFloat sensitivityX;

		[RequiredField]
		[Tooltip("Sensitivity of movement in Y direction.")]
		public FsmFloat sensitivityY;

		[HasFloatSlider(-360,360)]
        [Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		public FsmFloat minimumX;

		[HasFloatSlider(-360, 360)]
        [Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
        public FsmFloat maximumX;

		[HasFloatSlider(-360, 360)]
        [Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
        public FsmFloat minimumY;

		[HasFloatSlider(-360, 360)]
        [Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
        public FsmFloat maximumY;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

        private float rotationX;
        private float rotationY;

        public override void Reset()
		{
			gameObject = null;
			axes = RotationAxes.MouseXAndY;
			sensitivityX = 15f;
			sensitivityY = 15f;
			minimumX = new FsmFloat {UseVariable = true};
            maximumX = new FsmFloat { UseVariable = true };
			minimumY = -60f;
			maximumY = 60f;
			everyFrame = true;
		}

		public override void OnEnter()
		{
            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
				Finish();
				return;
			}

			// Make the rigid body not change rotation
			// TODO: Original Unity script had this. Expose as option?
		    var rigidbody = cachedGameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
			{
				rigidbody.freezeRotation = true;
			}

            // initialize rotation

            rotationX = cachedTransform.localRotation.eulerAngles.y;
            rotationY = cachedTransform.localRotation.eulerAngles.x;

            if (!everyFrame)
			{
                DoMouseLook();
                Finish();
			}
        }

		public override void OnUpdate()
		{
			DoMouseLook();
		}

        private void DoMouseLook()
		{
            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            switch (axes)
			{
				case RotationAxes.MouseXAndY:
					
					cachedTransform.localEulerAngles = new Vector3(GetYRotation(), GetXRotation(), 0);
					break;
				
				case RotationAxes.MouseX:

                    cachedTransform.localEulerAngles = new Vector3(cachedTransform.localEulerAngles.x, GetXRotation(), 0);
					break;

				case RotationAxes.MouseY:

                    cachedTransform.localEulerAngles = new Vector3(GetYRotation(-1), cachedTransform.localEulerAngles.y, 0);
					break;
			}
        }

        private float GetXRotation()
		{
#if NEW_INPUT_SYSTEM_ONLY
            if (Mouse.current == null) return rotationX;
			// fudge factor accounts for sensitivity of old input system
            rotationX += Mouse.current.delta.ReadValue().x * sensitivityY.Value * 0.05f; 
#else
            rotationX += Input.GetAxis("Mouse X") * sensitivityX.Value;
#endif
            rotationX = ClampAngle(rotationX, minimumX, maximumX) % 360;
			return rotationX;
		}

        private float GetYRotation(float invert = 1)
		{
#if NEW_INPUT_SYSTEM_ONLY
            if (Mouse.current == null) return rotationY;
            // fudge factor accounts for sensitivity of old input system
            rotationY += Mouse.current.delta.ReadValue().y * sensitivityY.Value * invert * -0.05f; 
#else
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY.Value * invert;
#endif
			rotationY = ClampAngle(rotationY, minimumY, maximumY) % 360;
			return rotationY;
		}

		// Clamp function that respects IsNone and 360 wrapping
        private static float ClampAngle(float angle, FsmFloat min, FsmFloat max)
		{
            if (angle < 0f) angle = 360 + angle;

            var from = min.IsNone ? -720 : min.Value;
            var to = max.IsNone ? 720 : max.Value;

            if (angle > 180f) return Mathf.Max(angle, 360 + from);
            return Mathf.Min(angle, to);
		}
	}
}