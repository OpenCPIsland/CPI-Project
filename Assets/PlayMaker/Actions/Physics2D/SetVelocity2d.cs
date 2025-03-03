// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
    public class SetVelocity2d : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Use a Vector2 value for the velocity and/or set individual axis below. If set to None, keeps current velocity.")]
		public FsmVector2 vector;

		[Tooltip("Set the x value of the velocity. If None keep current x velocity.")]
		public FsmFloat x;

		[Tooltip("Set the y value of the velocity. If None keep current y velocity.")]
		public FsmFloat y;

        [Tooltip("Set velocity in local or word space.")]
        public Space space;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			vector = null;
            // default axis to variable dropdown with None selected.
			x = new FsmFloat { UseVariable = true };
			y = new FsmFloat { UseVariable = true };
            space = Space.World;
			everyFrame = false;
		}
		
		public override void Awake()
		{
			Fsm.HandleFixedUpdate = true;
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
		
		public override void OnUpdate()
		{
			DoSetVelocity();

			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnFixedUpdate()
		{
			DoSetVelocity();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}

        private void DoSetVelocity()
		{
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (!UpdateCacheAndTransform(go))
            {
                return;
            }

            // setup velocity

            Vector2 velocity;

            if (vector.IsNone)
            {
                if (space == Space.World)
                {
                    velocity = rigidbody2d.velocity;
                }
                else
                {
                    var localVelocity = cachedTransform.InverseTransformDirection(rigidbody2d.velocity);
                    velocity.x = localVelocity.x;
                    velocity.y = localVelocity.y;
                }
            }
            else
            {
                velocity = vector.Value;
            }

			// override any axis
			
			if (!x.IsNone) velocity.x = x.Value;
			if (!y.IsNone) velocity.y = y.Value;

            // apply

            if (space == Space.Self)
            {
                var v = cachedTransform.TransformDirection(velocity);
                velocity.Set(v.x, v.y);
            }

            rigidbody2d.velocity = velocity;
        }
	}
}