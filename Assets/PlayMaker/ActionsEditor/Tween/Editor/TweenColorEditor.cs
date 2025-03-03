// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.TweenEnums;
using UnityEditor;

// Note: We're fully qualifying tween types to avoid conflicts with NGUI.
// NGUI doesn't use namespaces for its tween scripts :(
// Also Tween is a common name, and others might do the same! 

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenColor))]
    public class TweenColorEditor : TweenPropertyEditor<FsmColor>
    {
        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("target");

            var tweenColor = (PlayMaker.Actions.TweenColor) target;

            switch (tweenColor.target)
            {
                case PlayMaker.Actions.TweenColor.Target.GameObject:
                    EditField("gameObject");
                    FsmEditorGUILayout.ReadonlyTextField("Type: " + tweenColor.type);
                    break;
                case PlayMaker.Actions.TweenColor.Target.Variable:
                    EditField("variable");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditField("fromOption");
            DoTargetValueGUI(tweenAction.fromOption, "fromValue");
            if (tweenAction.fromOption == TargetValueOptions.Offset)
            {
                EditField("fromOffsetBlendMode","Blend Mode");
            }

            EditField("toOption");
            DoTargetValueGUI(tweenAction.toOption, "toValue");
            if (tweenAction.fromOption == TargetValueOptions.Offset)
            {
                EditField("toOffsetBlendMode","Blend Mode");
            }

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }


    }
}