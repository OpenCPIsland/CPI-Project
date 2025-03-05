#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    [CustomPropertyDrawer(typeof(AudioClipData))]
    public class AudioClipDataDrawer : PropertyDrawer
    {
        #region Variables

        private SerializedProperty audioClip;
        private SerializedProperty fileName;
        private SerializedProperty assetPath;

        private SerializedProperty forceToMono;
        private SerializedProperty loadInBackground;
        private SerializedProperty ambisonic;

        private SerializedProperty loadType;
        private SerializedProperty preloadAudioData;
        private SerializedProperty compressionFormat;
        private SerializedProperty quality;
        private SerializedProperty sampleRateSetting;

        private float spacer = 2;
        private float singleLine = EditorGUIUtility.singleLineHeight;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Property Height
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return singleLine + ((singleLine + spacer) * 10);
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
            audioClip = property.FindPropertyRelative("audioClip");
            fileName = property.FindPropertyRelative("fileName");
            assetPath = property.FindPropertyRelative("assetPath");
            forceToMono = property.FindPropertyRelative("forceToMono");
            loadInBackground = property.FindPropertyRelative("loadInBackground");
            ambisonic = property.FindPropertyRelative("ambisonic");
            loadType = property.FindPropertyRelative("loadType");
            preloadAudioData = property.FindPropertyRelative("preloadAudioData");
            compressionFormat = property.FindPropertyRelative("compressionFormat");
            quality = property.FindPropertyRelative("quality");
            sampleRateSetting = property.FindPropertyRelative("sampleRateSetting");
            
            // open property and get reference to instance
            EditorGUI.BeginProperty(position, label, property);
            
            // draw property
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUI.PrefixLabel(position, label);
            float audioClipPositionX = position.x + (EditorGUIUtility.labelWidth + spacer);
            float audioClipWidth = position.width - (EditorGUIUtility.labelWidth + spacer + singleLine + spacer);
            Rect audioClipRect = new Rect(audioClipPositionX, position.y, audioClipWidth, singleLine);
            audioClip.objectReferenceValue = EditorGUI.ObjectField(audioClipRect, audioClip.objectReferenceValue, typeof(AudioClip), false);
            if (EditorGUI.EndChangeCheck())
            {
                // cache AudioClipData if AudioClip updated in inspector
                AudioClip audioClipRef = audioClip.objectReferenceValue as AudioClip;
                if (audioClip.objectReferenceValue != null && audioClipRef)
                {
                    if (AudioClipData.Editor_CreateAudioClipData(audioClipRef, out AudioClipData audioClipData))
                    {
                        fileName.stringValue = audioClipData.FileName;
                        assetPath.stringValue = audioClipData.AssetPath;
                        forceToMono.boolValue = audioClipData.ForceToMono;
                        loadInBackground.boolValue = audioClipData.LoadInBackground;
                        ambisonic.boolValue = audioClipData.Ambisonic;
                        loadType.enumValueIndex = audioClipData.LoadType.ToInt();
                        preloadAudioData.boolValue = audioClipData.PreloadAudioData;
                        compressionFormat.enumValueIndex = audioClipData.CompressionFormat.ToInt();
                        quality.floatValue = audioClipData.Quality;
                        sampleRateSetting.intValue = audioClipData.Editor_SampleRateSetting.ToInt();
                    }
                    else
                    {
                        fileName.stringValue = "";
                        assetPath.stringValue = "";
                        forceToMono.boolValue = false;
                        loadInBackground.boolValue = false;
                        ambisonic.boolValue = false;
                        loadType.enumValueIndex = 0;
                        preloadAudioData.boolValue = false;
                        compressionFormat.enumValueIndex = 0;
                        quality.floatValue = 100;
                        sampleRateSetting.intValue = 0;
                    }
                }
                else
                {
                    fileName.stringValue = "";
                    assetPath.stringValue = "";
                    forceToMono.boolValue = false;
                    loadInBackground.boolValue = false;
                    ambisonic.boolValue = false;
                    loadType.enumValueIndex = 0;
                    preloadAudioData.boolValue = false;
                    compressionFormat.enumValueIndex = 0;
                    quality.floatValue = 100;
                    sampleRateSetting.intValue = 0;
                }
            }

            // draw toggle
            GUI.enabled = true;
            string tooltip = "Toggle:\n- Show/Hide audioClip data.";
            float togglePositionX = audioClipPositionX + spacer + audioClipWidth + spacer;
            Rect toggleRect = new Rect(togglePositionX, position.y, singleLine, singleLine);
            GUIContent toggleLabel = new GUIContent("", tooltip);
            property.isExpanded = EditorGUI.Toggle(toggleRect, toggleLabel, property.isExpanded);
            GUI.enabled = false;
            
            // draw extra data if toggle selected
            EditorGUILayout.EndHorizontal();
            if (property.isExpanded)
            {
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                
                Rect moreInfoRect = new Rect(position.x, position.y + singleLine + spacer, position.width, singleLine);
                GUIContent moreInfoLabel = new GUIContent("File Name", "Runtime reference to the fileName of the audioClip.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, fileName.stringValue);
                
                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("AudioClip File Path", "Runtime reference to the assetPath of the audioClip.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, assetPath.stringValue);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Force To Mono", "Runtime reference to the forceToMono value of the audioClip.");
                EditorGUI.Toggle(moreInfoRect, moreInfoLabel, forceToMono.boolValue);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Load In Background", "Runtime reference to the loadInBackground value of the audioClip.");
                EditorGUI.Toggle(moreInfoRect, moreInfoLabel, loadInBackground.boolValue);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Ambisonic", "Runtime reference to the ambisonic value of the audioClip.");
                EditorGUI.Toggle(moreInfoRect, moreInfoLabel, ambisonic.boolValue);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Load Type", "Runtime reference to the loadType value of the audioClip.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, loadType.enumNames[loadType.enumValueIndex]);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Preload Audio Data", "Runtime reference to the preloadAudioData value of the audioClip.");
                EditorGUI.Toggle(moreInfoRect, moreInfoLabel, preloadAudioData.boolValue);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Compression Format", "Runtime reference to the compressionFormat value of the audioClip.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, compressionFormat.enumNames[compressionFormat.enumValueIndex]);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("Quality", "Runtime reference to the quality value of the audioClip.");
                EditorGUI.FloatField(moreInfoRect, moreInfoLabel, quality.floatValue);

                moreInfoRect.y += singleLine + spacer;
                moreInfoLabel = new GUIContent("SampleRateSetting", "Runtime reference to the sampleRateSetting value of the audioClip.");
                EditorGUI.TextField(moreInfoRect, moreInfoLabel, sampleRateSetting.enumNames[sampleRateSetting.enumValueIndex]);
                
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