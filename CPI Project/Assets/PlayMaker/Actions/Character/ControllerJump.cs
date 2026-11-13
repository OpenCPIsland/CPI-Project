// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Makes a CharacterController Jump.")]
	public class ControllerJump : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

        [Tooltip("How high to jump.")]
        public FsmFloat jumpHeight;

        [Tooltip("Jump in local or word space.")]
        public Space space;

        [Tooltip("Multiplies the speed of the CharacterController at moment of jumping. " +
                 "Higher numbers will jump further. Note: Does not effect the jump height.")]
        public FsmFloat jumpSpeedMultiplier;

        [Tooltip("Gravity multiplier used in air, to correctly calculate jump height.")]
        public FsmFloat gravityMultiplier;

        [Tooltip("Extra gravity multiplier when falling. " +
                 "Note: This is on top of the gravity multiplier above. " +
                 "This can be used to make jumps less 'floaty.'")]
        public FsmFloat fallMultiplier;

        [ActionSection("In Air Controls")]

        [UIHint(UIHint.Variable)]
        [Tooltip("Movement vector applied while in the air. Usually from a Get Axis Vector, allowing the player to influence the jump.")]
        public FsmVector3 moveVector;

        [Tooltip("Multiplies the Move Vector by a Speed factor.")]
        public FsmFloat speed;

        [Tooltip("Clamp horizontal speed while jumping. Set to None for no clamping.")]
        public FsmFloat maxSpeed;

        [ActionSection("Landing")]

        [Tooltip("Event to send when landing. Use this to transition back to a grounded State.")]
        public FsmEvent landedEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store how fast the Character Controlling was moving when it landed.")]
        public FsmFloat landingSpeed;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the last movement before landing.")]
        public FsmVector3 landingMotion;

        [UIHint(UIHint.Variable)]
        [Tooltip("The total distance fallen, from the start of the jump to landing point. " +
                 "NOTE: This will be negative when jumping to higher ground.")]
        public FsmFloat fallDistance;

        private Vector3 startJumpPosition;
        private Vector3 totalJumpMovement;

        private CharacterController controller
        {
            get {return cachedComponent;}
        }

		public override void Reset()
		{
			gameObject = null;
            jumpHeight = new FsmFloat {Value = 0.5f};
            jumpSpeedMultiplier = new FsmFloat { Value = 1f };
            gravityMultiplier = new FsmFloat { Value = 1f };
            space = Space.World;
            moveVector = null;
            speed = new FsmFloat { Value = 1f };
            maxSpeed = new FsmFloat {  Value = 2f };
            fallMultiplier = new FsmFloat { Value = 1f };
        }

        public override void OnEnter()
        {
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            startJumpPosition = cachedTransform.position;

            // Get starting velocity

            var velocity = controller.velocity * (jumpSpeedMultiplier.IsNone ? 1: jumpSpeedMultiplier.Value);
            velocity.y = 0; // for consistent jump height
            if (space == Space.Self)
            {
                velocity = cachedTransform.InverseTransformDirection(velocity);
            }

            // Calculate the move required to reach the desired jump height

            var gravity = Physics.gravity.y * (gravityMultiplier.IsNone ? 1 : gravityMultiplier.Value);

            var verticalMotion = velocity.y + Mathf.Sqrt(jumpHeight.Value * -3.0f * gravity);
            var move = new Vector3(velocity.x, verticalMotion, velocity.z);

            if (space == Space.Self)
            {
                move = cachedTransform.TransformDirection(move);
            }

            controller.Move(move * Time.deltaTime);
        }

        public override void OnUpdate()
        {
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            var move = controller.velocity;

            if (!moveVector.IsNone)
            {
                var inAirMove = moveVector.Value;
                if (!speed.IsNone)
                {
                    inAirMove *= speed.Value;
                }
                move += inAirMove;
            }

            var gravity = Physics.gravity.y * gravityMultiplier.Value * (move.y < 0 ? fallMultiplier.Value : 1);
            move.y += gravity * Time.deltaTime;

            if (!maxSpeed.IsNone)
            {
                var xz = Vector2.ClampMagnitude(new Vector2(move.x, move.z), maxSpeed.Value);
                move.Set(xz.x, move.y, xz.y);
            }

            if (space == Space.Self)
            {
                move = cachedTransform.TransformDirection(move);
            }

            controller.Move(move * Time.deltaTime);

            if (controller.isGrounded && controller.velocity.y < 0.1f)
            {
                controller.Move(Vector3.zero);

                landingMotion.Value = move;
                landingSpeed.Value = move.magnitude;
                fallDistance.Value = startJumpPosition.y - cachedTransform.position.y;

                //totalJumpMovement = cachedTransform.position - startJumpPosition;
                //Debug.Log(totalJumpMovement.magnitude);
                
                Fsm.Event(landedEvent);
            }
        }

    }
}
