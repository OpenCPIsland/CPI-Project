// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Sets a Rect's value using Vector2 points.")]
	public class SetRectFromPoints : FsmStateAction
	{
		[RequiredField]
        [UIHint(UIHint.Variable)]
		[Tooltip("Rectangle to set.")]
		public FsmRect rectangle;

		[Tooltip("First point.")]
		public FsmVector2 point1;

        [Tooltip("Second point.")]
        public FsmVector2 point2;

        [Tooltip("Avoid negative width and height values. This is useful for UI rects that don't draw if they have negative dimensions.")]
        public FsmBool positiveDimensions;

        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
        {
            rectangle = null;
            point1 = new FsmVector2 {UseVariable = true};
            point2 = new FsmVector2 { UseVariable = true };
            positiveDimensions = new FsmBool { Value = true};
            everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetValue();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetValue();
		}

        private void DoSetValue()
		{
			if (rectangle.IsNone)
			{
				return;
			}

            if (positiveDimensions.Value)
            {
                var rect = new Rect
                {
                    x = Mathf.Min(point1.Value.x, point2.Value.x),
                    y = Mathf.Min(point1.Value.y, point2.Value.y),
                    width = Mathf.Abs(point2.Value.x - point1.Value.x),
                    height = Mathf.Abs(point2.Value.y - point1.Value.y)
                };
                rectangle.Value = rect;
            }
            else
            {
                rectangle.Value = new Rect {min = point1.Value, max = point2.Value};
            }
		}

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, rectangle, point1, point2);
        }
#endif
    }
}