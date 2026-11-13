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
    [Obsolete("This action is not supported in the new Input System. " +
              "Use PlayerInputGetButtonValues or GamepadGetButtonValues instead.")]
#endif
    [ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the pressed state of the specified Button and stores it in a Bool Variable. See Unity Input Manager docs.")]
	public class GetButton : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The name of the button. Set in the Unity Input Manager.")]
        public FsmString buttonName;		

		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the button state in a Bool Variable.")]
		public FsmBool storeResult;

        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			buttonName = "Fire1";
			storeResult = null;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			DoGetButton();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetButton();
		}

        private void DoGetButton()
		{
#if !NEW_INPUT_SYSTEM_ONLY
			storeResult.Value = Input.GetButton(buttonName.Value);
#endif
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, buttonName);
        }

#if NEW_INPUT_SYSTEM_ONLY

        public override string ErrorCheck()
        {
            return "This action is not supported in the new Input System. " +
                   "Use PlayerInputGetButtonValues or GamepadGetButtonValues instead.";
        }
#endif

#endif
    }
}

