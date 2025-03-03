// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    public abstract class TweenActionBase : BaseUpdateAction
    {
        [ActionSection("Easing")]

        [Tooltip("Delay before starting the tween.")]
        public FsmFloat startDelay;

        [Tooltip("The type of easing to apply.")]
        [ObjectType(typeof(EasingFunction.Ease))]
        [PreviewField("DrawPreview")]
        public FsmEnum easeType;

        [Tooltip("Custom tween curve. Note: Typically you would use the 0-1 range.")]
        [HideIf("HideCustomCurve")]
        public FsmAnimationCurve customCurve;

        [Tooltip("Length of tween in seconds.")]
        public FsmFloat time;

        [Tooltip("Ignore any time scaling.")]                                           
        public FsmBool realTime;

        [Tooltip("Looping options.")]
        public LoopType loopType;

        /* Too much...? There are other ways to do this...
        [Tooltip("Store the current Tween time (0 - time).")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeCurrentTime;

        [Tooltip("Store the current normalized time (0-1).")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeNormalizedTime;
        */

        [Tooltip("Event to send when tween is finished.")]
        public FsmEvent finishEvent;

        [NonSerialized] public float normalizedTime;

        protected bool tweenStarted;
        protected bool tweenFinished;
        protected float currentTime;
        protected bool playPreview;

        private HutongGames.EasingFunction.Ease cachedEase;
        private EasingFunction.Function func;
        private static bool showPreviewCurve;

        private bool reverse;

        public EasingFunction.Function easingFunction
        {
            get
            {
                var ease = (EasingFunction.Ease) easeType.Value;
                if (cachedEase != ease || func == null)
                {
                    func = HutongGames.EasingFunction.GetEasingFunction(ease);
                    cachedEase = ease;
                }
                return func;
            }
        }

        public override void Reset()
        {
            base.Reset();
            
            startDelay = null;
            easeType = null;
            time = 1f;
            realTime = false;
            finishEvent = null;
            loopType = LoopType.None;
        }

        public override void OnEnter()
        {
            currentTime = 0f;
            normalizedTime = 0f;
            tweenFinished = false;
            tweenStarted = false;
            everyFrame = true;
            reverse = false;
        }

        public override void OnActionUpdate()
        {
            var deltaTime = realTime.Value ? Time.unscaledDeltaTime : Time.deltaTime;
            currentTime += deltaTime;

            // Do start delay

            if (!tweenStarted)
            {
                if (currentTime < startDelay.Value)
                {
                    return;
                }

                tweenStarted = true;
                currentTime -= startDelay.Value;
            }

            // Loop or finish?

            if (currentTime > time.Value)
            {
                switch (loopType)
                {
                    case LoopType.None:
                        tweenFinished = true;
                        currentTime = time.Value;
                        break;
                    case LoopType.Loop:
                        currentTime -= time.Value;
                        break;
                    case LoopType.PingPong:
                        currentTime -= time.Value;
                        reverse = !reverse;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!reverse)
            {
                normalizedTime = currentTime / time.Value;
            }
            else
            {
                normalizedTime = 1 - currentTime / time.Value;
            }

            //storeNormalizedTime.Value = normalizedTime;
            //storeCurrentTime.Value = currentTime;

            HutongGames.EasingFunction.AnimationCurve = customCurve.curve;
            DoTween();

            if (tweenFinished)
            {
                Finish();
                Fsm.Event(finishEvent);
            }
        }

        // derived actions implement the tween:
        protected abstract void DoTween();

#if UNITY_EDITOR

        private const string showPreviewPrefsKey = "PlayMaker.ShowEasingPreview";

        public override void InitEditor(Fsm fsmOwner)
        {
            Fsm = fsmOwner; // not serialized to avoid recursive serialization errors
            showPreviewCurve = EditorPrefs.GetBool(showPreviewPrefsKey, true);
            playPreview = false;
        }

        public bool HideCustomCurve()
        {
            return (EasingFunction.Ease) easeType.Value != HutongGames.EasingFunction.Ease.CustomCurve;
        }

        [SettingsMenuItem("Show Easing Preview")]
        public static void ToggleEasingPreview()
        {
            showPreviewCurve = !showPreviewCurve;
            EditorPrefs.SetBool(showPreviewPrefsKey, showPreviewCurve);
        }

        [SettingsMenuItemState("Show Easing Preview")]
        public static bool GetPreviewState()
        {
            return showPreviewCurve;
        }

        public static void SetEasingPreview(bool state)
        {
            showPreviewCurve = state;
            EditorPrefs.SetBool(showPreviewPrefsKey, showPreviewCurve);
        }

        public void DrawPreview(object fieldValue)
        {
            if (!showPreviewCurve) return;

            var ease = fieldValue as FsmEnum;
            if (ease == null || ease.IsNone) return;

            var easeValue = (EasingFunction.Ease) ease.Value; 
            EaseEditor.DrawPreviewCurve(easeValue, ref playPreview, ref normalizedTime, ActiveHighlightColor);

            /* WIP edit time preview
            if (playPreview)
            {
                UpdateSceneViewPreview();
            }
            else if (isPlaying)
            {
                isPlaying = false;
                StopPlayingPreview();
            }*/
        }

        /* WIP edit time preview
        protected virtual void StartPlayingPreview() { }
        protected virtual void StopPlayingPreview() { }

        private bool isPlaying;
        private float lastTime;

        private void UpdateSceneViewPreview()
        {
            if (!isPlaying)
            {
                isPlaying = true;
                StartPlayingPreview();
                lastTime = Time.realtimeSinceStartup;
            }

            if (lastTime > 0)
            {
                normalizedTime += (Time.realtimeSinceStartup - lastTime) / time.Value;
                if (normalizedTime > 1)
                {
                    normalizedTime -= 1;
                }
            } 

            lastTime = Time.realtimeSinceStartup;
            Repaint = true;
        }
        */

        public override float GetProgress()
        {
            return normalizedTime;
        }

#endif
    }

}
