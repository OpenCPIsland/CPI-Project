// (c) Copyright// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Clamps an orthographic camera's position to keep the view inside min/max ranges. " +
             "Set any limit to None to leave that axis un-clamped.")]
    public class ClampOrthographicView : ComponentAction<Camera>
    {
        public enum ScreenPlane
        {
            XY,
            XZ
        }

        [RequiredField]
        [Tooltip("The GameObject with a Camera component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Orientation of the view.")]
        public ScreenPlane view;

        [Tooltip("The left edge of the view to stay inside.")]
        public FsmFloat minX;

        [Tooltip("The right edge of the view to stay inside.")]
        public FsmFloat maxX;

        [Tooltip("The bottom edge of the view to stay inside.")]
        public FsmFloat minY;

        [Tooltip("The top edge of the view to stay inside.")]
        public FsmFloat maxY;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        [Tooltip("Perform in LateUpdate. This is useful if you want to clamp the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        public override void Reset()
        {
            gameObject = null;
            minX = new FsmFloat { UseVariable = true };
            maxX = new FsmFloat { UseVariable = true };
            minY = new FsmFloat { UseVariable = true };
            maxY = new FsmFloat { UseVariable = true };
            everyFrame = false;
            lateUpdate = false;
        }

        public override void OnPreprocess()
        {
            if (lateUpdate) Fsm.HandleLateUpdate = true;
        }

        public override void OnEnter()
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
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            var pos = cachedTransform.position;

            var padY = camera.orthographicSize;
            var padX = camera.orthographicSize * Screen.width / Screen.height;

            if (!minX.IsNone)
                pos.x = Mathf.Max(minX.Value + padX, pos.x);
            if (!maxX.IsNone)
                pos.x = Mathf.Min(maxX.Value - padX, pos.x);

            if (view == ScreenPlane.XY)
            {
                if (!minY.IsNone)
                    pos.y = Mathf.Max(minY.Value + padY, pos.y);
                if (!maxY.IsNone)
                    pos.y = Mathf.Min(maxY.Value - padY, pos.y);
            }
            else
            {
                if (!minY.IsNone)
                    pos.z = Mathf.Max(minY.Value + padY, pos.z);
                if (!maxY.IsNone)
                    pos.z = Mathf.Min(maxY.Value - padY, pos.z);
            }

            camera.transform.position = pos;
        }
    }
}
