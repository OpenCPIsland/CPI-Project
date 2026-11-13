// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
	[Tooltip("Enable/Disable an InputActionMap in a PlayerInput component.")]
	public class PlayerInputEnableActionMap: PlayerInputUpdateActionBase
	{
        [RequiredField]
        [Tooltip("Enable/Disable the Input Action.")]
        public FsmBool enable;

        public override void Reset()
        {
            base.Reset();
            enable = false;
            updateMode = UpdateMode.Once;
        }

        protected override void Execute()
        {
            if (action == null) return;

            if (enable.Value)
            {
                 action.actionMap.Enable();
            }
            else
            {
                action.actionMap.Disable();
            }
        }
    }
}

#endif