// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [Tooltip("Tween a Vector2 variable using a custom easing function.")]
    public class TweenVector2 : TweenVariableBase<FsmVector2>
    {
        protected override object GetOffsetValue(object value, object offset)
        {
            return (Vector2) value + (Vector2) offset;
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);
            variable.Value = Vector2.Lerp((Vector2) StartValue, (Vector2) EndValue, lerp);
        }
    }

}
