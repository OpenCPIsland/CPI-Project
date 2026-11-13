// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [Tooltip("Punches a GameObject's position, rotation, or scale and springs back to starting state")]
    public class TweenPunch : TweenComponentBase<Transform>
    {
        public enum PunchType { Position, Rotation, Scale}

        [Tooltip("Punch position, rotation, or scale.")]
        public PunchType punchType;

        [Tooltip("Punch magnitude.")]
        public FsmVector3 value;

        private Transform transform;
        private RectTransform rectTransform;
        
        private Vector3 startVector3;
        private Vector3 endVector3;

        private Quaternion startRotation;
        private Quaternion midRotation;
        private Quaternion endRotation;

        public override void Reset()
        {
            base.Reset();

            punchType = PunchType.Position;
            value = null;        
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Finished) return;

            easeType.Value = EasingFunction.Ease.Punch;

            transform = cachedComponent;
            rectTransform = transform as RectTransform;

            switch (punchType)
            {
                case PunchType.Position:
                    startVector3 = rectTransform != null ? rectTransform.anchoredPosition3D : transform.position;
                    endVector3 = startVector3 + value.Value;
                    break;
                case PunchType.Rotation:
                    startRotation = transform.rotation;
                    midRotation = startRotation * Quaternion.Euler(value.Value * 0.5f);
                    endRotation = startRotation * Quaternion.Euler(value.Value);
                    break;
                case PunchType.Scale:
                    startVector3 = transform.localScale;
                    endVector3 = startVector3 + value.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);

            switch (punchType)
            {
                case PunchType.Position:
                    if (rectTransform != null)
                    {
                        rectTransform.anchoredPosition = Vector3.Lerp(startVector3, endVector3, easingFunction(0, 1, normalizedTime));
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(startVector3, endVector3, easingFunction(0, 1, normalizedTime));
                    }
                    break;
                case PunchType.Rotation:
                    transform.rotation = lerp < 0.5 ? 
                        Quaternion.Slerp(startRotation, midRotation, lerp * 2f) : 
                        Quaternion.Slerp(midRotation, endRotation, (lerp - 0.5f) * 2f);
                    break;
                case PunchType.Scale:
                    transform.localScale = Vector3.Lerp(startVector3, endVector3, lerp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

#if UNITY_EDITOR

        public override string AutoName()
        {
            return "TweenPunch: " + ActionHelpers.GetValueLabel(Fsm, gameObject) + " " + punchType + " " + ActionHelpers.GetValueLabel(value);
        }
#endif

    }

}
