// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using HutongGames.PlayMaker.Actions;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Derive custom tween editors from this to use DrawEasingUI
    /// </summary>
	public abstract class TweenEditorBase : CustomActionEditor
    {
        private TweenActionBase tweenActionBase;

        public override void OnEnable()
        {
            base.OnEnable();

            tweenActionBase = target as TweenActionBase;
        }

        public void DoEasingUI()
        {
            EditField("startDelay");
            EditField("easeType");

            if ((EasingFunction.Ease) tweenActionBase.easeType.Value == EasingFunction.Ease.CustomCurve)
            {
                EditField("customCurve");
            }

            EditField("time");
            EditField("realTime");
            EditField("updateType");
            EditField("loopType");
            EditField("finishEvent");
        }
	}
}