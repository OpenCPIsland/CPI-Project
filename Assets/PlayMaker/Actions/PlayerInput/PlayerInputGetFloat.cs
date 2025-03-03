// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
	[Tooltip("Get the float value from an InputAction in a PlayerInput component.")]
	public class PlayerInputGetFloat: PlayerInputUpdateActionBase
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the Input Float value.")]
        public FsmFloat storeFloat;

        public override void Reset()
		{
			base.Reset();
            storeFloat = null;
        }

        // Note we're updating by polling instead of using callbacks.
        // This is because this action is in a stack with other actions
        // updated in order, from top to bottom.
        // If this action was updated out of sync with the other actions,
        // it could lead to unexpected behaviour that would be hard to debug.

        protected override void Execute()
        {
            if (action == null) return;

            storeFloat.Value = action.ReadValue<float>();
        }
    }
}

#endif