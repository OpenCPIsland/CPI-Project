// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
    [Tooltip("Sets the velocity of a CharacterController on a GameObject. To leave any axis unchanged, set variable to 'None'.")]
    public class SetControllerVelocity : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
        [Tooltip("The GameObject with the Character Controller component.")]
        public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Set velocity using Vector3 variable and/or individual channels below.")]
        public FsmVector3 vector;

        [Tooltip("Velocity in X axis.")]
        public FsmFloat x;
        [Tooltip("Velocity in Y axis.")]
        public FsmFloat y;
        [Tooltip("Velocity in Z axis.")]
        public FsmFloat z;

        [Tooltip("You can set velocity in world or local space.")]
        public Space space;

        [Tooltip("Set the velocity every frame.")]
        public bool everyFrame;


        private CharacterController controller
        {
            get { return cachedComponent; }
        }


        public override void Reset()
		{
			gameObject = null;
			vector = null;
			// default axis to variable dropdown with None selected.
			x = new FsmFloat { UseVariable = true };
			y = new FsmFloat { UseVariable = true };
			z = new FsmFloat { UseVariable = true };
			space = Space.Self;
			everyFrame = false;
		}

        // TODO: test this works in OnEnter!
		public override void OnEnter()
		{
			DoSetVelocity();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}

        void DoSetVelocity()
		{
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            // init position

            Vector3 velocity;

			if (vector.IsNone)
			{
				velocity = space == Space.World ?
					controller.velocity : 
					cachedTransform.InverseTransformDirection(controller.velocity);
			}
			else
			{
				velocity = vector.Value;
			}
			
			// override any axis

			if (!x.IsNone) velocity.x = x.Value;
			if (!y.IsNone) velocity.y = y.Value;
			if (!z.IsNone) velocity.z = z.Value;

			// apply

            if (space == Space.Self)
            {
                velocity = cachedTransform.TransformDirection(velocity);
            }

            controller.Move(velocity * Time.deltaTime);
		}
	}
}