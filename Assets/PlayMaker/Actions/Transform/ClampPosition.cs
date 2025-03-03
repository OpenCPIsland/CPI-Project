// (c) Copyright// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Clamps a position to min/max ranges. Set any limit to None to leave un-clamped.")]
	public class ClampPosition : ComponentAction<Transform>
	{
		[RequiredField]
		[Tooltip("The GameObject to clamp position.")]
		public FsmOwnerDefault gameObject;

        [Tooltip("Clamp the minimum value of x.")]
        public FsmFloat minX;

        [Tooltip("Clamp the maximum value of x.")]
        public FsmFloat maxX;

        [Tooltip("Clamp the minimum value of y.")]
        public FsmFloat minY;

        [Tooltip("Clamp the maximum value of y.")]
        public FsmFloat maxY;

        [Tooltip("Clamp the minimum value of z.")]
        public FsmFloat minZ;

        [Tooltip("Clamp the maximum value of z.")]
        public FsmFloat maxZ;

        [Tooltip("Clamp position in local (relative to parent) or world space.")]
        public Space space;

        [Tooltip("Repeat every frame")]
		public bool everyFrame;

        [Tooltip("Perform in LateUpdate. This is useful if you want to clamp the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        public override void Reset()
        {
            gameObject = null;
            minX = new FsmFloat {UseVariable = true};
            maxX = new FsmFloat { UseVariable = true };
            minY = new FsmFloat { UseVariable = true };
            maxY = new FsmFloat { UseVariable = true };
            minZ = new FsmFloat { UseVariable = true };
            maxZ = new FsmFloat { UseVariable = true };
            space = Space.Self;
            everyFrame = false;
            lateUpdate = false;
        }
		
		public override void OnPreprocess()
		{
            if (lateUpdate) Fsm.HandleLateUpdate = true;
        }

		public override void OnEnter ()
		{
            if (!everyFrame && !lateUpdate)
            {
                DoClampPosition();
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (!lateUpdate)
            {
                DoClampPosition();
            }
        }

        public override void OnLateUpdate()
		{
			DoClampPosition();

            if (lateUpdate)
            {
                DoClampPosition();
            }

            if (!everyFrame)
            {
                Finish();
            }
        }
		
		private void DoClampPosition()
		{
			if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            var pos = space == Space.World ? cachedTransform.position : cachedTransform.localPosition;

            if (!minX.IsNone)
                pos.x = Mathf.Max(minX.Value, pos.x);
            if (!maxX.IsNone)
                pos.x = Mathf.Min(maxX.Value, pos.x);
            if (!minY.IsNone)
                pos.y = Mathf.Max(minY.Value, pos.y);
            if (!maxY.IsNone)
                pos.y = Mathf.Min(maxY.Value, pos.y);
            if (!minZ.IsNone)
                pos.z = Mathf.Max(minZ.Value, pos.z);
            if (!maxZ.IsNone)
                pos.z = Mathf.Min(maxZ.Value, pos.z);

            // apply clamped pos

            if (space == Space.World)
            {
                cachedTransform.position = pos;
            }
            else
            {
                cachedTransform.localPosition = pos;
            }
        }
    }
}
