// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using UnityEditor;

// Note: We're fully qualifying tween types to avoid conflicts with NGUI.
// NGUI doesn't use namespaces for its tween scripts :(
// Also Tween is a common name, and others might do the same! 

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenFade))]
    public class TweenFadeEditor : TweenEditorBase
    {
        public override bool OnGUI()
        {
            var tweenFade = target as PlayMaker.Actions.TweenFade;

            EditorGUI.BeginChangeCheck();

            EditField("gameObject");

            FsmEditorGUILayout.ReadonlyTextField("Fade Type: " + tweenFade.type);

            EditField("tweenDirection", "Fade");
            EditField("value");

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }


    }
}