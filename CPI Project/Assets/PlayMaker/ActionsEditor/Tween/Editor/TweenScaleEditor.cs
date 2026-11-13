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
    [CustomActionEditor(typeof(PlayMaker.Actions.TweenScale))]
	public class TweenScaleEditor : TweenEditorBase
    {
        private PlayMaker.Actions.TweenScale tweenAction;

	    public override void OnEnable()
	    {
            base.OnEnable();

	        tweenAction = (PlayMaker.Actions.TweenScale) target;
	    }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditField("gameObject");

            EditorGUI.BeginChangeCheck();
            EditField("fromOptions");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.fromTarget = null;
                ResetSharedParameter(ref tweenAction.fromScale);
                FsmEditor.SaveActions();
            }

            switch (tweenAction.fromOptions)
            {
                case ScaleOptions.CurrentScale:
                    break;
                case ScaleOptions.LocalScale:
                    EditField("fromScale", "Local Scale");
                    break;
                case ScaleOptions.MultiplyScale:
                    EditField("fromScale", "Multiply Scale");
                    break;
                case ScaleOptions.AddToScale:
                    EditField("fromScale", "Add To Scale");
                    break;
                case ScaleOptions.MatchGameObject:
                    EditField("fromTarget", "GameObject");
                    EditField("fromScale", "Multiplier");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.BeginChangeCheck();
            EditField("toOptions");
            if (EditorGUI.EndChangeCheck())
            {
                tweenAction.toTarget = null;
                ResetSharedParameter(ref tweenAction.toScale);
                FsmEditor.SaveActions();
            }

            switch (tweenAction.toOptions)
            {
                case ScaleOptions.CurrentScale:
                    break;
                case ScaleOptions.LocalScale:
                    EditField("toScale", "Local Scale");
                    break;
                case ScaleOptions.MultiplyScale:
                    EditField("toScale", "Multiply Scale");
                    break;
                case ScaleOptions.AddToScale:
                    EditField("toScale", "Add To Scale");
                    break;
                case ScaleOptions.MatchGameObject:
                    EditField("toTarget", "GameObject");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DoEasingUI();

            return EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// Reset scale parameter with sensible default value when changing options
        /// </summary>
        private void ResetSharedParameter(ref FsmVector3 scale)
        {
            if (scale.UsesVariable) return; // don't reset variable value!

            switch (tweenAction.toOptions)
            {
                case ScaleOptions.LocalScale:
                    var go = ActionHelpers.GetOwnerDefault(tweenAction, tweenAction.gameObject);
                    scale.Value = go != null ? go.transform.localScale : Vector3.one;
                    break;
                case ScaleOptions.MultiplyScale:
                    scale.Value = Vector3.one;
                    break;
                case ScaleOptions.AddToScale:
                    scale.Value = Vector3.zero;
                    break;
            }
        }

        private Vector3 tempScale = new Vector3();

        public override void OnSceneGUI()
        {
            if (Application.isPlaying) return;

            tweenAction = target as PlayMaker.Actions.TweenScale;
            if (tweenAction == null) // shouldn't happen!
            {
                return;
            }

            // setup start and end positions

            var go = ActionHelpers.GetOwnerDefault(tweenAction, tweenAction.gameObject);
            if (go == null) return;

            // handles

            var transform = go.transform;
            var position = transform.position;
            var handleSize = HandleUtility.GetHandleSize(position);

            var showFromHandles = false;
            switch (tweenAction.fromOptions)
            {
                case ScaleOptions.CurrentScale:
                    break;

                case ScaleOptions.LocalScale:
                    if (tweenAction.fromScale.IsNone) break; 
                    tempScale.Set(tweenAction.fromScale.Value.x/transform.localScale.x, 
                                  tweenAction.fromScale.Value.y/transform.localScale.y,
                                  tweenAction.fromScale.Value.z/transform.localScale.z);
                    ActionHelpers.DrawWireBounds(transform, tempScale, PlayMakerPrefs.TweenFromColor);
                    showFromHandles = true;
                    break;

                case ScaleOptions.MultiplyScale:
                    if (tweenAction.fromScale.IsNone) break;
                    ActionHelpers.DrawWireBounds(transform, tweenAction.fromScale.Value, PlayMakerPrefs.TweenFromColor);
                    showFromHandles = true;
                    break;

                case ScaleOptions.AddToScale:
                    if (tweenAction.fromScale.IsNone) break; 
                    tempScale.Set((tweenAction.fromScale.Value.x + transform.localScale.x)/transform.localScale.x, 
                                  (tweenAction.fromScale.Value.y + transform.localScale.y)/transform.localScale.y,
                                  (tweenAction.fromScale.Value.z + transform.localScale.z)/transform.localScale.z);
                    ActionHelpers.DrawWireBounds(transform, tempScale, PlayMakerPrefs.TweenFromColor);
                    showFromHandles = true;
                    break;


                case ScaleOptions.MatchGameObject:
                    var fromGo = tweenAction.fromTarget.Value;
                    if (fromGo == null) break;
                    ActionHelpers.DrawWireBounds(transform, fromGo.transform.localScale, PlayMakerPrefs.TweenFromColor);
                    showFromHandles = true;
                    break;

                /*
                case ScaleOptions.MatchGameObjectMultiply:
                    var fromGo = tweenAction.fromTarget.Value;
                    if (fromGo == null) break;
                    var matchScale = fromGo.transform.localScale;
                    tempScale.Set(tweenAction.fromScale.Value.x * matchScale.x / transform.localScale.x, 
                        tweenAction.fromScale.Value.y * matchScale.y / transform.localScale.y,
                        tweenAction.fromScale.Value.z * matchScale.z / transform.localScale.z);
                    ActionHelpers.DrawWireBounds(transform, tempScale, PlayMakerPrefs.TweenFromColor);
                    showFromHandles = true;
                    break;*/

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (showFromHandles)
            {
                tweenAction.fromScale.Value = ActionHelpers.SingleColorScaleHandle(go, tweenAction.fromScale.Value,
                    handleSize * 1.4f, PlayMakerPrefs.TweenFromColor);
            }

            var showToHandles = false;
            switch (tweenAction.toOptions)
            {
                case ScaleOptions.CurrentScale:
                    break;

                case ScaleOptions.LocalScale:
                    if (tweenAction.toScale.IsNone) break; 
                    tempScale.Set(tweenAction.toScale.Value.x/transform.localScale.x, 
                                  tweenAction.toScale.Value.y/transform.localScale.y,
                                  tweenAction.toScale.Value.z/transform.localScale.z);
                    ActionHelpers.DrawWireBounds(transform, tempScale, PlayMakerPrefs.TweenToColor);
                    showToHandles = true;
                    break;

                case ScaleOptions.MultiplyScale:
                    if (tweenAction.toScale.IsNone) break;
                    ActionHelpers.DrawWireBounds(transform, tweenAction.toScale.Value, PlayMakerPrefs.TweenToColor);
                    showToHandles = true;
                    break;

                case ScaleOptions.AddToScale:
                    if (tweenAction.toScale.IsNone) break; 
                    tempScale.Set((tweenAction.toScale.Value.x + transform.localScale.x) / transform.localScale.x, 
                                  (tweenAction.toScale.Value.y + transform.localScale.y) / transform.localScale.y,
                                  (tweenAction.toScale.Value.z + transform.localScale.z) / transform.localScale.z);
                    ActionHelpers.DrawWireBounds(transform, tempScale, PlayMakerPrefs.TweenToColor);
                    showToHandles = true;
                    break;

                case ScaleOptions.MatchGameObject:
                    var toGo = tweenAction.toTarget.Value;
                    if (toGo == null) break;
                    var matchScale = toGo.transform.localScale;
                    tempScale.Set(tweenAction.toScale.Value.x * matchScale.x / transform.localScale.x, 
                                  tweenAction.toScale.Value.y * matchScale.y / transform.localScale.y,
                                  tweenAction.toScale.Value.z * matchScale.z / transform.localScale.z);
                    ActionHelpers.DrawWireBounds(transform, tempScale, PlayMakerPrefs.TweenToColor);
                    showToHandles = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (showToHandles)
            {
                tweenAction.toScale.Value = ActionHelpers.SingleColorScaleHandle(go, tweenAction.toScale.Value,
                    handleSize * 1.8f, PlayMakerPrefs.TweenToColor);
            }

            if (GUI.changed)
            {
                FsmEditor.SaveActions();
            }
        }
	}
}