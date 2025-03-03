// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using HutongGames.PlayMaker;
using UnityEditor;

// Note: We're fully qualifying tween types to avoid conflicts with NGUI.
// NGUI doesn't use namespaces for its tween scripts :(
// Also Tween is a common name, and others might do the same! 

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenPunch))]
    public class TweenPunchEditor : TweenEditorBase
    {
        private PlayMaker.Actions.TweenPunch tweenAction;

        public override void OnEnable()
        {
            base.OnEnable();

            tweenAction = (PlayMaker.Actions.TweenPunch) target;
        }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("gameObject");
            
            DoPunchTypeSelector();

            EditField("value", "Punch");

            // Easing (skipping EaseType selection)

            EditField("startDelay");
            EditField("time");
            EditField("realTime");
            EditField("updateType");
            EditField("loopType");
            EditField("finishEvent");

            return EditorGUI.EndChangeCheck();
        }

        private void DoPunchTypeSelector()
        {
            EditorGUI.BeginChangeCheck();
            EditField("punchType");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.value = new FsmVector3();
            }
        }

    }
}