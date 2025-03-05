#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(TagDropdownAttribute))]
    public class TagDropdownDrawer : PropertyDrawer
    {
        #region Variables

        private TagDropdownAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as TagDropdownAttribute;
            
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                if (attributeAsType.UseDefaultTagFieldDrawer)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    // generate the taglist
                    List<string> tagList = new List<string>();
                    tagList.AddRange(InternalEditorUtility.tags);
                    string propertyString = property.stringValue;
                    int index = -1;
                    if (propertyString == "")
                    {
                        index = 0; // first index is a special case: Untagged
                    }
                    else
                    {
                        // check if entry matches and get the index
                        for (int i = 1; i < tagList.Count; i++)
                        {
                            if (tagList[i] == propertyString)
                            {
                                index = i;
                                break;
                            }
                        }
                    }

                    // draw the popup box with the current selected index
                    List<GUIContent> values = new List<GUIContent>();
                    foreach (string tag in tagList) { values.Add(new GUIContent(tag, "")); }
                    index = EditorGUI.Popup(position, label, index, values.ToArray());
                    
                    // adjust the actual string value of the property based on the selection
                    if (index >= 1)
                    {
                        property.stringValue = tagList[index];
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        #endregion
        
    } // class end
}

#endif