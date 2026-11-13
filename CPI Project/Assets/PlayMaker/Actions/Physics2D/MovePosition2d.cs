// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Moves a Game Object's RigidBody2D to a new position. Unlike SetPosition this will respect physics collisions.")]
	public class MovePosition2d : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Movement vector.")]
		public FsmVector2 vector;

        [Tooltip("Movement in x axis.")]
        public FsmFloat x;

        [Tooltip("Movement in y axis.")]
        public FsmFloat y;

        [Tooltip("Coordinate space to move in.")]
        public Space space;

        [Tooltip("Keep running every frame.")]
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

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }

        public override void OnFixedUpdate()
		{
			DoMovePosition();
			
			if (!everyFrame)
				Finish();	
		}

		void DoMovePosition()
		{
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (!UpdateCache(go))
            {
                return;
            }

			// init position
			
			Vector3 position;

			if (vector.IsNone)
            {
                position = space == Space.World ? 
                    new Vector3(rigidbody2d.position.x, rigidbody2d.position.y, 0) : 
                    cachedTransform.TransformPoint(rigidbody.position);
            }
			else
			{
				position = vector.Value;
			}
			
			// override any axis

			if (!x.IsNone) position.x = x.Value;
			if (!y.IsNone) position.y = y.Value;

			// apply

            rigidbody2d.MovePosition(space == Space.World ? 
                position : go.transform.InverseTransformPoint(position));
        }


	}
}
