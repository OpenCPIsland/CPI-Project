// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
 
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Set the screen rect of a RectTransform using 2 Vector2 points.")]
	public class RectTransformSetScreenRectFromPoints : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

        [RequiredField]
		[Tooltip("The screen position of the first point to define the rect.")]
		public FsmVector2 point1;

        [RequiredField]
        [Tooltip("The screen position of the second point to define the rect.")]
        public FsmVector2 point2;

        [Tooltip("Screen points use normalized coordinates (0-1).")]
        public FsmBool normalized;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the resulting screen rect.")]
        public FsmRect storeScreenRect;

        private GameObject cachedGameObject;
        private RectTransform _rt;
        private Canvas rootCanvas;
        private RectTransform rootRectTransform;
        private Camera canvasCamera;

        public override void Reset()
		{
			base.Reset();
			gameObject = null;
            point1 = new FsmVector2 { UseVariable = true };
            point2 = new FsmVector2 { UseVariable = true };
            normalized = null;
            storeScreenRect = null;
        }


        private bool UpdateCache()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go != cachedGameObject)
            {
                cachedGameObject = go;
                _rt = go.GetComponent<RectTransform>();

                rootCanvas = go.transform.GetComponentInParent<Canvas>().rootCanvas;
                rootRectTransform = rootCanvas.GetComponent<RectTransform>();
                canvasCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
            }

            return _rt != null;
        }

        public override void OnEnter()
		{
            if (!UpdateCache())
            {
                Finish();
                return;
            }

            DoSetValues();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}
		
		public override void OnActionUpdate()
		{
			DoSetValues();
		}

        private void DoSetValues()
		{
            if (!UpdateCache())
            {
                Finish();
                return;
            }

            // UI cannot use negative dimensions
            // So we calculate a rect with proper min and max

            var rect = new Rect
            {
                x = Mathf.Min(point1.Value.x, point2.Value.x),
                y = Mathf.Min(point1.Value.y, point2.Value.y),
                width = Mathf.Abs(point2.Value.x - point1.Value.x),
                height = Mathf.Abs(point2.Value.y - point1.Value.y)
            };
            storeScreenRect.Value = rect;

            var screenPoint = rect.min;
            var size = rect.size;

            if (normalized.Value)
            {
                screenPoint.x *= Screen.width;
                screenPoint.y *= Screen.height;
                size.x *= Screen.width;
                size.y *= Screen.height;
            }

            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTransform, screenPoint, canvasCamera, out localPosition);

            _rt.localPosition = localPosition;
            _rt.sizeDelta = size;
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("SetScreenRectFrom", point1, point2);
        }
#endif
    }
}