// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using System;

#if !NEW_INPUT_SYSTEM_ONLY
using UnityEngine;
#endif

namespace HutongGames.PlayMaker.Actions
{
#if NEW_INPUT_SYSTEM_ONLY
    [Obsolete("This action is not supported in the new Input System. " +
              "Use PlayerInputGetMoveVector or GamepadGetStickValue instead.")]
#endif
    [NoActionTargets]
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets a world direction Vector from 2 Input Axis. Typically used for a third person controller with Relative To set to the camera.")]
    [SeeAlso("Unity Input Manager")]
	public class GetAxisVector : FsmStateAction
	{
		public enum AxisPlane
		{
			XZ,
			XY,
			YZ
		}
		
		[Tooltip("The name of the horizontal input axis. See Unity Input Manager.")]
		public FsmString horizontalAxis;
		
		[Tooltip("The name of the vertical input axis. See Unity Input Manager.")]
		public FsmString verticalAxis;
		
		[Tooltip("Normally axis values are in the range -1 to 1. Use the multiplier to make this range bigger. " +
                 "\nE.g., A multiplier of 100 returns values from -100 to 100.\nTypically this represents the maximum movement speed.")]
		public FsmFloat multiplier;
		
		[RequiredField]
		[Tooltip("Sets the world axis the input maps to. The remaining axis will be set to zero.")]
		public AxisPlane mapToPlane;
		
		[Tooltip("Calculate a vector relative to this game object. Typically the camera.")]
		public FsmGameObject relativeTo;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the resulting vector. You can use this in {{Translate}} or other movement actions.")]
		public FsmVector3 storeVector;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the magnitude of the vector. Useful if you want to measure the strength of the input and react accordingly. " +
                 "Hint: Use {{Float Compare}}.")]
		public FsmFloat storeMagnitude;

		public override void Reset()
		{
			horizontalAxis = "Horizontal";
			verticalAxis = "Vertical";
			multiplier = 1.0f;
			mapToPlane = AxisPlane.XZ;
			storeVector = null;
			storeMagnitude = null;
		}

		public override void OnUpdate()
		{
#if !NEW_INPUT_SYSTEM_ONLY
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
			
			// get individual axis
			// leaving an axis blank or set to None sets it to 0

			var h = (horizontalAxis.IsNone || string.IsNullOrEmpty(horizontalAxis.Value)) ? 0f : Input.GetAxis(horizontalAxis.Value);
			var v = (verticalAxis.IsNone || string.IsNullOrEmpty(verticalAxis.Value)) ? 0f : Input.GetAxis(verticalAxis.Value);
			
			// calculate resulting direction vector

			var direction = h * right + v * forward;
			direction *= multiplier.Value;
			
			storeVector.Value = direction;
			
			if (!storeMagnitude.IsNone)
			{
				storeMagnitude.Value = direction.magnitude;
			}
#endif
		}

#if NEW_INPUT_SYSTEM_ONLY

        public override string ErrorCheck()
        {
            return "This action is not supported in the new Input System." +
                   "Use PlayerInputGetMoveVector or GamepadGetStickValue instead.";
        }
#endif
    }
}

