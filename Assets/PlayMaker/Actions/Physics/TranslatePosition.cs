// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Translates a Game Object's RigidBody. Unlike Translate this will respect physics collisions.")]
	public class TranslatePosition : ComponentAction<Rigidbody>
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

        [Tooltip("Translate over one second")]
        public bool perSecond;

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
            perSecond = true;
			everyFrame = true;
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

            // Use vector if specified

            var translate = vector.IsNone ? new Vector3(x.Value, y.Value, z.Value) : vector.Value;

            // override any axis

            if (!x.IsNone) translate.x = x.Value;
            if (!y.IsNone) translate.y = y.Value;
            if (!z.IsNone) translate.z = z.Value;

            // apply

            if (perSecond)
                translate *= Time.deltaTime;

            rigidbody.MovePosition(space == Space.World ?
                rigidbody.position + translate : rigidbody.position + go.transform.TransformVector(translate));
        }


	}
}
