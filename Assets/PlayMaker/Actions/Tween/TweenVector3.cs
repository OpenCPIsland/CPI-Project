// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [Tooltip("Tween a Vector3 variable using a custom easing function.")]
    public class TweenVector3 : TweenVariableBase<FsmVector3>
    {
        protected override object GetOffsetValue(object value, object offset)
        {
            return (Vector3) value + (Vector3) offset;
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);
            variable.Value = Vector3.Lerp((Vector3) StartValue, (Vector3) EndValue, lerp);
        }
    }

}
