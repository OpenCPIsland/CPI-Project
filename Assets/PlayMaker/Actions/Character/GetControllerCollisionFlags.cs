// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Gets the Collision Flags from a CharacterController on a GameObject. " +
             "Collision flags give you a broad overview of where the character collided with another object.")]
	public class GetControllerCollisionFlags : ComponentAction<CharacterController>
    {
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
        [Tooltip("The GameObject with a Character Controller component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
        [Tooltip("True if the Character Controller capsule is on the ground")]
        public FsmBool isGrounded;

		[UIHint(UIHint.Variable)]
        [Tooltip("True if no collisions in last move.")]
        public FsmBool none;

		[UIHint(UIHint.Variable)]
        [Tooltip("True if the Character Controller capsule was hit on the sides.")]
		public FsmBool sides;

		[UIHint(UIHint.Variable)]
        [Tooltip("True if the Character Controller capsule was hit from above.")]
		public FsmBool above;

		[UIHint(UIHint.Variable)]
        [Tooltip("True if the Character Controller capsule was hit from below.")]
		public FsmBool below;
		
        private CharacterController controller
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			gameObject = null;
			isGrounded = null;
			none = null;
			sides = null;
			above = null;
			below = null;
		}

		[SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
        public override void OnUpdate()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

			isGrounded.Value = controller.isGrounded;
			none.Value = controller.collisionFlags == CollisionFlags.None;
			sides.Value = (controller.collisionFlags & CollisionFlags.Sides) != 0;
			above.Value = (controller.collisionFlags & CollisionFlags.Above) != 0;
			below.Value = (controller.collisionFlags & CollisionFlags.Below) != 0;
		}
	}
}
