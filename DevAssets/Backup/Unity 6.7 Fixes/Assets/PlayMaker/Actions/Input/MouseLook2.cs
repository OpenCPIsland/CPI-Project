// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
// This action was developed for the old input manager, so we will use it if its available. 
// If only the new input system is available we will try to use that instead,
// but there might be subtle differences in the behaviour in the new system!

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using System;
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
	[Obsolete("Use MouseLook instead.")]
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("NOTE: This is a duplicate action and will be removed in a future update. Please use Mouse Look instead." +
             "\nRotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
	public class MouseLook2 : ComponentAction<Rigidbody>
	{
		public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }

		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The axes to rotate around.")]
		public RotationAxes axes = RotationAxes.MouseXAndY;

		[RequiredField]
        [Tooltip("Speed around X axis. Higher = faster.")]
		public FsmFloat sensitivityX;

		[RequiredField]
        [Tooltip("Speed around Y axis. Higher = faster.")]
        public FsmFloat sensitivityY;

		[RequiredField]
		[HasFloatSlider(-360,360)]
        [Tooltip("Minimum angle around X axis.")]
		public FsmFloat minimumX;

		[RequiredField]
		[HasFloatSlider(-360, 360)]
        [Tooltip("Maximum angle around X axis.")]
        public FsmFloat maximumX;

		[RequiredField]
		[HasFloatSlider(-360, 360)]
        [Tooltip("Minimum angle around Y axis.")]
        public FsmFloat minimumY;

		[RequiredField]
		[HasFloatSlider(-360, 360)]
        [Tooltip("Maximum angle around X axis.")]
        public FsmFloat maximumY;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		float rotationX;
		float rotationY;

		public override void Reset()
		{
			gameObject = null;
			axes = RotationAxes.MouseXAndY;
			sensitivityX = 15f;
			sensitivityY = 15f;
			minimumX = -360f;
			maximumX = 360f;
			minimumY = -60f;
			maximumY = 60f;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				Finish();
				return;
			}

            // Make the rigid body not change rotation			
            // TODO: Original Unity script had this. Expose as option?
            if (!UpdateCache(go))
            {
                if (rigidbody)
                {
                    rigidbody.freezeRotation = true;
                }
            }

			DoMouseLook();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoMouseLook();
		}

		void DoMouseLook()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}

			var transform = go.transform;

			switch (axes)
			{
				case RotationAxes.MouseXAndY:
					
					transform.localEulerAngles = new Vector3(GetYRotation(), GetXRotation(), 0);
					break;
				
				case RotationAxes.MouseX:

					transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, GetXRotation(), 0);
					break;

				case RotationAxes.MouseY:

					transform.localEulerAngles = new Vector3(-GetYRotation(), transform.localEulerAngles.y, 0);
					break;
			}
		}

		float GetXRotation()
		{
#if NEW_INPUT_SYSTEM_ONLY
            if (Mouse.current == null) return rotationX;
			// fudge factor accounts for sensitivity of old input system
            rotationX += Mouse.current.delta.ReadValue().x * sensitivityY.Value * 0.05f; 
#else
            rotationX += Input.GetAxis("Mouse X") * sensitivityX.Value;
#endif
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
			return rotationX;
		}

		float GetYRotation()
		{
#if NEW_INPUT_SYSTEM_ONLY
            if (Mouse.current == null) return rotationY;
            // fudge factor accounts for sensitivity of old input system
            rotationY += Mouse.current.delta.ReadValue().y * sensitivityY.Value * -0.05f; 
#else
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY.Value;
#endif
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
			return rotationY;
		}

		// Clamp function that respects IsNone
		static float ClampAngle(float angle, FsmFloat min, FsmFloat max)
		{
			if (!min.IsNone && angle < min.Value)
			{
				angle = min.Value;
			}

			if (!max.IsNone && angle > max.Value)
			{
				angle = max.Value;
			}
			
			return angle;
		}
	}
}