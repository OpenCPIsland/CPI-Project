// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Moves a Game Object's Rigid Body to a new position. Unlike Set Position this will respect physics collisions.")]
	public class MovePosition : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Movement vector.")]
		public FsmVector3 vector;

        [Tooltip("Movement in x axis.")]
        public FsmFloat x;

        [Tooltip("Movement in y axis.")]
        public FsmFloat y;

        [Tooltip("Movement in z axis.")]
        public FsmFloat z;

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
			z = new FsmFloat { UseVariable = true };
			space = Space.Self;
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
                    rigidbody.position : 
                    go.transform.TransformPoint(rigidbody.position);
            }
			else
			{
				position = vector.Value;
			}
			
			// override any axis

			if (!x.IsNone) position.x = x.Value;
			if (!y.IsNone) position.y = y.Value;
			if (!z.IsNone) position.z = z.Value;

			// apply

            rigidbody.MovePosition(space == Space.World ? 
                position : go.transform.InverseTransformPoint(position));
        }


	}
}
