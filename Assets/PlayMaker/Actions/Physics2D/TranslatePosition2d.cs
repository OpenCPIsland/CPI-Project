// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Translates a Game Object's RigidBody2d. Unlike Translate2d this will respect physics collisions.")]
	public class TranslatePosition2d : ComponentAction<Rigidbody2D>
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
			space = Space.World;
            perSecond = true;
			everyFrame = true;
		}

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }

        public override void OnFixedUpdate()
		{
			DoTranslatePosition2d();
			
			if (!everyFrame)
				Finish();	
		}

        private void DoTranslatePosition2d()
		{
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (!UpdateCache(go))
            {
                return;
            }

            // Use vector if specified

            var translate = vector.IsNone ? new Vector2(x.Value, y.Value) : vector.Value;

            // override any axis

            if (!x.IsNone) translate.x = x.Value;
            if (!y.IsNone) translate.y = y.Value;

            // apply

            if (perSecond)
            {
                translate *= Time.deltaTime;
            }

            if (space == Space.Self)
            {
                translate = cachedTransform.TransformVector(new Vector3(translate.x, translate.y, 0));
            }

            rigidbody2d.MovePosition(rigidbody2d.position + translate);
        }
	}
}
