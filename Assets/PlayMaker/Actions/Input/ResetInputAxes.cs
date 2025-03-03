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
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Resets all Input. After ResetInputAxes all axes return to 0 and all buttons return to 0 for one frame")]
	public class ResetInputAxes : FsmStateAction
	{
		public override void Reset(){}
		
		public override void OnEnter()
		{
#if !NEW_INPUT_SYSTEM_ONLY
            Input.ResetInputAxes();
#endif
            Finish();
		}

#if NEW_INPUT_SYSTEM_ONLY
        public override string ErrorCheck()
        {
            return "This action has no equivalent in the new Input System.";
        }
#endif
    }
}