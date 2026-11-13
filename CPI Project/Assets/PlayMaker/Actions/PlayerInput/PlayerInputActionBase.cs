// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
    public abstract class PlayerInputActionBase : ComponentAction<UnityEngine.InputSystem.PlayerInput>
    {
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

        protected UnityEngine.InputSystem.PlayerInput m_playerInput;
        protected UnityEngine.InputSystem.InputAction m_inputAction;

        protected virtual void OnPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) { }
        protected virtual void OnCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) { }

        public override void Reset()
		{
			gameObject = null;
            inputAction = null;
            m_inputAction = null;
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

            if (m_playerInput != cachedComponent)
            {
                RemoveDelegates();

                m_playerInput = cachedComponent;

                m_inputAction = m_playerInput.actions.FindAction(reference.action.id);

                if (m_inputAction == null)
                {
                    LogWarning("Could not find action " + reference.name);
                    return false;
                }

                AddDelegates();
            }

            return true;
        }

        private void AddDelegates()
        {
            if (m_inputAction == null) return;
            m_inputAction.performed += OnPerformed;
            m_inputAction.canceled += OnCanceled;
        }

        private void RemoveDelegates()
        {
            if (m_inputAction == null) return;
            m_inputAction.performed -= OnPerformed;
            m_inputAction.canceled -= OnCanceled;
        }

        public override void OnEnter()
        {
            if (!UpdateCache())
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (!UpdateCache())
            {
                Finish();
            }
        }

        public override void OnExit()
        {
            RemoveDelegates();

            // force reset next time we enter
            m_playerInput = null;
        }
    }
}
#endif
