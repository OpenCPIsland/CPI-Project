// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

// The new Input System optionally supports the legacy input manager 
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
    [Obsolete("This action has no equivalent in the new Input System.")]
#endif
    [ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets Location Info from a mobile device. NOTE: Use StartLocationService before trying to get location info.")]
	public class GetLocationInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the location in a Vector3 Variable.")]
		public FsmVector3 vectorPosition;		
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Longitude in a Float Variable.")]
		public FsmFloat longitude;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Latitude in a Float Variable.")]
		public FsmFloat latitude;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Altitude in a Float Variable.")]
		public FsmFloat altitude;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the horizontal accuracy of the location.")]
		public FsmFloat horizontalAccuracy;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the vertical accuracy of the location.")]
		public FsmFloat verticalAccuracy;
		// TODO: figure out useful way to expose timeStamp
		// maybe how old is the location...?
		//[UIHint(UIHint.Variable)]
		//[Tooltip("Timestamp (in seconds since the game started) when location was last updated.")]
		//public FsmFloat timeStamp;
		[Tooltip("Event to send if the location cannot be queried.")]
		public FsmEvent errorEvent;

		public override void Reset()
		{
			longitude = null;
			latitude = null;
			altitude = null;
			horizontalAccuracy = null;
			verticalAccuracy = null;
			//timeStamp = null;
			errorEvent = null;
		}

		public override void OnEnter()
		{
			DoGetLocationInfo();

			Finish();
		}

        private void DoGetLocationInfo()
        {
#if !NEW_INPUT_SYSTEM_ONLY
#if UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8

			if (Input.location.status != LocationServiceStatus.Running)
			{
				Fsm.Event(errorEvent);
				return;
			}
			
			float x = Input.location.lastData.longitude;
			float y = Input.location.lastData.latitude;
			float z = Input.location.lastData.altitude;
			
			vectorPosition.Value = new Vector3(x,y,z);
			
			longitude.Value = x;
			latitude.Value = y;
			altitude.Value = z;

			horizontalAccuracy.Value = Input.location.lastData.horizontalAccuracy;
			verticalAccuracy.Value = Input.location.lastData.verticalAccuracy;
			
#endif
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