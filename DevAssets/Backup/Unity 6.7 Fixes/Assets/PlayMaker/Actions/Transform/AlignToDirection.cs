// (c) Copyright HutongGames, LLC 2021. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Align a GameObject to the specified Direction.")]
    public class AlignToDirection : ComponentAction<Transform>
    {
        public enum AlignAxis
        {
            x,y,z
        }

        [RequiredField]
        [Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The direction to look at. E.g. the Hit Normal from a Raycast.")]
        public FsmVector3 targetDirection;

        [RequiredField]
        [Tooltip("The GameObject axis to align to the direction.")]
        [ObjectType(typeof(AlignAxis))]
        public FsmEnum alignAxis;

        [Tooltip("Flip the alignment axis. So x becomes -x.")]
        public FsmBool flipAxis;

        [Tooltip("Repeat every update.")]
        public bool everyFrame;

        [Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
        public bool lateUpdate;

        public override void Reset()
        {
            gameObject = null;
            targetDirection = new FsmVector3 { UseVariable = true };
            alignAxis = null;
            flipAxis = null;
            everyFrame = false;
            lateUpdate = false;
        }

        public override void OnPreprocess()
        {
            Fsm.HandleLateUpdate = lateUpdate;
        }

        public override void OnEnter()
        {
            DoAlignToDirection();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (!lateUpdate)
            {
                DoAlignToDirection();
            }
        }

        public override void OnLateUpdate()
        {
            if (lateUpdate)
            {
                DoAlignToDirection();
            }
        }

        private void DoAlignToDirection()
        {
            if (targetDirection.IsNone) return;

            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            var alignDirection = new Vector3();

            switch ((AlignAxis) alignAxis.Value)
            {
                case AlignAxis.x:
                    alignDirection = cachedTransform.right;
                    break;
                case AlignAxis.y:
                    alignDirection = cachedTransform.up;
                    break;
                case AlignAxis.z:
                    alignDirection = cachedTransform.forward;
                    break;
            }

            if (flipAxis.Value)
            {
                alignDirection *= -1;
            }

            cachedTransform.rotation = Quaternion.FromToRotation(alignDirection, targetDirection.Value) * cachedTransform.rotation;
        }
    }
}