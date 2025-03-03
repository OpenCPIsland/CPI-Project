// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
	[Tooltip("Get the Vector2 value from a named InputAction in a PlayerInput component and store it in a Vector3 variable.")]
	public class PlayerInputGetVector2AsVector3 : PlayerInputUpdateActionBase
    {
        public enum Mapping
        {
            XZ,
            XY,
            YZ
        }

        [Tooltip("Plane to map the 2d input to.")]
        public Mapping mapping;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the Vector3 value.")]
        public FsmVector3 storeVector3;

        public override void Reset()
		{
			base.Reset();
            mapping = Mapping.XZ;
            storeVector3 = null;
        }

        // Note we're updating by polling instead of using callbacks.
        // This is because this action is in a stack with other actions
        // updated in order, from top to bottom.
        // If this action was updated out of sync with the other actions,
        // it could lead to unexpected behaviour that would be hard to debug.

        protected override void Execute()
        {
            if (action == null) return;

            var v2 = action.ReadValue<Vector2>();

            switch (mapping)
            {
                case Mapping.XZ:
                    storeVector3.Value = new Vector3(v2.x, 0, v2.y);
                    break;
                case Mapping.XY:
                    storeVector3.Value = new Vector3(v2.x, v2.y, 0);
                    break;
                case Mapping.YZ:
                    storeVector3.Value = new Vector3(0, v2.y, v2.x);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

#endif
