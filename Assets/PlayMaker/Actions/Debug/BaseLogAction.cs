// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    // Base class for logging actions 
    public abstract class BaseLogAction : FsmStateAction
    {
        [Tooltip("Also send to the Unity Log.")]
        public bool sendToUnityLog;

        public override void Reset()
        {
            sendToUnityLog = false;
        }
    }
}
