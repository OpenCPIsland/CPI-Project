// (c) Copyright HutongGames, LLC. All rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEditor;
using UnityEngine;

// Note: We're fully qualifying tween types to avoid conflicts with NGUI.
// NGUI doesn't use namespaces for its tween scripts :(
// Also Tween is a common name, and others might do the same! 

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenUiPosition))]
	public class TweenUiPositionEditor : TweenEditorBase
    {
        private PlayMaker.Actions.TweenUiPosition tweenAction;

	    public override void OnEnable()
	    {
            base.OnEnable();

	        tweenAction = (PlayMaker.Actions.TweenUiPosition) target;
	    }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("gameObject");

            EditorGUI.BeginChangeCheck();
            EditField("fromOption");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.fromTarget.Value = null;
                tweenAction.fromPosition.Value = Vector3.zero;
                FsmEditor.SaveActions();
            }

            DoOptionsGUI(tweenAction.fromOption, "fromPosition", "fromTarget");

            EditorGUI.BeginChangeCheck();
            EditField("toOption");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.toTarget.Value = null;
                tweenAction.toPosition.Value = Vector3.zero;
                FsmEditor.SaveActions();
            }

            DoOptionsGUI(tweenAction.toOption, "toPosition", "toTarget");

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }

        private void DoOptionsGUI(UiPositionOptions option, string positionField, string targetField )
        {
            switch (option)
            {
                case UiPositionOptions.CurrentPosition:
                    break;
                case UiPositionOptions.Position:
                    EditField(positionField, "Position");
                    break;
                case UiPositionOptions.Offset:
                    EditField(positionField, "Offset");
                    break;
                case UiPositionOptions.OffscreenTop:
                case UiPositionOptions.OffscreenBottom:
                case UiPositionOptions.OffscreenLeft:
                case UiPositionOptions.OffscreenRight:
                    break;
                case UiPositionOptions.TargetGameObject:
                    EditField(targetField, "GameObject");
                    EditField(positionField, "Offset");
                    break;

                default:
                    throw new ArgumentOutOfRangeException("option", option, null);
            }
        }

        public override void OnSceneGUI()
        {
            /*
            if (Application.isPlaying) return;

            // setup start and end positions
            
            // TODO

            if (EditorGUI.EndChangeCheck())
            {
                FsmEditor.SaveActions();
            }*/
        }



	}
}