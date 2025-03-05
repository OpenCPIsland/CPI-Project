#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gaskellgames
{
    [CustomPropertyDrawer(typeof(SceneData))]
    public class SceneDataDrawer : PropertyDrawer
    {
        #region Variables

        private SerializedProperty sceneAsset;
        private SerializedProperty guid;
        private SerializedProperty buildIndex;
        private SerializedProperty sceneName;
        private SerializedProperty sceneFilePath;

        private string tooltipSuffix = "\n\nEditorOnly References:\n- SceneAsset\n\nRuntime references:\n- Scene (no data to show)\n- buildIndex\n- sceneName\n- sceneFilePath\n- GUID (Set in editor, or via SceneDataList)";
        private float spacer = 2;
        private float singleLine = EditorGUIUtility.singleLineHeight;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Property Height
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return singleLine + ((singleLine + spacer) * 4);
            }
            else
            {
                return singleLine;
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            sceneAsset = property.FindPropertyRelative("sceneAsset");
            buildIndex = property.FindPropertyRelative("buildIndex");
            sceneName = property.FindPropertyRelative("sceneName");
            sceneFilePath = property.FindPropertyRelative("sceneFilePath");
            guid = property.FindPropertyRelative("guid");
            
            // open property and get reference to instance
            EditorGUI.BeginProperty(position, label, property);
            
            // draw property
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUIContent newLabel = new GUIContent(label.text, label.tooltip + tooltipSuffix);
            EditorGUI.PrefixLabel(position, newLabel);
            float sceneAssetPositionX = position.x + EditorGUIUtility.labelWidth + spacer;
            float sceneAssetWidth = position.width - (EditorGUIUtility.labelWidth + spacer + spacer + 14);
            Rect sceneAssetRect = new Rect(sceneAssetPositionX, position.y, sceneAssetWidth, singleLine);
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(sceneAssetRect, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                // cache SceneData if SceneAsset updated in inspector
                if (sceneAsset.objectReferenceValue != null)
                {
                    buildIndex.intValue = SceneUtility.GetBuildIndexByScenePath(sceneFilePath.stringValue);
                    sceneName.stringValue = sceneAsset.objectReferenceValue.name;
                    sceneFilePath.stringValue = AssetDatabase.GetAssetOrScenePath(sceneAsset.objectReferenceValue);
                    guid.stringValue = AssetDatabase.AssetPathToGUID(sceneFilePath.stringValue);
                }
                else
                {
                    buildIndex.intValue = -1;
                    sceneName.stringValue = null;
                    sceneFilePath.stringValue = null;
                    guid.stringValue = null;
                }
            }

            // draw toggle
            GUI.enabled = true;
            string tooltip = "Toggle:\n- Show/Hide scene data.\n\nLine:\n- Green if scene is in build scenes and enabled\n- Orange if scene is not in build scenes or is in build scenes and disabled \n- Red if null";
            float togglePositionX = sceneAssetPositionX + sceneAssetWidth + spacer;
            Rect toggleRect = new Rect(togglePositionX, position.y, 14, 14);
            GUIContent toggleLabel = new GUIContent("", tooltip);
            property.isExpanded = EditorGUI.Toggle(toggleRect, toggleLabel, property.isExpanded);
            GUI.enabled = false;

            // draw line
            float linePositionX = position.xMax + (spacer * 2) - singleLine;
            float linePositionY = position.y + singleLine - spacer;
            float lineWidth = singleLine - (spacer * 2);
            Rect lineRect = new Rect(linePositionX, linePositionY, lineWidth, spacer);
            bool validBuildIndex = 0 <= buildIndex.intValue && buildIndex.intValue < EditorBuildSettings.scenes.Length;
            if (sceneAsset.objectReferenceValue == null)
            {
                EditorGUI.DrawRect(lineRect, InspectorExtensions.redColorDark);
            }
            else if (sceneAsset.objectReferenceValue != null && !validBuildIndex)
            {
                EditorGUI.DrawRect(lineRect, InspectorExtensions.yellowColorDark);
            }
            else if (sceneAsset.objectReferenceValue != null && validBuildIndex && EditorBuildSettings.scenes[buildIndex.intValue].enabled)
            {
                EditorGUI.DrawRect(lineRect, InspectorExtensions.greenColorDark);
            }
            
            // draw extra data if toggle selected
            EditorGUILayout.EndHorizontal();
            if (property.isExpanded)
            {
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                
                Rect moreInfoRect = new Rect(position.x, position.y + singleLine + spacer, position.width, singleLine);
                GUIContent moreInfoLabel = new GUIContent("Build Index", "Runtime reference to the BuildIndex of the scene.");
                EditorGUI.IntField(moreInfoRect, moreInfoLabel, buildIndex.intValue);
                
                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Scene Name", "Runtime reference to the SceneName of the scene.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, sceneName.stringValue);
                
                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Scene File Path", "Runtime reference to the SceneFilePath of the scene.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, sceneFilePath.stringValue);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("GUID", "Runtime reference to the GUID of the scene.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, guid.stringValue);
                
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }

            // close property
            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}
#endif