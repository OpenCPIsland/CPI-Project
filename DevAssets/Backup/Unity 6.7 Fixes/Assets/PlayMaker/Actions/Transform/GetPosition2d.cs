// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Gets the 2D Position of a GameObject and stores it in a Vector2 Variable or each Axis in a Float Variable")]
    public class GetPosition2d : ComponentAction<Transform>
    {
        [RequiredField]
        [Tooltip("The game object to examine.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Title("Vector2")]
        [Tooltip("Store the position in a Vector2 Variable.")]
        public FsmVector2 vector_2d;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the X coordinate in a Float Variable.")]
        public FsmFloat x;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the Y coordinate in a Float Variable.")]
        public FsmFloat y;

        [Tooltip("Use world or local coordinates.")]
        public Space space;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            vector_2d = null;
            x = null;
            y = null;
            space = Space.World;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoGetPosition();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetPosition();
        }

        private void DoGetPosition()
        {
            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            var position = space == Space.World ? cachedTransform.position : cachedTransform.localPosition;

            vector_2d.Value = position;
            x.Value = position.x;
            y.Value = position.y;
        }
    }
}