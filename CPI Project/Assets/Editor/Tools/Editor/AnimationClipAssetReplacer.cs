using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AnimationClipAssetReplacer : EditorWindow
{
    private AnimationClip clip;

    private class BindingEntry
    {
        public EditorCurveBinding binding;
        public Object currentValue;
        public Object newValue;
        public string displayName;
    }

    private List<BindingEntry> entries = new List<BindingEntry>();
    private Vector2 scroll;

    [MenuItem("Project/Tools/Editor/Animation Clip Asset Replacer")]
    public static void Open()
    {
        GetWindow<AnimationClipAssetReplacer>("Clip Asset Replacer");
    }

    private void OnGUI()
    {
        clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", clip, typeof(AnimationClip), false);

        if (clip == null)
            return;

        if (GUILayout.Button("Scan Clip"))
        {
            Scan();
        }

        if (entries.Count == 0)
            return;

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var e in entries)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(e.displayName, EditorStyles.boldLabel);
            EditorGUILayout.ObjectField("Current", e.currentValue, typeof(Object), false);
            e.newValue = EditorGUILayout.ObjectField("Replace With", e.newValue, typeof(Object), false);

            if (GUILayout.Button("Apply This"))
            {
                ApplySingle(e);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Apply All Changes"))
        {
            ApplyAll();
        }
    }

    private void Scan()
    {
        entries.Clear();

        var objBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

        foreach (var b in objBindings)
        {
            var keys = AnimationUtility.GetObjectReferenceCurve(clip, b);

            Object last = null;
            if (keys != null && keys.Length > 0)
                last = keys[keys.Length - 1].value;

            entries.Add(new BindingEntry
            {
                binding = b,
                currentValue = last,
                newValue = last,
                displayName = $"{b.path} / {b.propertyName}"
            });
        }
    }

    private void ApplySingle(BindingEntry e)
    {
        if (e.newValue == null)
            return;

        var keys = AnimationUtility.GetObjectReferenceCurve(clip, e.binding);

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].value = e.newValue;
        }

        AnimationUtility.SetObjectReferenceCurve(clip, e.binding, keys);

        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssets();
    }

    private void ApplyAll()
    {
        foreach (var e in entries)
        {
            if (e.newValue == null)
                continue;

            var keys = AnimationUtility.GetObjectReferenceCurve(clip, e.binding);

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].value = e.newValue;
            }

            AnimationUtility.SetObjectReferenceCurve(clip, e.binding, keys);
        }

        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssets();
    }
}