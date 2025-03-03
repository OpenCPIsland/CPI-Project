// (c) Copyright HutongGames, all rights reserved.
// See also: EasingFunctionLicense.txt

using System;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEngine;

// Note: We're fully qualifying tween types to avoid conflicts with NGUI.
// NGUI doesn't use namespaces for its tween scripts :(
// Also Tween is a common name, and others might do the same! 

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenCamera))]
    public class TweenCameraEditor : TweenEditorBase
    {
        private PlayMaker.Actions.TweenCamera tweenAction;

        public override void OnEnable()
        {
            base.OnEnable();

            tweenAction = (PlayMaker.Actions.TweenCamera) target;
        }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("gameObject");
            
            DoPropertySelector();

            EditField("tweenDirection", "Tween");

            switch (tweenAction.property)
            {
                case PlayMaker.Actions.TweenCamera.CameraProperty.BackgroundColor:
                    EditField("targetColor", "Color");
                    break;
                case PlayMaker.Actions.TweenCamera.CameraProperty.Aspect:
                case PlayMaker.Actions.TweenCamera.CameraProperty.FieldOfView:
                case PlayMaker.Actions.TweenCamera.CameraProperty.OrthoSize:
                    EditField("targetFloat", "Value");
                    break;
                case PlayMaker.Actions.TweenCamera.CameraProperty.PixelRect:
                case PlayMaker.Actions.TweenCamera.CameraProperty.ViewportRect:
                    EditField("targetRect", "Rect");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }

        private void DoPropertySelector()
        {
            EditorGUI.BeginChangeCheck();
            EditField("property");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.targetColor = new FsmColor { Value = Color.black };
                tweenAction.targetFloat = new FsmFloat { Value = 0 };
                tweenAction.targetRect = new FsmRect { Value = new Rect(0,0,1,1)};
            }
        }

    }
}