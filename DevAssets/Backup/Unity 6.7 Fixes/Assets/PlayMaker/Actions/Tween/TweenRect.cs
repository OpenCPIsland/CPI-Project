// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using HutongGames.Extensions;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [Tooltip("Tween a Rect variable using a custom easing function.")]
    public class TweenRect : TweenVariableBase<FsmRect>
    {
        protected override object GetOffsetValue(object value, object offset)
        {
            var rect = (Rect) value;
            var off = (Rect) offset;
            return new Rect(rect.x + off.x, rect.y + off.y, rect.width + off.width, rect.height + off.height);
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);
            variable.Value = variable.Value.Lerp((Rect) StartValue, (Rect) EndValue, lerp);
        }
    }

}
