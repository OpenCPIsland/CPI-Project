// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.Extensions;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [ActionTarget(typeof(AudioSource))]
    [Tooltip("Tween common AudioSource properties.")]
    public class TweenAudio : TweenComponentBase<AudioSource>
    {
        public enum AudioProperty { Volume, Pitch }

        [Tooltip("Audio property to tween.")]
        public AudioProperty property;

        [Tooltip("Tween To/From values set below.")]
        public TweenDirection tweenDirection;

        // Serialize all potential tween targets
        // a little wasteful but not too bad...

        [Tooltip("Value for the selected property.")]
        public FsmFloat value;

        // tween setup 

        private AudioSource audio;
        private float fromFloat, toFloat;

        public override void Reset()
        {
            base.Reset();

            property = AudioProperty.Volume;
            tweenDirection = TweenDirection.To;
            value = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Finished) return;

            audio = cachedComponent;

            if (tweenDirection == TweenDirection.From)
            {
                switch (property)
                {
                    case AudioProperty.Volume:
                        fromFloat = value.Value;
                        toFloat = audio.volume;
                        break;
                    case AudioProperty.Pitch:
                        fromFloat = value.Value;
                        toFloat = audio.pitch;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (property)
                {
                    case AudioProperty.Volume:
                        fromFloat = audio.volume;
                        toFloat = value.Value;
                        break;
                    case AudioProperty.Pitch:
                        fromFloat = audio.pitch;
                        toFloat = value.Value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);

            switch (property)
            {
                case AudioProperty.Volume:
                    audio.volume = Mathf.Lerp(fromFloat, toFloat, lerp);
                    break;
                case AudioProperty.Pitch:
                    audio.pitch = Mathf.Lerp(fromFloat, toFloat, lerp);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return "TweenAudio: " + property + " " + tweenDirection + " " + ActionHelpers.GetValueLabel(value);
        }
#endif
    }

}
