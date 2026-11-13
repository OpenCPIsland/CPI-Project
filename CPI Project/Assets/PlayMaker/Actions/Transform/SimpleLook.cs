// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	/// <summary>
	/// Based on Unity's builtin MouseLook behaviour.
	/// TODO: Expose invert Y option.
	/// </summary>
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a GameObject based on a Vector2 input, typically from a PlayerInput action. " +
             "Use it on a player GameObject for MouseLook type behaviour. " +
             "It is common to setup the camera as a child of the 'body', so the body rotates left/right while the camera tilts up/down." +
             "Minimum and Maximum values can be used to constrain the rotation.")]
	public class SimpleLook : ComponentAction<Transform>
	{
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Camera is often the child of the GameObject 'body'. " +
                 "If you specify a Camera, it will tilt up down, while the body rotates left/right. " +
                 "If you leave this empty, all rotations will be applied to the main GameObject.")]
        public new FsmGameObject camera;

		[RequiredField]
		[Tooltip("Vector2 input, typically from a PlayerInput action.")]
        public FsmVector2 vector2Input;

		[RequiredField]
		[Tooltip("Sensitivity of movement in X direction (rotate left/right).")]
		public FsmFloat sensitivityX;

		[RequiredField]
		[Tooltip("Sensitivity of movement in Y direction (tilt up/down).")]
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

        private Transform cachedCameraTransform;

        public override void Reset()
		{
			gameObject = null;
            vector2Input = new FsmVector2 {UseVariable = true};
			sensitivityX = 1;
			sensitivityY = 1;
			minimumX = new FsmFloat {UseVariable = true};
            maximumX = new FsmFloat { UseVariable = true };
			minimumY = -60f;
			maximumY = 60f;
			everyFrame = true;
		}

		public override void OnEnter()
		{
            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)) ||
                vector2Input.IsNone)
            {
				Finish();
				return;
			}

            if (camera.Value != null)
            {
                cachedCameraTransform = camera.Value.transform;
            }

			// Make the rigid body not change rotation
			// TODO: Original Unity script had this. Expose as option?
		    var myRigidbody = cachedGameObject.GetComponent<Rigidbody>();
            if (myRigidbody != null)
			{
                myRigidbody.freezeRotation = true;
			}

            // initialize rotation

            var localRotation = cachedTransform.localRotation;
            rotationX = localRotation.eulerAngles.y;
            rotationY = cachedCameraTransform == null ? 
                localRotation.eulerAngles.x :
                cachedCameraTransform.localRotation.eulerAngles.x ;

            if (!everyFrame)
			{
                DoLookRotate();
                Finish();
			}
        }

		public override void OnUpdate()
		{
			DoLookRotate();
		}

        private void DoLookRotate()
		{
            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            if (cachedCameraTransform == null)
            {
                cachedTransform.localEulerAngles = new Vector3(GetYRotation(), GetXRotation(), 0);
            }
            else
            {
                cachedTransform.localEulerAngles = new Vector3(cachedTransform.localEulerAngles.x, GetXRotation(), 0);
                cachedCameraTransform.localEulerAngles = new Vector3(GetYRotation(-1), cachedCameraTransform.localEulerAngles.y, 0);
            }
        }

        private float GetXRotation()
        {
            rotationX += vector2Input.Value.x * sensitivityX.Value;
            rotationX = ClampAngle(rotationX, minimumX, maximumX) % 360;
			return rotationX;
		}

        private float GetYRotation(float invert = 1)
        {
            rotationY += vector2Input.Value.y * invert * sensitivityY.Value;
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