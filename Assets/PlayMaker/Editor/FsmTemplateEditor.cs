using System.ComponentModel;
using HutongGames.PlayMakerEditor;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(FsmTemplate))]
public class FsmTemplateEditor : Editor
{
    private SerializedProperty categoryProperty;
    private SerializedProperty descriptionProperty;
    private GUIStyle multiline;

    private GUIContent findInBrowser;

    private GUIContent editButton;

    [Localizable(false)]
    public void OnEnable()
    {
        categoryProperty = serializedObject.FindProperty("category");
        descriptionProperty = serializedObject.FindProperty("fsm.description");

        findInBrowser = new GUIContent("Find in Template Browser");
        editButton = new GUIContent("Edit", Strings.FsmTemplateEditor_Open_In_Editor);
    }

    public override void OnInspectorGUI()
    {
        FsmEditorStyles.Init();

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Category - used in Template Browser and menus");
        EditorGUILayout.PropertyField(categoryProperty, GUIContent.none);

        if (multiline == null)
        {
            multiline = new GUIStyle(EditorStyles.textField) { wordWrap = true };
        }
        descriptionProperty.stringValue = EditorGUILayout.TextArea(descriptionProperty.stringValue, multiline, GUILayout.MinHeight(60));
        FsmEditorGUILayout.DrawHintText(descriptionProperty.stringValue, "Description");

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            FsmTemplateSelector.Refresh();
        }
        GUILayout.BeginHorizontal();
        
        //GUILayout.FlexibleSpace();

        if (GUILayout.Button(findInBrowser))
        {
            FsmTemplateSelector.FindTemplateInBrowser((FsmTemplate) target);
        }

        if (GUILayout.Button(editButton))
        {
            FsmEditorWindow.OpenWindow((FsmTemplate) target);
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(Strings.Hint_Exporting_Templates, MessageType.None );
    }
}
