// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
    public abstract class PlayerInputUpdateActionBase: ComponentAction<UnityEngine.InputSystem.PlayerInput>
	{
        public enum UpdateMode
        {
            Once,
            Update,
            FixedUpdate,
        }

        [DisplayOrder(0)]
		[RequiredField]
		[CheckForComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
		[Tooltip("The GameObject with the PlayerInput component.")]
		public FsmOwnerDefault gameObject;

        [DisplayOrder(1)]
        [RequiredField]
        [ObjectType(typeof(UnityEngine.InputSystem.InputActionReference))]
        [Tooltip("An InputAction used by the PlayerInput component.")]
        public FsmObject inputAction;

        [Tooltip("When to read the Input.")]
        public UpdateMode updateMode;

        protected UnityEngine.InputSystem.PlayerInput playerInput;
        protected UnityEngine.InputSystem.InputAction action;

        public override void Reset()
		{
			gameObject = null;
            updateMode = UpdateMode.Update;
            inputAction = null;
            action = null;
        }

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = updateMode == UpdateMode.FixedUpdate;
        }

        protected bool UpdateCache()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return false;
            }

            var reference = inputAction.Value as UnityEngine.InputSystem.InputActionReference;
            if (reference == null)
            {
                return false;
            }

            if (playerInput != cachedComponent)
            {
                playerInput = cachedComponent;

                action = playerInput.actions.FindAction(reference.action.id);

                if (action == null)
                {
                    LogWarning("Could not find action " + reference.name);
                    return false;
                }
            }

            return true;
        }

        public override void OnEnter()
        {
            if (!UpdateCache())
            {
                Finish();
            }

            Execute();

            if (updateMode == UpdateMode.Once)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (updateMode != UpdateMode.Update) return;

            if (!UpdateCache())
            {
                Finish();
            }

            Execute();
        }

        public override void OnFixedUpdate()
        {
            if (updateMode != UpdateMode.FixedUpdate) return;

            if (!UpdateCache())
            {
                Finish();
            }

            Execute();
        }

        protected virtual void Execute(){}

        public override void OnExit()
        {
            // force reset next time we enter
            playerInput = null;
        }
    }
}

#endif