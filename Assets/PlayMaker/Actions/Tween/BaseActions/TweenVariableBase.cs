// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker.TweenEnums;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base property for tweening variables.
    /// Note: Offset calculations should be defined in derived classes by implementing GetOffsetValue
    /// </summary>
    [ActionCategory(ActionCategory.Tween)]
    public abstract class TweenVariableBase<T> : TweenPropertyBase<T> where T: NamedVariable
    {
        [RequiredField]
        [Tooltip("The variable to tween.")]
        [UIHint(UIHint.Variable)]
        public T variable;

        public override void Reset()
        {
            base.Reset();

            variable = null;
            fromOption = TargetValueOptions.CurrentValue;
            fromValue = null;
            toOption = TargetValueOptions.Value;
            toValue = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // DoTween at 0
            // Important if startDelay is used

            DoTween();
        }

        protected override void InitTargets()
        {
            switch (fromOption)
            {
                case TargetValueOptions.CurrentValue:
                    StartValue = variable.RawValue;
                    break;
                case TargetValueOptions.Value:
                    StartValue = fromValue.RawValue;
                    break;
                case TargetValueOptions.Offset:
                    // Derived classes need to implement GetOffsetValue:
                    StartValue = GetOffsetValue(variable.RawValue, fromValue.RawValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (toOption)
            {
                case TargetValueOptions.CurrentValue:
                    EndValue = variable.RawValue;
                    break;
                case TargetValueOptions.Value:
                    EndValue = toValue.RawValue;
                    break;
                case TargetValueOptions.Offset:
                    // Derived classes need to implement GetOffsetValue:
                    EndValue = GetOffsetValue(variable.RawValue, toValue.RawValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR

        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, variable, fromValue, toValue) + " " + easeType;
        }

#endif

    }

}
