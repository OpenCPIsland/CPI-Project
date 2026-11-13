// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Wraps a GameObject's position across screen edges. " +
             "For example, a GameObject that moves off the left side of the screen wraps to the right side. " +
             "This is often used in 2d arcade style games like Asteroids.")]
    public class ScreenWrap : ComponentAction<Transform, Camera>
    {
        [RequiredField]
        [Tooltip("The GameObject to wrap.")]
        public FsmOwnerDefault gameObject;

        [CheckForComponent(typeof(Camera))]
        [Tooltip("GameObject with a Camera component used to render the view (or MainCamera if not set). The Viewport Rect is used for wrapping.")]
        public FsmGameObject camera;

        [Tooltip("Wrap the position of the GameObject if it moves off the left side of the screen.")]
        public FsmBool wrapLeft;

        [Tooltip("Wrap the position of the GameObject if it moves off the right side of the screen.")]
        public FsmBool wrapRight;

        [Tooltip("Wrap the position of the GameObject if it moves off the top of the screen.")]
        public FsmBool wrapTop;

        [Tooltip("Wrap the position of the GameObject if it moves off the top of the screen.")]
        public FsmBool wrapBottom;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [Tooltip("Use LateUpdate. Useful if you want to wrap after any other operations in Update.")]
        public bool lateUpdate;

        private Camera cameraComponent
        {
            get { return cachedComponent2;}
        }

        private Transform cameraTransform
        {
            get { return cachedTransform2; }
        }

        private Transform gameObjectTransform
        {
            get { return cachedComponent1; }
        }

        public override void Reset()
        {
            gameObject = null;
            wrapLeft = new FsmBool {Value = true};
            wrapRight = new FsmBool { Value = true };
            wrapTop = new FsmBool { Value = true };
            wrapBottom = new FsmBool { Value = true };
            everyFrame = true;
            lateUpdate = true;
        }

        public override void OnPreprocess()
        {
            if (lateUpdate) Fsm.HandleLateUpdate = true;
        }

        public override void OnEnter()
        {
            if (!everyFrame && !lateUpdate)
            {
                DoScreenWrap();
                Finish();
            }
        }


        public override void OnUpdate()
        {
            if (!lateUpdate)
            {
                DoScreenWrap();
            }
        }

        public override void OnLateUpdate()
        {
            if (lateUpdate)
            {
                DoScreenWrap();
            }

            if (!everyFrame)
            {
                Finish();
            }
        }


        private void DoScreenWrap()
        {
            if (camera.Value == null)
            {
                camera.Value = Camera.main != null ? Camera.main.gameObject : null;
            }

            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject), camera.Value))
            {
                return; // Couldn't get required components
            }

            var screenPos = cameraComponent.WorldToViewportPoint(gameObjectTransform.position);
            var wrapped = false; // only do expensive operations if we wrapped

            if (wrapLeft.Value && screenPos.x < 0 ||
                wrapRight.Value && screenPos.x >= 1)
            {
                screenPos.x = Wrap01(screenPos.x);
                wrapped = true;
            }

            if (wrapTop.Value && screenPos.y >= 1 ||
                wrapBottom.Value && screenPos.y < 0)
            {
                screenPos.y = Wrap01(screenPos.y);
                wrapped = true;
            }

            if (wrapped)
            {
                // get z distance from camera to transform new screen position back into world position
                screenPos.z = cameraTransform.InverseTransformPoint(gameObjectTransform.position).z;
                gameObjectTransform.position = cameraComponent.ViewportToWorldPoint(screenPos);
            }
        }

        private static float Wrap01(float x)
        {
            return Wrap(x, 0, 1);
        }

        private static float Wrap(float x, float xMin, float xMax)
        {
            if (x < xMin)
                x = xMax - (xMin - x) % (xMax - xMin);
            else
                x = xMin + (x - xMin) % (xMax - xMin);

            return x;
        }
    }
}