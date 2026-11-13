// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Makes a CharacterController Crouch. Handles scaling the collider and transitions between standing and crouching.")]
	public class ControllerCrouch : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("Crouch while this to true. Normally set by an Input action like Get Key." +
                 "\n\nNOTE: The controller might not be able to stand up when this is false if there's not enough headroom.")]
        public FsmBool isCrouching;

        [RequiredField]
        [Tooltip("Height of capsule when crouching.")]
        public FsmFloat crouchHeight;

        [Tooltip("Move children so their height scales with capsule. This is useful for weapon attach points etc.")]
        public FsmBool adjustChildren;

        [RequiredField]
        [Tooltip("How long it takes to crouch/stand in seconds.")]
        public FsmFloat transitionTime;

        [Tooltip("Always complete the full transition to crouching, even if the input is brief.")]
        public FsmBool completeTransition;

        [Tooltip("Can the CharacterController stand if isCrouching is false (e.g. if the crouch button is released). " +
                 "Usually set by a some kind of raycast checking the headroom above the controller," +
                 "but could also be set to false to prevent standing for other reasons, " +
                 "e.g., crouch because the ground is shaking.")]
        public FsmBool canStand;

        [UIHint(UIHint.Variable)]
        [Tooltip("Try to stand if true. Useful if want to toggle crouch with a button.")]
        public FsmBool standToggle;

        //[RequiredField]
        [Tooltip("Event to send when crouch button is released AND there is enough headroom.")]
        public FsmEvent standEvent;

        [Tooltip("Reset the controller height if the State exits before crouch has finished. " +
                 "Also restores children to original offsets if Adjust Children was used." +
                 "\n\nNOTE: You probably want to keep this checked most of the time.")]
        public FsmBool resetHeightOnExit;

        private CharacterController controller
        {
            get {return cachedComponent;}
        }

		public override void Reset()
		{
			gameObject = null;
            isCrouching = new FsmBool {UseVariable = true};
            crouchHeight = new FsmFloat {Value = 0.5f};
            adjustChildren = new FsmBool { Value = true };
            transitionTime = new FsmFloat {Value = 0.2f};
            completeTransition = null;
            canStand = new FsmBool { Value = true};
            standToggle = null;
            standEvent = null;
            resetHeightOnExit = new FsmBool {Value = true};
        }

        private float originalHeight;
        private float startTransitionHeight;
        private float transitionTimeElapsed;

        // Store original child offsets to use with adjustChildren
        // We don't want to use deltas because errors might accumulate
        // This method guarantees the children don't drift over time
        private Dictionary<Transform, float> childOffsets = new Dictionary<Transform, float>();

        private enum CrouchState
        {
            stand,
            standToCrouch,
            crouch,
            crouchToStand
        }

        private CrouchState crouchState;

        public override void OnEnter()
        {
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            originalHeight = controller.height;

            crouchState = CrouchState.stand;
            transitionTimeElapsed = 0;

            childOffsets.Clear();
            foreach (Transform child in cachedTransform)
            {
                childOffsets.Add(child, child.localPosition.y);
            }
        }

        public override void OnUpdate()
        {
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            float newHeight;

            switch (crouchState)
            {
                case CrouchState.stand:

                    if (isCrouching.Value)
                    {
                        crouchState = CrouchState.standToCrouch;
                        startTransitionHeight = controller.height;
                        transitionTimeElapsed = 0;
                    }
                    break;

                case CrouchState.standToCrouch:

                    if (transitionTimeElapsed < transitionTime.Value)
                    {
                        newHeight = Mathf.Lerp(startTransitionHeight, crouchHeight.Value, transitionTimeElapsed / transitionTime.Value);
                        transitionTimeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        newHeight = crouchHeight.Value;
                        crouchState = CrouchState.crouch;
                    }

                    SetHeight(newHeight);

                    if (!completeTransition.Value && !isCrouching.Value)
                    {
                        crouchState = CrouchState.crouchToStand;
                        startTransitionHeight = controller.height;
                        transitionTimeElapsed = 0;
                    }
                    break;

                case CrouchState.crouch:

                    if (canStand.Value && (!isCrouching.Value || standToggle.Value))
                    {
                        crouchState = CrouchState.crouchToStand;
                        startTransitionHeight = controller.height;
                        transitionTimeElapsed = 0;
                    }
                    
                    break;

                case CrouchState.crouchToStand:

                    if (transitionTimeElapsed < transitionTime.Value)
                    {
                        newHeight = Mathf.Lerp(startTransitionHeight, originalHeight, transitionTimeElapsed / transitionTime.Value);
                        transitionTimeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        newHeight = originalHeight;
                        crouchState = CrouchState.stand;
                    }

                    SetHeight(newHeight);

                    if (crouchState == CrouchState.stand)
                    {
                        Fsm.Event(standEvent);
                    }

                    break;
            }

        }

        private void SetHeight(float newHeight)
        {
            var delta = controller.height - newHeight;

            // if not grounded it's like the player is pulling their legs up
            // so the transform and camera position shouldn't change.

            if (controller.isGrounded)
            {
                cachedTransform.Translate(0, -delta * 0.5f, 0);
            }

            controller.height = newHeight;

            if (adjustChildren.Value)
            {
                var adjust = controller.height / originalHeight;
                foreach (var kvp in childOffsets)
                {
                    var pos = kvp.Key.localPosition;
                    kvp.Key.localPosition = new Vector3(pos.x, kvp.Value * adjust, pos.z);
                }
            }
        }

        public override void OnExit()
        {
            if (resetHeightOnExit.Value)
            {
                SetHeight(originalHeight);

                foreach (var kvp in childOffsets)
                {
                    var pos = kvp.Key.localPosition;
                    kvp.Key.localPosition = new Vector3(pos.x, pos.y, pos.z);
                }
            }

            childOffsets.Clear();
        }
    }
}
