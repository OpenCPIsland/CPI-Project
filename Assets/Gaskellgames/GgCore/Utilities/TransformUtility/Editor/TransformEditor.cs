#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_TransformUtilities
    /// </summary>

    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    public class TransformEditor : Editor
    {
        #region Serialized Properties & Variables
        
        private Type transformInspectorType => typeof(EditorApplication).Assembly.GetType("UnityEditor.TransformInspector");
        private Editor transformEditor;
        private List<Transform> transformTargets = new List<Transform>();
        private Texture iconTexture;
        private Rect[] repaintPositions;
        private GUIStyle iconButtonStyle = new GUIStyle();
        private GUIStyle buttonStyle2 = new GUIStyle();
        
        private readonly float standardGap = 4; // double standard gap width
        private static bool utilitiesOpen;

        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region OnEnable / OnDisable

        private void OnEnable()
        {
            AssignTransformEditor();
        }

        private void OnDisable()
        {
            ClearTransformEditor();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // null check
            if (!transformEditor)
            {
                base.OnInspectorGUI();
                return;
            }
            
            // get reference changes
            serializedObject.Update();
            
            GUI.changed = false;
            if (EditorWindowUtility.settings.enableTransformInspector == GaskellgamesHubSettings_SO.TransformInspector.DefaultUnity)
            {
                transformEditor.OnInspectorGUI();
            }
            else
            {
                repaintPositions = new Rect[6];
                CreateButtons();
            
                // draw inspector
                OnInspectorGUI_WrappedTransform();
                if (EditorWindowUtility.settings.enableTransformInspector == GaskellgamesHubSettings_SO.TransformInspector.TransformUtilities)
                {
                    OnInspectorGUI_TransformUtilities();
                }
            
                // force update window (to have snappy hover on buttons) if mouse over buttons
                foreach (var repaintPosition in repaintPositions)
                {
                    if (repaintPosition.Contains(Event.current.mousePosition)) { Repaint(); }
                }
            }
            
            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        private void OnInspectorGUI_WrappedTransform()
        {
            GUILayout.BeginHorizontal();
            
            // default transform
            GUILayout.BeginVertical();
            transformEditor.OnInspectorGUI();
            GUILayout.EndVertical();
            
            // reset buttons
            GUILayout.BeginVertical(GUILayout.Width(20));
            iconTexture = EditorGUIUtility.IconContent("d_Refresh").image;
            if (GUILayout.Button(iconTexture, iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localPosition == Vector3.zero) { continue;}
                    
                    Undo.RecordObject(transform, $"Local position reset for {transform.gameObject.name}");
                    transform.localPosition = Vector3.zero;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local position reset for '{names}'", this);
                }
            }
            repaintPositions[0] = GUILayoutUtility.GetLastRect();
            GUILayout.Space(1);
            iconTexture = EditorGUIUtility.IconContent("d_Refresh").image;
            if (GUILayout.Button(iconTexture, iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localEulerAngles == Vector3.zero) { continue;}
                    Undo.RecordObject(transform, $"Local rotation reset for {transform.gameObject.name}");
                    transform.localEulerAngles = Vector3.zero;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local rotation reset for '{names}'", this);
                }
            }
            repaintPositions[1] = GUILayoutUtility.GetLastRect();
            GUILayout.Space(1);
            iconTexture = EditorGUIUtility.IconContent("d_Refresh").image;
            if (GUILayout.Button(iconTexture, iconButtonStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                string names = "";
                foreach (var transform in transformTargets)
                {
                    if (transform.localScale == Vector3.one) { continue;}
                    Undo.RecordObject(transform, $"Local scale reset for {transform.gameObject.name}");
                    transform.localScale = Vector3.one;
                    names += names == "" ? transform.gameObject.name : ", " + transform.gameObject.name;
                }
                if (names != "")
                {
                    AssignTransformEditor();
                    Debug.Log($"Local scale reset for '{names}'", this);
                }
            }
            repaintPositions[2] = GUILayoutUtility.GetLastRect();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        private void OnInspectorGUI_TransformUtilities()
        {
            // get & update references
            Color defaultBackground = GUI.backgroundColor;
            
            // scale warning: top
            EditorGUILayout.Space();
            DrawScaleWarning(transformTargets, 1, -2);
            
            // utilities
            EditorExtensions.BeginCustomInspectorBackground(InspectorExtensions.backgroundNormalColor, 1, -3);
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = new Color32(223, 223, 223, 079);
            if (GUILayout.Button(new GUIContent("Transform Utilities", "View global properties"), buttonStyle2, GUILayout.Width(100), GUILayout.Height(buttonStyle2.fontSize)))
            {
                utilitiesOpen = !utilitiesOpen;
            }
            repaintPositions[3] = GUILayoutUtility.GetLastRect();
            GUI.backgroundColor = defaultBackground;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            if (utilitiesOpen)
            {
                GUILayout.Space(-1);
                EditorExtensions.DrawInspectorLineFull(InspectorExtensions.backgroundSeperatorColor, 2, 2);
                GUILayout.Space(2);
                GUI.enabled = false;
                GUILayout.Space(2);
                if (1 == transformTargets.Count && NonUniformScaleInObject(transformTargets[0]) || NonUniformScaleInParent(transformTargets))
                {
                    // non-uniform scale
                    GUI.enabled = true;
                    EditorGUILayout.HelpBox("Non-uniform scale detected. It is recommended to keep scale at '1, 1, 1' where possible.", MessageType.Warning);
                    GUI.enabled = false;
                }
                GUILayout.Space(2);
                OnInspectorGUI_GlobalPositions();
                EditorExtensions.DrawInspectorLineFull(InspectorExtensions.backgroundSeperatorColor, 2, 2);
                OnInspectorGUI_AssetTags();
                GUILayout.Space(2);
                GUI.enabled = true;
            }
            GUILayout.Space(2);
            EditorExtensions.EndCustomInspectorBackground();
            
            // scale warning: bottom
            DrawScaleWarning(transformTargets, 2, -3);
        }
        
        private void OnInspectorGUI_GlobalPositions()
        {
            if (1 == targets.Length)
            {
                // global transform properties
                EditorGUILayout.Vector3Field(new GUIContent("Global Position", "The world space position of the Transform."), transformTargets[0].position);
                EditorGUILayout.Vector3Field(new GUIContent("Global Rotation", "A Quaternion that stores the rotation of the Transform in world space."), transformTargets[0].eulerAngles);
                EditorGUILayout.Vector3Field(new GUIContent("Lossy Scale", "The global scale of the object (Read Only)."), transformTargets[0].lossyScale);
            }
            else
            {
                EditorGUILayout.LabelField("Global properties not available when multiple objects are selected.");
            }
        }

        private void OnInspectorGUI_AssetTags()
        {
            if (1 == targets.Length)
            {
                // prefab labels
                EditorGUILayout.LabelField(new GUIContent("Asset Labels:", "If this gameObject is a prefab, all labels applied to it will show here."));
                Object sourceObject = PrefabUtility.GetCorrespondingObjectFromSource(transformTargets[0].gameObject);
                string[] labels = EditorExtensions.GetAllObjectLabels(sourceObject);
                if (0 < labels.Length)
                {
                    float labelLineWidth = 0;
                    EditorGUILayout.BeginHorizontal();
                    Color defaultBackground = GUI.backgroundColor;
                    GUI.backgroundColor = new Color32(179, 179, 179, 255);
                    for (int i = 0; i < labels.Length; i++)
                    {
                        float labelWidth = StringExtensions.GetStringWidth(labels[i]) + (standardGap * 1.5f);
                        labelLineWidth += labelWidth + standardGap;
                        if (Screen.width <= labelLineWidth + standardGap)
                        {
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            labelLineWidth = labelWidth + standardGap;
                        }
                        GUILayout.Button(labels[i], GUILayout.Width(labelWidth));
                    }
                    GUILayout.FlexibleSpace();
                    GUI.backgroundColor = defaultBackground;
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField("n/a");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Labels not available when multiple objects are selected.");
            }
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Private Functions

        private void ClearTransformEditor()
        {
            // clear target transforms
            transformTargets = new List<Transform>();
            
            // destroy editor instance if exists
            if (!transformEditor) { return; }
            MethodInfo disableMethod = transformEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (disableMethod != null) { disableMethod.Invoke(transformEditor, null); }
            DestroyImmediate(transformEditor);
            transformEditor = null;
        }
        
        private void AssignTransformEditor()
        {
            ClearTransformEditor();
            
            // null checks
            if (target == null || transformInspectorType == null) { return; }
            if (1 < targets.Length && targets[0] != null)
            {
                // assign selected targets
                transformTargets.Clear();
                foreach (var targetObject in targets)
                {
                    Transform transform = targetObject as Transform;
                    if (transform && !transformTargets.Contains(transform))
                    {
                        transformTargets.Add(transform);
                    }
                }
                
                // assign transform editor
                transformEditor = CreateEditorWithContext(targets, target, transformInspectorType);
            }
            else
            {
                // assign selected targets
                transformTargets.Clear();
                Transform transform = target as Transform;
                transformTargets.Add(transform);
                
                // assign transform editor
                transformEditor = CreateEditor(target, transformInspectorType);
            }
        }
        
        private void DrawScaleWarning(List<Transform> transformTargets, float paddingTop = -4F, float paddingBottom = -15F, float paddingLeft = -18F, float paddingRight = -4F)
        {
            if (1 == transformTargets.Count && NonUniformScaleInObject(transformTargets[0]))
            {
                EditorExtensions.BeginCustomInspectorBackground(new Color32(223, 050, 050, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EditorExtensions.EndCustomInspectorBackground();
            }
            else if (NonUniformScaleInParent(transformTargets))
            {
                EditorExtensions.BeginCustomInspectorBackground(new Color32(179, 128, 000, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EditorExtensions.EndCustomInspectorBackground();
            }
            else
            {
                EditorExtensions.BeginCustomInspectorBackground(new Color32(028, 128, 028, 255), paddingTop, paddingBottom, paddingLeft, paddingRight);
                EditorExtensions.EndCustomInspectorBackground();
            }
        }
        
        private bool NonUniformScaleInParent(List<Transform> transformTargets)
        {
            foreach (var transformTarget in transformTargets)
            {
                if (transformTarget.lossyScale != Vector3.one) { return true; }
            }
            
            return false;
        }

        private bool NonUniformScaleInObject(Transform transformTarget)
        {
            return transformTarget.localScale != Vector3.one;
        }
        
        private void CreateButtons()
        {
            // button style 1 (reset button)
            iconButtonStyle.fontSize = 9;
            iconButtonStyle.alignment = TextAnchor.MiddleCenter;
            iconButtonStyle.normal.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.hover.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.active.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.normal.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.blankColor, InspectorExtensions.blankColor);
            iconButtonStyle.hover.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.buttonHoverColor, InspectorExtensions.blankColor);
            iconButtonStyle.active.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.buttonActiveColor, InspectorExtensions.buttonActiveBorderColor);

            // button style 2 (utilities)
            buttonStyle2.fontSize = 10;
            buttonStyle2.normal.textColor = InspectorExtensions.textDisabledColor;
        }

        #endregion

    } // class end
}
#endif
