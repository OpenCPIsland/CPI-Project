// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.TweenEnums;
using UnityEditor;
using UnityEngine;

// Note: We're fully qualifying tween types to avoid conflicts with NGUI.
// NGUI doesn't use namespaces for its tween scripts :(
// Also Tween is a common name, and others might do the same! 

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenPosition))]
	public class TweenPositionEditor : TweenEditorBase
    {
        private PlayMaker.Actions.TweenPosition tweenAction;

	    public override void OnEnable()
	    {
            base.OnEnable();

	        tweenAction = (PlayMaker.Actions.TweenPosition) target;
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

        private void DoOptionsGUI(PositionOptions option, string positionField, string targetField )
        {
            switch (option)
            {
                case PositionOptions.CurrentPosition:
                    break;
                case PositionOptions.WorldPosition:
                    EditField(positionField, "World Position");
                    break;
                case PositionOptions.LocalPosition:
                    EditField(positionField, "Local Position");
                    break;                   
                case PositionOptions.WorldOffset:
                    EditField(positionField, "World Offset");
                    break;
                case PositionOptions.LocalOffset:
                    EditField(positionField, "Local Offset");
                    break;
                case PositionOptions.TargetGameObject:
                    EditField(targetField, "GameObject");
                    EditField(positionField, "Local Offset");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnSceneGUI()
        {
            if (Application.isPlaying) return;

            tweenAction = target as PlayMaker.Actions.TweenPosition;
            if (tweenAction == null) // shouldn't happen!
            {
                return;
            }

            // setup start and end positions

            var go = ActionHelpers.GetOwnerDefault(tweenAction, tweenAction.gameObject);
            if (go == null) return;

            var transform = go.transform;
            var startPos = new Vector3();
            var endPos = new Vector3();

            if (!PlayMaker.Actions.TweenHelpers.GetTargetPosition(tweenAction.fromOption, transform, 
                tweenAction.fromPosition, tweenAction.fromTarget, out startPos))
                return;

            if (!PlayMaker.Actions.TweenHelpers.GetTargetPosition(tweenAction.toOption, transform, 
                tweenAction.toPosition, tweenAction.toTarget, out endPos))
                return;

            EditorGUI.BeginChangeCheck();

            if (PlayMaker.Actions.TweenHelpers.CanEditTargetPosition(tweenAction.fromOption, tweenAction.fromPosition, tweenAction.fromTarget))
            {
                tweenAction.fromPosition.Value = PlayMaker.Actions.TweenHelpers.DoTargetPositionHandle(startPos, tweenAction.fromOption,
                    transform, tweenAction.fromTarget);
            }
            
            if (PlayMaker.Actions.TweenHelpers.CanEditTargetPosition(tweenAction.toOption, tweenAction.toPosition, tweenAction.toTarget))
            {
                tweenAction.toPosition.Value = PlayMaker.Actions.TweenHelpers.DoTargetPositionHandle(endPos, tweenAction.toOption,
                    transform, tweenAction.toTarget);
            }

            var rotation = transform.rotation;
            ActionHelpers.DrawWireBounds(transform, startPos, rotation, PlayMakerPrefs.TweenFromColor);
            ActionHelpers.DrawWireBounds(transform, endPos, rotation, PlayMakerPrefs.TweenToColor);            
            ActionHelpers.DrawArrow(startPos, endPos, PlayMakerPrefs.TweenToColor);

            if (EditorGUI.EndChangeCheck())
            {
                FsmEditor.SaveActions();
            }

        }



	}
}