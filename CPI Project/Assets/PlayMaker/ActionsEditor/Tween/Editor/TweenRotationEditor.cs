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
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenRotation))]
	public class TweenRotationEditor : TweenEditorBase
    {
        private PlayMaker.Actions.TweenRotation tweenAction;

	    public override void OnEnable()
	    {
            base.OnEnable();

	        tweenAction = (PlayMaker.Actions.TweenRotation) target;
	    }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("gameObject");

            EditorGUI.BeginChangeCheck();
            EditField("fromOptions");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.fromTarget.Value = null;
                tweenAction.fromRotation.Value = Vector3.zero;
                FsmEditor.SaveActions();
            }

            DoOptionUI(tweenAction.fromOptions, "fromRotation", "fromTarget");

            EditorGUI.BeginChangeCheck();
            EditField("toOptions");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.toTarget.Value = null;
                tweenAction.toRotation.Value = Vector3.zero;
                FsmEditor.SaveActions();
            }

            DoOptionUI(tweenAction.toOptions, "toRotation", "toTarget");

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }

        private void DoOptionUI(RotationOptions option, string rotationField, string targetField )
        {
            switch (option)
            {
                case RotationOptions.CurrentRotation:
                    break;
                case RotationOptions.WorldRotation:
                    EditField(rotationField, "World Rotation");
                    break;
                case RotationOptions.LocalRotation:
                    EditField(rotationField, "Local Rotation");
                    break;
                case RotationOptions.WorldOffsetRotation:
                    EditField(rotationField, "World Offset Rotation");
                    break;
                case RotationOptions.LocalOffsetRotation:
                    EditField(rotationField, "Local Offset Rotation");
                    break;
                case RotationOptions.MatchGameObjectRotation:
                    EditField(targetField, "GameObject");
                    EditField(rotationField, "Local Offset Rotation");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        public override void OnSceneGUI()
        {
            if (Application.isPlaying) return;

            tweenAction = target as PlayMaker.Actions.TweenRotation;
            if (tweenAction == null) // shouldn't happen!
            {
                return;
            }

            // setup start and end positions

            var go = ActionHelpers.GetOwnerDefault(tweenAction, tweenAction.gameObject);
            if (go == null) return;

            // setup handles
      
            var transform = go.transform;
            //var position = transform.position;
            //var rotation = transform.rotation;
            //var handleSize = HandleUtility.GetHandleSize(position);

            var startRotation = Quaternion.identity;
            if (PlayMaker.Actions.TweenHelpers.GetTargetRotation(tweenAction.fromOptions, transform, 
                tweenAction.fromRotation, tweenAction.fromTarget, out startRotation))
            {
                ActionHelpers.DrawWireBounds(transform, startRotation, PlayMakerPrefs.TweenFromColor);
            }

            var endRotation = Quaternion.identity;
            if (PlayMaker.Actions.TweenHelpers.GetTargetRotation(tweenAction.toOptions, transform, 
                tweenAction.toRotation, tweenAction.toTarget, out endRotation))
            {
                ActionHelpers.DrawWireBounds(transform, endRotation, PlayMakerPrefs.TweenToColor);
            }

            /*
            Transform fromTransform = null;
            if (tweenAction.fromTarget != null)
            {
                fromTransform = tweenAction.fromTarget.Value != null ? tweenAction.fromTarget.Value.transform : null;
            }

            Transform toTransform = null;
            if (tweenAction.toTarget != null)
            {
                toTransform = tweenAction.toTarget.Value != null ? tweenAction.toTarget.Value.transform : null;
            }

            var startRotation = ActionHelpers.GetTargetRotation(tweenAction.fromOptions, transform, fromTransform, tweenAction.fromRotation.Value);
            var endRotation = ActionHelpers.GetTargetRotation(tweenAction.toOptions, transform, toTransform, tweenAction.toRotation.Value);

            var showFromHandles = ActionHelpers.CanEditTargetRotation(tweenAction.fromOptions, tweenAction.fromRotation, tweenAction.fromTarget);
            if (showFromHandles)
            {
                ActionHelpers.DrawWireBounds(transform, startRotation, PlayMakerPrefs.TweenFromColor);
                // Need a custom control for this?
                // tweenAction.fromRotation.Value = Handles.RotationHandle(tweenAction.fromRotation.Value, transform.position);
            }

            var showToHandles = ActionHelpers.CanEditTargetRotation(tweenAction.toOptions, tweenAction.toRotation, tweenAction.toTarget);
            if (showToHandles)
            {
                ActionHelpers.DrawWireBounds(transform, endRotation, PlayMakerPrefs.TweenToColor);
                // Need a custom control for this?
                // tweenAction.fromRotation.Value = Handles.RotationHandle(tweenAction.fromRotation.Value, transform.position);
            }*/

            if (GUI.changed)
            {
                FsmEditor.SaveActions();
            }

        }
	}
}