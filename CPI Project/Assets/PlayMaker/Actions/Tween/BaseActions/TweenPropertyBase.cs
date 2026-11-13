// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker.TweenEnums;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base class for tweening a property.
    /// The property could be a variable value, a component property etc.
    /// </summary>
    [ActionCategory(ActionCategory.Tween)]
    public abstract class TweenPropertyBase<T> : TweenActionBase where T: NamedVariable
    {
        [Title("From")]
        [Tooltip("Setup where to tween from.")]
        public TargetValueOptions fromOption;

        [Tooltip("Tween from this value.")]
        [HideIf("HideFromValue")]
        public T fromValue;

        [Title("To")]
        [Tooltip("Setup where to tween to.")]
        public TargetValueOptions toOption;

        [Tooltip("Tween to this value.")]
        [HideIf("HideToValue")]
        public T toValue;

        public override void Reset()
        {
            base.Reset();

            fromOption = TargetValueOptions.CurrentValue;
            fromValue = null;
            toOption = TargetValueOptions.Value;
            toValue = null;
        }

        public object StartValue { get; protected set; }
        public object EndValue { get; protected set; }

        public override void OnEnter()
        {
            base.OnEnter();

            InitTargets();

            // DoTween at 0
            // Important if startDelay is used

            DoTween();
        }

        protected virtual void InitTargets()
        {
            throw new NotImplementedException();
        }

        protected virtual object GetOffsetValue(object value, object offset)
        {
            throw new NotImplementedException();
        }

        protected override void DoTween()
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR

        public bool HideFromValue()
        {
            return fromOption == TargetValueOptions.CurrentValue;
        }

        public bool HideToValue()
        {
            return toOption == TargetValueOptions.CurrentValue;
        }

#endif

    }

}
