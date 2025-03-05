#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.Rendering;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code updated by Gaskellgames: https://github.com/Gaskellgames
    /// Original code from vertxxyz: https://gist.github.com/vertxxyz/5a00dbca58aee033b35be2e227e80f8d
    /// </summary>
	
    [CustomPropertyDrawer(typeof(GraphAttribute))]
    public class GraphAttributeDrawer : PropertyDrawer
    {
        #region Variables

        private GraphAttribute attributeAsType;

        private readonly Color graphDivisionColour = new Color32(079, 079, 079, 179);
        private readonly Color xAxiscolour = new Color32(050, 179, 050, 179);
        private readonly Color yAxiscolour = new Color32(223, 050, 050, 179);
        private readonly Color positionColour = new Color32(000, 179, 223, 255);

        private int singleLine = (int)EditorExtensions.singleLine;
        private int gap = (int)EditorExtensions.spacer;
        private int indent = 15;
		
        private const float graphSize = 90f;
        private const float graphSizeQuarter = graphSize * 0.25f;
        private const float graphSizeHalf = graphSize * 0.5f;
        private const float graphSizeThreeQuarters = graphSize * 0.75f;
		
        private int controlID = -1;
		
        private MethodInfo applyWireMaterial;
        private MethodInfo ApplyWireMaterial
        {
            get
            {
                if (applyWireMaterial == null)
                {
                    string name = "ApplyWireMaterial";
                    BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Static;
                    Type[] types = new[] { typeof(CompareFunction) };
					
                    applyWireMaterial = typeof(HandleUtility).GetMethod(name, bindingAttr, null, types, null);
                }

                return applyWireMaterial;
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as GraphAttribute;
            
            return graphSize;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GraphAttribute graphAttribute = attribute as GraphAttribute;
            HandleGraph(position, property, label, graphAttribute);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private void HandleGraph(Rect position, SerializedProperty property, GUIContent label, GraphAttribute graphAttribute)
        {
            // add indent
            Rect indentPosition = new Rect(position.xMin + indent, position.yMin, position.width - indent, position.height);
            
            // cache values
            bool defaultGui = GUI.enabled;
            Rect graphRect = new Rect(indentPosition.xMin, indentPosition.yMin, graphSize, graphSize);
            float labelSliderWidth = indentPosition.width - (graphSize + (gap * 3));
            float labelSliderXMin = indentPosition.xMin + (graphSize + (gap * 3));
            float labelSliderYMin = indentPosition.yMin + ((graphSize - (singleLine * 5)) * 0.5f);
            Rect labelRect = new Rect(labelSliderXMin, labelSliderYMin, labelSliderWidth, singleLine);
            Rect xLabelRect = new Rect(labelSliderXMin, labelRect.yMin + singleLine, labelSliderWidth, singleLine);
            Rect xSliderRect = new Rect(labelSliderXMin, xLabelRect.yMin + singleLine, labelSliderWidth, singleLine);
            Rect yLabelRect = new Rect(labelSliderXMin, xSliderRect.yMin + singleLine, labelSliderWidth, singleLine);
            Rect ySliderRect = new Rect(labelSliderXMin, yLabelRect.yMin + singleLine, labelSliderWidth, singleLine);
            Event currentEvent = Event.current;
			
            EditorGUIUtility.AddCursorRect(graphRect, MouseCursor.MoveArrow);
			
            // check if mouse clicked (inside the graph)
            if (currentEvent.rawType == EventType.MouseDown)
            {
                if (graphRect.Contains(currentEvent.mousePosition))
                {
                    int hash = property.name.GetHashCode();
                    controlID = GUIUtility.GetControlID(hash, FocusType.Passive, graphRect);
                    GUIUtility.hotControl = controlID;
                    currentEvent.Use();
                }
            }

            // check if left mouse un-clicked (anywhere) and right click menu
            if (currentEvent.isMouse && currentEvent.rawType == EventType.MouseUp)
            {
                switch (currentEvent.button)
                {
                    case 0: // left mouse
                        controlID = -1;
                        GUIUtility.hotControl = 0;
                        currentEvent.Use();
                        break;
                    case 1: // right click
                        if (!graphRect.Contains(currentEvent.mousePosition)) { break; }
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Center"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(0.5f, 0.5f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Center Left"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(0.0f, 0.5f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Center Right"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(1.0f, 0.5f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Top"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(0.5f, 1.0f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Top Left"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(0.0f, 1.0f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Top Right"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(1.0f, 1.0f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Bottom"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(0.5f, 0.0f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Bottom Left"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(0.0f, 0.0f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.AddItem(new GUIContent("Bottom Right"), false, () =>
                        {
                            property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, new Vector2(1.0f, 0.0f));
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        menu.ShowAsContext();
                        currentEvent.Use();
                        break;
                }
            }
			
            // set values
            if (GUIUtility.hotControl == controlID)
            {
                if (currentEvent.isMouse && currentEvent.button == 0 && GUI.enabled)
                {
                    // calculate x value
                    Vector2 value = new Vector2();
                    if (currentEvent.mousePosition.x < graphRect.xMin) { value.x = 0; }
                    else if (graphRect.xMax < currentEvent.mousePosition.x) { value.x = 1; }
                    else { value.x = (currentEvent.mousePosition.x - graphRect.xMin) / graphRect.width; }
				
                    // calculate y value
                    if (currentEvent.mousePosition.y < graphRect.yMin) { value.y = 1; }
                    else if (graphRect.yMax < currentEvent.mousePosition.y) { value.y = 0; }
                    else { value.y = 1 - (currentEvent.mousePosition.y - graphRect.yMin) / graphRect.height; }
					
                    // apply value
                    property.vector2Value = GgMaths.LerpVector2(graphAttribute.min, graphAttribute.max, value);
                    currentEvent.Use();
                }
            }
		
            // draw graph
            using (new GUI.GroupScope(graphRect, EditorStyles.helpBox))
            {
                if (currentEvent.type == EventType.Repaint)
                {
                    // draw axis lines
                    GL.Begin(Application.platform == RuntimePlatform.WindowsEditor ? GL.QUADS : GL.LINES);
                    ApplyWireMaterial.Invoke(null, new object[] {CompareFunction.Always});
                    GLExtensions.GL_DrawLine(new Vector2(graphSizeQuarter, 1), new Vector2(graphSizeQuarter, graphSize - 2), graphDivisionColour);
                    GLExtensions.GL_DrawLine(new Vector2(graphSizeQuarter, 1), new Vector2(graphSizeQuarter, graphSize - 2), graphDivisionColour);
                    GLExtensions.GL_DrawLine(new Vector2(graphSizeThreeQuarters, 1), new Vector2(graphSizeThreeQuarters, graphSize - 2), graphDivisionColour);
                    GLExtensions.GL_DrawLine(new Vector2(1, graphSizeQuarter), new Vector2(graphSize - 2, graphSizeQuarter), graphDivisionColour);
                    GLExtensions.GL_DrawLine(new Vector2(1, graphSizeThreeQuarters), new Vector2(graphSize - 2, graphSizeThreeQuarters), graphDivisionColour);
                    GLExtensions.GL_DrawLine(new Vector2(graphSizeHalf, 1), new Vector2(graphSizeHalf, graphSize - 2), GUI.enabled ? xAxiscolour : xAxiscolour * 0.631f);
                    GLExtensions.GL_DrawLine(new Vector2(1, graphSizeHalf), new Vector2(graphSize - 2, graphSizeHalf), GUI.enabled ? yAxiscolour : yAxiscolour * 0.631f);
                    GL.End();

                    // position circle
                    GL.Begin(Application.platform == RuntimePlatform.WindowsEditor ? GL.QUADS : GL.LINES);
                    ApplyWireMaterial.Invoke(null, new object[] {CompareFunction.Always});
                    Vector2 circlePos = GgMaths.InverseLerpVector2(graphAttribute.min, graphAttribute.max, property.vector2Value);
                    circlePos.y = 1 - circlePos.y;
                    circlePos *= graphSize;
                    GLExtensions.GL_DrawCircle(circlePos, graphSize * 0.03f, 2, GUI.enabled ? positionColour : positionColour * 0.631f);
                    GL.End();
                }
            }

            // title, subtitles & sliders
            Vector2 propertyVector2Value = property.vector2Value;
            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            GUI.enabled = false;
            EditorGUI.LabelField(xLabelRect, graphAttribute.xAxis);
            GUI.enabled = defaultGui;
            propertyVector2Value.x = EditorGUI.Slider(xSliderRect, propertyVector2Value.x, graphAttribute.min.x, graphAttribute.max.x);
            GUI.enabled = false;
            EditorGUI.LabelField(yLabelRect, graphAttribute.yAxis);
            GUI.enabled = defaultGui;
            propertyVector2Value.y = EditorGUI.Slider(ySliderRect, propertyVector2Value.y, graphAttribute.min.y, graphAttribute.max.y);
            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = propertyVector2Value;
            }
        }

        #endregion
		
    } // class end
}
#endif