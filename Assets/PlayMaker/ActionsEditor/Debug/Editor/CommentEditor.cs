// (c) Copyright HutongGames, LLC. 2020.All rights reserved.

using HutongGames.Editor;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof (PlayMaker.Actions.Comment))]
    public class CommentEditor : CustomActionEditor
    {
        private PlayMaker.Actions.Comment commentAction;
        private HtmlTextEditor htmlTextEditor;

        public override bool showCategoryIcon { get { return false; } }
        public override bool showEnabledCheckbox { get { return false; } }

        public override void OnEnable()
        {
            commentAction = (PlayMaker.Actions.Comment) target;
            htmlTextEditor = new HtmlTextEditor();// {commitOnEnter = true};
        }

        public override bool OnGUI()
        {
            //return DrawDefaultInspector();
            
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical(FsmEditorStyles.StandardMargins);
            GUILayout.Space(5);
            commentAction.comment = htmlTextEditor.OnGUI(commentAction.comment, maxEditorWidth);
            if (commentAction.comment == "")
            {
                commentAction.comment = "Double-Click to Edit";
                GUI.changed = true;
            }
            GUILayout.Space(10);
            GUILayout.EndVertical();

            return EditorGUI.EndChangeCheck();
        }
    }
}