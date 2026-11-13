using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PlayMakerUpdater
{
    private const string PlayMakerDllPath = "Assets/Plugins/PlayMaker/PlayMaker.dll";

    private struct IconMapping
    {
        public string ClassName;
        public string IconGuid;

        public IconMapping(string className, string iconGuid)
        {
            ClassName = className;
            IconGuid = iconGuid;
        }
    }

    private static readonly IconMapping[] IconMappings =
    {
        new IconMapping("FsmTemplate", "b79a8c6a7f59ffe4caefb7ee0dbb6c28"),
        new IconMapping("PlayMakerControls", "4ac9185bf8ba41e4d81d2d443a326b2f"),
        new IconMapping("PlayMakerFSM", "4889c347147d7844a93bbdd28089ce27"),
        new IconMapping("PlayMakerGlobals", "60204a42898e09e44afa4f4ca5ed1da1"),
        new IconMapping("PlayMakerGUI", "2c7db6fc3f6ab6b429b6a3924d3820eb"),
        new IconMapping("HutongGames.PlayMaker.HtmlNotes", "3c6ce8269c92d4a4f99f911813c0b613"),
        new IconMapping("PlayMakerPrefs", "70b5f8288a5c3a64b9b1bc002bcf0c28")
    };

    static PlayMakerUpdater()
    {
        // Delay until first update
        // Otherwise process gets stomped on by other Unity initializations
        // E.g., Unity loading last layout stomps on PlayMakerUpgradeGuide window.
        EditorApplication.update += Update;
    }

    static void Update()
    {
        EditorApplication.update -= Update;

#if UNITY_6000_OR_NEWER
        RestorePlayMakerDllIcons();
#endif
        /*
        var showUpgradeGuide = EditorPrefs.GetBool(EditorPrefStrings.ShowUpgradeGuide, true);
        if (showUpgradeGuide)
        {
            EditorWindow.GetWindow<PlayMakerUpgradeGuide>(true);
        }*/
    }

#if UNITY_6000_OR_NEWER

    private static void RestorePlayMakerDllIcons()
    {
        var dllPath = AssetDatabase.GUIDToAssetPath(AssetGUIDs.PlayMakerDll);
        if (string.IsNullOrEmpty(dllPath))
        {
            dllPath = PlayMakerDllPath;
        }

        var importer = AssetImporter.GetAtPath(dllPath) as PluginImporter;
        if (importer == null)
        {
            return;
        }

        var changed = false;

        foreach (var mapping in IconMappings)
        {
            var iconPath = AssetDatabase.GUIDToAssetPath(mapping.IconGuid);
            if (string.IsNullOrEmpty(iconPath))
            {
                continue;
            }

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon == null || importer.GetIcon(mapping.ClassName) == icon)
            {
                continue;
            }

            importer.SetIcon(mapping.ClassName, icon);
            changed = true;
        }

        if (changed)
        {
            importer.SaveAndReimport();
        }
    }

#endif    
}
