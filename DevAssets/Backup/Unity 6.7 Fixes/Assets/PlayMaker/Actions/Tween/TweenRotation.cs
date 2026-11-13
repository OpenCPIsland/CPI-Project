// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Tween)]
    [Tooltip("Tween a GameObject's rotation.")]
    public class TweenRotation : TweenComponentBase<Transform>
    {
        /* Just use Slerp - perf difference is minimal, not worth complicating the UI
        [Tooltip("Type of interpolation. Linear is faster but looks worse if the rotations are far apart.")]
        [DisplayOrder(0)]
        public RotationInterpolation interpolation;*/

        [ActionSection("From")]

        [Title("Options")]
        [Tooltip("Setup where to tween from.")]
        public RotationOptions fromOptions;

        [Tooltip("Use this GameObject's rotation.")]
        public FsmGameObject fromTarget;
        
        [Tooltip("Tween from this rotation")]
        public FsmVector3 fromRotation;

        [ActionSection("To")]

        [Title("Options")]
        [Tooltip("Setup where to tween to.")]
        public RotationOptions toOptions;

        [Tooltip("Use this GameObject's rotation")]
        public FsmGameObject toTarget;

        [Tooltip("Tween to this rotation.")]
        public FsmVector3 toRotation;

        private Transform transform;
        private Transform fromTransform;
        private Transform toTransform;

        public Quaternion StartRotation { get; private set; }
        public Quaternion EndRotation { get; private set; }

        private Quaternion midRotation;

        public override void Reset()
        {
            base.Reset();

            fromOptions = RotationOptions.CurrentRotation;
            fromTarget = null;
            fromRotation = null;
            toOptions = RotationOptions.WorldRotation;
            toTarget = null;
            toRotation = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Finished) return;

            transform = cachedComponent;
            fromTransform = fromTarget.Value != null ? fromTarget.Value.transform : null;
            toTransform = toTarget.Value != null ? toTarget.Value.transform : null;

            InitStartRotation();
            InitEndRotation();

            DoTween(); // first frame
        }

        private void InitStartRotation()
        {
            StartRotation = TweenHelpers.GetTargetRotation(fromOptions, transform, fromTransform, fromRotation.Value);
        }

        private void InitEndRotation()
        {
            EndRotation = TweenHelpers.GetTargetRotation(toOptions, transform, toTransform, toRotation.Value);

            // also store a mid point rotation to allow slerping past 180 degrees.
            // this doesn't handle entering angles past 360
             
            midRotation = TweenHelpers.GetTargetRotation(toOptions, transform, toTransform, toRotation.Value / 2);
        }

        private void UpdateStartRotation()
        {
            if (fromOptions == RotationOptions.MatchGameObjectRotation)
            {             
                InitStartRotation();
            }
        }

        private void UpdateEndRotation()
        {
            if (toOptions == RotationOptions.MatchGameObjectRotation)
            {
                InitEndRotation();
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // allow start and end rotations to change?
            // not sure how useful this is...

            UpdateStartRotation();
            UpdateEndRotation();
        }

        protected override void DoTween()
        {
            var lerp = easingFunction(0, 1, normalizedTime);
            if (lerp < 0.5)
            {
                transform.rotation = Quaternion.Slerp(StartRotation, midRotation, lerp * 2f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(midRotation, EndRotation, (lerp - 0.5f) * 2f);
            }
            
            /* Just use Slerp - perf difference is minimal, not worth complicating the UI 
            transform.rotation = interpolation == RotationInterpolation.Linear ? 
                Quaternion.Lerp(StartRotation, EndRotation, lerp) : 
                Quaternion.Slerp(StartRotation, EndRotation, lerp);*/
        }

    }

}
