using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace ASoliman.Utils.ClipboardPlus
{
    /// <summary>
    /// Handles the rendering and preview functionality for clipboard data in the Unity Editor.
    /// Provides visual representation of component data with expandable UI elements and property inspection.
    /// </summary>
    public static class ClipPreviewRenderer
    {
        private static readonly Color _headerColor = new Color(0.235f, 0.235f, 0.235f, 1f);
        private static GUIStyle _foldButtonStyle;
        private static GUIStyle _headerLabelStyle;
        private static GUIStyle _boxStyle;

        public static readonly Color FavoriteActiveColor = new Color(1f, 0.92f, 0.016f, 1f);
        public static GUIStyle ModernButtonStyle { get; private set; }
        public static GUIContent PasteContent { get; private set; }
        public static GUIContent RemoveContent { get; private set; }

        private static void InitializeStyles()
        {
            if (_foldButtonStyle != null) return;

            _foldButtonStyle = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(2, 2, 2, 2),
                margin = new RectOffset(0, 0, 0, 0),
                fixedHeight = 20,
                fixedWidth = 20,
                alignment = TextAnchor.MiddleCenter
            };

            _headerLabelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                padding = new RectOffset(4, 4, 0, 0),
                fontSize = 11
            };

            _boxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            };

            InitializeModernButtonStyle();
        }

        private static void InitializeModernButtonStyle()
        {
            if (ModernButtonStyle != null) return;

            ModernButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(2, 2, 2, 2),
                fontSize = 11,
                fixedHeight = 24,
                richText = true
            };

            PasteContent = new GUIContent(" Paste", Resources.Load<Texture2D>("clip_paste"));
            RemoveContent = new GUIContent(" Remove", EditorGUIUtility.IconContent("TreeEditor.Trash").image);
        }

        /// <summary>
        /// Calculates the total height needed to display the clip preview.
        /// </summary>
        public static float GetPreviewHeight(ClipData clip)
        {
            if (!clip.isExpanded) return 24f; // Header height only

            float totalHeight = 24f; // Header height
            
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(clip.serializedData);
                if (clip.clipType == ClipType.Component)
                {
                    totalHeight += CalculateComponentHeight(json, clip.DataType);
                }
            }
            catch (Exception)
            {
                totalHeight += 50f; // Error message height
            }

            return totalHeight + 20f; // Add padding
        }

        /// <summary>
        /// Draws the complete preview UI for the given clip data.
        /// </summary>
        public static void DrawPreview(ClipData clip, Rect previewRect)
        {
            if (clip == null) return;
            InitializeStyles();

            DrawHeader(clip, previewRect);
            if (!clip.isExpanded) return;
            
            DrawContent(clip, previewRect);
        }

        private static float CalculateComponentHeight(string json, Type componentType)
        {
            float height = 0f;
            GameObject tempGO = null;
            
            try
            {
                var serializedObject = CreateTemporarySerializedObject(json, componentType);
                if (serializedObject != null)
                {
                    height = CalculatePropertyHeight(serializedObject);
                    tempGO = (serializedObject.targetObject as Component)?.gameObject;
                }
            }
            finally
            {
                if (tempGO != null)
                {
                    GameObject.DestroyImmediate(tempGO);
                }
            }
            
            return height + 10f; // Add padding
        }

        private static float CalculatePropertyHeight(SerializedObject serializedObject)
        {
            float height = 0f;
            var property = serializedObject.GetIterator();
            
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name == "m_Script") continue;
                    height += EditorGUI.GetPropertyHeight(property, true) + 2f;
                }
                while (property.NextVisible(false));
            }

            return height;
        }

        private static void DrawHeader(ClipData clip, Rect previewRect)
        {
            var headerRect = new Rect(previewRect.x, previewRect.y, previewRect.width, 24);
            EditorGUI.DrawRect(headerRect, _headerColor);

            float infoWidth = headerRect.width - 10;
            var sourceRect = new Rect(headerRect.x + 5, headerRect.y + 4, infoWidth / 2, EditorGUIUtility.singleLineHeight);
            var createdRect = new Rect(sourceRect.xMax, headerRect.y + 4, infoWidth / 2, EditorGUIUtility.singleLineHeight);

            string sourceText = FormatSourceText(clip);
            
            EditorGUI.LabelField(sourceRect, sourceText, _headerLabelStyle);
            EditorGUI.LabelField(createdRect, $"{clip.creationDate:MM/dd/yy - hh:mm:ss tt}", _headerLabelStyle);
        }

        private static string FormatSourceText(ClipData clip)
        {
            if (clip.clipType != ClipType.Component) 
                return string.IsNullOrEmpty(clip.sourcePath) ? clip.sourceType : clip.sourcePath;
            
            var componentName = clip.sourceType.Split('.').Last();
            return $"{clip.sourcePath}.{componentName}";
        }

        private static void DrawContent(ClipData clip, Rect previewRect)
        {
            var contentRect = new Rect(
                previewRect.x + 5,
                previewRect.y + 29, // Header height (24) + padding (5)
                previewRect.width - 10,
                previewRect.height - 34 // Total padding
            );

            try
            {
                if (clip.clipType == ClipType.Component)
                {
                    DrawComponentInspectorPreview(clip, contentRect);
                }
            }
            catch (Exception e)
            {
                DrawErrorPreview(contentRect, e.Message);
            }
        }

        private static SerializedObject CreateTemporarySerializedObject(string json, Type componentType)
        {
            GameObject tempGO = null;
            try
            {
                if (componentType == typeof(Transform) || componentType == typeof(RectTransform))
                {
                    return CreateTransformSerializedObject(json, componentType, ref tempGO);
                }
                
                return CreateGenericSerializedObject(json, componentType, ref tempGO);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating temporary object: {e.Message}");
                if (tempGO != null)
                {
                    GameObject.DestroyImmediate(tempGO);
                }
                return null;
            }
        }

        private static SerializedObject CreateTransformSerializedObject(string json, Type componentType, ref GameObject tempGO)
        {
            if (componentType == typeof(RectTransform))
            {
                tempGO = new GameObject("TempUI", typeof(RectTransform));
                var component = tempGO.GetComponent<RectTransform>();
                tempGO.hideFlags = HideFlags.HideAndDontSave;
                EditorJsonUtility.FromJsonOverwrite(json, component);
                return new SerializedObject(component);
            }
            
            tempGO = new GameObject("Temp");
            var transform = tempGO.transform;
            tempGO.hideFlags = HideFlags.HideAndDontSave;
            EditorJsonUtility.FromJsonOverwrite(json, transform);
            return new SerializedObject(transform);
        }

        private static SerializedObject CreateGenericSerializedObject(string json, Type componentType, ref GameObject tempGO)
        {
            if (typeof(Component).IsAssignableFrom(componentType))
            {
                tempGO = new GameObject("Temp") { hideFlags = HideFlags.HideAndDontSave };
                Component component = tempGO.AddComponent(componentType);
                
                if (component != null)
                {
                    EditorJsonUtility.FromJsonOverwrite(json, component);
                    return new SerializedObject(component);
                }
            }
            else if (componentType.IsSubclassOf(typeof(ScriptableObject)))
            {
                var obj = ScriptableObject.CreateInstance(componentType);
                if (obj != null)
                {
                    EditorJsonUtility.FromJsonOverwrite(json, obj);
                    return new SerializedObject(obj);
                }
            }
            
            return null;
        }
        
        private static void DrawComponentInspectorPreview(ClipData clip, Rect rect)
        {
            var json = System.Text.Encoding.UTF8.GetString(clip.serializedData);
            GUI.Box(rect, "", _boxStyle);
            
            var innerRect = new Rect(
                rect.x + _boxStyle.padding.left,
                rect.y + _boxStyle.padding.top,
                rect.width - _boxStyle.padding.horizontal,
                rect.height - _boxStyle.padding.vertical
            );

            using var serializedObject = CreateTemporarySerializedObject(json, clip.DataType);
            if (serializedObject != null)
            {
                DrawSerializedProperties(serializedObject, innerRect);
            }
        }

        private static void DrawSerializedProperties(SerializedObject serializedObject, Rect rect)
        {
            try
            {
                float yOffset = 0;
                serializedObject.Update();
                var property = serializedObject.GetIterator();
                
                if (property.NextVisible(true))
                {
                    do
                    {
                        if (ShouldSkipProperty(property.name)) continue;

                        float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
                        var propertyRect = new Rect(rect.x, rect.y + yOffset, rect.width, propertyHeight);

                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUI.PropertyField(propertyRect, property, true);
                        }

                        yOffset += propertyHeight + 2;

                    } while (property.NextVisible(false) && yOffset < rect.height);
                }
            }
            finally
            {
                CleanupSerializedObject(serializedObject);
            }
        }

        private static bool ShouldSkipProperty(string propertyName)
        {
            return propertyName == "m_Script" || 
                propertyName == "m_LocalEulerAnglesHint" || 
                propertyName == "m_RootOrder" ||
                propertyName == "m_Father";
        }

        private static void CleanupSerializedObject(SerializedObject serializedObject)
        {
            if (serializedObject == null) return;

            if (serializedObject.targetObject is Component comp && comp != null && comp.gameObject != null)
            {
                GameObject.DestroyImmediate(comp.gameObject);
            }
            else if (serializedObject.targetObject != null)
            {
                GameObject.DestroyImmediate(serializedObject.targetObject);
            }
        }

        private static void DrawErrorPreview(Rect rect, string errorMessage)
        {
            GUI.Box(rect, "", _boxStyle);
            var style = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.red },
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            EditorGUI.LabelField(rect, $"Preview Error: {errorMessage}", style);
        }
    }
}