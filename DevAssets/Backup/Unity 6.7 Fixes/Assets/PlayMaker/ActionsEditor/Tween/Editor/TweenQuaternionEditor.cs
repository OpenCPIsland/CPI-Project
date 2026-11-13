// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEditor;

// Note: We're fully qualifying tween types to avoid conflicts with NGUI.
// NGUI doesn't use namespaces for its tween scripts :(
// Also Tween is a common name, and others might do the same! 

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenQuaternion))]
	public class TweenQuaternionEditor : TweenEditorBase
	{
        protected PlayMaker.Actions.TweenQuaternion tweenAction;

        public override void OnEnable()
        {
            base.OnEnable();

            tweenAction = target as PlayMaker.Actions.TweenQuaternion;
        }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("variable");

            EditField("interpolation");

            EditField("fromOption");
            DoTargetValueGUI(tweenAction.fromOption, "fromValue");

            EditField("toOption");
            DoTargetValueGUI(tweenAction.toOption, "toValue");

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }

        protected void DoTargetValueGUI(TargetValueOptions option, string valueFieldName)
        {
            switch (option)
            {
                case TargetValueOptions.CurrentValue:
                    break;
                case TargetValueOptions.Offset:
                    EditField(valueFieldName, "Offset");
                    break;
                case TargetValueOptions.Value:
                    EditField(valueFieldName, "Value");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}