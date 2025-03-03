// (c) Copyright HutongGames,  LLC 2010-2020. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Checks project for potential issues before importing PlayMaker.
    ///
    /// Results:
    ///
    /// 1. PlayMaker Not Installed: No Conflicts Found...
    /// 2. Previous Version of PlayMaker Installed: No Conflicts Found...
    /// 3. Previous Version of PlayMaker Installed: Conflicts Found!
    /// 4. PlayMaker Up To Date: No Conflicts Found...
    /// 5. PlayMaker Up To Date: Conflicts Found!
    ///
    /// Conflicts Found!
    /// These actions found in the project are now included with PlayMaker.
    /// Delete these actions before importing the update to avoid errors.
    /// Click on the action below to find the script.
    /// 
    /// > Category Foldout [count]
    ///  Action [Could not find script]
    ///  Action [Click to ping]
    ///
    /// Problems:
    /// - Type names do not match file names (fix type names below)
    /// - Lists are unwieldy (use data file)
    /// 
    /// </summary>
    public class PreUpdateChecker : EditorWindow
    {
        private class Styles
        {
            public readonly GUIStyle ConflictLine;
            public readonly GUIStyle WordWrapLabel;

            public Styles()
            {
                ConflictLine = new GUIStyle(EditorStyles.label) { margin = new RectOffset(16,0,0,0)} ;
                WordWrapLabel = new GUIStyle(EditorStyles.label) {wordWrap = true, richText = true};
            }
        }
        private static Styles styles;

        public class ActionConflict
        {
            public string actionName;
            public string label;
            public Object asset;
        }

        [Serializable]
        public class CheckActions
        {
            [Serializable]
            public class Category
            {
                public string name;
                public string officialPath;
                public List<string> typeNames;

                [Serializable]
                public class Type
                {
                    public string typeName;
                    public string fileName;
                }
            }

            public List<Category> categories = new List<Category>();
        }

        private CheckActions checkActions;

        // Action conflicts sorted into categories
        public readonly Dictionary<string,  List<ActionConflict>> actionConflicts = new Dictionary<string,  List<ActionConflict>>();

        // Lookup MonoScript for given action type
        private static Dictionary<Type, Object> actionScriptLookup;

        private Type actionType;

        // Store open state of category foldouts
        private bool[] categoryIsOpen = new bool[0];

        private Vector2 scrollPosition;

        private bool playmakerInstalled;

        [MenuItem("PlayMaker/Tools/Pre-Update Check",  false,  66)]
        public static void Open()
        {
            GetWindow<PreUpdateChecker>(true);
        }

        public void OnEnable()
        {
            checkActions = new CheckActions();
            EditorJsonUtility.FromJsonOverwrite(Resources.Load<TextAsset>("CheckActions").text, checkActions);
            //Debug.Log(EditorJsonUtility.ToJson(checkActions, true));

            var titleText = string.Format("Pre-Update Check: PlayMaker {0}",  PlayMakerWelcomeWindow.InstallCurrentVersion);

            titleContent = new GUIContent(titleText);
            minSize = new Vector2(250, 100);

            actionType = GetTypeByName("HutongGames.PlayMaker.FsmStateAction");
            playmakerInstalled = actionType != null;

            DoCheck();

            // Uncomment to resave CheckActions text file
            // Useful to check or prettify

            //SaveCheckActionsFile();
        }

        /*
        private void SaveCheckActionsFile()
        {
            if (checkActions == null) return;

            var textAsset = Resources.Load<TextAsset>("CheckActions");

            File.WriteAllText(AssetDatabase.GetAssetPath(textAsset), EditorJsonUtility.ToJson(checkActions, true));
            EditorUtility.SetDirty(textAsset);
        }*/

        public void OnGUI()
        {
            if (styles == null) styles = new Styles();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Summary", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (!playmakerInstalled)
            {
                GUILayout.Label("PlayMaker Not Installed");
            }

            var failedCategories = actionConflicts.Keys.ToList();

            if (failedCategories.Count == 0)
            {
                GUILayout.Label("No Conflicts Found...");
            }
            else
            {
                GUILayout.Label("Potential conflicts found!" +
                                "\n\nSome actions found in the project are now included with PlayMaker:",
                    styles.WordWrapLabel);

                GUILayout.Space(5);

                for (var i = 0; i < failedCategories.Count; i++)
                {
                    var category = failedCategories[i];
                    var conflicts = actionConflicts[category];
                    var label = category + " [" + conflicts.Count + "]";
                    categoryIsOpen[i] = EditorGUILayout.Foldout(categoryIsOpen[i], label);
                    if (categoryIsOpen[i])
                    {
                        foreach (var conflict in conflicts)
                        {
                            if (conflict.asset == null)
                            {
                                GUILayout.Label(conflict.actionName + " [Could not find script]", styles.ConflictLine);
                            }
                            else
                            {
                                var rect = GUILayoutUtility.GetRect(GUIContent.none, styles.ConflictLine);
                                if (GUI.Button(rect, conflict.label, styles.ConflictLine))
                                {
                                    EditorGUIUtility.PingObject(conflict.asset);
                                }
                                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
                            }
                        }

                        if (GUILayout.Button("Select Files in Project View"))
                        {
                            Selection.objects = conflicts.Select(l => l.asset).ToArray();
                        }
                    }
                }

                const string output = "If you imported these actions from official unitypackages the update should replace them automatically." +
                                      "\n\nHowever, if you downloaded them from the Ecosystem or Forums you might get errors from duplicate files after updating PlayMaker." +
                                      "\n\nRemove these files now, or after updating to fix any errors in the Unity Console.\n" +
                                      "\nUsually older versions (to delete) are in:\n<i>Assets/PlayMaker Custom Actions</i>\n" +
                                      "\nNewer versions (to keep) will be in:\n<i>Assets/PlayMaker/Actions</i>\n";

                GUILayout.Space(10);
                GUILayout.Label(output, styles.WordWrapLabel);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();

            GUILayout.Space(5);
            if (GUILayout.Button("Run Check Again"))
            {
                DoCheck();
            }
            GUILayout.Space(5);
        }

        public void DoCheck()
        {
            actionConflicts.Clear();

            if (playmakerInstalled)
            {
                FindScripts();
                CheckForActionConflicts();
            }
        }

        private void FindScripts()
        {
            actionScriptLookup = new Dictionary<Type, Object>();
            var allMonoScripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));

            foreach (var script in allMonoScripts)
            {
                // Note: GetClass only works if script is compiled
                var scriptClass = script.GetClass();
                if (scriptClass == null) continue; // Debug.LogWarning("Not compiled: " + script.name);

                if (scriptClass.IsSubclassOf(actionType))
                {
                    actionScriptLookup.Add(scriptClass, script);
                }
            }
        }

        private void CheckForActionConflicts()
        {
            if (!playmakerInstalled) return;

            foreach (var category in checkActions.categories)
            {
                foreach (var action in category.typeNames)
                {
                    CheckAction(action, category.name, category.officialPath);
                }
            }

            var failedCategoryCount = actionConflicts.Keys.Count;
            if (failedCategoryCount != categoryIsOpen.Length)
            {
                categoryIsOpen = new bool[actionConflicts.Keys.Count];
            }
        }

        private void CheckAction(string actionName, string category, string officialPath)
        {
            if (!playmakerInstalled) return;

            if (actionName.EndsWith("Editor") ||
                actionName.EndsWith("EditorBase"))
            {
                // This doesn't work...?
                //actionName = "HutongGames.PlayMakerEditor." + actionName;
                return;
            }

            actionName = "HutongGames.PlayMaker.Actions." + actionName;

            var foundType = GetTypeByName(actionName);
            if (foundType == null)
            {
                // Type is not in this project
                //Debug.Log("Could not find type: " + actionName);
                return;
            }

            Object asset;
            if (actionScriptLookup.TryGetValue(foundType, out asset))
            {
                if (!AssetDatabase.GetAssetPath(asset).Contains(officialPath))
                {
                    AddConflict(actionName, category, asset);
                }
            }

        }

        private void AddConflict(string actionName, string category, Object asset)
        {
            List<ActionConflict> conflictsInCategory;
            if (!actionConflicts.TryGetValue(category, out conflictsInCategory))
            {
                conflictsInCategory = new List<ActionConflict>();
                actionConflicts.Add(category, conflictsInCategory);
            }

            var label = actionName.Replace("HutongGames.PlayMaker.Actions.", "");
            conflictsInCategory.Add(new ActionConflict { actionName = actionName, label = label, asset = asset });
        }

        private static Type GetTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(name, false, true);
                if (type != null) return type;
            }

            return null;
        }

    }
}

