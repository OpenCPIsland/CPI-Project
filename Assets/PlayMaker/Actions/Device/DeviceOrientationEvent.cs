// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// NOTE: The new Input System and legacy Input Manager can both be enabled in a project.
// This action can only run if the legacy Input Manager is enabled.

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
#if NEW_INPUT_SYSTEM_ONLY
    [Obsolete("This action has no equivalent in the new Input System.")]
#endif
    [ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends an Event based on the Orientation of the mobile device.")]
	public class DeviceOrientationEvent : FsmStateAction
	{
		[Tooltip("Note: If device is physically situated between discrete positions, as when (for example) rotated diagonally, system will report Unknown orientation.")]
		public DeviceOrientation orientation;

		[Tooltip("The event to send if the device orientation matches Orientation.")]
		public FsmEvent sendEvent;

		[Tooltip("Repeat every frame. Useful if you want to wait for the orientation to be true.")]
		public bool everyFrame;
		
		public override void Reset()
		{
			orientation = DeviceOrientation.Portrait;
			sendEvent = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoDetectDeviceOrientation();
			
			if (!everyFrame)
			{
			    Finish();
			}
		}

        public override void OnUpdate()
		{
			DoDetectDeviceOrientation();
		}

        private void DoDetectDeviceOrientation()
		{
#if !NEW_INPUT_SYSTEM_ONLY
			if (Input.deviceOrientation == orientation)
			{
			    Fsm.Event(sendEvent);
			}
#endif
        }

#if NEW_INPUT_SYSTEM_ONLY

        public override string ErrorCheck()
        {
            return "This action is not supported in the new Input System.";
        }
#endif

    }
}